namespace Capture;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("hit to capture & send...");
        Console.ReadLine();

        var sender = new Shared.Sender();

        var data = sender.CaptureScreen();

        Console.WriteLine($"send data size: {data.Length}");

        sender.SendData("192.168.0.5", 8088, data, 1024);

        Console.WriteLine("hit to exit...");
        Console.ReadLine();
    }
}