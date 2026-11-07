using System;

namespace Lawn.Creative;

public struct SecretCrc
{
	public uint Value { get; set; }

	public void Init()
	{
		Value = 2166136261u;
	}

	public void Update(ReadOnlySpan<byte> data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			Value = ((Value << 1) + (Value << 4) + (Value << 7) + (Value << 8) + (Value << 24) + Value) ^ data[i];
		}
	}
}
