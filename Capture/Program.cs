namespace Capture;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("hit to capture & send...");
        Console.ReadLine();

        var sender = new Shared.Sender();

        var data = sender.CaptureScreen();

        Console.WriteLine($"send data size: {data.Length}");

        await sender.SendDataByTCP("192.168.0.5", 8088, data);

        Console.WriteLine("hit to exit...");
        Console.ReadLine();
    }
}