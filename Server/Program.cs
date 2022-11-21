using Server;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Loopback;
var port = 27001;
var listener = new TcpListener(ip, port);

listener.Start(100);

while (true)
{

    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var br = new BinaryReader(stream);
    var bw = new BinaryWriter(stream);

    while (true)
    {
        var input = br.ReadString();
        var command = JsonSerializer.Deserialize<Command>(input);

        if (command == null)
            continue;


        Console.WriteLine(command.Text);
        Console.WriteLine(command.Param);

        switch (command.Text)
        {
            case Command.ProcessList:
                bw.Write(JsonSerializer.Serialize(Process.GetProcesses().ToList().Select(p => p.ProcessName)));
                break;

            case Command.Kill:

                string status = "Failed";

                string param = command.Param;

                Process.GetProcesses().ToList().ForEach(p =>
                {
                    try
                    {
                        if (p.ProcessName.ToUpper() == param)
                        {
                            p.Kill();
                            status = "Succes";
                        }
                    }
                    catch
                    {
                        status = "Failed";
                    }
                });

                bw.Write(status);

                break;

            case Command.Run:

                status = "Failed";

                param = command.Param;

                try
                {
                    Process.Start(param);
                    status = "Succes";
                }
                catch
                {
                    status = "Failed";
                }


                bw.Write(status);

                break;

            default:
                break;
        }

    }

}

