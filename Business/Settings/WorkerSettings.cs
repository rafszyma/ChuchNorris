namespace Business.Settings;

public class WorkerSettings
{
    public int HarvestIntervalInSeconds { get; set; }
    
    public string ApiKey { get; set; }
    
    public string ApiHost { get; set; }
    
    public int BatchSize { get; set; }
}