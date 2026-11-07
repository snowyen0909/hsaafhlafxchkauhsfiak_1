using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class MiniGamesWidget : Widget
{
	private ChallengeDefinition[][] mCachedChallengeDefs;

	public LawnApp mApp;

	public MiniGamesWidgetListener mListener;

	public MiniGameMode mMode;

	public MiniGamesWidget(LawnApp theApp, MiniGamesWidgetListener theListener)
	{
		mApp = theApp;
		mListener = theListener;
		mWidth = 0;
		mHeight = AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight + 20;
		mMode = MiniGameMode.Games;
		GenerateCachedChallengeDefs();
	}

	private void GenerateCachedChallengeDefs()
	{
		mCachedChallengeDefs = new ChallengeDefinition[8][];
		List<ChallengeDefinition>[] array = new List<ChallengeDefinition>[8];
		for (int i = 0; i < 8; i++)
		{
			array[i] = new List<ChallengeDefinition>();
		}
		ChallengeDefinition[] gChallengeDefs = ChallengeScreen.gChallengeDefs;
		foreach (ChallengeDefinition challengeDefinition in gChallengeDefs)
		{
			for (int k = 0; k < 8; k++)
			{
				if (challengeDefinition.mPage == (ChallengePage)k && !IsLevelExcluded(challengeDefinition))
				{
					array[k].Add(challengeDefinition);
				}
			}
		}
		for (int l = 0; l < 8; l++)
		{
			mCachedChallengeDefs[l] = array[l].ToArray();
		}
		static bool IsLevelExcluded(ChallengeDefinition ch)
		{
			GameMode mChallengeMode = ch.mChallengeMode;
			if ((uint)(mChallengeMode - 41) <= 1u || (uint)(mChallengeMode - 48) <= 1u || (uint)(mChallengeMode - 70) <= 1u)
			{
				return true;
			}
			return false;
		}
	}

	public void SwitchMode(MiniGameMode mode)
	{
		mMode = mode;
		SizeToFit();
	}

	public int GetModeLevelCount()
	{
		if (mApp.mPlayerInfo == null)
		{
			return 0;
		}
		switch (mMode)
		{
		case MiniGameMode.Games:
			if (mApp.mPlayerInfo.mMiniGamesUnlocked < 20)
			{
				return mApp.mPlayerInfo.mMiniGamesUnlocked;
			}
			return 20;
		case MiniGameMode.IZombie:
			if (mApp.mPlayerInfo.mIZombieUnlocked < 10)
			{
				return mApp.mPlayerInfo.mIZombieUnlocked;
			}
			return 10;
		case MiniGameMode.Vasebreaker:
			if (mApp.mPlayerInfo.mVasebreakerUnlocked < 10)
			{
				return mApp.mPlayerInfo.mVasebreakerUnlocked;
			}
			return 10;
		case MiniGameMode.Survival:
			return mCachedChallengeDefs[0].Length;
		case MiniGameMode.Limbo:
			return mCachedChallengeDefs[2].Length;
		case MiniGameMode.Extra:
			return 11;
		default:
			return -1;
		}
	}

	public bool GetDrawPadlock()
	{
		if (mApp.mPlayerInfo == null)
		{
			return false;
		}
		switch (mMode)
		{
		case MiniGameMode.Games:
			if (mApp.mPlayerInfo.mMiniGamesUnlocked != 20)
			{
				return true;
			}
			break;
		case MiniGameMode.IZombie:
			if (mApp.mPlayerInfo.mIZombieUnlocked != 10)
			{
				return true;
			}
			break;
		case MiniGameMode.Vasebreaker:
			if (mApp.mPlayerInfo.mVasebreakerUnlocked != 10)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public bool GetDrawWaves(int index)
	{
		switch ((GameMode)GetGameMode(index + 1))
		{
		case GameMode.SurvivalEndlessStage1:
		case GameMode.SurvivalEndlessStage2:
		case GameMode.SurvivalEndlessStage3:
		case GameMode.SurvivalEndlessStage4:
		case GameMode.SurvivalEndlessStage5:
		case GameMode.ScaryPotterEndless:
		case GameMode.PuzzleIZombieEndless:
		case GameMode.RogueConveyorbelt:
			return true;
		case GameMode.SurvivalNormalStage1:
		case GameMode.SurvivalNormalStage2:
		case GameMode.SurvivalNormalStage3:
		case GameMode.SurvivalNormalStage4:
		case GameMode.SurvivalNormalStage5:
		case GameMode.SurvivalHardStage1:
		case GameMode.SurvivalHardStage2:
		case GameMode.SurvivalHardStage3:
		case GameMode.SurvivalHardStage4:
		case GameMode.SurvivalHardStage5:
		case GameMode.ChallengeLastStand:
			return !HasBeenBeaten(index);
		default:
			return false;
		}
	}

	public bool HasBeenBeaten(int index)
	{
		return mApp.HasBeatenChallenge((GameMode)GetGameMode(index + 1));
	}

	public override void Draw(Graphics g)
	{
		int num = 10;
		int modeLevelCount = GetModeLevelCount();
		bool drawPadlock = GetDrawPadlock();
		for (int i = 0; i < modeLevelCount; i++)
		{
			DrawBackgroundThumbnailForLevel(g, num + Constants.QuickPlayWidget_Thumb_X, Constants.QuickPlayWidget_Thumb_Y, i + 1);
			num += AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10;
		}
		num = 10;
		for (int j = 0; j < modeLevelCount; j++)
		{
			g.DrawImage(AtlasResources.IMAGE_MINI_GAME_FRAME, num, 0);
			if (HasBeenBeaten(j))
			{
				g.DrawImage(AtlasResources.IMAGE_MINIGAME_TROPHY, num + 3, 6);
			}
			string levelName = GetLevelName(j + 1);
			g.SetFont(Resources.FONT_DWARVENTODCRAFT12);
			g.SetColor(Color.White);
			TRect mClipRect = g.mClipRect;
			g.ClearClipRect();
			g.WriteWordWrapped(new TRect(num + 15, AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight, AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth - 30, 100), levelName, 5, 0);
			if (GetDrawWaves(j))
			{
				int value = mApp.mPlayerInfo.mChallengeRecords[GetGameMode(j + 1) - 1];
				string text = $"{value}";
				float num2 = (float)AtlasResources.IMAGE_FLAGMETERPARTS.GetCelWidth() * 0.5f;
				g.WriteWordWrapped(new TRect(num + 15, AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight / 6 * 5, AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth - 30 - (int)num2 / 2, 100), text, 5, 0);
				TodCommon.TodDrawImageCelScaled(g, AtlasResources.IMAGE_FLAGMETERPARTS, num + 15 + (AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth - 30) / 2 + (int)g.GetFont().MeasureString(text).X / 2, AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight / 6 * 5 + 5, 2, 0, 0.5f, 0.5f);
			}
			g.mClipRect = mClipRect;
			num += AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10;
		}
		if (drawPadlock)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new Color(100, 100, 100));
			DrawBackgroundThumbnailForLevel(g, num + Constants.QuickPlayWidget_Thumb_X, Constants.QuickPlayWidget_Thumb_Y, modeLevelCount + 1);
			g.SetColorizeImages(colorizeImages: false);
			g.DrawImage(AtlasResources.IMAGE_LOCK_BIG, num + 95 - AtlasResources.IMAGE_LOCK_BIG.mWidth / 2, 50);
			g.DrawImage(AtlasResources.IMAGE_MINI_GAME_FRAME, num, 0);
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (y <= AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight)
		{
			int num = x / (AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10);
			if (num == GetModeLevelCount())
			{
				DisplayLockedMessage();
			}
			else if (num <= GetModeLevelCount())
			{
				int gameMode = GetGameMode(num + 1);
				mListener.MiniGamesStageSelected(gameMode - 1);
			}
		}
	}

	public void DisplayLockedMessage()
	{
		switch (mMode)
		{
		case MiniGameMode.Games:
			if (!mApp.HasFinishedAdventure())
			{
				mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[FINISH_ADVENTURE_TOOLTIP]", "[DIALOG_BUTTON_OK]", "", 3, null);
			}
			else
			{
				mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[ONE_MORE_CHALLENGE_TOOLTIP]", "[DIALOG_BUTTON_OK]", "", 3, null);
			}
			break;
		case MiniGameMode.IZombie:
			if (!mApp.HasFinishedAdventure() && mApp.mPlayerInfo.mIZombieUnlocked == 3)
			{
				mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[FINISH_ADVENTURE_TOOLTIP]", "[DIALOG_BUTTON_OK]", "", 3, null);
			}
			else
			{
				mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[ONE_MORE_IZOMBIE_TOOLTIP]", "[DIALOG_BUTTON_OK]", "", 3, null);
			}
			break;
		case MiniGameMode.Vasebreaker:
			if (!mApp.HasFinishedAdventure() && mApp.mPlayerInfo.mVasebreakerUnlocked == 3)
			{
				mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[FINISH_ADVENTURE_TOOLTIP]", "[DIALOG_BUTTON_OK]", "", 3, null);
			}
			else
			{
				mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[ONE_MORE_SCARY_POTTER_TOOLTIP]", "[DIALOG_BUTTON_OK]", "", 3, null);
			}
			break;
		}
	}

	public void SizeToFit()
	{
		if (GetDrawPadlock())
		{
			mWidth = (10 + AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth) * (GetModeLevelCount() + 1);
		}
		else
		{
			mWidth = (10 + AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth) * GetModeLevelCount();
		}
		mHeight = AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight + 10;
	}

	public int GetGameMode(int index)
	{
		return mMode switch
		{
			MiniGameMode.Games => GetGameModeMiniGames(index), 
			MiniGameMode.IZombie => GetGameModeIZombie(index), 
			MiniGameMode.Vasebreaker => GetGameModeVasebreaker(index), 
			MiniGameMode.Survival => (int)mCachedChallengeDefs[0][index - 1].mChallengeMode, 
			MiniGameMode.Limbo => (int)mCachedChallengeDefs[2][index - 1].mChallengeMode, 
			MiniGameMode.Extra => GetGameModeExtraGames(index), 
			_ => -1, 
		};
	}

	public string GetLevelName(int index)
	{
		switch (mMode)
		{
		case MiniGameMode.Games:
			return TodStringFile.TodStringTranslate(ChallengeScreen.GetChallengeDefFromMode((GameMode)GetGameModeMiniGames(index)).mChallengeName);
		case MiniGameMode.IZombie:
			if (index == 10)
			{
				return TodStringFile.TodStringTranslate("[I_ZOMBIE_ENDLESS]");
			}
			return TodStringFile.TodStringTranslate("[I_ZOMBIE_" + index + "]");
		case MiniGameMode.Vasebreaker:
			if (index == 10)
			{
				return TodStringFile.TodStringTranslate("[SCARY_POTTER_ENDLESS]");
			}
			return TodStringFile.TodStringTranslate("[SCARY_POTTER_" + index + "]");
		case MiniGameMode.Survival:
			return TodStringFile.TodStringTranslate(mCachedChallengeDefs[0][index - 1].mChallengeName);
		case MiniGameMode.Limbo:
			return TodStringFile.TodStringTranslate(mCachedChallengeDefs[2][index - 1].mChallengeName);
		case MiniGameMode.Extra:
			return TodStringFile.TodStringTranslate(mCachedChallengeDefs[4][ExtraGamesIndexToMiniGameIndex(index) - 1].mChallengeName);
		default:
			return string.Empty;
		}
	}

	private int ExtraGamesIndexToMiniGameIndex(int index)
	{
		if (index < 6)
		{
			return 11 - index + 1;
		}
		return (index - 1 - -5 + 1) % 11 + 1;
	}

	public Image GetImageForMode(int index)
	{
		return mMode switch
		{
			MiniGameMode.Games => GetImageForGames(index), 
			MiniGameMode.IZombie => GetImageForIZombie(index), 
			MiniGameMode.Vasebreaker => GetImageForVasebreaker(index), 
			MiniGameMode.Survival => GetImageForSurvival(index), 
			MiniGameMode.Limbo => GetImageForLimbo(index), 
			MiniGameMode.Extra => GetImageForExtra(index), 
			_ => null, 
		};
	}

	public int GetGameModeVasebreaker(int index)
	{
		return index - 1 + 50;
	}

	public int GetGameModeIZombie(int index)
	{
		return index - 1 + 60;
	}

	public int GetGameModeExtraGames(int index)
	{
		index = ExtraGamesIndexToMiniGameIndex(index);
		if (1 > index || index > 11)
		{
			return -1;
		}
		return index - 1 + 123;
	}

	public int GetGameModeMiniGames(int index)
	{
		if (1 > index || index > 6)
		{
			if (8 > index || index > 151)
			{
				if (7 != index)
				{
					return -1;
				}
				return 122;
			}
			return index - 2 + 16;
		}
		return index - 1 + 16;
	}

	public void DrawBackgroundThumbnailForLevel(Graphics g, int theX, int theY, int theLevel)
	{
		Image imageForMode = GetImageForMode(theLevel);
		if (imageForMode != null)
		{
			g.DrawImage(imageForMode, theX, theY + 2);
		}
	}

	public Image GetImageForGames(int index)
	{
		GameMode gameModeMiniGames = (GameMode)GetGameModeMiniGames(index);
		Image result = null;
		switch (gameModeMiniGames)
		{
		case GameMode.ChallengeWarAndPeas:
			result = AtlasResources.IMAGE_MINIGAMES_ZOMBOTANY;
			break;
		case GameMode.ChallengeWallnutBowling:
			result = AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING;
			break;
		case GameMode.ChallengeSlotMachine:
			result = AtlasResources.IMAGE_MINIGAMES_SLOT_MACHINE;
			break;
		case GameMode.ChallengeRainingSeeds:
			result = AtlasResources.IMAGE_MINIGAMES_RAINING_SEEDS;
			break;
		case GameMode.ChallengeBeghouled:
			result = AtlasResources.IMAGE_MINIGAMES_BEGHOULED;
			break;
		case GameMode.ChallengeInvisighoul:
			result = AtlasResources.IMAGE_MINIGAMES_INVISIBLE;
			break;
		case GameMode.ChallengeSeeingStars:
			result = AtlasResources.IMAGE_MINIGAMES_SEEING_STARS;
			break;
		case GameMode.ChallengeZombiquarium:
			result = AtlasResources.IMAGE_MINIGAMES_ZOMBIQUARIUM;
			break;
		case GameMode.ChallengeBeghouledTwist:
			result = AtlasResources.IMAGE_MINIGAMES_BEGHOULED_TWIST;
			break;
		case GameMode.ChallengeLittleTrouble:
			result = AtlasResources.IMAGE_MINIGAMES_LITTLE_ZOMBIE;
			break;
		case GameMode.ChallengePortalCombat:
			result = AtlasResources.IMAGE_MINIGAMES_PORTAL;
			break;
		case GameMode.ChallengeColumn:
		case GameMode.ChallengeColumn2:
		case GameMode.ChallengeColumn3:
		case GameMode.ChallengeColumn4:
			result = AtlasResources.IMAGE_MINIGAMES_COLUMN;
			break;
		case GameMode.ChallengeBobsledBonanza:
			result = AtlasResources.IMAGE_MINIGAMES_BOBSLED_BONANZA;
			break;
		case GameMode.ChallengeSpeed:
			result = AtlasResources.IMAGE_MINIGAMES_ZOMBIE_NIMBLE;
			break;
		case GameMode.ChallengeWhackAZombie:
			result = AtlasResources.IMAGE_MINIGAMES_WHACK_A_ZOMBIE;
			break;
		case GameMode.ChallengeLastStand:
			result = AtlasResources.IMAGE_MINIGAMES_LAST_STAND;
			break;
		case GameMode.ChallengeWarAndPeas2:
			result = AtlasResources.IMAGE_MINIGAMES_ZOMBOTANY2;
			break;
		case GameMode.ChallengeWallnutBowling2:
			result = AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING2;
			break;
		case GameMode.ChallengePogoParty:
			result = AtlasResources.IMAGE_MINIGAMES_POGO_PARTY;
			break;
		case GameMode.ChallengeFinalBoss:
			result = AtlasResources.IMAGE_MINIGAMES_ZOMBOSS;
			break;
		}
		return result;
	}

	public Image GetImageForIZombie(int index)
	{
		return AtlasResources.IMAGE_MINIGAMES_IZOMBIE;
	}

	public Image GetImageForVasebreaker(int index)
	{
		return AtlasResources.IMAGE_MINIGAMES_VASEBREAKER;
	}

	public Image GetImageForSurvival(int index)
	{
		Image result = null;
		switch (index)
		{
		case 1:
		case 6:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND1_THUMB;
			break;
		case 2:
		case 7:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND2_THUMB;
			break;
		case 3:
		case 8:
		case 11:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND3_THUMB;
			break;
		case 4:
		case 9:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND4_THUMB;
			break;
		case 5:
		case 10:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND5_THUMB;
			break;
		}
		return result;
	}

	public Image GetImageForLimbo(int index)
	{
		Image result = null;
		switch (index)
		{
		case 1:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND1_THUMB;
			break;
		case 2:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND2_THUMB;
			break;
		case 3:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND4_THUMB;
			break;
		case 4:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND5_THUMB;
			break;
		case 5:
			result = AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING;
			break;
		case 6:
			result = AtlasResources.IMAGE_MINIGAMES_IZOMBIE;
			break;
		case 7:
			result = AtlasResources.IMAGE_MINIGAMES_LAST_STAND;
			break;
		case 8:
			result = AtlasResources.IMAGE_MINIGAMES_BIG_TIME;
			break;
		case 9:
			result = AtlasResources.IMAGE_MINIGAMES_ART_CHALLENGE_2;
			break;
		case 10:
			result = AtlasResources.IMAGE_MINIGAMES_PORTAL;
			break;
		case 11:
			result = AtlasResources.IMAGE_MINIGAMES_GRAVITY;
			break;
		case 12:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND2_THUMB;
			break;
		case 13:
			result = AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING;
			break;
		case 14:
			result = AtlasResources.IMAGE_MINIGAMES_INVISIBLE;
			break;
		case 15:
			result = AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND5_THUMB;
			break;
		}
		return result;
	}

	public Image GetImageForExtra(int index)
	{
		switch (ExtraGamesIndexToMiniGameIndex(index))
		{
		case 1:
			return AtlasResources.IMAGE_MINIGAMES_ATTACK_ON_TITANS;
		case 2:
			return AtlasResources.IMAGE_MINIGAMES_ATTACK_ON_TITANS_2;
		case 3:
			return AtlasResources.IMAGE_MINIGAMES_ATTACK_ON_TITANS_3;
		case 4:
			return AtlasResources.IMAGE_MINIGAMES_MARVELOUS_PEOPLE;
		case 5:
			return AtlasResources.IMAGE_MINIGAMES_MARVELOUS_PEOPLE_2;
		case 6:
			return AtlasResources.IMAGE_MINIGAMES_MARVELOUS_PEOPLE_3;
		case 9:
			return AtlasResources.IMAGE_MINIGAMES_FINALBOSS2;
		case 7:
		case 8:
			return AtlasResources.IMAGE_MINIGAMES_ROGUECONVEYERBELT;
		case 10:
			return AtlasResources.IMAGE_MINIGAME_IMITATER_RANDOM;
		case 11:
		case 12:
			return AtlasResources.IMAGE_MINIGAMES_COLUMN;
		default:
			return null;
		}
	}

	public void RecoverLastPlayedMode()
	{
		GameMode mGameMode = mApp.mGameMode;
		int num = -1;
		switch (mMode)
		{
		case MiniGameMode.Games:
			if (mGameMode + 1 <= GameMode.ChallengeSunnyDay)
			{
				num = (int)(mGameMode + 1 - 16);
			}
			break;
		case MiniGameMode.IZombie:
			if (mApp.IsIZombieLevel())
			{
				num = (int)(mGameMode + 1 - 60);
			}
			break;
		case MiniGameMode.Vasebreaker:
			if (mApp.IsScaryPotterLevel())
			{
				num = (int)(mGameMode + 1 - 50);
			}
			break;
		}
		if (num >= 0)
		{
			ScrollWidget mMiniGamesScrollWidget = mApp.mGameSelector.mMiniGamesScrollWidget;
			int num2 = (AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10) * (num - 1);
			mMiniGamesScrollWidget.ScrollToPoint(new CGPoint((num2 > mWidth - mMiniGamesScrollWidget.mWidth) ? (mWidth - mMiniGamesScrollWidget.mWidth) : num2, 0f), animated: true);
		}
	}

	public static Image GetChallengeImage(GameMode mode)
	{
		switch (mode)
		{
		case GameMode.SurvivalNormalStage1:
		case GameMode.SurvivalHardStage1:
		case GameMode.SurvivalEndlessStage1:
		case GameMode.SurvivalHellStage1:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND1_THUMB;
		case GameMode.SurvivalNormalStage2:
		case GameMode.SurvivalHardStage2:
		case GameMode.SurvivalEndlessStage2:
		case GameMode.SurvivalHellStage2:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND2_THUMB;
		case GameMode.SurvivalNormalStage3:
		case GameMode.SurvivalHardStage3:
		case GameMode.SurvivalEndlessStage3:
		case GameMode.SurvivalHellStage3:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND3_THUMB;
		case GameMode.SurvivalNormalStage4:
		case GameMode.SurvivalHardStage4:
		case GameMode.SurvivalEndlessStage4:
		case GameMode.SurvivalHellStage4:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND4_THUMB;
		case GameMode.SurvivalNormalStage5:
		case GameMode.SurvivalHardStage5:
		case GameMode.SurvivalEndlessStage5:
		case GameMode.SurvivalHellStage5:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND5_THUMB;
		case GameMode.BigPoolSurvivalNormalStage:
		case GameMode.BigPoolSurvivalHardStage:
		case GameMode.BigPoolSurvivalHellStage:
			return AtlasResources.IMAGE_MINIGAMES_QUICKPLAY_BACKGROUND_BIGPOOL_THUMB;
		case GameMode.ChallengeWarAndPeas:
			return AtlasResources.IMAGE_MINIGAMES_ZOMBOTANY;
		case GameMode.ChallengeWallnutBowling:
			return AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING;
		case GameMode.ChallengeSlotMachine:
			return AtlasResources.IMAGE_MINIGAMES_SLOT_MACHINE;
		case GameMode.ChallengeRainingSeeds:
			return AtlasResources.IMAGE_MINIGAMES_RAINING_SEEDS;
		case GameMode.ChallengeBeghouled:
			return AtlasResources.IMAGE_MINIGAMES_BEGHOULED;
		case GameMode.ChallengeInvisighoul:
			return AtlasResources.IMAGE_MINIGAMES_INVISIBLE;
		case GameMode.ChallengeSeeingStars:
			return AtlasResources.IMAGE_MINIGAMES_SEEING_STARS;
		case GameMode.ChallengeZombiquarium:
			return AtlasResources.IMAGE_MINIGAMES_ZOMBIQUARIUM;
		case GameMode.ChallengeBeghouledTwist:
			return AtlasResources.IMAGE_MINIGAMES_BEGHOULED_TWIST;
		case GameMode.ChallengeLittleTrouble:
			return AtlasResources.IMAGE_MINIGAMES_LITTLE_ZOMBIE;
		case GameMode.ChallengePortalCombat:
			return AtlasResources.IMAGE_MINIGAMES_PORTAL;
		case GameMode.ChallengeColumn:
			return AtlasResources.IMAGE_MINIGAMES_COLUMN;
		case GameMode.ChallengeBobsledBonanza:
		case GameMode.ChallengeBobsledBonanza2:
			return AtlasResources.IMAGE_MINIGAMES_BOBSLED_BONANZA;
		case GameMode.ChallengeSpeed:
			return AtlasResources.IMAGE_MINIGAMES_ZOMBIE_NIMBLE;
		case GameMode.ChallengeWhackAZombie:
			return AtlasResources.IMAGE_MINIGAMES_WHACK_A_ZOMBIE;
		case GameMode.ChallengeLastStand:
			return AtlasResources.IMAGE_MINIGAMES_LAST_STAND;
		case GameMode.ChallengeWarAndPeas2:
			return AtlasResources.IMAGE_MINIGAMES_ZOMBOTANY2;
		case GameMode.ChallengeWallnutBowling2:
			return AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING2;
		case GameMode.ChallengePogoParty:
			return AtlasResources.IMAGE_MINIGAMES_POGO_PARTY;
		case GameMode.ChallengeFinalBoss:
			return AtlasResources.IMAGE_MINIGAMES_ZOMBOSS;
		case GameMode.ChallengeArtChallenge1:
			return AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING;
		case GameMode.ChallengeSunnyDay:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND1_THUMB;
		case GameMode.ChallengeResodded:
			return AtlasResources.IMAGE_MINIGAMES_LAST_STAND;
		case GameMode.ChallengeBigTime:
			return AtlasResources.IMAGE_MINIGAMES_BIG_TIME;
		case GameMode.ChallengeArtChallenge2:
			return AtlasResources.IMAGE_MINIGAMES_ART_CHALLENGE_2;
		case GameMode.ChallengeAirRaid:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND4_THUMB;
		case GameMode.ChallengeHighGravity:
			return AtlasResources.IMAGE_MINIGAMES_GRAVITY;
		case GameMode.ChallengeGraveDanger:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND2_THUMB;
		case GameMode.ChallengeShovel:
			return AtlasResources.IMAGE_MINIGAMES_WALLNUT_BOWLING;
		case GameMode.ChallengeStormyNight:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND4_THUMB;
		case GameMode.ChallengeBungeeBlitz:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND5_THUMB;
		case GameMode.ScaryPotter1:
		case GameMode.ScaryPotter2:
		case GameMode.ScaryPotter3:
		case GameMode.ScaryPotter4:
		case GameMode.ScaryPotter5:
		case GameMode.ScaryPotter6:
		case GameMode.ScaryPotter7:
		case GameMode.ScaryPotter8:
		case GameMode.ScaryPotter9:
		case GameMode.ScaryPotterEndless:
			return AtlasResources.IMAGE_MINIGAMES_VASEBREAKER;
		case GameMode.PuzzleIZombie1:
		case GameMode.PuzzleIZombie2:
		case GameMode.PuzzleIZombie3:
		case GameMode.PuzzleIZombie4:
		case GameMode.PuzzleIZombie5:
		case GameMode.PuzzleIZombie6:
		case GameMode.PuzzleIZombie7:
		case GameMode.PuzzleIZombie8:
		case GameMode.PuzzleIZombie9:
		case GameMode.PuzzleIZombieEndless:
			return AtlasResources.IMAGE_MINIGAMES_IZOMBIE;
		case GameMode.ChallengeAttackOnTitans:
			return AtlasResources.IMAGE_MINIGAMES_ATTACK_ON_TITANS;
		case GameMode.ChallengeAttackOnTitans2:
			return AtlasResources.IMAGE_MINIGAMES_ATTACK_ON_TITANS_2;
		case GameMode.ChallengeAttackOnTitans3:
			return AtlasResources.IMAGE_MINIGAMES_ATTACK_ON_TITANS_3;
		case GameMode.MarvelousPeople:
			return AtlasResources.IMAGE_MINIGAMES_MARVELOUS_PEOPLE;
		case GameMode.MarvelousPeople2:
			return AtlasResources.IMAGE_MINIGAMES_MARVELOUS_PEOPLE_2;
		case GameMode.MarvelousPeople3:
			return AtlasResources.IMAGE_MINIGAMES_MARVELOUS_PEOPLE_3;
		case GameMode.RogueConveyorbelt:
			return AtlasResources.IMAGE_MINIGAMES_ROGUECONVEYERBELT;
		case GameMode.RogueConveyorbeltHard:
			return AtlasResources.IMAGE_MINIGAMES_ROGUECONVEYERBELT;
		case GameMode.ChallengeFinalBoss2:
			return AtlasResources.IMAGE_MINIGAMES_FINALBOSS2;
		case GameMode.ImitaterRandom:
			return AtlasResources.IMAGE_MINIGAME_IMITATER_RANDOM;
		case GameMode.ChallengeColumn2:
		case GameMode.ChallengeColumn3:
		case GameMode.ChallengeColumn4:
			return AtlasResources.IMAGE_MINIGAMES_COLUMN;
		case GameMode.MarvelousPeople4:
			return AtlasResources.IMAGE_MINIGAMES_MARVELOUS_PEOPLE_4;
		case GameMode.PoolParty:
			return AtlasResources.IMAGE_MINIGAMES_QUICKPLAY_BACKGROUND_BIGPOOL_THUMB;
		case GameMode.ChallengeMoreAirRaid:
			return AtlasResources.IMAGE_MINIGAME_QUICKPLAY_BACKGROUND5_THUMB;
		case GameMode.ChallengeFusion:
			return AtlasResources.IMAGE_MINIGAME_FUSION;
		default:
			return AtlasResources.IMAGE_MINIGAME_IMITATER_RANDOM;
		}
	}
}
