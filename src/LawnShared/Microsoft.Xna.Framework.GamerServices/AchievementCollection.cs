using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.GamerServices;

public class AchievementCollection : IList<Achievement>, ICollection<Achievement>, IEnumerable<Achievement>, IEnumerable, IDisposable
{
	private List<Achievement> innerlist;

	private bool isReadOnly;

	public int Count => innerlist.Count;

	public Achievement this[int index]
	{
		get
		{
			return innerlist[index];
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (index >= innerlist.Count)
			{
				throw new IndexOutOfRangeException();
			}
		}
	}

	public bool IsReadOnly => isReadOnly;

	public AchievementCollection()
	{
		innerlist = new List<Achievement>();
	}

	~AchievementCollection()
	{
		Dispose(disposing: false);
	}

	public void Add(Achievement item)
	{
		if (item == null)
		{
			throw new ArgumentNullException();
		}
		if (innerlist.Count == 0)
		{
			innerlist.Add(item);
			return;
		}
		for (int i = 0; i < innerlist.Count; i++)
		{
		}
		innerlist.Add(item);
	}

	public void Clear()
	{
		innerlist.Clear();
	}

	public bool Contains(Achievement item)
	{
		return innerlist.Contains(item);
	}

	public void CopyTo(Achievement[] array, int arrayIndex)
	{
		innerlist.CopyTo(array, arrayIndex);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	public int IndexOf(Achievement item)
	{
		return innerlist.IndexOf(item);
	}

	public void Insert(int index, Achievement item)
	{
		innerlist.Insert(index, item);
	}

	public bool Remove(Achievement item)
	{
		return innerlist.Remove(item);
	}

	public void RemoveAt(int index)
	{
		innerlist.RemoveAt(index);
	}

	public IEnumerator<Achievement> GetEnumerator()
	{
		return innerlist.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return innerlist.GetEnumerator();
	}
}
