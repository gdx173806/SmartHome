using SmartHome.Core;
using Spectre.Console;

AnsiConsole.Write(new FigletText("SMART HOME").Color(Color.Blue));
AnsiConsole.MarkupLine("[bold yellow]Central Control Station Initialized.[/]\n");

var myHome = new SmartHomeHub("My Sweet Home");


var kitchenLight = new SmartLight("LGT-01", "Ceiling Light", "Kitchen");
var livingLight = new SmartLight("LGT-02", "Main Chandelier", "LivingRoom");
var bedroomLight = new SmartLight("LGT-03", "Bedside Lamp", "Bedroom");

// wykorzystanie przeciazenia operatora += do dodawania urzadzen
myHome += kitchenLight;
myHome += livingLight;
myHome += bedroomLight;

foreach (var device in myHome.GetDevices())
{
    // subskrypcja eventu OnFailure
    device.OnFailure += (sender, args) =>
    {
        if (sender is IDevice failedDevice)
        {
            // Spectre pozwala na tworzenie stylowych paneli (brzegów) wokół tekstu
            var alertPanel = new Panel(
                $"[bold white]Device:[/] [yellow]{failedDevice.Name}[/] ({failedDevice.Id})\n" +
                $"[bold white]Room:[/] {failedDevice.Room}\n" +
                $"[bold white]Reason:[/] [red]{args.Reason}[/]")
            {
                Header = new PanelHeader("[bold red]!!! CRITICAL FAILURE !!![/]"),
                Border = BoxBorder.Heavy,
                BorderStyle = new Style(Color.Red)
            };

            AnsiConsole.Write(alertPanel);
            AnsiConsole.MarkupLine("\n[grey]Press arrow keys to refresh menu layout...[/]");
        }
    };
}

// symulacja pracy wszystkich urzadzen
var cts = new CancellationTokenSource();
foreach (var device in myHome.GetDevices())
{
    if (device is Device dev)
    {
        // proces w tle dla kazdego urzadzenia
        _ = dev.StartSimulationAsync(cts.Token);
    }
}

// glowna petla - menu
while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold green]Select an action from the menu:[/]")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down using arrow keys)[/]")
            .AddChoices(new[] {
                "Show all devices status",
                "Turn ON all devices",
                "Turn OFF all devices",
                "Find device by ID (Indexer)",
                "Filter devices by Room (LINQ)",
                "Exit system"
            }));
    
    switch (choice)
    {
        case "Show all devices status":
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("[bold blue]ID[/]");
            table.AddColumn("[bold blue]Device Name[/]");
            table.AddColumn("[bold blue]Room[/]");
            table.AddColumn("[bold blue]Status[/]");

            foreach (var d in myHome.GetDevices())
            {
                string statusText = d.IsEnabled ? "[green]ON[/]" : "[red]OFF[/]";
                table.AddRow(d.Id, d.Name, d.Room, statusText);
            }

            AnsiConsole.Write(table);
            break;

        case "Turn ON all devices":
            AnsiConsole.MarkupLine("[yellow]Sending 'Turn On' signal to all nodes...[/]");
            foreach (var d in myHome.GetDevices()) d.TurnOn();
            AnsiConsole.MarkupLine("[green]All devices activated successfully.[/]");
            break;

        case "Turn OFF all devices":
            AnsiConsole.MarkupLine("[yellow]Sending 'Turn Off' signal to all nodes...[/]");
            foreach (var d in myHome.GetDevices()) d.TurnOff();
            AnsiConsole.MarkupLine("[red]All devices deactivated.[/]");
            break;

        case "Find device by ID (Indexer)":
            string searchId = AnsiConsole.Ask<string>("[bold green]Enter device ID to look for (e.g. LGT-01):[/]");
            
            // uzycie indeksatora
            IDevice? found = myHome[searchId];
            
            if (found != null)
                AnsiConsole.MarkupLine($"[green]Found:[/] [yellow]{found.Name}[/] in room: [blue]{found.Room}[/] (Status: {(found.IsEnabled ? "[green]ON[/]" : "[red]OFF[/]")})");
            else
                AnsiConsole.MarkupLine("[red]Error: Device with this ID does not exist![/]");
            break;

        case "Filter devices by Room (LINQ)":
            var uniqueRooms = myHome.GetDevices()
                .Select(d => d.Room)
                .Distinct()       
                .ToList();   
            
            string roomName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold green]Select a room to filter devices:[/]")
                    .AddChoices(uniqueRooms));
            
            if (uniqueRooms.Count == 0)
            {
                AnsiConsole.MarkupLine("[orange1]No rooms available because there are no devices in the system![/]");
                break;
            }
            // uzycie filtra LINQ
            var roomDevices = myHome.GetDevicesInRoom(roomName);
            
            if (roomDevices.Count > 0)
            {
                AnsiConsole.MarkupLine($"\n--- Devices in [blue]{roomName}[/] ---");
                foreach (var d in roomDevices) AnsiConsole.MarkupLine($" * [yellow]{d.Name}[/] ({d.Id})");
            }
            else
            {
                AnsiConsole.MarkupLine("[orange1]No devices found in this room.[/]");
            }
            break;

        case "Exit system":
            AnsiConsole.MarkupLine("\n[bold red]Shutting down Smart Home core. Goodbye![/]");
            cts.Cancel();
            return;
    }

    AnsiConsole.MarkupLine("\n[grey]Press any key to clear screen and continue...[/]");
    Console.ReadKey(true);
    Console.Clear();
}