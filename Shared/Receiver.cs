using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

#pragma warning disable

namespace Shared
{
    public class Receiver
    {
        public async Task<Packet> RecvDataByTCP(TcpListener listener)
        {
            var result = new Packet();

            using (TcpClient tc = await listener.AcceptTcpClientAsync())
            {
                using NetworkStream ns = tc.GetStream();
                using MemoryStream ms = new MemoryStream();

                var buffer = new byte[1024 * 1024];

                int bytesRead;

                while ((bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                ms.Position = 0; 

                using (var gs = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (var ds = new MemoryStream())
                    {
                        gs.CopyTo(ds);

                        //ds.Position = 0;
                        result.image = Image.FromStream(ds);
                    }
                }
            }

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

                    //Debug.WriteLine($"recv packet: {packet.seqno} / {packet.lastno}");

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

        public Packet DeserializePacket(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<Packet>(jsonString);
        }
    }
}