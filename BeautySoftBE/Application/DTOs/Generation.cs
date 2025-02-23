public class Generation
{
    public string Id { get; set; }
    public string Text { get; set; }
    public Meta Meta { get; set; }
    public string FinishReason { get; set; }
}
public class Meta
{
    public ApiVersion ApiVersion { get; set; }
    public List<string> Warnings { get; set; }
    public BilledUnits BilledUnits { get; set; }
}

public class ApiVersion
{
    public string Version { get; set; }
    public bool IsDeprecated { get; set; }
}

public class BilledUnits
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
}