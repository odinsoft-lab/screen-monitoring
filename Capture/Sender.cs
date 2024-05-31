using System.Collections.Concurrent;
using System.Drawing;
using System.Net;

namespace Capture;

public class Sender
{
    static List<ConcurrentQueue<Bitmap>> frameQueues = new List<ConcurrentQueue<Bitmap>>();
    static List<Thread> transmitThreads = new List<Thread>();
    static Dictionary<string, bool> connectionStatus = new Dictionary<string, bool>(); // 통신 상태 저장

    static Shared.Sender sender = new Shared.Sender();
    
    const int DelayBetweenSendsMs = 0; // 100ms delay between screen captures

    // 프레임 전송 대상 PC의 IP 주소 리스트
    static List<string> targetIPs = new List<string> { "192.168.0.27", "192.168.0.54" };

    static async Task Main2(string[] args)
    {
        ApplicationConfiguration.Initialize();

        // 큐 및 스레드 초기화
        foreach (string ip in targetIPs)
        {
            if (IsMyIpAddress(ip))
            {
                Console.WriteLine($"IP 주소 {ip}는 현재 시스템의 IP 주소입니다. 해당 IP 주소는 제외됩니다.");
                continue;
            }

            ConcurrentQueue<Bitmap> frameQueue = new ConcurrentQueue<Bitmap>();
            frameQueues.Add(frameQueue);
            connectionStatus[ip] = true; // 초기 연결 상태 설정

            Thread transmitThread = new Thread(() => TransmitFrames(ip, frameQueue));
            transmitThread.Start();

            transmitThreads.Add(transmitThread);
        }

        // 화면 캡처 작업 시작
        Task captureTask = Task.Run(() => CaptureFrameAndEnqueue());

        // 연결 상태 확인 작업 시작
        Task connectionCheckTask = Task.Run(() => CheckConnectionStatus());

        // 작업 완료 대기
        Task.WaitAll(captureTask, connectionCheckTask);

        // 모든 큐와 스레드 정리
        foreach (ConcurrentQueue<Bitmap> frameQueue in frameQueues)
        {
            while (frameQueue.TryDequeue(out Bitmap? frame))
            {
                frame?.Dispose();
            }
        }

        foreach (Thread transmitThread in transmitThreads)
        {
            transmitThread.Join();
        }

        Console.WriteLine("모든 작업이 완료되었습니다. 프로그램을 종료합니다.");

        //while (true)
        //{
        //    try
        //    {
        //        using (TcpClient client = new TcpClient())
        //        {
        //            await client.ConnectAsync("192.168.0.5", 8088);
        //            using NetworkStream stream = client.GetStream();

        //            while (true)
        //            {
        //                var data = sender.CaptureScreen();
        //                data = sender.CompressData(data);

        //                //Debug.WriteLine($"Sending data: {data.Length} bytes");

        //                var header = new List<byte>(BitConverter.GetBytes(data.Length));
        //                header.Add(0x01);

        //                await stream.WriteAsync(header.ToArray(), 0, 5); // Send the length of the data as 4 bytes
        //                await stream.WriteAsync(data, 0, data.Length);

        //                await Task.Delay(DelayBetweenSendsMs);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error sending data: {ex.Message}");
        //    }

        //    //await Task.Delay(1000);
        //}

        await Task.CompletedTask;
    }

    static void CaptureFrameAndEnqueue()
    {
        while (true)
        {
            // 화면 캡처 로직
            var frame = sender.CaptureFrame();

            // 프레임을 모든 큐에 추가
            foreach (ConcurrentQueue<Bitmap> frameQueue in frameQueues)
            {
                frameQueue.Enqueue(frame);
            }

            // 잠시 대기
            Thread.Sleep(100);
        }
    }

    static void TransmitFrames(string targetIP, ConcurrentQueue<Bitmap> frameQueue)
    {
        while (true)
        {
            // 큐에서 최신 프레임 가져오기
            if (frameQueue.TryPeek(out Bitmap? frame))
            {
                // 해당 IP의 연결 상태 확인
                bool isConnected = connectionStatus[targetIP];

                if (isConnected)
                {
                    // 프레임을 해당 IP로 전송하는 로직
                    TransmitFrame(frame, targetIP);
                }

                // 프레임 해제
                frame?.Dispose();
            }

            // 잠시 대기
            Thread.Sleep(100);
        }
    }

    static void TransmitFrame(Bitmap? frame, string targetIP)
    {
        // 프레임을 해당 IP로 전송하는 로직 구현
        // ...

        // 전송 완료 후 프레임 해제
        frame?.Dispose();
    }

    static void CheckConnectionStatus()
    {
        while (true)
        {
            // 각 IP에 대해 연결 상태 확인
            foreach (string ip in connectionStatus.Keys.ToList())
            {
                bool isConnected = IsConnected(ip);
                connectionStatus[ip] = isConnected;
            }

            // 잠시 대기
            Thread.Sleep(1000);
        }
    }

    static bool IsConnected(string targetIP)
    {
        // 특정 IP에 대한 연결 상태 확인 로직 구현
        // ...

        return true;
    }

    static bool IsMyIpAddress(string ipAddress)
    {
        string hostName = Dns.GetHostName(); // 현재 시스템의 호스트 이름을 얻습니다.
        IPAddress[] addresses = Dns.GetHostAddresses(hostName); // 호스트 이름에 대한 IP 주소를 얻습니다.

        foreach (IPAddress address in addresses)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                if (address.ToString() == ipAddress)
                {
                    return true;
                }
            }
        }

        return false;
    }
}