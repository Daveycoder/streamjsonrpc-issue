using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RpcTest.Client
{
    public class Client
    {
        internal async Task ConnectAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Connecting to server...");

            var stream = new NamedPipeClientStream(".", "JsonRpcTest", PipeDirection.InOut, PipeOptions.Asynchronous);
            await stream.ConnectAsync();

            var formatter = new JsonMessageFormatter(Encoding.UTF8);
            //formatter.ProtocolVersion = new Version(1, 0);

            var handler = new NewLineDelimitedMessageHandler(stream, stream, formatter)
            {
                NewLine = NewLineDelimitedMessageHandler.NewLineStyle.Lf
            };

            var jsonRpc = new JsonRpc(handler);
            jsonRpc.StartListening();

            jsonRpc.TraceSource = new TraceSource("ClientTraceSource")
            {
                Switch = new SourceSwitch("ClientSourceSwitch", "Verbose")
            };
            jsonRpc.TraceSource.Listeners.Add(new ConsoleTraceListener());

            await jsonRpc.Completion.WithCancellation(cancellationToken);
        }

        public void Tick(int usercount) => Console.WriteLine($"Tick: {usercount}");
    }
}
