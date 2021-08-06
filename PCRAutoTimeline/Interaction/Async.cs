using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCRAutoTimeline.Interaction
{
    public class Async
    {
        private static LinkedList<Semaphore> coroutines = new ();

        private static LinkedListNode<Semaphore> nowRunning = null; // we need a global interpreter lock to achieve thread safe
        private static LinkedListNode<Semaphore> firstawait = null;

        private static LinkedListNode<T> NextOrFirst<T>(LinkedListNode<T> node)
        {
            return node.Next ?? node.List.First;
        }

        private static void Wakeup(LinkedListNode<Semaphore> node)
        {
            nowRunning = node;
            node.Value.Release();
        }

        internal static void start(Action coroutine)
        {
            start(() =>
            {
                coroutine();
                return null;
            });
        }

        public static void start(Func<LuaResult> coroutine)
        {
            var sema = new Semaphore(0, 1);
            coroutines.AddLast(sema);
            var mynode = coroutines.Last;

            new Thread(() =>
            {
                sema.WaitOne();
                coroutine();
                //nowrunning is sema, so just dispose it and wake up the next coroutines
                sema.Dispose();
                var next = NextOrFirst(mynode);
                coroutines.Remove(mynode);
                // wait up next
                if (next != mynode) Wakeup(next);
            }).Start();


            if (nowRunning == null) // if first
            {
                nowRunning = mynode;
                sema.Release();
            }

        }

        public static void await()
        {
            if (firstawait == null) firstawait = nowRunning;
            var next = NextOrFirst(nowRunning);
            var mysema = nowRunning.Value;
            if (next == firstawait) // already all in awaiting, sleep and clear
            {
                Thread.Sleep(1);
                firstawait = null;
            }
            Wakeup(next);
            mysema.WaitOne();
        }
    }
}
