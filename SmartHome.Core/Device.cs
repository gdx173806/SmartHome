namespace SmartHome.Core;

public abstract class Device : IDevice
{
    public string Id { get; }
    public string Name { get; }
    public string Room { get; }
    public bool IsEnabled { get; protected set; }
    
    public event EventHandler<DeviceFailureEventArgs>? OnFailure;
    
    protected Device(string id, string name, string room)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(room);

        Id = id;
        Name = name;
        Room = room;
        IsEnabled = false;
    }

    public virtual void TurnOn()
    {
        IsEnabled = true;
    }

    public virtual void TurnOff()
    {
        IsEnabled = false;
    }
    
    public abstract string GetStatus();
    
    public async Task StartSimulationAsync(CancellationToken cancellationToken)
    {
        var random = new Random();

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(2000, cancellationToken);

            if (IsEnabled)
            {
                if (random.Next(1, 101) <= 5)
                {
                    RaiseFailure("Hardware malfunction / Overheating");
                    TurnOff();
                }
            }
        }
}
    protected void RaiseFailure(string reason)
    {
        OnFailure?.Invoke(this, new DeviceFailureEventArgs(reason));
    }
}