using System;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework.GamerServices;

[DataContract]
public abstract class Gamer
{
	private static SignedInGamerCollection _signedInGamers = new SignedInGamerCollection();

	private string _gamer = "MonoGame";

	private object _tag;

	private bool disposed;

	private LeaderboardWriter _leaderboardWriter;

	[DataMember]
	public string DisplayName { get; internal set; }

	[DataMember]
	public string Gamertag
	{
		get
		{
			return _gamer;
		}
		internal set
		{
			_gamer = value;
		}
	}

	[DataMember]
	public bool IsDisposed => disposed;

	public object Tag
	{
		get
		{
			return _tag;
		}
		set
		{
			if (_tag != value)
			{
				_tag = value;
			}
		}
	}

	public static SignedInGamerCollection SignedInGamers => _signedInGamers;

	public LeaderboardWriter LeaderboardWriter
	{
		get
		{
			return _leaderboardWriter;
		}
		internal set
		{
			_leaderboardWriter = value;
		}
	}

	public IAsyncResult BeginGetProfile(AsyncCallback callback, object asyncState)
	{
		throw new NotImplementedException();
	}

	public GamerProfile EndGetProfile(IAsyncResult result)
	{
		throw new NotImplementedException();
	}

	public GamerProfile GetProfile()
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return _gamer;
	}

	internal void Dispose()
	{
		disposed = true;
	}
}
