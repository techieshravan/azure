using System.Runtime.Serialization;

namespace EventHub
{
    [DataContract]
    public class MetricEvent
    {
        [DataMember]
        public int DeviceId { get; set; }

        [DataMember]
        public int Temperature { get; set; }
    }
}
