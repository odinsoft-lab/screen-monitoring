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
                    listener.Start();

                    try
                    {
                        using TcpClient client = await listener.AcceptTcpClientAsync();
                        using NetworkStream stream = client.GetStream();

                        while (true)
                        {
                            // Get the size of the next data packet from the first 4 bytes
                            byte[] sizeBytes = new byte[4];
                            await stream.ReadAsync(sizeBytes, 0, 4);
                            int size = BitConverter.ToInt32(sizeBytes, 0);

                            // Read the actual data
                            byte[] data = new byte[size];
                            int bytesRead = 0;
                            while (bytesRead < size)
                            {
                                bytesRead += await stream.ReadAsync(data, bytesRead, size - bytesRead);
                            }

                            //Debug.WriteLine($"recv data: {data.Length}");

                            var image = receiver.DecompressToImage(data);

                            this.BeginInvoke(() =>
                            {
                                pictureBox1.Image = image;
                            });

                            await Task.Delay(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error receiving data: {ex.Message}");
                    }
                    finally
                    {
                        listener.Stop();
                    }

                    await Task.Delay(1000);
                }
            },
            TaskCreationOptions.LongRunning
            );
        }
    }
}