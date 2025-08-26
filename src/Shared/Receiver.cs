﻿using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

#pragma warning disable

namespace Capture.Shared
{
    public class Receiver
    {
        public async Task<byte[]> RecvDataByTCP(TcpListener listener)
        {
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

                return ms.ToArray();
            }
        }

        public Image DecompressToImage(byte[] compressedData)
        {
            using MemoryStream ms = new MemoryStream(compressedData);
            
            using GZipStream gs = new GZipStream(ms, CompressionMode.Decompress);
            using MemoryStream os = new MemoryStream();
            {
                var buffer = new byte[1024 * 1024];
                
                int bytesRead;

                while ((bytesRead = gs.Read(buffer, 0, buffer.Length)) != 0)
                {
                    os.Write(buffer, 0, bytesRead);
                }
            }

            return Image.FromStream(os);
        }

        public Image ByteArrayToImage(byte[] bytes)
        {
            using MemoryStream ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }

        public Packet RecvDataByUDP(UdpClient client)
        {
            var result = new Packet();

            var total_data = new byte[1024 * 1024];

            try
            {
                while (true)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    var recv_data = client.Receive(ref remoteEP);

                    var packet = DeserializePacket(recv_data);
                    Array.Copy(packet.data, 0, total_data, packet.seqno * packet.size, packet.data.Length);

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

        Packet DeserializePacket(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<Packet>(jsonString);
        }
    }
}