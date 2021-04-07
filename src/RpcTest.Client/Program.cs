using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpcTest.Client
{
    class Program
    {
        static async Task Main()
        {
            // Give server time to wakeup.
            Thread.Sleep(1000);

            var client = new Client();
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            try
            {
                Console.WriteLine("Press Ctrl+C to end.");
                await client.ConnectAsync(cts.Token);
            }
            catch (OperationCanceledException) { }
        }
    }
}
