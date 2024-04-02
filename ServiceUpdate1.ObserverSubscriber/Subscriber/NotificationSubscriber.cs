using ServiceUpdate1.ObserverSubscriber.Event;

namespace ServiceUpdate1.ObserverSubscriber.Subscriber
{
    public class NotificationSubscriber : IObserver<UpdateServiceEvent>
    {
        public string SubscriberName { get; private set; }
        private IDisposable _unsubscriber;

        public NotificationSubscriber(string _subscriberName)
        {
            SubscriberName = _subscriberName;
        }

        public virtual void Subscribe(IObservable<UpdateServiceEvent> provider)
        {
            // Subscribe to the Observable
            if (provider != null)
                _unsubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("Done");
        }

        public virtual void OnError(Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }

        public virtual void OnNext(UpdateServiceEvent ev)
        {
            Console.WriteLine($"Hey {SubscriberName} -> you received {ev.EventProviderName} {ev.ServiceName} @ {ev.ServiceUpdateDate} ");
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }
    }
}
