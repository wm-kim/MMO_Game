using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class JobSerializer
    {
        JobTimer _timer = new JobTimer();
        Queue<IJob> _jobQueue = new Queue<IJob>();
        // _jobQueue에만 동시다발적으로 접근 할 수 없도록 막음
        object _lock = new object();
        // _flush : flush를 실행 중인가?
        bool _flush = false;

        public void PushAfter(int tickAfter, Action action) { PushAfter(tickAfter, new Job(action)); }
        public void PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1) { PushAfter(tickAfter, new Job<T1>(action, t1)); }
        public void PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2) { PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2)); }
        public void PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3)); }

        public void PushAfter(int tickAfter, IJob job)
        {
            _timer.Push(job, tickAfter);
        }

        public void Push(Action action) { Push(new Job(action)); }
        public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }

        // Push는 여러명이서 할 수 있음
        public void Push(IJob job)
        {
            // 실행하는 동안에는 lock에 접근할 수 없으므로 단계적으로 접근
            // flush : queue에 쌓인것을 실행할것인가?
            bool flush = false; 
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
                // 아무도 실행하고 있지않다면 직접실행
                if (_flush == false)
                {
                    flush = _flush = true;
                }
            }

            if (flush)
                Flush();
        }

        void Flush()
        {
            // 실행할 수 있는 일감을 모두 실행한다.
            _timer.Flush();

            while (true)
            {
                IJob action = Pop();
                if (action == null) return;

                action.Execute();
            }
        }

        // Flush는 혼자서 하는데 왜 Pop을 할 때 lock을 잡는 이유
        // 하나하나씩 꺼내는 중에도 다른애가 Push할 수 있어서
        IJob Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count == 0)
                {
                    // flush가 끝났다는 상태
                    _flush = false;
                    return null;
                }

                return _jobQueue.Dequeue();
            }
        }

    }
}
