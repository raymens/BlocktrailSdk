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
            

            var client = new BlocktrailDataClient(apiKey);

            var transaction = client.Transaction("c326105f7fbfa4e8fe971569ef8858f47ee7e4aa5e8e7c458be8002be3d86aad");
            var block = client.Block(transaction.block_hash);
            var address = client.Address(transaction.inputs[0].address);

            Console.WriteLine(transaction.hash);
            Console.WriteLine(block.hash);
            Console.WriteLine(address.hash160);

            var blockTransacctions = client.BlockTransactions(block.hash, 0, 100, "asc");

            Console.WriteLine(blockTransacctions.Total + " " + blockTransacctions.Page + " " + blockTransacctions.Limit);

            var nextPage = blockTransacctions.NextPage();
            var samePage = blockTransacctions.GetPage(2);

            Console.WriteLine(nextPage.Data[0].hash);
            Console.WriteLine(samePage.Data[0].hash);

            Console.Read();
        }
    }
}
