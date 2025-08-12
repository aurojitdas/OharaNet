using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Data
{
    public class Peer : INotifyPropertyChanged
    {
        private string _name = "";
        private string _ipAddress = "";
        private string _avatarLetter = "";
        private string _avatarColor = "";
        private string _avatarTextColor = "White";
        private string _statusColor = "";
        private string _lastSeen = "";
        private bool _isOnline = false;
        private bool _hasUnreadMessages = false;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); }
        }

        public string AvatarLetter
        {
            get => _avatarLetter;
            set { _avatarLetter = value; OnPropertyChanged(); }
        }

        public string AvatarColor
        {
            get => _avatarColor;
            set { _avatarColor = value; OnPropertyChanged(); }
        }

        public string AvatarTextColor
        {
            get => _avatarTextColor;
            set { _avatarTextColor = value; OnPropertyChanged(); }
        }

        public string StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(); }
        }

        public string LastSeen
        {
            get => _lastSeen;
            set { _lastSeen = value; OnPropertyChanged(); }
        }

        public bool IsOnline
        {
            get => _isOnline;
            set { _isOnline = value; OnPropertyChanged(); }
        }

        public bool HasUnreadMessages
        {
            get => _hasUnreadMessages;
            set { _hasUnreadMessages = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}