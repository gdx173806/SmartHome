namespace SmartHome.Core;

public class SmartLight : Device
{
    private int _brightness;
    
    public int Brightness
    {
        get => _brightness;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Brightness must be between 0 and 100.");
            
            _brightness = value;
            
            if (_brightness > 0 && !IsEnabled)
                TurnOn();
        }
    }
    
    public SmartLight(string id, string name, string room) : base(id, name, room)
    {
        _brightness = 0;
    }
    
    public override string GetStatus()
    {
        return IsEnabled 
            ? $"ON (Brightness: {Brightness}%)" 
            : "OFF";
    }
}