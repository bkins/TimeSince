using SQLite;
using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.Models;

[Table($"{nameof(ColorName)}s")]
public class ColorName : BaseModel
{
    public string?   Name  { get; init; }
    public Color?    Color { get; init; }
    public ColorType Type  { get; set; }
}

public enum ColorType
{
    Primary
  , Secondary
  , Tertiary
}
