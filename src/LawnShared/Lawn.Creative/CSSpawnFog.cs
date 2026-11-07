using System.Text.Json;

namespace Lawn.Creative;

public class CSSpawnFog : IComponent
{
	public int mColumn;

	public int Version { get; init; }

	public CSSpawnFog()
	{
		mColumn = 3;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Column", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mColumn = value.GetInt32();
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
