using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; // 실행시간
        public IJob job;

        public int CompareTo(JobTimerElem other)
        {
            // 작은 애가 먼저 튀어나오길 원함
            return other.execTick - execTick;
            
        }
    }

    // 경우에 따라서 여기서 더 최적하는 경우도 있다. 2개로 분리
    // 시간이 임박하면 리스트로 관리
    public class JobTimer
    {
        // multithread에서 공용으로 쓰므로 lock 필요
        PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
        object _lock = new object();
        
        // 당장에 실행하기 원하면 tickafter 인지를 받지 않음
        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerElem jobElemnt;
            jobElemnt.execTick = System.Environment.TickCount + tickAfter;
            jobElemnt.job = job;   

            lock(_lock)
            {
                _pq.Push(jobElemnt);
            }
        }

        // Server의 Main Thread에서 while문을 걸어 실행된다.
        public void Flush()
        {
            while(true)
            {
                int now = System.Environment.TickCount;

                JobTimerElem jobElement;

                lock(_lock)
                {
                    if (_pq.Count == 0)
                        break;

                    // 다음으로 실행할 것이 현재 시간보다 나중이라면 바로 종료시켜버린다.
                    jobElement = _pq.Peek();
                    if (jobElement.execTick > now) break;

                    _pq.Pop();
                }

                jobElement.job.Execute();
            }
        }
    }
}
// background thread에서 unity game에서 관리하는 객체에 접근하거나 코드를 실행하면 crash
// game logic은 mainthread에서만 하도록