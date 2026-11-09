using OrderMatchingEngine.Enums;
using OrderMatchingEngine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderMatchingEngine_test
{
    public class OrderBook_DeterminismTest
    {
     
        /*
        Determinism tests
        This is one of the most valuable tests for your project.

        The idea:
        1. Generate 10,000 commands using a fixed random seed.
        2. Create two fresh matching engine instances.
        3. Feed the exact same commands to both engines.
        4. Capture every returned event.
        5. Compare the complete event sequences.

        */


        public static TheoryData<int, int> GenerationCount => new TheoryData<int, int>
        {
            //{ 100, 12345 },
            { 1000000, 23456 }
        };


        [Theory]
        [MemberData(nameof(GenerationCount))]
        public void CreateOrder_DeterministicTest(int OrderCount, int seed)
        {
            //  Arrange         
            // 1.create 2 new instance of Orderbook 
            OrderBook _firstinstance = new();
            OrderBook _secondInstance = new();

            // 1.Lets generate the input 
            var buyorderList = new List<OrderInfo>(OrderCount);
            var sellorderList = new List<OrderInfo>(OrderCount);


            var rand = new Random(seed);
            var dataTime = DateTime.Now; // this is fine, since it is not used for logic processing

            for (int i = 1; i <=OrderCount; i++)
            {
                var buyOrder = new OrderInfo( i, rand.Next(100, 120), rand.Next(50, 150), orderType.buy);
                var sellOrder = new OrderInfo( i+OrderCount, rand.Next(100, 120), rand.Next(50, 150), orderType.sell);

                buyorderList.Add( buyOrder) ;
                sellorderList.Add( sellOrder) ;
            }


            // ACT
            // Loop through and hit both the Engine.
            int pt1 = 0;
            int pt2 = 0;

            while ( pt1 < OrderCount || pt2 < OrderCount)
            {
                var choice = rand.Next(0, 2);

                if (pt1 < OrderCount && (choice == 0 || pt2 >= OrderCount))
                {
                    var buyResponse1 = _firstinstance.CreateOrder(buyorderList[pt1], dataTime);
                    var buyResponse2 = _secondInstance.CreateOrder(buyorderList[pt1], dataTime);

                    Assert.True(
                        buyResponse1.SequenceEqual(buyResponse2),
                        $"Buy mismatch at index {pt1}, seed: {seed}"
                    );

                    pt1++;
                    Console.WriteLine(String.Join(Environment.NewLine, buyResponse1));
                }
                else
                {
                    var sellResponse1 = _firstinstance.CreateOrder(sellorderList[pt2], dataTime);
                    var sellResponse2 = _secondInstance.CreateOrder(sellorderList[pt2], dataTime);

                    Assert.True(
                        sellResponse1.SequenceEqual(sellResponse2),
                        $"Sell mismatch at index {pt2}, seed: {seed}"
                    );

                    pt2++;
                    Console.WriteLine(String.Join(Environment.NewLine, sellResponse1));
                }
            }
        }
    }
}
