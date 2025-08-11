using OharaNet.Data;
using OharaNet.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OharaNet
{   

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializeData();
        }

        private void InitializeData()
        {
            // Initialize current user
            _viewModel.CurrentUser = new UserInfo
            {
                Name = "Your_Node",
                Status = "Online",
                AvatarLetter = "Y",
                AvatarColor = "#FF5865F2",
                AvatarTextColor = "White"
            };

            // Initialize current channel
            _viewModel.CurrentChannel = new ChannelInfo
            {
                Name = "general-chat",
                Description = "P2P Network Chat"
            };

            // Initialize online peers
            _viewModel.OnlinePeers.Add(new Peer
            {
                Name = "Alice_Node",
                IpAddress = "192.168.1.100",
                AvatarLetter = "A",
                AvatarColor = "#FF5865F2",
                AvatarTextColor = "White",
                StatusColor = "#FF3BA55D",
                IsOnline = true
            });

            _viewModel.OnlinePeers.Add(new Peer
            {
                Name = "Bob_Crypto",
                IpAddress = "10.0.0.25",
                AvatarLetter = "B",
                AvatarColor = "#FF57F287",
                AvatarTextColor = "White",
                StatusColor = "#FF3BA55D",
                IsOnline = true
            });

            _viewModel.OnlinePeers.Add(new Peer
            {
                Name = "Charlie_Dev",
                IpAddress = "172.16.0.5",
                AvatarLetter = "C",
                AvatarColor = "#FFFEE75C",
                AvatarTextColor = "Black",
                StatusColor = "#FFFAA61A",
                IsOnline = true
            });

            // Initialize offline peers
            _viewModel.OfflinePeers.Add(new Peer
            {
                Name = "Dave_Offline",
                LastSeen = "Last seen 2h ago",
                AvatarLetter = "D",
                AvatarColor = "#FF747F8D",
                AvatarTextColor = "White",
                IsOnline = false
            });

            // Initialize sample messages
            _viewModel.Messages.Add(new ChatMessage
            {
                Username = "Alice_Node",
                Content = "Hey everyone! The P2P network is running smoothly. Anyone want to test file sharing?",
                Timestamp = "Today at 2:30 PM",
                AvatarLetter = "A",
                AvatarColor = "#FF5865F2",
                AvatarTextColor = "White"
            });

            _viewModel.Messages.Add(new ChatMessage
            {
                Username = "Bob_Crypto",
                Content = "I'm ready to test. My node has been stable for the past 24 hours.",
                Timestamp = "Today at 2:32 PM",
                AvatarLetter = "B",
                AvatarColor = "#FF57F287",
                AvatarTextColor = "White"
            });

            _viewModel.Messages.Add(new ChatMessage
            {
                Username = "Your_Node",
                Content = "Great! Let me know when you're ready to start the file transfer test.",
                Timestamp = "Today at 2:35 PM",
                AvatarLetter = "Y",
                AvatarColor = "#FF5865F2",
                AvatarTextColor = "White"
            });

            // Set selected user (default to Alice)
            _viewModel.SelectedUser = new UserInfo
            {
                Name = "Alice_Node",
                UserId = "alice#1234",
                AvatarLetter = "A",
                AvatarColor = "#FF5865F2",
                AvatarTextColor = "White",
                StatusColor = "#FF3BA55D",
                StatusText = "🟢 Online",
                IpAddress = "192.168.1.100",
                Port = "8080",
                Protocol = "TCP/UDP",
                Latency = "23ms",
                Uptime = "2d 14h 32m",
                MessagesSent = "1,247",
                FilesShared = "23",
                DataTransferred = "2.4 GB"
            };

            // Update peer counts
            _viewModel.UpdateOnlinePeersCount();
            _viewModel.UpdateOfflinePeersCount();
        }

        // Public methods to update data from your P2P network logic
        public void AddOnlinePeer(string name, string ipAddress, string avatarLetter = "", string avatarColor = "#FF5865F2")
        {
            var peer = new Peer
            {
                Name = name,
                IpAddress = ipAddress,
                AvatarLetter = string.IsNullOrEmpty(avatarLetter) ? name.Substring(0, 1).ToUpper() : avatarLetter,
                AvatarColor = avatarColor,
                AvatarTextColor = "White",
                StatusColor = "#FF3BA55D",
                IsOnline = true
            };

            _viewModel.OnlinePeers.Add(peer);
            _viewModel.UpdateOnlinePeersCount();
        }

        public void RemoveOnlinePeer(string name)
        {
            var peer = _viewModel.OnlinePeers.FirstOrDefault(p => p.Name == name);
            if (peer != null)
            {
                _viewModel.OnlinePeers.Remove(peer);
                _viewModel.UpdateOnlinePeersCount();
            }
        }

        public void AddOfflinePeer(string name, string lastSeen, string avatarLetter = "")
        {
            var peer = new Peer
            {
                Name = name,
                LastSeen = lastSeen,
                AvatarLetter = string.IsNullOrEmpty(avatarLetter) ? name.Substring(0, 1).ToUpper() : avatarLetter,
                AvatarColor = "#FF747F8D",
                AvatarTextColor = "White",
                IsOnline = false
            };

            _viewModel.OfflinePeers.Add(peer);
            _viewModel.UpdateOfflinePeersCount();
        }

        public void AddMessage(string username, string content, string avatarLetter = "", string avatarColor = "#FF5865F2")
        {
            var message = new ChatMessage
            {
                Username = username,
                Content = content,
                Timestamp = DateTime.Now.ToString("'Today at' h:mm tt"),
                AvatarLetter = string.IsNullOrEmpty(avatarLetter) ? username.Substring(0, 1).ToUpper() : avatarLetter,
                AvatarColor = avatarColor,
                AvatarTextColor = "White"
            };

            _viewModel.Messages.Add(message);
        }

        public void UpdateSelectedUser(UserInfo userInfo)
        {
            _viewModel.SelectedUser = userInfo;
        }

        public void UpdateCurrentUser(string name, string status = "Online")
        {
            _viewModel.CurrentUser.Name = name;
            _viewModel.CurrentUser.Status = status;
            _viewModel.CurrentUser.AvatarLetter = name.Substring(0, 1).ToUpper();
        }

        public void UpdateNetworkTitle(string title)
        {
            _viewModel.NetworkTitle = title;
        }

        public void UpdateCurrentChannel(string name, string description)
        {
            _viewModel.CurrentChannel.Name = name;
            _viewModel.CurrentChannel.Description = description;
            _viewModel.MessagePlaceholder = $"Message #{name}";
        }
    }
}