using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

#pragma warning disable

namespace Capture.Shared
{
    public class Sender
    {
        public Bitmap CaptureFrame()
        {
            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }

            return bitmap;
        }

        public byte[] CaptureScreen()
        {
            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public byte[] CompressData(byte[] data)
        {
            using var ms = new MemoryStream();
         
            using (var gs = new GZipStream(ms, CompressionMode.Compress))
            {
                gs.Write(data, 0, data.Length);
            }

            return ms.ToArray();
        }

        public async Task SendDataByUDP(string serverIp, int serverPort, byte[] data, int packetSize)
        {
            try
            {
                var end_point = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);


                using (UdpClient udp_client = new UdpClient())
                {
                    var lastno = data.Length / packetSize + (data.Length % packetSize > 0 ? 1 : 0);
                    var offset = 0;

                    for (int i = 0; i < lastno; i++)
                    {
                        var size = Math.Min(packetSize, data.Length - offset);
                        var packet_data = new byte[size];
                        Array.Copy(data, offset, packet_data, 0, size);

                        offset += size;

                        var packet = new Packet { seqno = i, size = packetSize, lastno = lastno, data = packet_data };
                        var send_data = SerializePacket(packet);
                        udp_client.Send(send_data, send_data.Length, end_point);

                        await Task.Delay(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        byte[] SerializePacket(Packet packet)
        {
            string jsonString = JsonSerializer.Serialize(packet);
            return Encoding.UTF8.GetBytes(jsonString);
        }
    }
}