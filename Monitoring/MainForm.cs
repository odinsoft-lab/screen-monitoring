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
                    //Debug.WriteLine("listen...");

                    listener.Start();

                    var packet = await receiver.RecvDataByTCP(listener);

                    this.BeginInvoke(() =>
                    {
                        //Debug.WriteLine($"recv data: {packet.data.Length}");
                        pictureBox1.Image = packet.image;
                    });

                    //listener.Stop();

                    await Task.Delay(0);
                }
            },
            TaskCreationOptions.LongRunning
            );
        }
    }
}