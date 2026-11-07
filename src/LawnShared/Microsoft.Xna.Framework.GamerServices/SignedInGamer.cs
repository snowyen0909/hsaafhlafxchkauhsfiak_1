using System;

namespace Microsoft.Xna.Framework.GamerServices;

public class SignedInGamer : Gamer
{
	private delegate void AuthenticationDelegate();

	private delegate AchievementCollection GetAchievementsDelegate();

	private delegate void AwardAchievementDelegate(string achievementId, double percentageComplete);

	private AchievementCollection gamerAchievements;

	private FriendCollection friendCollection;

	private bool isSignedInToLive = true;

	private GamerPrivileges _privileges = new GamerPrivileges();

	public GameDefaults GameDefaults
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool IsGuest
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool IsSignedInToLive
	{
		get
		{
			return isSignedInToLive;
		}
		internal set
		{
			isSignedInToLive = value;
		}
	}

	public int PartySize
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public PlayerIndex PlayerIndex => PlayerIndex.One;

	public GamerPresence Presence
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public GamerPrivileges Privileges => _privileges;

	public static event EventHandler<SignedInEventArgs> SignedIn;

	public static event EventHandler<SignedOutEventArgs> SignedOut;

	public IAsyncResult BeginAuthentication(AsyncCallback callback, object asyncState)
	{
		AuthenticationDelegate authenticationDelegate = DoAuthentication;
		return authenticationDelegate.BeginInvoke(callback, authenticationDelegate);
	}

	public void EndAuthentication(IAsyncResult result)
	{
		AuthenticationDelegate authenticationDelegate = (AuthenticationDelegate)result.AsyncState;
		authenticationDelegate.EndInvoke(result);
	}

	private void DoAuthentication()
	{
	}

	public SignedInGamer()
	{
		IAsyncResult result = BeginAuthentication(null, null);
		EndAuthentication(result);
	}

	private void AuthenticationCompletedCallback(IAsyncResult result)
	{
		EndAuthentication(result);
	}

	public FriendCollection GetFriends()
	{
		if (IsSignedInToLive && friendCollection == null)
		{
			friendCollection = new FriendCollection();
		}
		return friendCollection;
	}

	public bool IsFriend(Gamer gamer)
	{
		if (gamer == null)
		{
			throw new ArgumentNullException();
		}
		if (gamer.IsDisposed)
		{
			throw new ObjectDisposedException(gamer.ToString());
		}
		bool result = false;
		foreach (FriendGamer item in friendCollection)
		{
			if (item.Gamertag == gamer.Gamertag)
			{
				result = true;
			}
		}
		return result;
	}

	public IAsyncResult BeginGetAchievements(AsyncCallback callback, object asyncState)
	{
		GetAchievementsDelegate getAchievementsDelegate = GetAchievements;
		return getAchievementsDelegate.BeginInvoke(callback, getAchievementsDelegate);
	}

	private void GetAchievementCompletedCallback(IAsyncResult result)
	{
		GetAchievementsDelegate getAchievementsDelegate = (GetAchievementsDelegate)result.AsyncState;
		gamerAchievements = getAchievementsDelegate.EndInvoke(result);
	}

	public AchievementCollection EndGetAchievements(IAsyncResult result)
	{
		GetAchievementsDelegate getAchievementsDelegate = (GetAchievementsDelegate)result.AsyncState;
		gamerAchievements = getAchievementsDelegate.EndInvoke(result);
		return gamerAchievements;
	}

	public AchievementCollection GetAchievements()
	{
		if (IsSignedInToLive && gamerAchievements == null)
		{
			gamerAchievements = new AchievementCollection();
		}
		return gamerAchievements;
	}

	public IAsyncResult BeginAwardAchievement(string achievementId, AsyncCallback callback, object state)
	{
		return BeginAwardAchievement(achievementId, 100.0, callback, state);
	}

	public IAsyncResult BeginAwardAchievement(string achievementId, double percentageComplete, AsyncCallback callback, object state)
	{
		AwardAchievementDelegate awardAchievementDelegate = DoAwardAchievement;
		return awardAchievementDelegate.BeginInvoke(achievementId, percentageComplete, callback, awardAchievementDelegate);
	}

	public void EndAwardAchievement(IAsyncResult result)
	{
		AwardAchievementDelegate awardAchievementDelegate = (AwardAchievementDelegate)result.AsyncState;
		awardAchievementDelegate.EndInvoke(result);
	}

	private void AwardAchievementCompletedCallback(IAsyncResult result)
	{
		EndAwardAchievement(result);
	}

	public void AwardAchievement(string achievementId)
	{
		AwardAchievement(achievementId, 100.0);
	}

	public void DoAwardAchievement(string achievementId, double percentageComplete)
	{
	}

	public void AwardAchievement(string achievementId, double percentageComplete)
	{
		if (IsSignedInToLive)
		{
			BeginAwardAchievement(achievementId, percentageComplete, AwardAchievementCompletedCallback, null);
		}
	}

	public void UpdateScore(string aCategory, long aScore)
	{
		_ = IsSignedInToLive;
	}

	public void ResetAchievements()
	{
		_ = IsSignedInToLive;
	}

	protected virtual void OnSignedIn(SignedInEventArgs e)
	{
		throw new NotImplementedException();
	}

	protected virtual void OnSignedOut(SignedOutEventArgs e)
	{
		throw new NotImplementedException();
	}
}
