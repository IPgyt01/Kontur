using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication
{

    public class LimitedSizeStack<T>
    {
        private LinkedList<T> stack;
        private int limit;
        public LimitedSizeStack(int limit)
        {
            stack = new LinkedList<T>();
            this.limit = limit;
        }

        public void Push(T item)
        {
            stack.AddLast(item);
            if (Count >= limit)
            {
                stack.RemoveFirst();
                Count--;
            }

            Count++;
        }

        public T Pop()
        {
           if(Count == 0) throw  new InvalidOperationException();
           var res = stack.Last.Value;
           stack.RemoveLast();
           Count--;
           return res;
        }

        public int Count
        {
            get;
            private set;
        }
    }
}
