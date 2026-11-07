using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Net;

public class NetworkSessionProperties : List<int?>
{
	public NetworkSessionProperties()
		: base(8)
	{
		Add(null);
		Add(null);
		Add(null);
		Add(null);
		Add(null);
		Add(null);
		Add(null);
		Add(null);
	}

	public static void WriteProperties(NetworkSessionProperties properties, int[] propertyData)
	{
		for (int i = 0; i < 8; i++)
		{
			if (properties != null && properties[i].HasValue)
			{
				propertyData[i * 2] = 1;
				propertyData[i * 2 + 1] = properties[i].Value;
			}
			else
			{
				propertyData[i * 2] = 0;
				propertyData[i * 2 + 1] = 0;
			}
		}
	}

	public static void ReadProperties(NetworkSessionProperties properties, int[] propertyData)
	{
		for (int i = 0; i < 8; i++)
		{
			properties[i] = null;
			if (propertyData[i * 2] > 0)
			{
				properties[i] = propertyData[i * 2 + 1];
			}
		}
	}
}
