using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class OnlineLevelDialog : LawnDialog
{
	public DialogButton mEnterButton;

	public GameMode mGameMode;

	public OnlineLevelPreview mPreview;

	public string mTooltip;

	public int mToolTipY;

	public int mToolTipHeight;

	public OnlineLevelDialog(LawnApp theApp, GameMode theGameMode, string title, OnlineLevelPreview thePreview)
		: base(theApp, null, 63, isModal: true, title, string.Empty, "[BACK]", 3)
	{
		mGameMode = theGameMode;
		mPreview = thePreview;
		string raw = string.Empty;
		AppendInfo(ref raw, "作者", thePreview.Author);
		AppendInfo(ref raw, "难度", thePreview.Difficulty.ToString());
		AppendInfo(ref raw, "下载量", thePreview.Downloads.ToString());
		mTooltip = raw;
		int num = 420;
		int num2 = 250;
		int num3 = 200;
		int theHeight = 40;
		int theX = (num - num3) / 2;
		int num4 = 100;
		int num5 = 10;
		Graphics graphics = Graphics.GetNew();
		int num6 = TodStringFile.TodDrawStringWrappedHeight(graphics, mTooltip, new TRect(0, 0, num - 100, 0), Resources.FONT_DWARVENTODCRAFT15, SexyColor.White, DrawStringJustification.CenterVerticalMiddle);
		graphics.PrepareForReuse();
		num2 += num6;
		mEnterButton = GameButton.MakeButton(100, this, TodStringFile.TodStringTranslate("[PLAY_BUTTON]"));
		mEnterButton.Resize(theX, num4 + num6 + num5, num3, theHeight);
		mToolTipY = num4;
		mToolTipHeight = num6;
		Resize((800 - num) / 2, (480 - num2) / 2, num, num2);
	}

	private static void AppendInfo(ref string raw, string label, string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (raw != string.Empty)
			{
				raw += "\n";
			}
			raw = raw + label + ": " + value;
		}
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		AddWidget(mEnterButton);
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		RemoveWidget(mEnterButton);
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		TodStringFile.TodDrawStringWrapped(g, mTooltip, new TRect(50, mToolTipY, mWidth - 100, mToolTipHeight), Resources.FONT_DWARVENTODCRAFT15, SexyColor.White, DrawStringJustification.CenterVerticalMiddle);
	}

	public override void ButtonDepress(int theId)
	{
		if (theId == 100)
		{
			GameMode theGameMode = mGameMode;
			OnlineLevelPreview thePreview = mPreview;
			mApp.KillDialog(this);
			mApp.mChallengeScreen?.StartEnterOnlineLevel(theGameMode, thePreview);
		}
		else
		{
			mApp.KillDialog(this);
		}
	}
}
