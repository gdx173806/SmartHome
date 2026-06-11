namespace SmartHome.Core;

public class SmartLight : Device
{
    private int _brightness;

    // Custom property with validation
    public int Brightness
    {
        get => _brightness;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Brightness must be between 0 and 100.");
            
            _brightness = value;
            
            // If brightness is set above 0, automatically turn on the light
            if (_brightness > 0 && !IsEnabled)
                TurnOn();
        }
    }

    // Passing parameters to the base constructor using 'base'
    public SmartLight(string id, string name, string room) : base(id, name, room)
    {
        _brightness = 0;
    }

    // Overriding the abstract method
    public override string GetStatus()
    {
        return IsEnabled 
            ? $"[GREEN]ON[/] (Brightness: {Brightness}%)" 
            : "[GRAY]OFF[/]";
    }
}