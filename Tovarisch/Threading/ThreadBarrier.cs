namespace Wacton.Tovarisch.Threading
{
    using System;
    using System.Threading;

    public class ThreadBarrier
    {
        private readonly ManualResetEvent manualResetEvent;

        public ThreadBarrier()
        {
            this.manualResetEvent = new ManualResetEvent(false);
        }

        public void RefuseAccess()
        {
            this.manualResetEvent.Reset();
        }

        public void AllowAccess()
        {
            this.manualResetEvent.Set();
        }

        public bool WaitForAccess()
        {
            return this.manualResetEvent.WaitOne(Timeout.Infinite);
        }

        public bool WaitForAccess(TimeSpan timeSpanBeforeJumpingOver)
        {
            return this.manualResetEvent.WaitOne(timeSpanBeforeJumpingOver);
        }
    }
}
