using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Core
{
    internal class TcpChatServer
    {
        private readonly TcpListener? _listener;
        private CancellationTokenSource? _cancellationTokenSource;
        public event Action<string, string>? MessageReceived;
        public int ListeningPort { get; private set; }

        public TcpChatServer()
        {
            // Initialize the TCP listener on a random port.
            _listener = new TcpListener(IPAddress.Any, 0);
        }

        public void Start()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested) return;

            _cancellationTokenSource = new CancellationTokenSource();
            _listener.Start();
            this.ListeningPort = ((IPEndPoint)_listener.LocalEndpoint).Port; // Get the port the listener is bound to.
            Console.WriteLine($"TCP Chat Server started. Listening on port {this.ListeningPort}...");
            // Start the main listening loop in the background.
            Task.Run(() => ListenForConnections(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _listener.Stop();
            Console.WriteLine("TCP Chat Server stopped.");
        }

        private async Task ListenForConnections(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Wait for a client to connect.
                    TcpClient connectedClient = await _listener.AcceptTcpClientAsync(token);
                    Console.WriteLine("--> [TCP] Accepted a new chat connection.");

                    // Handle the connected client in a separate task to allow multiple connections.
                    _ = Task.Run(() => HandleClient(connectedClient, token), token);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TCP listener loop: {ex.Message}");
            }
        }

        private async Task HandleClient(TcpClient client, CancellationToken token)
        {
            string peerId = "Unknown Peer";
            try
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Read the initial handshake message from the client.
                    var handshake = await reader.ReadLineAsync(token);
                    if (handshake != null && handshake.StartsWith("HANDSHAKE|"))
                    {
                        peerId = handshake.Split('|')[1];
                        Console.WriteLine($"--> [TCP] Handshake complete with {peerId.Substring(0, 10)}...");
                    }
                    else
                    {
                        Console.WriteLine("--> [TCP] Invalid handshake. Closing connection.");
                        return; 
                    }

                    // Now, loop to receive chat messages until the stream is closed.
                    while (!token.IsCancellationRequested)
                    {
                        var message = await reader.ReadLineAsync(token);
                        if (message == null)
                        {
                            
                            break;
                        }
                        
                        MessageReceived?.Invoke(peerId, message);
                    }
                }
            }
            catch (OperationCanceledException) { /* Expected */ }
            catch (IOException)
            {
                // This often happens if the other side disconnects unexpectedly.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {peerId}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"--> [TCP] Chat session ended with {peerId.Substring(0, 10)}...");
            }
        }


    }
}
