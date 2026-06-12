namespace SmartHome.Core;

public interface IDevice
{
    string Id { get; }
    string Name { get; }
    string Room { get; }
    bool IsEnabled { get; }
    
    event EventHandler<DeviceFailureEventArgs>? OnFailure;
    
    void TurnOn();
    void TurnOff();
    string GetStatus();
}