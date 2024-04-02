using ServiceUpdate1.ObserverSubscriber.Event;

namespace ServiceUpdate1.ObserverSubscriber.Provider
{
    public class NotificationProvider : IObservable<UpdateServiceEvent>
    {

        public string ProviderName { get; private set; }
        // Maintain a list of observers
        private List<IObserver<UpdateServiceEvent>> _observers;

        public NotificationProvider(string _providerName)
        {
            ProviderName = _providerName;
            _observers = new List<IObserver<UpdateServiceEvent>>();
        }

        // Define Unsubscriber class
        private class Unsubscriber : IDisposable
        {

            private List<IObserver<UpdateServiceEvent>> _observers;
            private IObserver<UpdateServiceEvent> _observer;

            public Unsubscriber(List<IObserver<UpdateServiceEvent>> observers,
                                IObserver<UpdateServiceEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        // Define Subscribe method
        public IDisposable Subscribe(IObserver<UpdateServiceEvent> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        // Notify observers when event occurs
        public void EventNotification(string serviceName)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(new UpdateServiceEvent(ProviderName, serviceName,
                                DateTime.Now));
            }
        }
    }
}
