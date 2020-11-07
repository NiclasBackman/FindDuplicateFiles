using System;
using System.Collections.Generic;

namespace DuplicatesGui.ViewModel
{
    public class ObservableProperty<T>
    {
        private List<Action<T>> observers;
        public ObservableProperty()
        {
            observers = new List<Action<T>>();
        }

        private class Unsubscriber : IDisposable
        {
            private List<Action<T>> _observers;
            private Action<T> _observer;

            public Unsubscriber(List<Action<T>> observers, Action<T> observer)
            {
                this._observers = observers;
                this._observer = observer;

            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(Action<T> action)
        {
            if (!observers.Contains(action))
                observers.Add(action);
            return new Unsubscriber(observers, action);
        }

        public void Publish(T value)
        {
            foreach (var observer in observers)
            {
                observer(value);
            }
        }
    }
}
