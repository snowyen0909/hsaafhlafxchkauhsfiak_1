using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

public sealed class LocalNetworkGamer : NetworkGamer
{
	private SignedInGamer sig;

	internal Queue<CommandReceiveData> receivedData;

	public bool IsDataAvailable
	{
		get
		{
			lock (receivedData)
			{
				return receivedData.Count > 0;
			}
		}
	}

	public SignedInGamer SignedInGamer
	{
		get
		{
			return sig;
		}
		internal set
		{
			sig = value;
			DisplayName = sig.DisplayName;
			Gamertag = sig.Gamertag;
		}
	}

	public LocalNetworkGamer()
		: base(null, 0, (GamerStates)0)
	{
		sig = new SignedInGamer();
		receivedData = new Queue<CommandReceiveData>();
	}

	public LocalNetworkGamer(NetworkSession session, byte id, GamerStates state)
		: base(session, id, state | GamerStates.Local)
	{
		sig = new SignedInGamer();
		receivedData = new Queue<CommandReceiveData>();
	}

	public int ReceiveData(byte[] data, int offset, out NetworkGamer sender)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (receivedData.Count <= 0)
		{
			sender = null;
			return 0;
		}
		lock (receivedData)
		{
			CommandReceiveData commandReceiveData = receivedData.Peek();
			if (offset + commandReceiveData.data.Length > data.Length)
			{
				throw new ArgumentOutOfRangeException("data", "The length + offset is greater than parameter can hold.");
			}
			receivedData.Dequeue();
			Array.Copy(commandReceiveData.data, offset, data, 0, data.Length);
			sender = commandReceiveData.gamer;
			return data.Length;
		}
	}

	public int ReceiveData(byte[] data, out NetworkGamer sender)
	{
		return ReceiveData(data, 0, out sender);
	}

	public int ReceiveData(PacketReader data, out NetworkGamer sender)
	{
		lock (receivedData)
		{
			if (receivedData.Count >= 0)
			{
				data.Reset(0);
				CommandReceiveData commandReceiveData = receivedData.Dequeue();
				if (data.Length < commandReceiveData.data.Length)
				{
					data.Reset(commandReceiveData.data.Length);
				}
				Array.Copy(commandReceiveData.data, data.Data, data.Length);
				sender = commandReceiveData.gamer;
				return data.Length;
			}
			sender = null;
			return 0;
		}
	}

	public void SendData(byte[] data, int offset, int count, SendDataOptions options)
	{
		CommandEvent item = new CommandEvent(new CommandSendData(data, offset, count, options, null, this));
		Session.commandQueue.Enqueue(item);
	}

	public void SendData(byte[] data, int offset, int count, SendDataOptions options, NetworkGamer recipient)
	{
		CommandEvent item = new CommandEvent(new CommandSendData(data, offset, count, options, recipient, this));
		Session.commandQueue.Enqueue(item);
	}

	public void SendData(byte[] data, SendDataOptions options)
	{
		CommandEvent item = new CommandEvent(new CommandSendData(data, 0, data.Length, options, null, this));
		Session.commandQueue.Enqueue(item);
	}

	public void SendData(byte[] data, SendDataOptions options, NetworkGamer recipient)
	{
		CommandEvent item = new CommandEvent(new CommandSendData(data, 0, data.Length, options, recipient, this));
		Session.commandQueue.Enqueue(item);
	}

	public void SendData(PacketWriter data, SendDataOptions options)
	{
		SendData(data.Data, 0, data.Length, options, null);
		data.Reset();
	}

	public void SendData(PacketWriter data, SendDataOptions options, NetworkGamer recipient)
	{
		SendData(data.Data, 0, data.Length, options, recipient);
		data.Reset();
	}
}
