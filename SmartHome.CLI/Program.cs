using SmartHome.Core;

Console.WriteLine("=== SmartHome Simulation Starting ===");

var livingRoomLight = new SmartLight("LGT-01", "Main Light", "Living Room");

// subskrybcja zdarzenia
livingRoomLight.OnFailure += (sender, args) =>
{
    // Ten kod wykona się AUTOMATYCZNIE, gdy żarówka ulegnie awarii w tle
    // var device = sender as IDevice;
    if (sender is IDevice device)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(
            $"\n[ALERT] [{args.TimeStamp:HH:mm:ss}] {device?.Name} in {device?.Room} FAILED! Reason: {args.Reason}");
        Console.ResetColor();
    }
};

// wlaczamy swiatlo i ustawiamy jasnosc
livingRoomLight.Brightness = 75;
Console.WriteLine($"Device status: {livingRoomLight.Name} is {livingRoomLight.GetStatus()}");

// uruchamiamy symulacje asynchroniczna w tle
using var cts = new CancellationTokenSource();
Task simulationTask = livingRoomLight.StartSimulationAsync(cts.Token);

Console.WriteLine("Simulation running in background. Press any key to exit...");
Console.ReadKey();

// zamkniecie watkow na koniec
cts.Cancel();
try
{
    await simulationTask;
}
catch (TaskCanceledException) { }