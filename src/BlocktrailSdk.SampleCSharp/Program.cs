using BlocktrailSdk.Models;
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

            Block specificBlock = BlocktrailSdk.Client.GetBlock("00000000000000000b0d6b7a84dd90137757db3efbee2c4a226a802ee7be8947");
            var blockTransactions = specificBlock.GetTransactions(page: 1, limit: 100);

            string transactionHash = blockTransactions.First().Hash;

            Transaction refetchTransaction = BlocktrailSdk.Client.GetTransaction(transactionHash);
            Block transactionBlock = refetchTransaction.Block;

            // Print nr of confirmations
            Console.WriteLine("Transaction: " + refetchTransaction.Hash);
            Console.WriteLine("\t# confirmations: " + refetchTransaction.Confirmations);
            Console.WriteLine("\tBlock: " + transactionBlock.Height + Environment.NewLine);

            Address addrObj = BlocktrailSdk.Client.GetAddress("1CjPR7Z5ZSyWk6WtXvSFgkptmpoi4UM9BC");

            var addrTrans = addrObj.GetTransactions();
            var uncAddrTrans = addrObj.GetUnconfirmedTransactions();
            var unspOutTrans = addrObj.GetUnspentOutputs();

            Console.WriteLine("Address: " + addrObj.Hash160);
            Console.WriteLine("\tBalance: " + addrObj.Balance + Environment.NewLine);

            Console.WriteLine("Press a key to fetch transactions of block: " + specificBlock.Height);
            Console.ReadKey();

            // Start printing rows of transactions
            int i = 0;

            var transactions = specificBlock.GetTransactions(1, 100);

            foreach (var item in transactions)
            {
                Console.WriteLine(String.Format("[{0}/{1}] {2}", ++i, transactions.Total, item.Hash));
            }

            Console.WriteLine("Press a key to fetch the next page of results.");
            Console.ReadKey();

            do
            {
                transactions = transactions.NextPage();

                foreach (var item in transactions)
                {
                    Console.WriteLine(String.Format("[{0}/{1}] {2}", ++i, transactions.Total, item.Hash));
                }

                Console.WriteLine("Press a key to fetch the next page of results.");
                Console.ReadKey();
            } while (transactions.NextPageAvailable());

            Console.WriteLine("Press a key to close the sample.");
            Console.ReadKey();
        }
    }
}
