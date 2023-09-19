using Renci.SshNet;
namespace sshTunel
{
    internal class Program
    {
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static ManualResetEvent _exitTimeout = new ManualResetEvent(false);
        private static SshClient _client = new SshClient("zistroyesh.zser.ir", 2233, "root", "qwedcxza");
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to Stop!");

            _client.Connect();
            //var port = new ForwardedPortLocal(2081, "127.0.0.1", 8098);
            var port = new ForwardedPortDynamic(2081);
            _client.AddForwardedPort(port);
            port.Start();


            //client.KeepAliveInterval.
            while (_client.IsConnected && !Console.KeyAvailable)
            {
                Console.WriteLine(port.BoundHost + port.BoundPort);
                System.Threading.Thread.Sleep(1000);
            }
            _client.Disconnect();

            //DNS2SOCKS.exe 127.0.0.1:2081 1.1.1.1
        }
    }
}