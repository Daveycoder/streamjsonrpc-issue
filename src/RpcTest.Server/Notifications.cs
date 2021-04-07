using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpcTest.Server
{
    public class Notifications
    {
        public event EventHandler<int> Tick;

        public Notifications(CancellationToken cancellationToken)
        {
            Task nowait = SendTicksAsync(cancellationToken);
        }

        public async Task SendTicksAsync(CancellationToken cancellationToken)
        {
            int tickNumber = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"Sending Tick {tickNumber}");
                await Task.Delay(1000, cancellationToken);
                Tick?.Invoke(this, ++tickNumber);
            }
        }
    }
}
