using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public interface IComponent
{
	int Version { get; init; }

	void Init(JsonElement root);

	void Check(CreativeLevel level);

	protected static void PrepareOneIntArrayFromString<T>(JsonElement oneArray, T[] items, Func<JsonElement, T> convertor)
	{
		Array.Clear(items);
		int num = 0;
		foreach (JsonElement item in oneArray.EnumerateArray())
		{
			if (num >= items.Length)
			{
				break;
			}
			items[num++] = convertor((item.ValueKind == JsonValueKind.Array) ? item[0] : item);
		}
	}

	protected static void PrepareOneIntListFromString<T>(JsonElement oneArray, List<T> items, Func<JsonElement, T> convertor)
	{
		items.Clear();
		foreach (JsonElement item in oneArray.EnumerateArray())
		{
			items.Add(convertor((item.ValueKind == JsonValueKind.Array) ? item[0] : item));
		}
	}

	protected static void PrepareTwoIntListFromString<T1, T2>(JsonElement twoArray, List<(T1, T2)> items, Func<JsonElement, JsonElement, (T1, T2)> convertor)
	{
		items.Clear();
		foreach (JsonElement item in twoArray.EnumerateArray())
		{
			items.Add(convertor(item[0], item[1]));
		}
	}

	protected static void PrepareThreeIntListFromString<T1, T2, T3>(JsonElement twoArray, List<(T1, T2, T3)> items, Func<JsonElement, JsonElement, JsonElement, (T1, T2, T3)> convertor)
	{
		items.Clear();
		foreach (JsonElement item in twoArray.EnumerateArray())
		{
			items.Add(convertor(item[0], item[1], item[2]));
		}
	}
}
