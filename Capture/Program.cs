using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Capture;

#pragma warning disable

public class Program
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

    public void SendPackets(UdpClient client, IPEndPoint remoteEndPoint, byte[] data, int packetSize)
    {
        for (int i = 0; i < data.Length; i += packetSize)
        {
            int size = Math.Min(packetSize, data.Length - i);
            byte[] packet = new byte[size];
            Array.Copy(data, i, packet, 0, size);
            client.Send(packet, packet.Length, remoteEndPoint);
        }
    }

    public void SendData(string serverIp, int serverPort, byte[] data, int packetSize)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.Client.SendBufferSize = packetSize;

        try
        {

            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            SendPackets(udpClient, remoteIpEndPoint, data, packetSize);
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

    static void Main(string[] args)
    {
        var p = new Program();

        Console.WriteLine("hit to capture & send...");
        Console.ReadLine();

        var data = p.CaptureScreenToJpegByteArray();

        Console.WriteLine($"send data size: {data.Length}");

        p.SendData("192.168.0.5", 8088, data, 1024);

        Console.WriteLine("hit to exit...");
        Console.ReadLine();
    }
}