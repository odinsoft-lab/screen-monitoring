using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

#pragma warning disable

namespace Shared
{
    public class Receiver
    {

        public async Task ReceiveData(int listenPort)
        {
            UdpClient udpClient = new UdpClient(listenPort);
            udpClient.Client.ReceiveBufferSize = 1024 * 1024; // 1MB

            try
            {
                while (true)
                {
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receiveBytes = udpClient.Receive(ref remoteIpEndPoint);

                    // Process received data
                    var capture = ByteArrayToImage(receiveBytes);
                    this.pictureBox1.Image = capture;

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                udpClient.Close();
            }
        }

        public Image ByteArrayToImage(byte[] byteArray)
        {
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return Image.FromStream(memoryStream);
            }
        }
    }
}