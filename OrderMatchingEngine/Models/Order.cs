using OrderMatchingEngine.Enums;

namespace OrderMatchingEngine.Models
{

    internal struct OrderInfo
    {
        public long id { get; set; }
        public long price { get; set; }
        public long quantity { get; set; }
        public long remainingQuantity { get; set; }
        public orderType Side;

        public OrderInfo(long _id, long _price, long _quant, orderType _side)
        {
            id = _id;
            price = _price;
            quantity = _quant;
            remainingQuantity = _quant;
            Side = _side;
        }
    }
}
