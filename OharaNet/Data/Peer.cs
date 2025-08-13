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
        private bool _isOnline = false;
        private bool _hasUnreadMessages = false;
        private DateTime _lastSeen = DateTime.Now;

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

        public DateTime LastSeen
        {
            get => _lastSeen;
            set
            {
                _lastSeen = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LastSeenDisplay)); // Notify UI that display text changed
            }
        }

        public string LastSeenDisplay
        {
            get
            {
                if (IsOnline)
                    return "Online";

                var timeDiff = DateTime.Now.Subtract(LastSeen);

                if (timeDiff.TotalMinutes < 1)
                    return "Last seen just now";
                else if (timeDiff.TotalMinutes < 60)
                    return $"Last seen {(int)timeDiff.TotalMinutes}m ago";
                else if (timeDiff.TotalHours < 24)
                    return $"Last seen {(int)timeDiff.TotalHours}h ago";
                else
                    return $"Last seen {(int)timeDiff.TotalDays}d ago";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}