using System;
using System.Threading.Tasks;
using Gtk;
using TitiusLabs.Messaging;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected override void OnShown()
    {
        base.OnShown();

        MessageBus.Current.Subscribe((TimeMessage message) =>
        {
            label1.Text = message.Timestamp;
        });

        MessageBus.Current.Subscribe((TimeMessage message) =>
        {
            label3.Text = message.Timestamp;
        });

        MessageBus.Current.Subscribe((AnotherMessage message) =>
        {
            label5.Text = label3.Text = message.Text;
        });

        Task.Run(async () =>
        {
            while(true)
            {
                await Task.Delay(10000);
                MessageBus.Current.Post(new TimeMessage
                {
                    Timestamp = DateTime.Now.ToString()
                });
            }
        });
        
        Task.Run(async () =>
        {
            await Task.Delay(5000);
            while (true)
            {
                await Task.Delay(10000);
                MessageBus.Current.Post(new TimeMessage
                {
                    Timestamp = "Hi"
                });
            }
        });

        Task.Run(async () =>
        {
            await Task.Delay(5000);
            while (true)
            {
                await Task.Delay(4000);
                MessageBus.Current.Post(new AnotherMessage
                {
                    Text = "Hi, Fabio " + DateTime.Now
                });
            }
        });
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    public class TimeMessage : IMessage
    {
        public string Timestamp { get; set; }

        public override string ToString()
        {
            return $"{nameof(TimeMessage)}: {Timestamp}";
        }
    }

    public class AnotherMessage : IMessage
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{nameof(AnotherMessage)}: {Text}";
        }
    }
}
