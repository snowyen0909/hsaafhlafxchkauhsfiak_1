using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

public class NetworkGamer : Gamer, INotifyPropertyChanged
{
	private byte id;

	private NetworkSession session;

	private long remoteUniqueIdentifier = -1L;

	private GamerStates gamerState;

	private GamerStates oldGamerState;

	private NetworkMachine _Machine;

	internal long RemoteUniqueIdentifier
	{
		get
		{
			return remoteUniqueIdentifier;
		}
		set
		{
			remoteUniqueIdentifier = value;
		}
	}

	public bool HasLeftSession => false;

	public bool HasVoice => (gamerState & GamerStates.HasVoice) != 0;

	public byte Id => id;

	public bool IsGuest => (gamerState & GamerStates.Guest) != 0;

	public bool IsHost => (gamerState & GamerStates.Host) != 0;

	public bool IsLocal => (gamerState & GamerStates.Local) != 0;

	public bool IsMutedByLocalUser => true;

	public bool IsPrivateSlot => false;

	public bool IsReady
	{
		get
		{
			return (gamerState & GamerStates.Ready) != 0;
		}
		set
		{
			if ((gamerState & GamerStates.Ready) != 0 != value)
			{
				if (value)
				{
					gamerState |= GamerStates.Ready;
				}
				else
				{
					gamerState &= ~GamerStates.Ready;
				}
				OnPropertyChanged("Ready");
			}
		}
	}

	public bool IsTalking => false;

	public NetworkMachine Machine
	{
		get
		{
			return _Machine;
		}
		set
		{
			if (_Machine != value)
			{
				_Machine = value;
			}
		}
	}

	public TimeSpan RoundtripTime => TimeSpan.MinValue;

	public NetworkSession Session => session;

	internal GamerStates State
	{
		get
		{
			return gamerState;
		}
		set
		{
			gamerState = value;
		}
	}

	internal GamerStates OldState => oldGamerState;

	public event PropertyChangedEventHandler PropertyChanged;

	public NetworkGamer(NetworkSession session, byte id, GamerStates state)
	{
		this.id = id;
		this.session = session;
		gamerState = state;
		gamerState = state;
		oldGamerState = state;
	}

	protected void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
