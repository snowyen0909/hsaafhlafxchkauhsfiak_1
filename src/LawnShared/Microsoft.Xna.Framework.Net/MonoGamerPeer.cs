using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Xna.Framework.Net;

internal class MonoGamerPeer
{
	public class NetConnection
	{
	}

	private class NetOutgoingMessage
	{
	}

	public class NetPeer
	{
	}

	private class NetIncomingMessage
	{
	}

	private BackgroundWorker MGServerWorker = new BackgroundWorker();

	private bool done;

	private object peer;

	private NetworkSession session;

	private AvailableNetworkSession availableSession;

	private string myLocalAddress = string.Empty;

	private IPEndPoint myLocalEndPoint;

	private bool online;

	private static int port;

	private static IPEndPoint m_masterServer;

	private static int masterserverport;

	private static string masterServer;

	internal static string applicationIdentifier;

	private static NetPeer netPeer;

	private static List<NetIncomingMessage> discoveryMsgs;

	public TimeSpan SimulatedLatency
	{
		get
		{
			return new TimeSpan(0L);
		}
		set
		{
		}
	}

	public float SimulatedPacketLoss
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	internal bool IsReady => peer != null;

	static MonoGamerPeer()
	{
		port = 3074;
		masterserverport = 6000;
		masterServer = "monolive.servegame.com";
		applicationIdentifier = "monogame";
	}

	public MonoGamerPeer(NetworkSession session, AvailableNetworkSession availableSession)
	{
		this.session = session;
		online = this.session.SessionType == NetworkSessionType.PlayerMatch;
		this.availableSession = availableSession;
		MGServerWorker.WorkerSupportsCancellation = true;
		MGServerWorker.DoWork += MGServer_DoWork;
		MGServerWorker.RunWorkerCompleted += MGServer_RunWorkerCompleted;
		MGServerWorker.RunWorkerAsync();
		HookEvents();
	}

	private void HookEvents()
	{
		session.GameEnded += HandleSessionStateChanged;
		session.SessionEnded += HandleSessionStateChanged;
		session.GameStarted += HandleSessionStateChanged;
	}

	private void HandleSessionStateChanged(object sender, EventArgs e)
	{
		SendSessionStateChange();
		if (session.SessionState == NetworkSessionState.Ended)
		{
			MGServerWorker.CancelAsync();
		}
	}

	internal void ShutDown()
	{
		MGServerWorker.CancelAsync();
	}

	private void MGServer_DoWork(object sender, DoWorkEventArgs e)
	{
		throw new NotImplementedException();
	}

	private bool AlreadyConnected(IPEndPoint endPoint)
	{
		throw new NotImplementedException();
	}

	private void MGServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		throw new NotImplementedException();
	}

	internal void SendProfile(NetConnection player)
	{
		throw new NotImplementedException();
	}

	internal void SendProfileRequest(NetConnection player)
	{
		throw new NotImplementedException();
	}

	internal void SendPeerIntroductions(NetworkGamer gamer)
	{
		throw new NotImplementedException();
	}

	internal void SendGamerStateChange(NetworkGamer gamer)
	{
		throw new NotImplementedException();
	}

	internal void SendSessionStateChange()
	{
		throw new NotImplementedException();
	}

	public static IPEndPoint ParseIPEndPoint(string endPoint)
	{
		string[] array = endPoint.Split(':');
		if (array.Length != 2)
		{
			throw new FormatException("Invalid endpoint format");
		}
		if (!IPAddress.TryParse(array[0], out IPAddress address))
		{
			throw new FormatException("Invalid ip-adress");
		}
		if (!int.TryParse(array[1], out var result))
		{
			throw new FormatException("Invalid port");
		}
		return new IPEndPoint(address, result);
	}

	internal static string GetMyLocalIpAddress()
	{
		string result = "?";
		IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress[] addressList = hostEntry.AddressList;
		foreach (IPAddress iPAddress in addressList)
		{
			if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				result = iPAddress.ToString();
				break;
			}
		}
		return result;
	}

	internal void DiscoverPeers()
	{
		throw new NotImplementedException();
	}

	internal void SendData(byte[] data, SendDataOptions options)
	{
		SendMessage(NetworkMessageType.Data, data, options, null);
	}

	internal void SendData(byte[] data, SendDataOptions options, NetworkGamer gamer)
	{
		SendMessage(NetworkMessageType.Data, data, options, gamer);
	}

	private void SendMessage(NetworkMessageType messageType, byte[] data, SendDataOptions options, NetworkGamer gamer)
	{
		throw new NotImplementedException();
	}

	private void SendMessage(NetOutgoingMessage om, SendDataOptions options, NetworkGamer gamer)
	{
		throw new NotImplementedException();
	}

	internal static void Find(NetworkSessionType sessionType)
	{
		throw new NotImplementedException();
	}

	private static void GetServerList(NetPeer netPeer)
	{
		throw new NotImplementedException();
	}

	public static void RequestNATIntroduction(IPEndPoint host, NetPeer peer)
	{
		throw new NotImplementedException();
	}

	internal static void FindResults(List<AvailableNetworkSession> networkSessions)
	{
		throw new NotImplementedException();
	}

	internal void UpdateLiveSession(NetworkSession networkSession)
	{
		throw new NotImplementedException();
	}
}
