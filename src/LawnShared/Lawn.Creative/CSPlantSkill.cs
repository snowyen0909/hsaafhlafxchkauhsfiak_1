using System.Text.Json;

namespace Lawn.Creative;

public class CSPlantSkill : IComponent
{
	public int Version { get; init; }

	public bool Enabled { get; private set; } = true;

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Enabled", out var value))
		{
			Enabled = value.GetBoolean();
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
