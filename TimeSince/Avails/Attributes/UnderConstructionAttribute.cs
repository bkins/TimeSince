namespace TimeSince.Avails.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class UnderConstructionAttribute : System.Attribute
{
    public string Reason;

    public UnderConstructionAttribute(string reason)
    {
        Reason = reason;
    }
}
