using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerSession : PacketSession
{
	public void Send(IMessage packet)
	{
		string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
		MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

		// 이렇게 만들어줘야지 Session에서 사용할 수 있다.
		ushort size = (ushort)packet.CalculateSize();
		byte[] sendBuffer = new byte[size + 4];

		// 내부에서 byte배열을 한번 더 할당하지만 비트연산을 이용해서 직접넣는 방법이 있다.
		Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
		Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
		Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

		Send(new ArraySegment<byte>(sendBuffer));
	}

	public override void OnConnected(EndPoint endPoint)
	{
		Debug.Log($"OnConnected : {endPoint}");

		PacketManager.Instance.CustomHandler = (s, m, i) =>
		{
			// server sesion는 필요없음
			PacketQueue.Instance.Push(i, m);
		};
	}
	 
	public override void OnDisconnected(EndPoint endPoint)
	{
		Debug.Log($"OnDisconnected : {endPoint}");
	}

	public override void OnRecvPacket(ArraySegment<byte> buffer)
	{
		PacketManager.Instance.OnRecvPacket(this, buffer);
	}

	public override void OnSend(int numOfBytes)
	{
		// Debug.Log($"Transferred bytes: {numOfBytes}");
	}
}