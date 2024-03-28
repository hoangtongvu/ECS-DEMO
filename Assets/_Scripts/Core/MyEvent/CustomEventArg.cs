using System;


namespace Core.MyEvent
{
    public class CustomEventArg<T>
    {
        protected event Action<T> action;

        public virtual void Invoke(T t)
        {
            action?.Invoke(t);
        }

        public virtual void AddListener(Action<T> call)
        {
            action += call;
        }

        public virtual void RemoveListener(Action<T> call)
        {
            action -= call;
        }
    }
}