using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Core
{
    internal class TcpChatClient
    {
        private TcpClient? _tcpClient;
        private StreamWriter? _writer;
        private readonly string _myPeerId;

        public bool IsConnected => _tcpClient?.Connected == true;

        public TcpChatClient(string myPeerId)
        {
            _myPeerId = myPeerId;
        }

        public async Task<bool> ConnectAsync(string ipAddress, int port)
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ipAddress, port);

                var stream = _tcpClient.GetStream();
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Send handshake
                await _writer.WriteLineAsync($"HANDSHAKE|{_myPeerId}");

                Console.WriteLine($"Connected to peer at {ipAddress}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to {ipAddress}:{port} - {ex.Message}");
                return false;
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (_writer != null && IsConnected)
            {
                try
                {
                    await _writer.WriteLineAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message: {ex.Message}");
                }
            }
        }

        public void Disconnect()
        {
            _writer?.Close();
            _tcpClient?.Close();
            _writer = null;
            _tcpClient = null;
        }
    }
}