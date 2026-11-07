using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.GamerServices;

public class GamerCollection<T> : ReadOnlyCollection<T>, IEnumerable<T>, IEnumerable where T : Gamer
{
	internal GamerCollection(List<T> list)
		: base((IList<T>)list)
	{
	}

	internal GamerCollection()
		: base((IList<T>)new List<T>())
	{
	}

	internal void AddGamer(T item)
	{
		if (Items.Count > 0)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (item.Gamertag.CompareTo(Items[i].Gamertag) > 0)
				{
					Items.Insert(i, item);
					return;
				}
			}
		}
		Items.Add(item);
	}

	internal void RemoveGamer(T item)
	{
		Items.Remove(item);
	}

	internal void RemoveGamerAt(int item)
	{
		Items.RemoveAt(item);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
