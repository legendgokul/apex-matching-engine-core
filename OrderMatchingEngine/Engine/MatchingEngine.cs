using OrderMatchingEngine.Interfaces;
using OrderMatchingEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Engine
{
    public class MatchingEngine : IMatchingEngine
    {
        /*public IReadOnlyList<Trade> SubmitOrder(OrderInfo order)
        {

        }*/
        IReadOnlyList<Trade> IMatchingEngine.SubmitOrder(OrderInfo order)
        {
            throw new NotImplementedException();
        }
    }
}
