using SQLite;
using TimeSince.MVVM.BaseClasses;

namespace TimeSince.MVVM.Models;

[Table($"{nameof(ColorName)}s")]
public class ColorName : BaseModel
{
    public string    Name  { get; set; }
    public Color     Color { get; set; }
    public ColorType Type  { get; set; }
}

public enum ColorType
{
    Primary
  , Secondary
  , Tertiary
}
