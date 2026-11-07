using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSManualZombieCountDown : IComponent
{
	public Dictionary<int, (int, int)> mInfo;

	public int Version { get; init; }

	public CSManualZombieCountDown()
	{
		mInfo = new Dictionary<int, (int, int)>();
	}

	public void Init(JsonElement root)
	{
		if (!root.TryGetProperty("Zombies", out var value) || value.ValueKind == JsonValueKind.Null)
		{
			return;
		}
		foreach (JsonElement item2 in value.EnumerateArray())
		{
			int @int = item2[0].GetInt32();
			int int2 = item2[1].GetInt32();
			int item = ((item2.GetArrayLength() >= 3) ? item2[2].GetInt32() : 0);
			mInfo.Add(@int, (int2, item));
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
