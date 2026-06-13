namespace SmartHome.Core;

public class SmartHomeHub
{
    private readonly List<IDevice> _devices = new List<IDevice>();

    public string Name { get; }

    public SmartHomeHub(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Hub name cannot be empty!");
        }
        Name = name;
    }
    
    public List<IDevice> GetDevices()
    {
        return _devices;
    }


    public static SmartHomeHub operator +(SmartHomeHub hub, IDevice? device)
    {
        if (device == null) return hub;

        // sprawdzenie czy urzadzenie o takim ID jest juz na liscie
        foreach (var d in hub._devices)
        {
            if (d.Id == device.Id)
            {
                throw new ArgumentException("Device with this ID already exists!");
            }
        }

        hub._devices.Add(device);
        return hub;
    }
    
    // wyszukiwanie po ID
    public IDevice? this[string id]
    {
        get
        {
            // LINQ do przeszukania listy
            return _devices.FirstOrDefault(d => d.Id.ToLower() == id.ToLower());
        }
    }

    // LINQ - filtrowanie po room
    public List<IDevice> GetDevicesInRoom(string room)
    {
        return _devices.Where(d => d.Room == room).ToList();
    }

    // LINQ - filtrowanie tylko wlaczonych urzadzen
    public List<IDevice> GetActiveDevices()
    {
        return _devices.Where(d => d.IsEnabled).ToList();
    }
}