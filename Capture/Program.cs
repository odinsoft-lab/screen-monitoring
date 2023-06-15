using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Capture;

#pragma warning disable

public class Program
{
    

    static void Main(string[] args)
    {
        var p = new Program();

        var data = p.CaptureScreenToJpegByteArray();
        p.SendData("192.168.0.5", 8088, data);

        Console.WriteLine("hit to exit...");
    }
}