namespace ServiceUpdate1.ObserverSubscriber.Event
{
    public class UpdateServiceEvent
    {
        public string EventProviderName { get; set; }
        public string ServiceName { get; set; }
        public DateTime ServiceUpdateDate { get; set; }

        public UpdateServiceEvent(string eventProviderName, string serviceName, DateTime serviceUpdateDate)
        {
            EventProviderName = eventProviderName;
            ServiceName = serviceName;
            ServiceUpdateDate = serviceUpdateDate;
        }
    }
}
