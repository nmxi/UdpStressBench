using System.Net;
using System.Net.Sockets;

namespace UdpStressBench;

public class UdpTransmitter
{
    private readonly IPEndPoint _destination;
    private readonly UdpClient _udpClient;

    public UdpTransmitter(IPEndPoint destination)
    {
        _destination = destination;
        
        //create udp client
        _udpClient = new UdpClient();
    }

    public async Task SendBytes(Byte[] data)
    {
        //send data
        await _udpClient.SendAsync(data, data.Length, _destination);
    }
}