namespace Wacton.Tovarisch.Threading
{
    using System;
    using System.Threading;

    public class ThreadTurnstile
    {
        private readonly AutoResetEvent autoResetEvent;

        public ThreadTurnstile()
        {
            this.autoResetEvent = new AutoResetEvent(false);
        }

        public void RefuseAccess()
        {
            this.autoResetEvent.Reset();
        }

        public void AllowAccessOnce()
        {
            this.autoResetEvent.Set();
        }

        public bool WaitForAccess()
        {
            return this.autoResetEvent.WaitOne(Timeout.Infinite);
        }

        public bool WaitForAccess(TimeSpan timeSpanBeforeJumpingOver)
        {
            return this.autoResetEvent.WaitOne(timeSpanBeforeJumpingOver);
        }
    }
}
