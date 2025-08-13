using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Core
{
    internal class UdpPeerListener
    {
        private readonly UdpClient? _udpClient;
        private readonly IPAddress? _multicastAddress;
        private readonly int _port;

        private CancellationTokenSource? _cancellationTokenSource;
        public event Action<string, IPEndPoint>? MessageReceived;


        public UdpPeerListener(string multicastAddress, int port)
        {
            _multicastAddress = IPAddress.Parse(multicastAddress);
            _port = port;
            _udpClient = new UdpClient();
            // Allows to bind to the same port on the same machine. This is crucial for testing.
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // Bind the client to listen on any local IP
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, _port));
            // Join the multicast group
            _udpClient.JoinMulticastGroup(_multicastAddress);

            Console.WriteLine("Listener initialized. Ready to receive messages.");
        }

        public void Start()
        {
            Console.WriteLine("Starting listener on a background thread...");

            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();


            Task.Run(() => ListenLoop(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            Console.WriteLine("Stopping listener...");
            _cancellationTokenSource?.Cancel();
            _udpClient.Close();
            _udpClient.Dispose();
        }

        private async Task ListenLoop(CancellationToken token)
        {
            Console.WriteLine("... Listening loop started.");
            try
            {
                while (!token.IsCancellationRequested)
                {
                    //Throws operation cancelled exception and used it to break the loop

                    UdpReceiveResult result = await _udpClient.ReceiveAsync(token);                   
                    string message = Encoding.UTF8.GetString(result.Buffer);
                    MessageReceived?.Invoke(message, result.RemoteEndPoint);
                }
            }
            catch (OperationCanceledException)
            {
               
                Console.WriteLine("... Listening loop cancelled successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in the listening loop: {ex.Message}");
            }


        }
    }
}