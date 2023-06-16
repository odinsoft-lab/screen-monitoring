using System.Diagnostics;
using System.Net.Sockets;

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
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("192.168.0.5", 8088);
                    using NetworkStream stream = client.GetStream();

                    while (true)
                    {
                        var data = sender.CaptureScreen();

                        Debug.WriteLine($"send data: {data.Length}");

                        await stream.WriteAsync(data, 0, data.Length);
                        await stream.WriteAsync(new byte[0], 0, 0);

                        await Task.Delay(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await Task.Delay(1000);
            }
        }

        //Console.WriteLine("hit to exit...");
        //Console.ReadLine();
    }
}