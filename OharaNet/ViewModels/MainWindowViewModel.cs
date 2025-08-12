using OharaNet.Core;
using OharaNet.Core.Data;
using OharaNet.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OharaNet.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        //Core Components
        private readonly UdpPeerListener _udpListener;
        private readonly UdpPeerAnnouncer _udpAnnouncer;
        private readonly TcpChatServer _tcpServer;
        private readonly string _myPeerId;
        private readonly ConcurrentDictionary<string, PeerInfo> _discoveredPeers = new();

        // Simple chat management
        private readonly Dictionary<string, List<ChatMessage>> _chatHistories = new();
        private TcpChatClient? _currentChatClient;
        private string _selectedPeerId = "";

        private string _networkTitle = "P2P Network";
        private string _onlinePeersHeader = "ONLINE PEERS — 0";
        private string _offlinePeersHeader = "OFFLINE PEERS — 0";
        private string _searchPlaceholder = "Search messages...";
        private string _messagePlaceholder = "Message #general-chat";
        private UserInfo _currentUser = new();
        private UserInfo _selectedUser = new();
        private ChannelInfo _currentChannel = new();

      
        public ObservableCollection<Peer> OnlinePeers { get; set; } = new();
        public ObservableCollection<Peer> OfflinePeers { get; set; } = new();
        public ObservableCollection<ChatMessage> Messages { get; set; } = new();

        public string NetworkTitle
        {
            get => _networkTitle;
            set { _networkTitle = value; OnPropertyChanged(); }
        }

        public string OnlinePeersHeader
        {
            get => _onlinePeersHeader;
            set { _onlinePeersHeader = value; OnPropertyChanged(); }
        }

        public string OfflinePeersHeader
        {
            get => _offlinePeersHeader;
            set { _offlinePeersHeader = value; OnPropertyChanged(); }
        }

        public string SearchPlaceholder
        {
            get => _searchPlaceholder;
            set { _searchPlaceholder = value; OnPropertyChanged(); }
        }

        public string MessagePlaceholder
        {
            get => _messagePlaceholder;
            set { _messagePlaceholder = value; OnPropertyChanged(); }
        }

        public UserInfo CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public UserInfo SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public ChannelInfo CurrentChannel
        {
            get => _currentChannel;
            set { _currentChannel = value; OnPropertyChanged(); }
        }

        public void UpdateOnlinePeersCount()
        {
            OnlinePeersHeader = $"ONLINE PEERS — {OnlinePeers.Count}";
        }

        public void UpdateOfflinePeersCount()
        {
            OfflinePeersHeader = $"OFFLINE PEERS — {OfflinePeers.Count}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public MainWindowViewModel()
        {
            // --- Configuration ---
            const string MulticastAddress = "239.0.0.222";
            const int UdpPort = 30000;
            TimeSpan AnnouncementInterval = TimeSpan.FromSeconds(5);

            // --- Initialization ---
            _myPeerId = $"Ohara_Node_{Guid.NewGuid().ToString().Substring(0, 6)}";            
            _tcpServer = new TcpChatServer();
            _tcpServer.Start();
            _udpListener = new UdpPeerListener(MulticastAddress, UdpPort);
            _udpAnnouncer = new UdpPeerAnnouncer(MulticastAddress, UdpPort, _myPeerId, _tcpServer.ListeningPort, AnnouncementInterval);
            _udpListener.MessageReceived += OnUdpMessageReceived;
            _tcpServer.MessageReceived += OnTcpMessageReceived;

            _udpListener.Start();
            _udpAnnouncer.Start();

            // Initialize current user info
            CurrentUser = new UserInfo
            {
                Name = _myPeerId,
                Status = "Online",
                AvatarLetter = _myPeerId.Substring(0, 1).ToUpper(),
                AvatarColor = "#FF5865F2",
                AvatarTextColor = "White"
            };

        }

        // Handle incoming messages from TCP server
        private void OnTcpMessageReceived(string peerId, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Create chat history for this peer if it doesn't exist
                if (!_chatHistories.ContainsKey(peerId))
                {
                    _chatHistories[peerId] = new List<ChatMessage>();
                }

                // Add message to history
                var peer = OnlinePeers.FirstOrDefault(p => p.Name == peerId);
                var chatMessage = new ChatMessage
                {
                    Username = peerId,
                    Content = message,
                    Timestamp = DateTime.Now.ToString("'Today at' h:mm tt"),
                    AvatarLetter = peerId.Substring(0, 1).ToUpper(),
                    AvatarColor = peer?.AvatarColor ?? "#FF5865F2",
                    AvatarTextColor = "White"
                };

                _chatHistories[peerId].Add(chatMessage);

                // If this is NOT the currently selected peer, mark as unread
                if (_selectedPeerId != peerId && peer != null)
                {
                    peer.HasUnreadMessages = true;
                }

                // If this is the currently selected peer, update the UI
                if (_selectedPeerId == peerId)
                {
                    Messages.Add(chatMessage);
                }
            });
        }


        // Select a peer and load chat history
        public async Task SelectPeerAsync(Peer peer)
        {
            // Disconnect from previous peer
            _currentChatClient?.Disconnect();
            _currentChatClient = null;

            _selectedPeerId = peer.Name;

            var peerInCollection = OnlinePeers.FirstOrDefault(p => p.Name == peer.Name);
            if (peerInCollection != null)
            {
                peerInCollection.HasUnreadMessages = false;
            }

            // Update selected user info for right panel
            SelectedUser = new UserInfo
            {
                Name = peer.Name,
                UserId = $"{peer.Name.ToLower()}#{Random.Shared.Next(1000, 9999)}",
                AvatarLetter = peer.AvatarLetter,
                AvatarColor = peer.AvatarColor,
                AvatarTextColor = peer.AvatarTextColor,
                StatusColor = peer.StatusColor,
                StatusText = peer.IsOnline ? "🟢 Online" : "🔴 Offline",
                IpAddress = peer.IpAddress,
                Port = "8080",
                Protocol = "TCP/UDP",
                Latency = "N/A",
                Uptime = "N/A",
                MessagesSent = "0",
                FilesShared = "0",
                DataTransferred = "0 B"
            };

            // Update channel info
            CurrentChannel = new ChannelInfo
            {
                Name = $"chat-{peer.Name}",
                Description = $"Direct message with {peer.Name}"
            };
            MessagePlaceholder = $"Message @{peer.Name}";

            // Load chat history
            Messages.Clear();
            if (_chatHistories.ContainsKey(peer.Name))
            {
                foreach (var msg in _chatHistories[peer.Name])
                {
                    Messages.Add(msg);
                }
            }

            // Connect to peer
            if (_discoveredPeers.ContainsKey(peer.Name))
            {
                var peerInfo = _discoveredPeers[peer.Name];
                _currentChatClient = new TcpChatClient(_myPeerId);

                bool connected = await _currentChatClient.ConnectAsync(peerInfo.IpAddress.ToString(), peerInfo.TcpPort);
                if (connected)
                {
                    Console.WriteLine($"Connected to {peer.Name}");
                }
                else
                {
                    Console.WriteLine($"Failed to connect to {peer.Name}");
                }
            }
        }

        // Send message to selected peer
        public async Task SendMessageAsync(string content)
        {
            if (_currentChatClient != null && _currentChatClient.IsConnected && !string.IsNullOrEmpty(_selectedPeerId))
            {
                // Send to peer
                await _currentChatClient.SendMessageAsync(content);

                // Add to our local chat history
                if (!_chatHistories.ContainsKey(_selectedPeerId))
                {
                    _chatHistories[_selectedPeerId] = new List<ChatMessage>();
                }

                var myMessage = new ChatMessage
                {
                    Username = _myPeerId,
                    Content = content,
                    Timestamp = DateTime.Now.ToString("'Today at' h:mm tt"),
                    AvatarLetter = _myPeerId.Substring(0, 1).ToUpper(),
                    AvatarColor = CurrentUser.AvatarColor,
                    AvatarTextColor = "White"
                };

                _chatHistories[_selectedPeerId].Add(myMessage);
                Messages.Add(myMessage);
            }
        }
        // Handle UDP peer discovery
        private void OnUdpMessageReceived(string message, IPEndPoint senderEndPoint)
        {
            string[] parts = message.Split('|');
            if (parts.Length == 3 && parts[0] == "PEER")
            {
                string receivedPeerId = parts[1];
                if (receivedPeerId == _myPeerId) return; // Ignore our own announcements

                if (int.TryParse(parts[2], out int receivedTcpPort))
                {
                    var peerInfo = new PeerInfo(senderEndPoint.Address, receivedTcpPort);
                    _discoveredPeers[receivedPeerId] = peerInfo; // Store or update the raw network info

                    // Use the Dispatcher to safely update the UI from this background thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Check if this peer is already in our UI list
                        var existingPeer = OnlinePeers.FirstOrDefault(p => p.Name == receivedPeerId);
                        if (existingPeer == null)
                        {
                            // It's a new peer, so create a Peer object for the UI
                            var newPeer = new Peer
                            {
                                Name = receivedPeerId,
                                IpAddress = senderEndPoint.Address.ToString(),
                                IsOnline = true,
                                StatusColor = "#FF3BA55D", // Green for online
                                AvatarLetter = receivedPeerId.Substring(0, 1).ToUpper(),
                                AvatarColor = GetRandomColorForPeer(receivedPeerId)
                            };

                            OnlinePeers.Add(newPeer);
                            UpdateOnlinePeersCount();
                        }
                    });
                }
            }
        }

        // Method to get a random color for a peer based on their ID
        private string GetRandomColorForPeer(string peerId)
        {
            var random = new Random(peerId.GetHashCode());
            return $"#FF{random.Next(0x1000000):X6}";
        }
    }
}
