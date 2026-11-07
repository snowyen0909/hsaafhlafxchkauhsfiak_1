using System.Text.Json;

namespace Lawn.Creative;

public class CSInvisibleZombie : IComponent
{
	public int Version { get; init; }

	public void Init(JsonElement root)
	{
	}

	public void Check(CreativeLevel level)
	{
	}
}
