namespace Capture;

public class Program
{
    static async Task Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        Console.WriteLine("hit to capture & send...");
        Console.ReadLine();

        var sender = new Shared.Sender();

        while (true)
        {
            var data = sender.CaptureScreen();

            //Console.WriteLine($"send data size: {data.Length}");

            await sender.SendDataByTCP("192.168.0.5", 8088, data);

            await Task.Delay(1);
        }

        //Console.WriteLine("hit to exit...");
        //Console.ReadLine();
    }
}