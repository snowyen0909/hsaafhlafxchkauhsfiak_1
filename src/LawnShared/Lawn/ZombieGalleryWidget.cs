using Sexy;

namespace Lawn;

public class ZombieGalleryWidget : Widget
{
	public AlmanacDialog mDialog;

	public ZombieGalleryWidget(AlmanacDialog theDialog)
	{
		mDialog = theDialog;
		mWidth = Constants.ZombieGallerySize.X;
		mHeight = Constants.ZombieGallerySize.Y;
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		ZombieType zombieType = ZombieHitTest(x, y);
		if (zombieType != ZombieType.Invalid)
		{
			mDialog.ZombieSelected(zombieType);
		}
	}

	public ZombieType GetZombieType(int theIndex)
	{
		if (theIndex < 0 || theIndex >= GameConstants.NUM_ALMANAC_ZOMBIES)
		{
			return ZombieType.Invalid;
		}
		return AlmanacZombieDataContainer.mData[theIndex].mType;
	}

	public ZombieType ZombieHitTest(int x, int y)
	{
		for (int i = 0; i < GameConstants.NUM_ALMANAC_ZOMBIES; i++)
		{
			if (i >= 40)
			{
				continue;
			}
			ZombieType zombieType = GetZombieType(i);
			if (zombieType != ZombieType.Invalid && ZombieIsShown(zombieType))
			{
				int x2 = 0;
				int y2 = 0;
				GetZombiePosition(i, ref x2, ref y2);
				if (x >= x2 && y >= y2 && (float)x < (float)x2 + Constants.InvertAndScale(76f) && (float)y < (float)y2 + Constants.InvertAndScale(76f))
				{
					return zombieType;
				}
			}
		}
		return ZombieType.Invalid;
	}

	public void GetZombiePosition(int theIndex, ref int x, ref int y)
	{
		x = theIndex % 3 * Constants.Almanac_ZombieSpace.X;
		y = 5 + theIndex / 3 * Constants.Almanac_ZombieSpace.Y;
	}

	public bool ZombieIsShown(ZombieType theZombieType)
	{
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(theZombieType);
		int level = mDialog.mApp.mPlayerInfo.GetLevel();
		if (mDialog.mApp.IsTrialStageLocked() && theZombieType > ZombieType.Snorkel)
		{
			return false;
		}
		if (theZombieType == ZombieType.Yeti)
		{
			if (!mDialog.mApp.CanSpawnYetis())
			{
				return mDialog.ZombieHasSilhouette(ZombieType.Yeti);
			}
			return true;
		}
		if (theZombieType == ZombieType.RedeyeGargantuar)
		{
			return mDialog.mApp.mPlayerInfo.mChallengeRecords[12] >= 10;
		}
		if (theZombieType == ZombieType.RobotTitan)
		{
			if (!mDialog.mApp.HasBeatenChallenge(GameMode.ChallengeAttackOnTitans) && !mDialog.mApp.HasBeatenChallenge(GameMode.ChallengeAttackOnTitans2) && !mDialog.mApp.HasBeatenChallenge(GameMode.ChallengeAttackOnTitans3) && !mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople))
			{
				return mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople3);
			}
			return true;
		}
		if (theZombieType == ZombieType.RedeyeRobotTitan)
		{
			if (!mDialog.mApp.HasBeatenChallenge(GameMode.ChallengeAttackOnTitans2) && !mDialog.mApp.HasBeatenChallenge(GameMode.ChallengeAttackOnTitans3))
			{
				return mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople3);
			}
			return true;
		}
		if (theZombieType == ZombieType.Monk)
		{
			if (!mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople) && !mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople2))
			{
				return mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople3);
			}
			return true;
		}
		if (theZombieType == ZombieType.FootballPremium || theZombieType == ZombieType.Ninja)
		{
			if (!mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople2))
			{
				return mDialog.mApp.HasBeatenChallenge(GameMode.MarvelousPeople3);
			}
			return true;
		}
		if (theZombieType == ZombieType.Talisman)
		{
			return true;
		}
		if (theZombieType == ZombieType.Propeller)
		{
			return true;
		}
		if (theZombieType <= ZombieType.Boss)
		{
			if (!mDialog.mApp.HasFinishedAdventure())
			{
				if (zombieDefinition.mStartingLevel <= level)
				{
					if (zombieDefinition.mStartingLevel == level && (theZombieType == ZombieType.Imp || theZombieType == ZombieType.Bobsled || theZombieType == ZombieType.BackupDancer))
					{
						return AlmanacDialog.gZombieDefeated[(int)theZombieType];
					}
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override void Draw(Graphics g)
	{
		for (int i = 0; i < GameConstants.NUM_ALMANAC_ZOMBIES; i++)
		{
			AlmanacZombieData almanacZombieData = AlmanacZombieDataContainer.mData[i];
			ZombieType zombieType = GetZombieType(i);
			int x = 0;
			int y = 0;
			GetZombiePosition(i, ref x, ref y);
			if (zombieType == ZombieType.Invalid)
			{
				continue;
			}
			if (!ZombieIsShown(zombieType))
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIEBLANK, x, y);
				continue;
			}
			g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIEWINDOW, x + (int)Constants.InvertAndScale(5f), y + (int)Constants.InvertAndScale(6f));
			ZombieType theZombieType = zombieType;
			Graphics graphics = Graphics.GetNew(g);
			graphics.ClipRect(x + Constants.ZombieGalleryWidget_Window_Clip.X, y + Constants.ZombieGalleryWidget_Window_Clip.Y, Constants.ZombieGalleryWidget_Window_Clip.Width, Constants.ZombieGalleryWidget_Window_Clip.Height);
			if (mDialog.ZombieHasSilhouette(zombieType))
			{
				graphics.SetColor(new SexyColor(0, 0, 0, 64));
				graphics.SetColorizeImages(colorizeImages: true);
			}
			float num = 15f + (float)almanacZombieData.mX;
			float num2 = 25f + (float)almanacZombieData.mY;
			mDialog.mApp.mReanimatorCache.DrawCachedZombieNew(graphics, x + (int)(Constants.S * num), y + (int)(Constants.S * num2), theZombieType);
			graphics.SetColorizeImages(colorizeImages: false);
			g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIEWINDOW2, x + Constants.ZombieGalleryWidget_Window_Offset.X, y + Constants.ZombieGalleryWidget_Window_Offset.Y);
			graphics.PrepareForReuse();
		}
	}
}
