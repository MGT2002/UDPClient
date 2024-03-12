using MyExtensions;
using System.Net;
using System.Net.Sockets;
using System.Text;

const int DefaultEchoPort = 7;

if (args.Length < 2 || args.Length > 3)
    throw new ArgumentException("Parameters: <Server>, <Word>, [<Port>]");

string server = args[0];
int servPort = args.Length == 3 ? int.Parse(args[2]) : DefaultEchoPort;
byte[] sendPacket = Encoding.ASCII.GetBytes(args[1]);

try
{
    using UdpClient client = new();

    client.Send(sendPacket, sendPacket.Length, server, servPort);
    $"Sent {sendPacket.Length} bytes to the server!".Log();

    IPEndPoint remote = null!;
    byte[] rcvPacket = client.Receive(ref remote);

    $"Received {rcvPacket.Length} bytes from {remote}:"
        .Log(Encoding.ASCII.GetString(rcvPacket));
}
catch (SocketException e)
{
    e.ErrorCode.Log(e.Message);
}
catch (Exception e)
{
    e.Message.Log();
}