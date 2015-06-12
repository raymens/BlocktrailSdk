using BlocktrailSdk.Models;
using System;
using System.Linq;

namespace BlocktrailSdk.SampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "";
            string apiSecret = "";

            var client = new BlocktrailSdkClient(apiKey, apiSecret);

            var createResult = client.CreateNewWallet("testwallet10", "test", 0);
            var deleteResult = client.DeleteWallet(createResult);

            /*

            Block specificBlock = client.GetBlock("00000000000000000b0d6b7a84dd90137757db3efbee2c4a226a802ee7be8947");
            var blockTransactions = client.GetBlockTransactions(specificBlock.Hash, page: 1, limit: 100);

            string transactionHash = blockTransactions.First().Hash;

            Transaction refetchTransaction = client.GetTransaction(transactionHash);
            Block transactionBlock = client.GetBlock(refetchTransaction.BlockHash);
            
            // Print nr of confirmations
            Console.WriteLine("Transaction: " + refetchTransaction.Hash);
            Console.WriteLine("\t# confirmations: " + refetchTransaction.Confirmations);
            Console.WriteLine("\tBlock: " + transactionBlock.Height + Environment.NewLine);

            Address addrObj = client.GetAddress("1CjPR7Z5ZSyWk6WtXvSFgkptmpoi4UM9BC");

            var addrTrans = client.GetAddressTransactions(addrObj.Hash160);
            var uncAddrTrans = client.GetAddressUnconfirmedTransactions(addrObj.Hash160);
            var unspOutTrans = client.GetAddressUnspentOutputs(addrObj.Hash160);

            Console.WriteLine("Address: " + addrObj.Hash160);
            Console.WriteLine("\tBalance: " + addrObj.Balance + Environment.NewLine);

            Console.WriteLine("Press a key to fetch transactions of block: " + specificBlock.Height);
            Console.ReadKey();

            // Start printing rows of transactions
            int i = 0;

            var transactions = client.GetBlockTransactions(transactionBlock.Hash, 1, 100);

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
            */

            //Console.WriteLine("Press a key to close the sample.");
            Console.ReadKey();
        }
    }
}
