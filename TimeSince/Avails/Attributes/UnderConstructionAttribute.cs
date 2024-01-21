namespace TimeSince.Avails.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class UnderConstructionAttribute(string reason) : System.Attribute
{
    public string Reason = reason;
}
