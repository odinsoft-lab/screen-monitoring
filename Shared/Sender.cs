using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Windows.Forms;

#pragma warning disable

namespace Shared
{
    public class Sender
    {
        public byte[] CaptureScreenToJpegByteArray()
        {
            var bounds = Screen.GetBounds(Point.Empty);
            var bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        public void SendData(string serverIp, int serverPort, byte[] data)
        {
            UdpClient udpClient = new UdpClient();

            try
            {
                udpClient.Connect(serverIp, serverPort);
                udpClient.Send(data, data.Length);
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
    }
}