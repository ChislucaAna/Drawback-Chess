using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DrawbackChess.Classes
{

    public class Matchmaking
    {
        public static async Task<string> TryConnect(string name)
        {
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("rapsy.go.ro");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipEndPoint = new(ipAddress, 7789);

            using Socket client = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            await client.ConnectAsync(ipEndPoint);
            var messageBytes = Encoding.UTF8.GetBytes(name);
            await client.SendAsync(messageBytes, SocketFlags.None);

            var buffer = new byte[1024];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            return response;
        }

    }
}
