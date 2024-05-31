using System.Diagnostics;
using System.Net.Sockets;

namespace Capture;

public class Program
{
    const int DelayBetweenSendsMs = 10; // ms delay between screen captures

    static Shared.Sender sender = new Shared.Sender();

    static async Task Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var loop_exit = false;

        Console.WriteLine("화면 캡쳐를 시작 합니다.");

        while (!loop_exit)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("192.168.0.27", 8088);
                    using NetworkStream stream = client.GetStream();

                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Escape)
                            {
                                loop_exit = true;

                                Console.WriteLine("화면 캡쳐를 종료 합니다.");
                                break;
                            }
                        }

                        var data = sender.CaptureScreen();
                        data = sender.CompressData(data);

                        //Debug.WriteLine($"Sending data: {data.Length} bytes");

                        var header = new List<byte>(BitConverter.GetBytes(data.Length));
                        header.Add(0x01);

                        await stream.WriteAsync(header.ToArray(), 0, 5); // Send the length of the data as 4 bytes
                        await stream.WriteAsync(data, 0, data.Length);

                        await Task.Delay(DelayBetweenSendsMs);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data: {ex.Message}");
            }

            await Task.Delay(1000);
        }
    }
}