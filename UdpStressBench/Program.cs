using System.Net;
using UdpStressBench;

var logger = new Logger();

const string defaultAddress = "127.0.0.1";
const int defaultPort = 8001;
const int defaultPacketSize = 1024;   //Bytes
const int defaultRate = 1;           //Packet per second

const int minRate = 1;
const int maxRate = 1000;

bool _isRunning;

var commandArgs = ParseCommandLineArgs(args);

var isHelp = commandArgs.TryGetValue("-help", out _) || commandArgs.TryGetValue("-h", out _) || commandArgs.TryGetValue("--help", out _);
if (isHelp)
{
    logger.Log("Usage: UdpStressBench [-address <address>] [-port <port>] [-size <size>] [-rate <rate>]");
    logger.Log("Options:");
    logger.Log($"-address <address> : Destination Address (default: {defaultAddress})");
    logger.Log($"-port <port> : Destination Port (default: {defaultPort})");
    logger.Log($"-size <size> : Packet Size (default: {defaultPacketSize})");
    logger.Log($"-rate <rate> : Packet Rate (default: {defaultRate})");
    return;
}

var address = commandArgs.GetValueOrDefault("-address", defaultAddress);
var port = commandArgs.TryGetValue("-port", out var portValue) ? int.Parse(portValue) : defaultPort;
var size = commandArgs.TryGetValue("-size", out var inputSizeValue) ? int.Parse(inputSizeValue) : defaultPacketSize;
var rate = commandArgs.TryGetValue("-rate", out var rateValueValue) ? int.Parse(rateValueValue) : defaultRate;

rate = Math.Max(rate, minRate);
rate = Math.Min(rate, maxRate);

logger.Log($"Start UdpStressBench PacketSize: {size}B, Rate: {rate}msg/s");

//parse address port 
var destination = new IPEndPoint(IPAddress.Parse(address), port);
if (destination == null)
{
    logger.LogError($"Invalid Address: {address}");
    return;
}

logger.Log($"Destination: {destination}");
var udpTransmitter = new UdpTransmitter(destination);

_isRunning = true;
var sendThread = new Thread(() => SendThread(logger, udpTransmitter, rate));
sendThread.Start();

while (true)
{
    Thread.Sleep(10);
    
    if (Console.IsInputRedirected)
    {
        continue;
    }
        
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.Q)
        {
            logger.Log($"Quit Application");

            _isRunning = false;
            sendThread.Join();
                
            break;
        }
    }
}

return;

async void SendThread(Logger logger, UdpTransmitter udpTransmitter, int rate)
{
    while (_isRunning)
    {
        //rateからsleepの計算
        //rateは1秒間に送信するパケット数
        var sleepMs = 1000 / rate;
        // logger.Log($"Sleep: {sleepMs}ms");
        Thread.Sleep(sleepMs);
        
        if(!_isRunning)
            break;
        
        //create random data
        var data = new byte[size];
        new Random().NextBytes(data);
        
        //send packet
        await udpTransmitter.SendBytes(data);
        
        var dataRate = size * 8 * rate;
        logger.Log($"Send Packet Size: {size}Bytes ({dataRate / 1000f}Kbps)");
    }
    
    logger.Log($"End SendThread");
}

static Dictionary<string, string> ParseCommandLineArgs(string[] args)
{
    var commandArgs = new Dictionary<string, string>();

    for (var i = 0; i < args.Length; i++)
    {
        if (args[i].StartsWith($"-") && i + 1 < args.Length && !args[i + 1].StartsWith($"-"))
        {
            commandArgs[args[i]] = args[i + 1];
            i++; // 値をスキップ
        }
        else if (args[i].StartsWith($"-") && (i + 1 == args.Length || args[i + 1].StartsWith($"-")))
        {
            // フラグの場合（値を持たない）
            commandArgs[args[i]] = "true";
        }
    }

    return commandArgs;
}