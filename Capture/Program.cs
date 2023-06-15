namespace Capture;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("hit to capture & send...");
        Console.ReadLine();

        var sender = new Shared.Sender();

        while (true)
        {
            var data = sender.CaptureScreen();

            Console.WriteLine($"send data size: {data.Length}");

            await sender.SendDataByTCP("127.0.0.1", 8088, data);

            await Task.Delay(10);
        }

        //Console.WriteLine("hit to exit...");
        //Console.ReadLine();
    }
}