using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Core
{
    internal class UdpPeerAnnouncer
    {
        private readonly UdpClient? _udpClient;
        private readonly IPEndPoint? _destinationEndPoint;
        private readonly string? _peerId;
        private readonly int _tcpPort;
        private readonly TimeSpan _announcementInterval;

        private CancellationTokenSource? _cancellationTokenSource;


        public UdpPeerAnnouncer(string multicastAddress, int udpPort, string peerId, int tcpPort, TimeSpan announcementInterval)
        {
            _peerId = peerId;
            _tcpPort = tcpPort;
            _announcementInterval = announcementInterval;
            _udpClient = new UdpClient();
            _destinationEndPoint = new IPEndPoint(IPAddress.Parse(multicastAddress), udpPort);
            //Setting TTL to 1 to limit the multicast packets to the local network segment
            _udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
            Console.WriteLine("Announcer initialized. Ready to send announcements.");
        }

        public void Start()
        {
            Console.WriteLine("Starting announcer on a background thread...");
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => AnnounceLoop(_cancellationTokenSource.Token));
        }


        public void Stop()
        {
            Console.WriteLine("Stopping announcer...");
            _cancellationTokenSource?.Cancel();
            _udpClient.Close();
            _udpClient.Dispose();
        }

        private async Task AnnounceLoop(CancellationToken token)
        {
            Console.WriteLine("... Announce loop started.");
            //Simple Message
            string message = $"PEER|{_peerId}|{_tcpPort}";
            byte[] data = Encoding.UTF8.GetBytes(message);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Broadcast our presence message to the multicast group.
                    await _udpClient.SendAsync(data, _destinationEndPoint, token);
                    //Console.WriteLine("--> Announced presence.");

                    //Waiting for the next announcement interval. or cancellation.
                    await Task.Delay(_announcementInterval, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when Stop() is called.
                Console.WriteLine("... Announce loop cancelled successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in the announce loop: {ex.Message}");
            }
        }
    }
}
