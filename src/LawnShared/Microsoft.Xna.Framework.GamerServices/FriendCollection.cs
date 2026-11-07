using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.GamerServices;

public class FriendCollection : IList<FriendGamer>, ICollection<FriendGamer>, IEnumerable<FriendGamer>, IEnumerable, IDisposable
{
	private List<FriendGamer> innerlist;

	private bool isReadOnly;

	public int Count => innerlist.Count;

	public FriendGamer this[int index]
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

	public bool IsReadOnly
	{
		get
		{
			return isReadOnly;
		}
		private set
		{
			isReadOnly = value;
		}
	}

	public FriendCollection()
	{
		innerlist = new List<FriendGamer>();
	}

	~FriendCollection()
	{
		Dispose(disposing: false);
	}

	public void Add(FriendGamer item)
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

	public bool Contains(FriendGamer item)
	{
		return innerlist.Contains(item);
	}

	public void CopyTo(FriendGamer[] array, int arrayIndex)
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

	public int IndexOf(FriendGamer item)
	{
		return innerlist.IndexOf(item);
	}

	public void Insert(int index, FriendGamer item)
	{
		innerlist.Insert(index, item);
	}

	public bool Remove(FriendGamer item)
	{
		return innerlist.Remove(item);
	}

	public void RemoveAt(int index)
	{
		innerlist.RemoveAt(index);
	}

	public IEnumerator<FriendGamer> GetEnumerator()
	{
		return innerlist.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return innerlist.GetEnumerator();
	}
}
