using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        // [size(2)][packetId(2)][...]  [size(2)][packetId(2)][...]
        // size는 자신을 포함한 크기
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0; // 몇byte를 처리했는지
            int packetCount = 0; // packet 몇개를 처리했는지

            while(true) // 처리할 수 있을때까지 반복
            {
                // 최소한 header은 parsing할 수 있는지 확인
                if (buffer.Count < HeaderSize) break;

                // 패킷이 완전체로 도착했는지
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize) break;

                // 여기까지 왔으면 패킷 조립 가능, 패킷을 만들어서 보내도 되고, ArraySegment을 보내도 되고
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                // ArraySegment는 class가 아니다. new를 붙여준다고 해도 heap 영역에 할당되는 것이 아님
                packetCount++;

                // 그 다음 부분을 집어준다. buffer를 slice해서 넘겨줘도 됨
                // Arraysegment는 struct이므로 new keyword를 붙여준다고해서 heap에 할당되는 것은 아니다.
                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }
            if (packetCount > 1)
                Console.WriteLine($"패킷 모아 보내기 : {packetCount}");
            // 만약에 100개씩 보냈는데 100개를 못받는 이유? - recv 버퍼 size가 너무 작아서 

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        object _lock = new object(); 
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);     
        public abstract void OnSend(int numofBytes); // 몇바이트를 보냈는지 콘솔에 출력
        public abstract void OnDisconnected(EndPoint endPoint);

        // disconnect할 때 _sendQueue와 _pendingList 정리
        void Clear()
        {
            lock(_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Start(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        // 패킷을 모아보내기 위한 인터페이스
        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0) return;
            // pending list에 빈 list를 넣어주고 send를하면
            // onSendCompleted에서 인자가 잘못됬으니까 exception으로 바로 튕겨낸다.

            lock (_lock)
            {
                foreach(ArraySegment<byte> sendBuff in sendBuffList)
                    _sendQueue.Enqueue(sendBuff);

                if (_pendingList.Count == 0) RegisterSend();
            }
        }

        // multithread 환경에서 호출할 수 있어야하므로 lock 필요
        public void Send(ArraySegment<byte> sendBuff)
        {
            lock(_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                // 만약에 packet 모아보내기를 엔진단에서 한다면 
                // 밀어넣는 작업이랑 보내는 작업이랑 분리를 한다. 어느정도 모인다음에 Send한다.
                if(_pendingList.Count == 0) RegisterSend();
            }
        }

        public void Disconnect()
        {
            // 중복 호출위한 안정장치, 하지만 모든 상황에 대한 처리가 안되어있다.
            // 동시다발적으로 누군가는 disconnect해서 socket을 close까지 했는데, 다른 누군가가 send나 recv를 한다면 뻑이날 것이다.
            // send recv부분을 try-catch로 감싸서 예외처리한다.

            // 동시 다발적으로 disconnect를 한다던가, 같은애가 2번하면? - error 한번만 하게끔
            // if(_socket != null) // ... multithread 환경에서 안전하지 않다. -> flag 사용
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) return;
           
            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both); 
            _socket.Close();
            // _socket = null;
            Clear();
        }

        #region 네트워크 통신

        // multithread 영역, send에서 lock을 잡은 이유
        void RegisterSend()
        {
            // 최소한의 방어. 이것만으로는 mutithread환경에서 구할 수 없음
            // 이 조건을 통과했는데 다른 Thread가 이 session을 disconnect해서 socket이 끊겼다면 문제가 생김 
            // try-catch 문 이용
            if (_disconnected == 1) return; 

            while(_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }

            _sendArgs.BufferList = _pendingList;

            // 소켓을 다루는 부분을 try-catch
            try 
            { 
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false) OnSendCompleted(null, _sendArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterSend Failed {e}");
            }
        }

        // RegisterSend 뿐만 아니라 Callback 방식으로 다른 Thread에서 호출될 수 있기 때문에 lock 필요
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null; // 꼭 null로 밀어줄 필요는 없음, 생략가능                  
                        _pendingList.Clear();

                        // 몇바이트를 보냈는지 콘솔에 출력
                        OnSend(_sendArgs.BytesTransferred);

                        if(_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed : {e.Message}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterRecv() 
        {
            if (_disconnected == 1) return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false) OnRecvCompleted(null, _recvArgs);
            }
            catch(Exception e)
            {
                Console.WriteLine($"RegisterRecv Failed {e}");
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // Write 커서 이동
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다.
                    // OnRecv에서는 패킷이 완성되지 않은 부분적인 데이터라면 처리했는지 여부를 알고 싶으므로
                    // 반환값을 int로 설정하고 얼만큼 데이터를 처리했는지 반환한다.
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if(processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // 여기까지 왔으면 처리했으므로 Read 커서 이동
                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                // packet 조작이 발생했을 경우, 어떤 부분에서 에러가 났는지 확인해서
                // packet조작이 의심되면 바로 disconnect하는 방식을 채택
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed : {e.Message}");
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}