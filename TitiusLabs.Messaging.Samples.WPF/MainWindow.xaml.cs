using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TitiusLabs.Messaging;

namespace TitiusLabs.Messaging.Samples.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBus.Current.Subscribe((TimeMessage message) =>
            {
                label1.Content = message.Timestamp;
            });
            MessageBus.Current.Subscribe((TimeMessage message) =>
            {
                label2.Content = message.Timestamp;
            });
            MessageBus.Current.Subscribe((TextMessage message) =>
            {
                label2.Content = message.Text;
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(3000);
                    MessageBus.Current.Post(new TimeMessage
                    {
                        Timestamp = DateTime.Now.ToString()
                    });
                }
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    MessageBus.Current.Post(new TextMessage
                    {
                        Text = "Hi, Fabio"
                    });
                }
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(8000);
                    MessageBus.Current.Post(new TimeMessage
                    {
                        Timestamp = "Hi, All"
                    });
                }
            });
        }

        public class TimeMessage : IMessage
        {
            public string Timestamp { get; set; }
        }

        public class TextMessage : IMessage
        {
            public string Text { get; set; }
        }
    }
}
