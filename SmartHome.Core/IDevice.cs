namespace SmartHome.Core;

public interface IDevice
{
    string Id { get; }
    string Name { get; }
    string Room { get; }
    bool IsEnabled { get; }
    
    void TurnOn();
    void TurnOff();
    string GetStatus();
}