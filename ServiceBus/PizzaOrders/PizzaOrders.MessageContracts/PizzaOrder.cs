namespace PizzaOrders.MessageContracts
{
    public class PizzaOrder
    {
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
    }
}