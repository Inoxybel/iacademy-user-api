namespace Infra.Options;

public class DatabaseInstanceOptions
{
    public static readonly string ConnectionStringConfigKey = "ConnectionString";
    public static readonly string DatabaseNameConfigKey = "DatabaseName";
    public string ConnectionString { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
}
