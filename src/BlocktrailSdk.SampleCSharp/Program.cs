using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocktrailSdk.SampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "INSERT_API_KEY_HERE";

            BlocktrailSdk.Config.ApiKey = apiKey;
            var block = BlocktrailSdk.Client.GetBlock("00000000000000000b0d6b7a84dd90137757db3efbee2c4a226a802ee7be8947");
            var transes = block.Transactions(0, 1);

            var transhash = transes[0].hash;

            var transaction = BlocktrailSdk.Client.GetTransaction(transhash);

            Console.WriteLine(transaction.confirmations);


            var block2 = transaction.Block;

            Console.WriteLine(block.hash);
            Console.WriteLine(block2.hash);


            //Console.WriteLine(block.Transactions(0, 1)[0].outputs.Count());

            //var blocks = client.GetAllBlocks(0, 100);

            //Console.WriteLine(blocks.Total);


            Console.Read();

            int i = 0;

            for(var transactions = block.Transactions(0, 1); transactions.NextPageAvailable(); transactions = transactions.NextPage())
            {
                foreach (var item in transactions)
                {
                    Console.WriteLine(String.Format("[{0}/{1}] {2}", ++i, transactions.Total, item.hash));
                }

                Console.ReadLine();
            }

            Console.ReadLine();
        }
    }
}
