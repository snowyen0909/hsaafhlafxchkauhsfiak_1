using System.Text.Json;

namespace Lawn.Creative;

public class CSBungeeBlitz : IComponent
{
	public bool mShowHugeFlagAdvice;

	public int Version { get; init; }

	public CSBungeeBlitz()
	{
		mShowHugeFlagAdvice = true;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("ShowHugeFlagAdvice", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mShowHugeFlagAdvice = value.GetBoolean();
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
