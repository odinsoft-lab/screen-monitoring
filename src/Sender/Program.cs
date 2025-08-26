using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Monitoring.Sender;

public class Program
{
    const int DelayBetweenSendsMs = 10; // ms delay between screen captures

    static Shared.Sender sender = new Shared.Sender();

    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Please enter IP address and port number.");
            return;
        }

        if (!IPAddress.TryParse(args[0], out IPAddress? ipAddress))
        {
            Console.WriteLine("Invalid IP address.");
            return;
        }

        if (!int.TryParse(args[1], out int port))
        {
            Console.WriteLine("The port number is not valid.");
            return;
        }

        Console.WriteLine($"IP address: {ipAddress}, port number: {port}");

        ApplicationConfiguration.Initialize();

        await StartScreenCapture(ipAddress, port);
    }

    static async Task StartScreenCapture(IPAddress ipAddress, int port)
    { 
        var loop_exit = false;

        Console.WriteLine("start capturing the screen.");

        while (!loop_exit)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, port);
                    using NetworkStream stream = client.GetStream();

                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Escape)
                            {
                                loop_exit = true;

                                Console.WriteLine("exits the screen capture.");
                                break;
                            }
                        }

                        var data = sender.CaptureScreen();
                        data = sender.CompressData(data);

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
                Debug.WriteLine($"sender: {ex.Message}");
            }

            await Task.Delay(1000);
        }
    }
}