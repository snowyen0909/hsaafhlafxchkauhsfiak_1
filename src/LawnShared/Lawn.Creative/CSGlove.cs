using System;
using System.Text.Json;

namespace Lawn.Creative;

public class CSGlove : IComponent
{
	public int Version { get; init; }

	public bool Enabled { get; private set; } = true;

	public int? Cooldown { get; private set; }

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Enabled", out var value))
		{
			Enabled = value.GetBoolean();
		}
		if (root.TryGetProperty("Cooldown", out var value2))
		{
			Cooldown = value2.GetInt32();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (Cooldown < 0)
		{
			throw new Exception("Cooldown must be non-negative");
		}
	}
}
