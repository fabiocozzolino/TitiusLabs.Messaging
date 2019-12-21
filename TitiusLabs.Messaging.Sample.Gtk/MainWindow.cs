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

        Task.Run(async () =>
        {
            while(true)
            {
                await Task.Delay(1000);
                MessageBus.Current.Post(new TimeMessage
                {
                    Timestamp = DateTime.Now.ToString()
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
    }
}
