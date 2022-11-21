using Client;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;

var ip = IPAddress.Loopback;
var port = 27001;

var client = new TcpClient();
client.Connect(ip, port);

var stream = client.GetStream();

var br = new BinaryReader(stream);
var bw = new BinaryWriter(stream);

Command command = null;
string responce = null;

while (true)
{
    Console.WriteLine("Write Command or HELP");
    var str = Console.ReadLine().ToUpper();

    if (str.ToUpper() == "HELP")
    {
        Console.WriteLine("\nCommand List:\n");
        Console.WriteLine(Command.ProcessList);
        Console.WriteLine($"{Command.Kill} <process_name>");
        Console.WriteLine($"{Command.Run}  <process_name>");
        Console.WriteLine("HELP");
        Console.ReadKey();
        Console.Clear();
        continue;
    }

    var input = str.Split(' ');

    switch (input[0])
    {
        case Command.ProcessList:
            command = new Command() { Text = input[0] };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();
            var processesName = JsonSerializer.Deserialize<string[]>(responce);
            foreach (var item in processesName)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
            Console.Clear();
            break;
        case Command.Kill:

            if (string.IsNullOrWhiteSpace(input[1]))
            {
                Console.WriteLine("Parametr can't be empty !");
                break;
            }

            command = new Command() { Text = input[0], Param = input[1] };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();

            ShowStatus(responce);
            break;
        case Command.Run:

            if (string.IsNullOrWhiteSpace(input[1]))
            {
                Console.WriteLine("Parametr can't be empty !");
                break;
            }

            command = new Command() { Text = input[0], Param = input[1] };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();

            ShowStatus(responce);
            break;

        default:
            break;
    }

}


void ShowStatus(string status)
{
    Console.Write("Status:  ");
    switch (status)
    {
        case "Succes":
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            break;
        case "Failed":
            Console.ForegroundColor = ConsoleColor.DarkRed;
            break;
        default:
            break;
    }
    Console.WriteLine(status);
    Console.ReadKey();
    Console.ForegroundColor = ConsoleColor.White;
    Console.Clear();
}