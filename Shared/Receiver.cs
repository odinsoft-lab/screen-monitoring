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

        public byte[] RecvData(UdpClient client)
        {
            var total_data = new List<byte>(new byte[100 * 1024]); // 전체 데이터 크기를 미리 설정합니다.

            try
            {
                while (true)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    var recv_data = client.Receive(ref remoteEP);

                    var packet = DeserializePacket(recv_data);
                    Array.Copy(packet.data, 0, total_data.ToArray(), packet.seqno * packet.size, packet.data.Length);

                    // 모든 패킷을 받았다면 루프를 종료합니다.
                    if (packet.seqno >= packet.lastno)
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return total_data.ToArray();
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