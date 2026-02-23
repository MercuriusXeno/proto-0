using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class DescriptionComponent : IComponent
{
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
}
