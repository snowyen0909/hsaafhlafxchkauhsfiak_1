// Cheat-menu behaviours that the Android build injected at runtime with MonoMod
// detours (not possible on iOS). The target methods now call these directly.
namespace Lawn;

internal static class ZBCheatHooks
{
	public static void CoinAutoCollect(Coin self)
	{
		if (self.mBoard == null)
		{
			return;
		}
		switch (self.mType)
		{
		case CoinType.Silver:
		case CoinType.Gold:
		case CoinType.Diamond:
		case CoinType.Sun:
		case CoinType.Smallsun:
		case CoinType.Largesun:
		case CoinType.PresentPlant:
		case CoinType.Chocolate:
		case CoinType.Tinysun:
			if (!self.mIsBeingCollected)
			{
				self.PlayCollectSound();
				self.Collect();
				if (self.mApp.IsFirstTimeAdventureMode() && self.mBoard.mLevel == 1)
				{
					self.mBoard.DisplayAdvice("[ADVICE_CLICKED_ON_SUN]", MessageStyle.TutorialLevel1Stay, AdviceType.ClickedOnSun);
				}
			}
			break;
		}
	}

	public static void AdvancedPauseUpdateGame(Board self)
	{
		if (self.mApp.mGameScene == GameScenes.LevelIntro)
		{
			int count = self.mZombies.Count;
			for (int i = 0; i < count; i++)
			{
				Zombie zombie = self.mZombies[i];
				if (!zombie.mDead)
				{
					zombie.UpdateReanim();
				}
			}
		}
		Coin theCoin = null;
		while (self.IterateCoins(ref theCoin))
		{
			if (theCoin.mIsBeingCollected)
			{
				theCoin.Update();
			}
		}
		self.mCursorPreview.Update();
		self.mCursorObject.Update();
	}
}
