using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

#pragma warning disable

namespace Shared
{
    public class Receiver
    {
        public Packet DeserializePacket(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<Packet>(jsonString);
        }

        public async Task<Packet> RecvDataByTCP(int port)
        {
            var result = new Packet();

            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            using TcpClient client = await listener.AcceptTcpClientAsync();
            using NetworkStream stream = client.GetStream();
            using MemoryStream ms = new MemoryStream();

            var buffer = new byte[1024];
            
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, bytesRead);
            }

            result.data = ms.ToArray();

            return result;
        }

        public Packet RecvDataByUDP(UdpClient client)
        {
            var result = new Packet();

            var total_data = new byte[256 * 1024];

            try
            {
                while (true)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    var recv_data = client.Receive(ref remoteEP);

                    var packet = DeserializePacket(recv_data);
                    Array.Copy(packet.data, 0, total_data, packet.seqno * packet.size, packet.data.Length);

                    Debug.WriteLine($"recv packet: {packet.seqno} / {packet.lastno}");

                    // 모든 패킷을 받았다면 루프를 종료합니다.
                    if (packet.seqno >= packet.lastno - 1)
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            result.data = total_data.ToArray();

            return result;
        }

        public Image ByteArrayToImage(byte[] byteArray)
        {
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return Image.FromStream(memoryStream);
            }
        }
    }
}