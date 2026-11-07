using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.GamerServices;

namespace Sexy;

internal static class LeaderBoardComm
{
	public enum ConnectionState
	{
		Connected,
		Connecting,
		CannotConnect
	}

	private enum ColumnIndices
	{
		Score
	}

	public delegate void LoadingCompletedHandler();

	[CompilerGenerated]
	private static ConnectionState State__BackingField;

	private const int cannotConnectDelay = int.MaxValue;

	public static object LeaderboardLock;

	private static DateTime cannotConnectSince;

	private static string[] columnIndexStrings;

	private static LeaderBoardLoader[] leaderboardLoaders;

	private static Dictionary<string, Image> gamerImages;

	private static string e;

	private static List<Gamer> loadingGamers;

	public static ConnectionState State
	{
		[CompilerGenerated]
		get
		{
			return State__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			State__BackingField = value;
		}
	}

	public static Image UnknownPlayerImage => null;

	public static event LoadingCompletedHandler LoadingCompleted;

	static LeaderBoardComm()
	{
		State__BackingField = ConnectionState.Connecting;
		LeaderboardLock = new object();
		columnIndexStrings = new string[1] { "SCORE" };
		leaderboardLoaders = new LeaderBoardLoader[3];
		gamerImages = new Dictionary<string, Image>();
		loadingGamers = new List<Gamer>();
		for (int i = 0; i < 3; i++)
		{
			if (LeaderBoardHelper.IsModeSupported((LeaderboardGameMode)i))
			{
				leaderboardLoaders[i] = new LeaderBoardLoader((LeaderboardGameMode)i);
				leaderboardLoaders[i].LoadingCompleted += GetResultsCallBack;
			}
		}
	}

	public static void RecordResult(LeaderboardGameMode gameMode, int score)
	{
		if (!SexyAppBase.UseLiveServers || SexyAppBase.IsInTrialMode || !LeaderBoardHelper.IsModeSupported(gameMode) || Gamer.SignedInGamers.Count == 0)
		{
			return;
		}
		SignedInGamer gamer = Main.GetGamer();
		lock (LeaderboardLock)
		{
			try
			{
				int leaderboardNumber = LeaderBoardHelper.GetLeaderboardNumber(gameMode);
				LeaderboardIdentity aLeaderboardIdentity = LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, leaderboardNumber);
				LeaderboardWriter leaderboardWriter = gamer.LeaderboardWriter;
				LeaderboardEntry leaderboard = leaderboardWriter.GetLeaderboard(aLeaderboardIdentity);
				leaderboard.Rating = score;
				LeaderBoardLoader[] array = leaderboardLoaders;
				foreach (LeaderBoardLoader leaderBoardLoader in array)
				{
					leaderBoardLoader.ResetCache();
				}
			}
			catch (GameUpdateRequiredException)
			{
				GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
			}
			catch (Exception)
			{
			}
		}
	}

	public static bool IsPlayer(Gamer signedInGamer, int index, LeaderboardState state)
	{
		if (!SexyAppBase.UseLiveServers)
		{
			return false;
		}
		lock (LeaderboardLock)
		{
			try
			{
				if (signedInGamer == null)
				{
					return false;
				}
				LeaderboardEntry entry = GetEntry(index, state);
				if (entry != null)
				{
					return signedInGamer.Gamertag == entry.Gamer.Gamertag;
				}
			}
			catch (GameUpdateRequiredException)
			{
				GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
			}
			catch (Exception)
			{
			}
		}
		return false;
	}

	public static void LoadInitialLeaderboard()
	{
		for (int i = 0; i < 3; i++)
		{
			LoadResults((LeaderboardGameMode)i);
		}
	}

	public static int GetMaxEntries(LeaderboardState state)
	{
		LeaderBoardLoader loader = GetLoader(state);
		return loader.LeaderboardEntryCount;
	}

	private static void GetGamerCallBack(IAsyncResult result)
	{
		lock (LeaderboardLock)
		{
			try
			{
				if (result.AsyncState is Gamer gamer)
				{
					if (gamerImages.ContainsKey(gamer.Gamertag))
					{
						gamerImages.Remove(gamer.Gamertag);
					}
					GamerProfile gamerProfile = gamer.EndGetProfile(result);
				}
			}
			catch (GameUpdateRequiredException)
			{
				GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
			}
			catch (Exception)
			{
			}
		}
	}

	public static Image GetGamerImage(Gamer gamer)
	{
		Image value = UnknownPlayerImage;
		if (!SexyAppBase.UseLiveServers || gamer == null)
		{
			return UnknownPlayerImage;
		}
		lock (LeaderboardLock)
		{
			if (!gamerImages.TryGetValue(gamer.Gamertag, out value))
			{
				value = UnknownPlayerImage;
				gamerImages.Add(gamer.Gamertag, value);
				try
				{
					gamer.BeginGetProfile(GetGamerCallBack, gamer);
				}
				catch (GameUpdateRequiredException)
				{
					GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
				}
				catch (Exception ex2)
				{
					e = ex2.Message;
				}
			}
		}
		return value;
	}

	public static Image GetLeaderboardGamerImage(int index, LeaderboardState state)
	{
		if (!SexyAppBase.UseLiveServers)
		{
			return UnknownPlayerImage;
		}
		lock (LeaderboardLock)
		{
			Gamer gamer = null;
			try
			{
				LeaderboardEntry entry = GetEntry(index, state);
				if (entry != null)
				{
					gamer = entry.Gamer;
				}
			}
			catch (GameUpdateRequiredException)
			{
				GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
			}
			catch (Exception)
			{
			}
			return GetGamerImage(gamer);
		}
	}

	public static Gamer GetLeaderboardGamer(int index, LeaderboardState state)
	{
		if (!SexyAppBase.UseLiveServers)
		{
			return null;
		}
		lock (LeaderboardLock)
		{
			Gamer result = null;
			try
			{
				LeaderboardEntry entry = GetEntry(index, state);
				if (entry != null)
				{
					result = entry.Gamer;
				}
			}
			catch (GameUpdateRequiredException)
			{
				GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
			}
			catch (Exception)
			{
			}
			return result;
		}
	}

	public static int GetSignedInGamerIndex(LeaderboardState state)
	{
		LeaderBoardLoader loader = GetLoader(state);
		return loader.SignedInGamerIndex;
	}

	private static LeaderBoardLoader GetLoader(LeaderboardState state)
	{
		return leaderboardLoaders[LeaderBoardHelper.GetLeaderboardNumber(state)];
	}

	private static LeaderBoardLoader GetLoader(LeaderboardGameMode mode)
	{
		return leaderboardLoaders[LeaderBoardHelper.GetLeaderboardNumber(mode)];
	}

	private static LeaderboardEntry GetEntry(int index, LeaderboardState state)
	{
		LeaderBoardLoader leaderBoardLoader = leaderboardLoaders[LeaderBoardHelper.GetLeaderboardNumber(state)];
		if (!leaderBoardLoader.LeaderboardEntries.TryGetValue(index, out var value))
		{
			leaderBoardLoader.LoadEntry(index);
		}
		return value;
	}

	public static long GetLeaderboardScore(int index, LeaderboardState state)
	{
		if (!SexyAppBase.UseLiveServers)
		{
			return 0L;
		}
		lock (LeaderboardLock)
		{
			try
			{
				LeaderboardEntry entry = GetEntry(index, state);
				if (entry != null)
				{
					return entry.Rating;
				}
			}
			catch (GameUpdateRequiredException)
			{
				GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
			}
			catch (Exception)
			{
			}
		}
		return 0L;
	}

	public static void SetCache(LeaderboardGameMode gameMode)
	{
		LeaderBoardLoader loader = GetLoader(gameMode);
		loader.CACHE_DURATION = 10;
	}

	public static int LoadResults(LeaderboardGameMode gameMode)
	{
		lock (LeaderboardLock)
		{
			LeaderBoardLoader loader = GetLoader(gameMode);
			loader.SendRequest();
			State = ConnectionState.Connecting;
			if (loader.LeaderboardConnectionState == LeaderBoardLoader.LoaderState.Loaded || (loader.LeaderboardConnectionState == LeaderBoardLoader.LoaderState.Loading && loader.LeaderboardEntryCount > 0))
			{
				loader.CACHE_DURATION = int.MaxValue;
				return loader.LeaderboardEntryCount;
			}
			loader.CACHE_DURATION = int.MaxValue;
		}
		return -1;
	}

	private static void GetResultsCallBack(LeaderBoardLoader loader)
	{
		lock (LeaderboardLock)
		{
			switch (loader.ErrorState)
			{
			case LeaderBoardLoader.ErrorStates.None:
				State = ConnectionState.Connected;
				break;
			case LeaderBoardLoader.ErrorStates.GameUpdateRequired:
				GlobalStaticVars.gSexyAppBase.ShowUpdateRequiredMessage();
				break;
			case LeaderBoardLoader.ErrorStates.Error:
				State = ConnectionState.CannotConnect;
				cannotConnectSince = DateTime.UtcNow;
				break;
			}
			if (LoadingCompleted != null)
			{
				LoadingCompleted();
			}
		}
	}
}
