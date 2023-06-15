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
                    var data = receiver.RecvData(udpClient);

                    this.BeginInvoke(() =>
                    {
                        var capture = receiver.ByteArrayToImage(data);
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