using System.Text.Json.Serialization;

public class RootObject
{
    [JsonPropertyName("routes")]
    public List<Route> Routes { get; set; }
}

public class Route
{
    [JsonPropertyName("localizedValues")]
    public LocalizedValues LocalizedValues { get; set; }
}

public class LocalizedValues
{
    [JsonPropertyName("transitFare")]
    public TransitFare TransitFare { get; set; }
}

public class TransitFare
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
