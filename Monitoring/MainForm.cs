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

                while (true)
                {
                    var listener = new TcpListener(IPAddress.Any, 8088);

                    try
                    {
                        while (true)
                        {
                            //Debug.WriteLine("listen...");

                            listener.Start();

                            var data = await receiver.RecvDataByTCP(listener);
                            var image = receiver.ByteArrayToImage(data);

                            this.BeginInvoke(() =>
                            {
                                pictureBox1.Image = image;
                            });

                            await Task.Delay(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        listener.Stop();

                        await Task.Delay(1000);
                    }
                }
            },
            TaskCreationOptions.LongRunning
            );
        }
    }
}