
using OrderMatchingEngine.Models;

namespace OrderMatchingEngine.Interfaces
{
    internal interface IMatchingEngine
    {
            IReadOnlyList<Trade> SubmitOrder(OrderInfo order);

    }
}
