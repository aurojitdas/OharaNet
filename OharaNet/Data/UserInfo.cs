using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Data
{
    public class UserInfo : INotifyPropertyChanged
    {
        private string _name = "";
        private string _userId = "";
        private string _status = "";
        private string _avatarLetter = "";
        private string _avatarColor = "";
        private string _avatarTextColor = "White";
        private string _statusColor = "";
        private string _statusText = "";
        private string _ipAddress = "";
        private string _port = "";
        private string _protocol = "";
        private string _latency = "";
        private string _uptime = "";
        private string _messagesSent = "";
        private string _filesShared = "";
        private string _dataTransferred = "";

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
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

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); }
        }

        public string Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        public string Protocol
        {
            get => _protocol;
            set { _protocol = value; OnPropertyChanged(); }
        }

        public string Latency
        {
            get => _latency;
            set { _latency = value; OnPropertyChanged(); }
        }

        public string Uptime
        {
            get => _uptime;
            set { _uptime = value; OnPropertyChanged(); }
        }

        public string MessagesSent
        {
            get => _messagesSent;
            set { _messagesSent = value; OnPropertyChanged(); }
        }

        public string FilesShared
        {
            get => _filesShared;
            set { _filesShared = value; OnPropertyChanged(); }
        }

        public string DataTransferred
        {
            get => _dataTransferred;
            set { _dataTransferred = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
