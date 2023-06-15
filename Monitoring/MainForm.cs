using System.Diagnostics;
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
                while (true)
                {
                    UdpClient udpClient = new UdpClient(8088);

                    var receiver = new Shared.Receiver();
                    var packet = receiver.RecvData(udpClient);

                    this.BeginInvoke(() =>
                    {
                        Debug.WriteLine($"recv data: {packet.data.Length}");

                        var capture = receiver.ByteArrayToImage(packet.data);
                        pictureBox1.Image = capture;
                    });

                    await Task.Delay(100);
                }
            },
            TaskCreationOptions.LongRunning
            );
        }
    }
}