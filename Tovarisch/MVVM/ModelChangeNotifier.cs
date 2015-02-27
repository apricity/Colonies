namespace Wacton.Tovarisch.MVVM
{
    using System;
    using System.Collections.Generic;

    public class ModelChangeNotifier
    {
        private readonly List<Listener> listeners = new List<Listener>();

        public void Notify(object changedObject)
        {
            foreach (var listener in this.listeners)
            {
                if (changedObject == listener.Reference.Target)
                {
                    listener.Action();
                }
            }
        }

        public void Subscribe(object objectToNotifyOn, Action actionOnChange)
        {
            this.listeners.Add(new Listener(new WeakReference(objectToNotifyOn), actionOnChange));
        }
    }

    public class Listener
    {
        public WeakReference Reference { get; private set; }
        public Action Action { get; private set; }

        public Listener(WeakReference reference, Action action)
        {
            this.Reference = reference;
            this.Action = action;
        }

        public override string ToString()
        {
            return string.Format("{0} >>> {1} ~ {2}", this.Reference.Target, this.Action.Target, this.Action.Method);
        }
    }
}
