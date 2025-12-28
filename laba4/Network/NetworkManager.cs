using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace OnlineWhiteboard.Network
{
    public class NetworkManager
    {
        TcpClient client;
        NetworkStream stream;

        public event Action<string> OnMessage;

        public async void StartServer(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            client = await listener.AcceptTcpClientAsync();
            stream = client.GetStream();
            Receive();
        }

        public async void Connect(string ip, int port)
        {
            client = new TcpClient();
            await client.ConnectAsync(ip, port);
            stream = client.GetStream();
            Receive();
        }

        async void Receive()
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytes == 0) break;
                OnMessage?.Invoke(Encoding.UTF8.GetString(buffer, 0, bytes));
            }
        }

        public void Send(object obj)
        {
            if (stream == null) return;
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }
    }
}
