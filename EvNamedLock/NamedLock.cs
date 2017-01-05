using System;
using System.Collections.Concurrent;
using System.Threading;

namespace EvNamedLock
{
    public class NamedLock
    {      
        internal    class Lock
        {
            int locked;
            SpinWait sw = new SpinWait();

            public void Enter()
            {
                while (Interlocked.CompareExchange(ref locked, 1, 0) != 0)
                    sw.SpinOnce();
            }
            public void Exit()
            {
                Interlocked.Exchange(ref locked, 0);
            }
        }

        static readonly ConcurrentDictionary<string, Lock> cd = new ConcurrentDictionary<string, Lock>();

        private Lock GetLock(string key)
        {
            return cd.GetOrAdd(key, new Lock());
        }

        public void RunWithNamedLock(string key, Action action)
        {
            Lock nl = GetLock(key);

            try
            {
                nl.Enter();
                action();
            }
            finally
            {
                nl.Exit();
   
                //Lock o;
                //cd.TryRemove(key, out o);
            }
        }
    }
}
