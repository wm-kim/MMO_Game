using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }

    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false; 

        // Push는 여러명이서 할 수 있음
        public void Push(Action job)
        {
            // 실행하는 동안에는 lock에 접근할 수 없으므로 단계적으로 접근
            bool flush = false; // queue에 쌓인것을 실행할것인지 내부 flush로 관리
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
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
            while(true)
            {
                Action action = Pop();
                if (action == null) return;

                action.Invoke();
            }
        }

        // Flush는 혼자서 하는데 왜 Pop을 할 때 lock을 잡는 이유
        // 하나하나씩 꺼내는 중에도 다른애가 Push할 수 있어서
        Action Pop()
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
