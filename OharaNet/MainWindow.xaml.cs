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
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            // Handle peer selection clicks
            OnlinePeersContainer.MouseLeftButtonUp += OnlinePeersContainer_MouseLeftButtonUp;

            // Handle message input
            MessageInputTextBox.KeyDown += MessageInputTextBox_KeyDown;

            // Handle placeholder text
            MessageInputTextBox.GotFocus += MessageInputTextBox_GotFocus;
            MessageInputTextBox.LostFocus += MessageInputTextBox_LostFocus;
        }

        private async void OnlinePeersContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Find the clicked peer
            var element = e.OriginalSource as FrameworkElement;
            while (element != null && !(element.DataContext is Peer))
            {
                element = element.Parent as FrameworkElement;
            }

            if (element?.DataContext is Peer clickedPeer)
            {
                await _viewModel.SelectPeerAsync(clickedPeer);
            }
        }

        private async void MessageInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(MessageInputTextBox.Text))
            {
                string message = MessageInputTextBox.Text.Trim();

                if (message != _viewModel.MessagePlaceholder)
                {
                    await _viewModel.SendMessageAsync(message);
                    MessageInputTextBox.Text = "";
                }
            }
        }

        private void MessageInputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MessageInputTextBox.Text == _viewModel.MessagePlaceholder)
            {
                MessageInputTextBox.Text = "";
                MessageInputTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            }
        }

        private void MessageInputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageInputTextBox.Text))
            {
                MessageInputTextBox.Text = _viewModel.MessagePlaceholder;
                MessageInputTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B9BBBE"));
            }
        }

        private void InitializeData()
        {
            // Initialize current channel
            _viewModel.CurrentChannel = new ChannelInfo
            {
                Name = "general-chat",
                Description = "Select a peer to start chatting"
            };

            // Welcome message
            _viewModel.Messages.Add(new ChatMessage
            {
                Username = "System",
                Content = "Welcome to OharaNet! Click on a peer to start chatting.",
                Timestamp = DateTime.Now.ToString("'Today at' h:mm tt"),
                AvatarLetter = "S",
                AvatarColor = "#FF747F8D",
                AvatarTextColor = "White"
            });

            _viewModel.UpdateOnlinePeersCount();
            _viewModel.UpdateOfflinePeersCount();
        }
    }
}