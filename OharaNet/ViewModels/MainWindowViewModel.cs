using OharaNet.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
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
    }
}
