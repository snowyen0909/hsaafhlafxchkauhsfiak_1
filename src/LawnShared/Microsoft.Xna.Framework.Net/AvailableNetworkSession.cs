using System.Net;

namespace Microsoft.Xna.Framework.Net;

public sealed class AvailableNetworkSession
{
	private int _currentGameCount;

	private string _hostGamertag;

	private int _openPrivateGamerSlots;

	private int _openPublicGamerSlots;

	private QualityOfService _QualityOfService;

	private NetworkSessionProperties _sessionProperties;

	private IPEndPoint _endPoint;

	private IPEndPoint _internalendPoint;

	public int CurrentGamerCount
	{
		get
		{
			return _currentGameCount;
		}
		internal set
		{
			_currentGameCount = value;
		}
	}

	public string HostGamertag
	{
		get
		{
			return _hostGamertag;
		}
		internal set
		{
			_hostGamertag = value;
		}
	}

	public int OpenPrivateGamerSlots
	{
		get
		{
			return _openPrivateGamerSlots;
		}
		internal set
		{
			_openPrivateGamerSlots = value;
		}
	}

	public int OpenPublicGamerSlots
	{
		get
		{
			return _openPublicGamerSlots;
		}
		internal set
		{
			_openPublicGamerSlots = value;
		}
	}

	public QualityOfService QualityOfService
	{
		get
		{
			return _QualityOfService;
		}
		internal set
		{
			_QualityOfService = value;
		}
	}

	public NetworkSessionProperties SessionProperties
	{
		get
		{
			return _sessionProperties;
		}
		internal set
		{
			_sessionProperties = value;
		}
	}

	internal IPEndPoint EndPoint
	{
		get
		{
			return _endPoint;
		}
		set
		{
			_endPoint = value;
		}
	}

	internal IPEndPoint InternalEndpont
	{
		get
		{
			return _internalendPoint;
		}
		set
		{
			_internalendPoint = value;
		}
	}

	internal NetworkSessionType SessionType { get; set; }

	public AvailableNetworkSession()
	{
		_QualityOfService = new QualityOfService();
	}
}
