using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lawn;

public static class OnlineLevel
{
	public const string ServerBaseUrl = "https://levels.pgvz.top";

	public const string ClientVersionUrl = "https://levels.pgvz.top/api/v2/version";

	public const int ClientVersion = 99999;

	private static readonly OnlineLevelManager mManager = new OnlineLevelManager();

	public static bool IsInitialized => mManager.IsInitialized;

	public static int CurrentPage => mManager.CurrentPage;

	public static int TotalPages => mManager.TotalPages;

	public static List<OnlineLevelPreview> CurrentLevels => mManager.CurrentLevels;

	public static string LastError => mManager.LastError;

	public static Task InitializeAsync()
	{
		return mManager.InitializeAsync();
	}

	public static Task<bool> ReloadAsync()
	{
		return mManager.ReloadAsync();
	}

	public static Task<bool> LoadPageAsync(int page, string sortBy = "recommended")
	{
		return mManager.LoadPageAsync(page, sortBy);
	}

	public static Task<bool> DownloadLevelDataAsync(OnlineLevelPreview preview, GameMode gameMode)
	{
		return mManager.DownloadLevelDataAsync(preview, gameMode);
	}

	public static void ClearCache()
	{
		mManager.ClearCache();
	}
}
