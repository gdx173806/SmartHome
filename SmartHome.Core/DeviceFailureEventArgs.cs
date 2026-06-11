namespace SmartHome.Core;

public class DeviceFailureEventArgs : EventArgs
{
    public string Reason { get; }
    public DateTime TimeStamp { get; }
    
    public DeviceFailureEventArgs(string reason)
    {
        Reason = reason;
        TimeStamp = DateTime.Now;
        
    }
}