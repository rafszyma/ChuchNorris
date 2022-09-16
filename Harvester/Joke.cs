namespace Harvester;

public class Joke
{
    public string Id { get; set; }
    
    public string Value { get; set; }

    public bool IsValid()
    {
        return Value.Length < 200;
    }
}