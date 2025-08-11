using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Data
{
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _username = "";
        private string _content = "";
        private string _timestamp = "";
        private string _avatarLetter = "";
        private string _avatarColor = "";
        private string _avatarTextColor = "White";

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Content
        {
            get => _content;
            set { _content = value; OnPropertyChanged(); }
        }

        public string Timestamp
        {
            get => _timestamp;
            set { _timestamp = value; OnPropertyChanged(); }
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
