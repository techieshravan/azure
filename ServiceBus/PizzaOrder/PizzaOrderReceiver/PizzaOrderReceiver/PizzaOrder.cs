using System.Runtime.Serialization;

namespace PizzaOrderCreator
{
    [DataContract]
    public class PizzaOrder
    {
        [DataMember]
        public string CustomerName { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Size { get; set; }
        
        [DataMember]
        public int Quantity { get; set; }
    }
}