using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Monitoring
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                var receiver = new Shared.Receiver();

                var listener = new TcpListener(IPAddress.Any, 8088);

                while (true)
                {
                    Debug.WriteLine("listen...");

                    listener.Start();

                    var packet = await receiver.RecvDataByTCP(listener);

                    this.BeginInvoke(() =>
                    {
                        Debug.WriteLine($"recv data: {packet.data.Length}");

                        var capture = receiver.ByteArrayToImage(packet.data);
                        pictureBox1.Image = capture;
                    });

                    listener.Stop();

                    await Task.Delay(5);
                }
            },
            TaskCreationOptions.LongRunning
            );
        }
    }
}