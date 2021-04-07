using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RpcTest.Server
{
    public class Server
    {
        internal async Task StartAsync(CancellationToken cancellationToken)
        {
            int clientId = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Waiting for client.");

                var stream = new NamedPipeServerStream("JsonRpcTest", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await stream.WaitForConnectionAsync();

                Task nowait = RespondToRpcRequestsAsync(stream, ++clientId);
            }
        }

        private async Task RespondToRpcRequestsAsync(Stream stream, int clientId)
        {
            Console.WriteLine($"Connection made, clientId = {clientId}");

            var cancellationTokenSource = new CancellationTokenSource();

            var formatter = new JsonMessageFormatter(Encoding.UTF8);
            //formatter.ProtocolVersion = new Version(1, 0);

            var handler = new NewLineDelimitedMessageHandler(stream, stream, formatter)
            {
                NewLine = NewLineDelimitedMessageHandler.NewLineStyle.Lf
            };

            var jsonRpc = new JsonRpc(handler);
            jsonRpc.AddLocalRpcTarget(new Notifications(cancellationTokenSource.Token));
            jsonRpc.StartListening();

            jsonRpc.TraceSource = new TraceSource("ServerTraceSource")
            {
                Switch = new SourceSwitch("ServerSourceSwitch", "Verbose")
            };
            jsonRpc.TraceSource.Listeners.Add(new ConsoleTraceListener());

            await jsonRpc.Completion;
            
            cancellationTokenSource.Cancel();
            
            Console.WriteLine($"Connection terminated, clientId = {clientId}");
        }
    }
}
