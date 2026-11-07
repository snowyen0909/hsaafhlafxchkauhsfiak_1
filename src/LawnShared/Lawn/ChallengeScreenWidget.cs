using System;
using System.IO;
using Lawn.Creative;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class ChallengeScreenWidget : Widget
{
	public LawnApp mApp;

	public ChallengeScreenWidget(LawnApp theApp)
	{
		mApp = theApp;
		mWidth = 280;
		mHeight = 220;
	}

	public void SetSize(int width, int height)
	{
		mWidth = width;
		mHeight = height;
	}

	public override void Draw(Graphics g)
	{
		bool flag = mApp.mChallengeScreen.mPageIndex == ChallengePage.DIY;
		bool flag2 = mApp.mChallengeScreen.mPageIndex == ChallengePage.OnlineLevel;
		g.HardwareClip();
		for (int i = 0; i < mApp.mChallengeScreen.mChallengeButton.Length; i++)
		{
			ButtonWidget buttonWidget = mApp.mChallengeScreen.mChallengeButton[i];
			if (buttonWidget == null || !buttonWidget.mVisible)
			{
				continue;
			}
			ChallengeDefinition challengeDefinition = ((i < ChallengeScreen.gChallengeDefs.Length) ? ChallengeScreen.gChallengeDefs[i] : null);
			GameMode gameMode = (((flag | flag2) && challengeDefinition != null) ? mApp.mChallengeScreen.IndexToGameMode(i) : challengeDefinition.mChallengeMode);
			CSLevelProperty cSLevelProperty = null;
			OnlineLevelPreview onlineLevelPreview = null;
			Image image = null;
			int num = -1;
			bool flag3 = false;
			string text = null;
			if (flag)
			{
				CreativeLevel levelDefine = CreativeLevelManager.GetLevelDefine(gameMode);
				if (levelDefine != null)
				{
					cSLevelProperty = levelDefine.GetComponent<CSLevelProperty>();
					if (cSLevelProperty != null)
					{
						image = cSLevelProperty.mIconImage;
						num = cSLevelProperty.mIcon;
					}
				}
			}
			else if (flag2 && i < OnlineLevel.CurrentLevels.Count)
			{
				onlineLevelPreview = OnlineLevel.CurrentLevels[i];
				if (Constants.Language == Constants.LanguageIndex.zh_cn && !string.IsNullOrEmpty(onlineLevelPreview.ChineseName))
				{
					text = onlineLevelPreview.ChineseName;
				}
				else if (Constants.Language == Constants.LanguageIndex.en && !string.IsNullOrEmpty(onlineLevelPreview.EnglishName))
				{
					text = onlineLevelPreview.EnglishName;
				}
				else
				{
					text = (string.IsNullOrEmpty(onlineLevelPreview.Name) ? onlineLevelPreview.LevelId : onlineLevelPreview.Name);
				}
				if (onlineLevelPreview.CachedIcon != null)
				{
					image = onlineLevelPreview.CachedIcon;
				}
				else if (onlineLevelPreview.CachedIconBytes != null)
				{
					try
					{
						using Stream stream = new MemoryStream(onlineLevelPreview.CachedIconBytes);
						onlineLevelPreview.CachedIcon = new Image(Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, stream));
						image = onlineLevelPreview.CachedIcon;
					}
					catch (Exception ex)
					{
						Debug.Log(DebugType.Error, "Create online level icon failed: " + ex.Message);
					}
				}
				if (image == null)
				{
					num = onlineLevelPreview.IconId;
				}
				flag3 = onlineLevelPreview.IsEndless;
			}
			int num2 = buttonWidget.mX;
			int num3 = buttonWidget.mY;
			if (buttonWidget.mIsDown)
			{
				num2++;
				num3++;
			}
			int num4 = ((!(flag | flag2) && challengeDefinition != null) ? mApp.mChallengeScreen.AccomplishmentsNeeded(i) : 0);
			if (num4 <= 1)
			{
				if (buttonWidget.mDisabled)
				{
					g.SetColor(new SexyColor(92, 92, 92));
					g.SetColorizeImages(colorizeImages: true);
				}
				if (!flag && !flag2 && i == mApp.mChallengeScreen.mUnlockChallengeIndex)
				{
					if (mApp.mChallengeScreen.mUnlockState == UnlockingState.Shaking)
					{
						g.SetColor(new SexyColor(92, 92, 92));
					}
					else if (mApp.mChallengeScreen.mUnlockState == UnlockingState.Fading)
					{
						int num5 = TodCommon.TodAnimateCurve(50, 25, mApp.mChallengeScreen.mUnlockStateCounter, 92, 255, TodCurves.Linear);
						g.SetColor(new SexyColor(num5, num5, num5));
					}
					g.SetColorizeImages(colorizeImages: true);
				}
				g.PushState();
				g.SetScale(0.52f);
				g.Translate(-18, -14);
				if (image != null)
				{
					g.DrawImage(image, num2 + 13, num3 + 4, 155, 126);
				}
				else if (flag && cSLevelProperty != null)
				{
					g.DrawImage(MiniGamesWidget.GetChallengeImage((GameMode)cSLevelProperty.mIcon), num2 + 13, num3 + 4);
				}
				else if (flag)
				{
					g.DrawImage(AtlasResources.IMAGE_MINIGAME_IMITATER_RANDOM, num2 + 13, num3 + 4);
				}
				else if (flag2)
				{
					g.DrawImage(AtlasResources.IMAGE_MINIGAME_IMITATER_RANDOM, num2 + 13, num3 + 4);
				}
				else
				{
					g.DrawImage(MiniGamesWidget.GetChallengeImage(challengeDefinition.mChallengeMode), num2 + 13, num3 + 4);
				}
				g.PopState();
				bool flag4 = buttonWidget.mIsOver && ((flag | flag2) || i != mApp.mChallengeScreen.mUnlockChallengeIndex);
				g.SetColorizeImages(colorizeImages: false);
				g.DrawImage(flag4 ? AtlasResources.IMAGE_CHALLENGE_WINDOW : AtlasResources.IMAGE_CHALLENGE_WINDOW_HIGHLIGHT, num2 - 6, num3 - 2);
				SexyColor color = new SexyColor(42, 42, 90);
				if (buttonWidget.mIsOver && ((flag | flag2) || i != mApp.mChallengeScreen.mUnlockChallengeIndex))
				{
					color = new SexyColor(250, 40, 40);
				}
				if (text == null)
				{
					text = (flag ? CreativeLevelManager.GetLevelName(gameMode) : TodStringFile.TodStringTranslate(challengeDefinition.mChallengeName));
				}
				if (text == null)
				{
					text = "Unnamed";
				}
				if (!flag && !flag2 && i == mApp.mChallengeScreen.mUnlockChallengeIndex && mApp.mChallengeScreen.mUnlockState == UnlockingState.Shaking)
				{
					text = "?";
				}
				else if (!flag2 && buttonWidget.mDisabled)
				{
					text = "?";
				}
				DrawTitle(g, text, num2, num3, color, mApp.mChallengeScreen.mPageIndex == ChallengePage.Survival && !buttonWidget.mDisabled);
				if (flag)
				{
					continue;
				}
				if (flag2)
				{
					if (onlineLevelPreview != null && !onlineLevelPreview.IsEndless && mApp.mPlayerInfo.GetOnlineChallengeRecord(onlineLevelPreview.LevelId) > 0)
					{
						g.DrawImage(AtlasResources.IMAGE_MINIGAME_TROPHY, num2 - 6, num3 - 2);
					}
					continue;
				}
				int num6 = mApp.mPlayerInfo.mChallengeRecords[(int)(challengeDefinition.mChallengeMode - 1)];
				if (i == mApp.mChallengeScreen.mUnlockChallengeIndex)
				{
					Image theImage = AtlasResources.IMAGE_LOCK_BIG;
					if (mApp.mChallengeScreen.mUnlockState == UnlockingState.Fading)
					{
						theImage = AtlasResources.IMAGE_LOCK_OPEN;
						int theAlpha = TodCommon.TodAnimateCurve(25, 0, mApp.mChallengeScreen.mUnlockStateCounter, 255, 0, TodCurves.Linear);
						g.SetColor(new SexyColor(255, 255, 255, theAlpha));
						g.SetColorizeImages(colorizeImages: true);
					}
					TodCommon.TodDrawImageScaledF(g, theImage, (float)(num2 + 24) + mApp.mChallengeScreen.mLockShakeX, (float)(num3 + 9) + mApp.mChallengeScreen.mLockShakeY, 0.7f, 0.7f);
					g.SetColorizeImages(colorizeImages: false);
				}
				else if (num6 > 0)
				{
					if (mApp.HasBeatenChallenge(challengeDefinition.mChallengeMode))
					{
						g.DrawImage(AtlasResources.IMAGE_MINIGAME_TROPHY, num2 - 6, num3 - 2);
					}
					else if (mApp.IsEndlessScaryPotter(challengeDefinition.mChallengeMode) || mApp.IsEndlessIZombie(challengeDefinition.mChallengeMode))
					{
						string theText = TodCommon.TodReplaceNumberString("[LONGEST_STREAK]", "{STREAK}", num6);
						TRect theRect = new TRect(num2, num3 + 15, 96, 200);
						TodStringFile.TodDrawStringWrapped(g, theText, theRect, Resources.FONT_CONTINUUMBOLD14OUTLINE, new SexyColor(255, 255, 255), DrawStringJustification.Center);
						TodStringFile.TodDrawStringWrapped(g, theText, theRect, Resources.FONT_CONTINUUMBOLD14, new SexyColor(255, 0, 0), DrawStringJustification.Center);
					}
				}
				else if (buttonWidget.mDisabled)
				{
					g.HardwareClip();
					TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_LOCK_BIG, num2 + 24, num3 + 9, 0.7f, 0.7f);
					g.EndHardwareClip();
				}
			}
			else
			{
				g.SetColorizeImages(colorizeImages: false);
				g.DrawImage(AtlasResources.IMAGE_CHALLENGE_BLANK, num2, num3);
			}
		}
		g.EndHardwareClip();
	}

	private static void DrawTitle(Graphics g, string text, int x, int y, SexyColor color, bool preferSurvivalBreak)
	{
		int num = text.IndexOf('|');
		if (num != -1)
		{
			string theText = text.Substring(0, num);
			string text2 = ((num + 1 < text.Length) ? text.Substring(num + 1) : string.Empty);
			TodCommon.TodDrawString(g, theText, x + 52, y + 78, Resources.FONT_BRIANNETOD12, color, DrawStringJustification.Center, 2f / 3f);
			if (text2.Length > 0)
			{
				TodCommon.TodDrawString(g, text2, x + 52, y + 90, Resources.FONT_BRIANNETOD12, color, DrawStringJustification.Center, 2f / 3f);
			}
			return;
		}
		if (text.Length < 13)
		{
			TodCommon.TodDrawString(g, text, x + 52, y + 84, Resources.FONT_BRIANNETOD12, color, DrawStringJustification.Center, 2f / 3f);
			return;
		}
		int val = text.Length / 2 - 1;
		if (preferSurvivalBreak)
		{
			val = 7;
		}
		int num2 = text.IndexOf(' ', Math.Max(val, 0));
		if (num2 == -1)
		{
			num2 = text.IndexOf(' ');
		}
		if (num2 == -1)
		{
			TodCommon.TodDrawString(g, text, x + 52, y + 84, Resources.FONT_BRIANNETOD12, color, DrawStringJustification.Center, 2f / 3f);
			return;
		}
		string theText2 = text.Substring(0, num2);
		string text3 = ((num2 + 1 < text.Length) ? text.Substring(num2 + 1) : string.Empty);
		TodCommon.TodDrawString(g, theText2, x + 52, y + 78, Resources.FONT_BRIANNETOD12, color, DrawStringJustification.Center, 2f / 3f);
		if (text3.Length > 0)
		{
			TodCommon.TodDrawString(g, text3, x + 52, y + 90, Resources.FONT_BRIANNETOD12, color, DrawStringJustification.Center, 2f / 3f);
		}
	}
}
