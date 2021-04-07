using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpcTest.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server();
            var cancellationTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cancellationTokenSource.Cancel();
                e.Cancel = true;
            };

            try
            {
                Console.WriteLine("Press Ctrl+C to end.");
                await server.StartAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) { }

        }
    }
}
