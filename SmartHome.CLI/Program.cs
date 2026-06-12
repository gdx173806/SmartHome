using System;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.Core;

Console.WriteLine("=== SMART HOME CENTRAL STATION ===");

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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[!!! ALERT !!!] [{args.TimeStamp:HH:mm:ss}]");
            Console.WriteLine($"Device failure detected: {failedDevice.Name} in {failedDevice.Room} stopped working!");
            Console.WriteLine($"Reason: {args.Reason}");
            Console.ResetColor();
            Console.Write("\nChoose option: ");
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
    Console.WriteLine("\n-----------------------------------");
    Console.WriteLine("1. Show all devices status");
    Console.WriteLine("2. Turn ON all devices");
    Console.WriteLine("3. Turn OFF all devices");
    Console.WriteLine("4. Find device by ID (Indexer test)");
    Console.WriteLine("5. Filter devices by Room (LINQ test)");
    Console.WriteLine("6. Exit system");
    Console.Write("Choose option: ");
    
    string? choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.WriteLine("\n--- Current Devices Status ---");
        foreach (var d in myHome.GetDevices())
        {
            Console.WriteLine($"[{d.Id}] {d.Name} in {d.Room} -> Status: {d.GetStatus()}");
        }
    }
    else if (choice == "2")
    {
        Console.WriteLine("\nTurning on all devices...");
        foreach (var d in myHome.GetDevices()) d.TurnOn();
    }
    else if (choice == "3")
    {
        Console.WriteLine("\nTurning off all devices...");
        foreach (var d in myHome.GetDevices()) d.TurnOff();
    }
    else if (choice == "4")
    {
        Console.Write("\nEnter device ID to look for: ");
        string? searchId = Console.ReadLine();
        
        // indeksator
        IDevice? found = myHome[searchId ?? ""];
        
        if (found != null)
            Console.WriteLine($"Found: {found.Name} in room: {found.Room}");
        else
            Console.WriteLine("Device not found!");
    }
    else if (choice == "5")
    {
        Console.Write("\nEnter room name (Kitchen/LivingRoom/Bedroom): ");
        string? roomName = Console.ReadLine();
        
        // filtr LINQ
        var roomDevices = myHome.GetDevicesInRoom(roomName ?? "");
        
        Console.WriteLine($"\n--- Devices in {roomName} ---");
        foreach (var d in roomDevices)
        {
            Console.WriteLine($"- {d.Name} ({d.Id})");
        }
    }
    else if (choice == "6")
    {
        Console.WriteLine("\nShutting down system. Goodbye!");
        cts.Cancel();
        break;
    }
    else
    {
        Console.WriteLine("Unknown option, try again.");
    }
}