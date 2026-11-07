using System.Text.Json;

namespace Lawn.Creative;

public class CSColumnPlanting : IComponent
{
	public int Version { get; init; }

	public void Init(JsonElement root)
	{
	}

	public void Check(CreativeLevel level)
	{
	}
}
