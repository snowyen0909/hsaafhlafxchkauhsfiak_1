using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

public sealed class NetworkSession : IDisposable
{
	internal static List<NetworkSession> activeSessions = new List<NetworkSession>();

	private NetworkSessionState sessionState;

	private GamerCollection<NetworkGamer> _allGamers;

	private GamerCollection<LocalNetworkGamer> _localGamers;

	private GamerCollection<NetworkGamer> _remoteGamers;

	private GamerCollection<NetworkGamer> _previousGamers;

	internal Queue<CommandEvent> commandQueue;

	private NetworkSessionType sessionType;

	private int maxGamers;

	private int privateGamerSlots;

	private NetworkSessionProperties sessionProperties;

	private bool isHost;

	private NetworkGamer hostingGamer;

	internal MonoGamerPeer networkPeer;

	private bool _AllowHostMigration;

	private bool _AllowJoinInProgress;

	private bool _isDisposed;

	private TimeSpan defaultSimulatedLatency = new TimeSpan(0, 0, 0);

	private float simulatedPacketLoss;

	public GamerCollection<NetworkGamer> AllGamers => _allGamers;

	public bool AllowHostMigration
	{
		get
		{
			return _AllowHostMigration;
		}
		set
		{
			if (_AllowHostMigration != value)
			{
				_AllowHostMigration = value;
			}
		}
	}

	public bool AllowJoinInProgress
	{
		get
		{
			return _AllowJoinInProgress;
		}
		set
		{
			if (_AllowJoinInProgress != value)
			{
				_AllowJoinInProgress = value;
			}
		}
	}

	public NetworkGamer Host => hostingGamer;

	public bool IsDisposed => _isDisposed;

	public bool IsEveryoneReady
	{
		get
		{
			if (_allGamers.Count == 0)
			{
				return false;
			}
			foreach (NetworkGamer allGamer in _allGamers)
			{
				if (!allGamer.IsReady)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool IsHost => isHost;

	public GamerCollection<LocalNetworkGamer> LocalGamers => _localGamers;

	public int MaxGamers
	{
		get
		{
			return maxGamers;
		}
		set
		{
			maxGamers = value;
		}
	}

	public GamerCollection<NetworkGamer> PreviousGamers => _previousGamers;

	public int PrivateGamerSlots
	{
		get
		{
			return privateGamerSlots;
		}
		set
		{
			privateGamerSlots = value;
		}
	}

	public GamerCollection<NetworkGamer> RemoteGamers => _remoteGamers;

	public NetworkSessionProperties SessionProperties => sessionProperties;

	public NetworkSessionState SessionState => sessionState;

	public NetworkSessionType SessionType => sessionType;

	public TimeSpan SimulatedLatency
	{
		get
		{
			return defaultSimulatedLatency;
		}
		set
		{
			defaultSimulatedLatency = value;
		}
	}

	public float SimulatedPacketLoss
	{
		get
		{
			if (networkPeer != null)
			{
				simulatedPacketLoss = networkPeer.SimulatedPacketLoss;
			}
			return simulatedPacketLoss;
		}
		set
		{
			if (networkPeer != null)
			{
				networkPeer.SimulatedPacketLoss = value;
			}
			simulatedPacketLoss = value;
		}
	}

	public event EventHandler<GameEndedEventArgs> GameEnded;

	public event EventHandler<GamerJoinedEventArgs> GamerJoined;

	public event EventHandler<GamerLeftEventArgs> GamerLeft;

	public event EventHandler<GameStartedEventArgs> GameStarted;

	public event EventHandler<HostChangedEventArgs> HostChanged;

	public static event EventHandler<InviteAcceptedEventArgs> InviteAccepted;

	public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

	private NetworkSession()
	{
		activeSessions.Add(this);
	}

	~NetworkSession()
	{
		Dispose(disposing: false);
	}

	private NetworkSession(NetworkSessionType sessionType, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, bool isHost, int hostGamer)
		: this(sessionType, maxGamers, privateGamerSlots, sessionProperties, isHost, hostGamer, null)
	{
	}

	private NetworkSession(NetworkSessionType sessionType, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, bool isHost, int hostGamer, AvailableNetworkSession availableSession)
		: this()
	{
		if (sessionProperties == null)
		{
			throw new ArgumentNullException("sessionProperties");
		}
		_allGamers = new GamerCollection<NetworkGamer>();
		_localGamers = new GamerCollection<LocalNetworkGamer>();
		_remoteGamers = new GamerCollection<NetworkGamer>();
		_previousGamers = new GamerCollection<NetworkGamer>();
		hostingGamer = null;
		commandQueue = new Queue<CommandEvent>();
		this.sessionType = sessionType;
		this.maxGamers = maxGamers;
		this.privateGamerSlots = privateGamerSlots;
		this.sessionProperties = sessionProperties;
		this.isHost = isHost;
		if (isHost)
		{
			networkPeer = new MonoGamerPeer(this, null);
		}
		else if (networkPeer == null)
		{
			networkPeer = new MonoGamerPeer(this, availableSession);
		}
		CommandGamerJoined command = new CommandGamerJoined(hostGamer, this.isHost, isLocal: true);
		commandQueue.Enqueue(new CommandEvent(command));
	}

	public static NetworkSession Create(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties)
	{
		return EndCreate(BeginCreate(sessionType, localGamers, maxGamers, privateGamerSlots, sessionProperties, null, null));
	}

	public static NetworkSession Create(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers)
	{
		return EndCreate(BeginCreate(sessionType, maxLocalGamers, maxGamers, null, null));
	}

	public static NetworkSession Create(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties)
	{
		return EndCreate(BeginCreate(sessionType, maxLocalGamers, maxGamers, privateGamerSlots, sessionProperties, null, null));
	}

	private static NetworkSession Create(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, int hostGamer, bool isHost)
	{
		NetworkSession networkSession = null;
		if (sessionProperties == null)
		{
			sessionProperties = new NetworkSessionProperties();
		}
		return new NetworkSession(sessionType, maxGamers, privateGamerSlots, sessionProperties, isHost, hostGamer);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Dispose(bool disposing)
	{
		if (_isDisposed)
		{
			return;
		}
		if (disposing)
		{
			foreach (NetworkGamer allGamer in _allGamers)
			{
				allGamer.Dispose();
			}
			if (networkPeer != null)
			{
				networkPeer.ShutDown();
			}
			if (networkPeer != null)
			{
				networkPeer.ShutDown();
			}
		}
		_isDisposed = true;
	}

	public void AddLocalGamer(SignedInGamer gamer)
	{
		if (gamer == null)
		{
			throw new ArgumentNullException("gamer");
		}
	}

	public static IAsyncResult BeginCreate(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, AsyncCallback callback, object asyncState)
	{
		int num = -1;
		num = GetHostingGamerIndex(localGamers);
		return BeginCreate(sessionType, num, 4, maxGamers, privateGamerSlots, sessionProperties, callback, asyncState);
	}

	public static IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, AsyncCallback callback, object asyncState)
	{
		return BeginCreate(sessionType, -1, maxLocalGamers, maxGamers, 0, null, callback, asyncState);
	}

	public static IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, AsyncCallback callback, object asyncState)
	{
		return BeginCreate(sessionType, -1, maxLocalGamers, maxGamers, privateGamerSlots, sessionProperties, callback, asyncState);
	}

	private static IAsyncResult BeginCreate(NetworkSessionType sessionType, int hostGamer, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, AsyncCallback callback, object asyncState)
	{
		if (maxLocalGamers < 1 || maxLocalGamers > 4)
		{
			throw new ArgumentOutOfRangeException("Maximum local players must be between 1 and 4.");
		}
		if (maxGamers < 2 || maxGamers > 32)
		{
			throw new ArgumentOutOfRangeException("Maximum number of gamers must be between 2 and 32.");
		}
		NetworkSessionAsynchronousCreate networkSessionAsynchronousCreate = Create;
		return networkSessionAsynchronousCreate.BeginInvoke(sessionType, maxLocalGamers, maxGamers, privateGamerSlots, sessionProperties, hostGamer, isHost: true, callback, asyncState);
	}

	internal static int GetHostingGamerIndex(IEnumerable<SignedInGamer> localGamers)
	{
		SignedInGamer signedInGamer = null;
		if (localGamers == null)
		{
			throw new ArgumentNullException("localGamers");
		}
		foreach (SignedInGamer localGamer in localGamers)
		{
			if (localGamer == null)
			{
				throw new ArgumentException("gamer can not be null in list of localGamers.");
			}
			if (localGamer.IsDisposed)
			{
				throw new ObjectDisposedException("localGamers", "A gamer is disposed in the list of localGamers");
			}
			if (signedInGamer == null)
			{
				signedInGamer = localGamer;
			}
		}
		if (signedInGamer == null)
		{
			throw new ArgumentException("Invalid gamer in localGamers.");
		}
		return (int)signedInGamer.PlayerIndex;
	}

	public static IAsyncResult BeginFind(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
	{
		int num = -1;
		num = GetHostingGamerIndex(localGamers);
		return BeginFind(sessionType, num, 4, searchProperties, callback, asyncState);
	}

	public static IAsyncResult BeginFind(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
	{
		return BeginFind(sessionType, -1, 4, searchProperties, callback, asyncState);
	}

	private static IAsyncResult BeginFind(NetworkSessionType sessionType, int hostGamer, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
	{
		if (sessionType == NetworkSessionType.Local)
		{
			throw new ArgumentException("NetworkSessionType cannot be NetworkSessionType.Local");
		}
		if (maxLocalGamers < 1 || maxLocalGamers > 4)
		{
			throw new ArgumentOutOfRangeException("maxLocalGamers must be between 1 and 4.");
		}
		NetworkSessionAsynchronousFind networkSessionAsynchronousFind = Find;
		return networkSessionAsynchronousFind.BeginInvoke(sessionType, hostGamer, maxLocalGamers, searchProperties, callback, asyncState);
	}

	public static IAsyncResult BeginJoin(AvailableNetworkSession availableSession, AsyncCallback callback, object asyncState)
	{
		if (availableSession == null)
		{
			throw new ArgumentNullException();
		}
		NetworkSessionAsynchronousJoin networkSessionAsynchronousJoin = JoinSession;
		return networkSessionAsynchronousJoin.BeginInvoke(availableSession, callback, asyncState);
	}

	public static NetworkSession EndCreate(IAsyncResult result)
	{
		NetworkSession result2 = null;
		try
		{
			AsyncResult asyncResult = (AsyncResult)result;
			result.AsyncWaitHandle.WaitOne();
			if (asyncResult.AsyncDelegate is NetworkSessionAsynchronousCreate)
			{
				result2 = ((NetworkSessionAsynchronousCreate)asyncResult.AsyncDelegate).EndInvoke(result);
			}
		}
		finally
		{
			result.AsyncWaitHandle.Close();
		}
		return result2;
	}

	public static AvailableNetworkSessionCollection EndFind(IAsyncResult result)
	{
		AvailableNetworkSessionCollection availableNetworkSessionCollection = null;
		List<AvailableNetworkSession> list = new List<AvailableNetworkSession>();
		try
		{
			AsyncResult asyncResult = (AsyncResult)result;
			result.AsyncWaitHandle.WaitOne();
			if (asyncResult.AsyncDelegate is NetworkSessionAsynchronousFind)
			{
				availableNetworkSessionCollection = ((NetworkSessionAsynchronousFind)asyncResult.AsyncDelegate).EndInvoke(result);
				MonoGamerPeer.FindResults(list);
			}
		}
		finally
		{
			result.AsyncWaitHandle.Close();
		}
		return new AvailableNetworkSessionCollection(list);
	}

	public void EndGame()
	{
		CommandSessionStateChange command = new CommandSessionStateChange(NetworkSessionState.Lobby, sessionState);
		commandQueue.Enqueue(new CommandEvent(command));
	}

	public static NetworkSession EndJoin(IAsyncResult result)
	{
		NetworkSession result2 = null;
		try
		{
			AsyncResult asyncResult = (AsyncResult)result;
			result.AsyncWaitHandle.WaitOne();
			if (asyncResult.AsyncDelegate is NetworkSessionAsynchronousJoin)
			{
				result2 = ((NetworkSessionAsynchronousJoin)asyncResult.AsyncDelegate).EndInvoke(result);
			}
		}
		finally
		{
			result.AsyncWaitHandle.Close();
		}
		return result2;
	}

	public static AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, NetworkSessionProperties searchProperties)
	{
		int num = -1;
		num = GetHostingGamerIndex(localGamers);
		return EndFind(BeginFind(sessionType, num, 4, searchProperties, null, null));
	}

	public static AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties)
	{
		return EndFind(BeginFind(sessionType, -1, maxLocalGamers, searchProperties, null, null));
	}

	private static AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, int hostGamer, int maxLocalGamers, NetworkSessionProperties searchProperties)
	{
		if (maxLocalGamers < 1 || maxLocalGamers > 4)
		{
			throw new ArgumentOutOfRangeException("maxLocalGamers must be between 1 and 4.");
		}
		List<AvailableNetworkSession> list = new List<AvailableNetworkSession>();
		MonoGamerPeer.Find(sessionType);
		return new AvailableNetworkSessionCollection(list);
	}

	public NetworkGamer FindGamerById(byte gamerId)
	{
		foreach (NetworkGamer allGamer in _allGamers)
		{
			if (allGamer.Id == gamerId)
			{
				return allGamer;
			}
		}
		return null;
	}

	public static NetworkSession Join(AvailableNetworkSession availableSession)
	{
		return EndJoin(BeginJoin(availableSession, null, null));
	}

	private static NetworkSession JoinSession(AvailableNetworkSession availableSession)
	{
		NetworkSession networkSession = null;
		NetworkSessionType networkSessionType = availableSession.SessionType;
		int num = 32;
		int num2 = 0;
		bool flag = false;
		int hostGamer = -1;
		NetworkSessionProperties networkSessionProperties = availableSession.SessionProperties;
		if (networkSessionProperties == null)
		{
			networkSessionProperties = new NetworkSessionProperties();
		}
		return new NetworkSession(networkSessionType, num, num2, networkSessionProperties, flag, hostGamer, availableSession);
	}

	public void ResetReady()
	{
		foreach (LocalNetworkGamer localGamer in _localGamers)
		{
			localGamer.IsReady = false;
		}
	}

	public void StartGame()
	{
		CommandSessionStateChange command = new CommandSessionStateChange(NetworkSessionState.Playing, sessionState);
		commandQueue.Enqueue(new CommandEvent(command));
	}

	public void Update()
	{
		try
		{
			while (commandQueue.Count > 0 && networkPeer.IsReady)
			{
				CommandEvent commandEvent = commandQueue.Dequeue();
				if (commandEvent != null)
				{
					switch (commandEvent.Command)
					{
					case CommandEventType.SendData:
						ProcessSendData((CommandSendData)commandEvent.CommandObject);
						break;
					case CommandEventType.ReceiveData:
						ProcessReceiveData((CommandReceiveData)commandEvent.CommandObject);
						break;
					case CommandEventType.GamerJoined:
						ProcessGamerJoined((CommandGamerJoined)commandEvent.CommandObject);
						break;
					case CommandEventType.GamerLeft:
						ProcessGamerLeft((CommandGamerLeft)commandEvent.CommandObject);
						break;
					case CommandEventType.SessionStateChange:
						ProcessSessionStateChange((CommandSessionStateChange)commandEvent.CommandObject);
						break;
					case CommandEventType.GamerStateChange:
						ProcessGamerStateChange((CommandGamerStateChange)commandEvent.CommandObject);
						break;
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void ProcessGamerStateChange(CommandGamerStateChange command)
	{
		networkPeer.SendGamerStateChange(command.Gamer);
	}

	private void ProcessSendData(CommandSendData command)
	{
		networkPeer.SendData(command.data, command.options);
		CommandReceiveData commandReceiveData = new CommandReceiveData(command.sender.RemoteUniqueIdentifier, command.data);
		commandReceiveData.gamer = command.sender;
		foreach (LocalNetworkGamer localGamer in _localGamers)
		{
			localGamer.receivedData.Enqueue(commandReceiveData);
		}
	}

	private void ProcessReceiveData(CommandReceiveData command)
	{
		foreach (NetworkGamer allGamer in _allGamers)
		{
			if (allGamer.RemoteUniqueIdentifier == command.remoteUniqueIdentifier)
			{
				command.gamer = allGamer;
			}
		}
		if (command.gamer == null)
		{
			return;
		}
		foreach (LocalNetworkGamer localGamer in LocalGamers)
		{
			lock (localGamer.receivedData)
			{
				localGamer.receivedData.Enqueue(command);
			}
		}
	}

	private void ProcessSessionStateChange(CommandSessionStateChange command)
	{
		if (sessionState == command.NewState)
		{
			return;
		}
		sessionState = command.NewState;
		switch (command.NewState)
		{
		case NetworkSessionState.Ended:
			ResetReady();
			throw new NotImplementedException();
		case NetworkSessionState.Playing:
			throw new NotImplementedException();
		}
		if (command.NewState != NetworkSessionState.Lobby || command.OldState != NetworkSessionState.Playing)
		{
			return;
		}
		ResetReady();
		throw new NotImplementedException();
	}

	private void ProcessGamerJoined(CommandGamerJoined command)
	{
		NetworkGamer networkGamer;
		if ((command.State & GamerStates.Local) != 0)
		{
			networkGamer = new LocalNetworkGamer(this, (byte)command.InternalIndex, command.State);
			_allGamers.AddGamer(networkGamer);
			_localGamers.AddGamer((LocalNetworkGamer)networkGamer);
			if (Gamer.SignedInGamers.Count >= _localGamers.Count)
			{
				((LocalNetworkGamer)networkGamer).SignedInGamer = Gamer.SignedInGamers[_localGamers.Count - 1];
			}
			networkGamer.PropertyChanged += HandleGamerPropertyChanged;
		}
		else
		{
			networkGamer = new NetworkGamer(this, (byte)command.InternalIndex, command.State);
			networkGamer.DisplayName = command.DisplayName;
			networkGamer.Gamertag = command.GamerTag;
			networkGamer.RemoteUniqueIdentifier = command.remoteUniqueIdentifier;
			_allGamers.AddGamer(networkGamer);
			_remoteGamers.AddGamer(networkGamer);
		}
		if ((command.State & GamerStates.Host) != 0)
		{
			hostingGamer = networkGamer;
		}
		networkGamer.Machine = new NetworkMachine();
		networkGamer.Machine.Gamers.AddGamer(networkGamer);
		throw new NotImplementedException();
	}

	private void ProcessGamerLeft(CommandGamerLeft command)
	{
		for (int i = 0; i < _remoteGamers.Count; i++)
		{
			if (_remoteGamers[i].RemoteUniqueIdentifier == command.remoteUniqueIdentifier)
			{
				NetworkGamer item = _remoteGamers[i];
				_remoteGamers.RemoveGamer(item);
				_allGamers.RemoveGamer(item);
				throw new NotImplementedException();
			}
		}
		if (networkPeer != null)
		{
			networkPeer.UpdateLiveSession(this);
		}
	}

	private void HandleGamerPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (sender is NetworkGamer { IsLocal: not false } networkGamer)
		{
			CommandGamerStateChange command = new CommandGamerStateChange(networkGamer);
			CommandEvent item = new CommandEvent(command);
			commandQueue.Enqueue(item);
		}
	}

	private bool SuppressEventHandlerWarningsUntilEventsAreProperlyImplemented()
	{
		if (HostChanged != null)
		{
			return InviteAccepted != null;
		}
		return false;
	}

	internal static void Exit()
	{
		if (activeSessions == null || activeSessions.Count <= 0)
		{
			return;
		}
		foreach (NetworkSession activeSession in activeSessions)
		{
			if (!activeSession.IsDisposed)
			{
				activeSession.Dispose();
			}
		}
	}
}
