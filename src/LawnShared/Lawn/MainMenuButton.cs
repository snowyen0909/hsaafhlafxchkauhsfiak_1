using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class MainMenuButton : NewLawnButton
{
	public Image mTopImage;

	public int mTopImageOffsetX;

	public int mTopImageOffsetY;

	public int mScaler;

	private bool? mPressedVisualOverride;

	public const int SCALER_MAX_VALUE = 10;

	internal bool DrawWithParent { get; set; }

	public MainMenuButton(Image theComponentImage, int theId, ButtonListener theListener)
		: base(theComponentImage, theId, theListener)
	{
	}

	public static MainMenuButton MakeNewButton(int theId, ButtonListener theListener, Font theFont, Image theImage, int theX = 0, int theY = 0)
	{
		return MakeNewButton(theId, theListener, null, theFont, theImage, theX, theY);
	}

	public static MainMenuButton MakeNewButton(int theId, ButtonListener theListener, string theLabel, Font theFont, Image theImage, int theX = 0, int theY = 0)
	{
		MainMenuButton mainMenuButton = new MainMenuButton(null, theId, theListener);
		mainMenuButton.InitializeAppearance(theLabel, theFont, theImage, theX, theY);
		return mainMenuButton;
	}

	protected void InitializeAppearance(string theLabel, Font theFont, Image theImage, int theX, int theY)
	{
		SetFont(theFont ?? Resources.FONT_BRIANNETOD12);
		if (theLabel != null)
		{
			SetLabel(theLabel);
		}
		mWidth = AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth;
		mHeight = AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight;
		mButtonImage = null;
		mOverImage = null;
		mDownImage = null;
		mHasAlpha = true;
		mHasTransparencies = true;
		mTopImage = theImage;
		mTextOffsetX = 32;
		mTextOffsetY = 45;
		mClip = false;
		mTopImageOffsetX = theX;
		mTopImageOffsetY = theY;
	}

	internal void SetPressedVisual(bool? pressed)
	{
		mPressedVisualOverride = pressed;
		MarkDirty();
	}

	public override void Update()
	{
		base.Update();
		bool? flag = mPressedVisualOverride;
		bool valueOrDefault;
		if (!flag.HasValue)
		{
			if (!mIsDown)
			{
				goto IL_003d;
			}
			valueOrDefault = mIsOver;
		}
		else
		{
			valueOrDefault = flag == true;
		}
		if (!valueOrDefault)
		{
			goto IL_003d;
		}
		int num = ((!mDisabled) ? 1 : 0);
		goto IL_003e;
		IL_003d:
		num = 0;
		goto IL_003e;
		IL_003e:
		bool flag2 = (byte)num != 0;
		if (flag2 ^ mInverted)
		{
			if (mScaler < 10)
			{
				mScaler++;
			}
		}
		else if (mScaler > 0)
		{
			mScaler--;
		}
	}

	public override void Draw(Graphics g)
	{
		if (!DrawWithParent)
		{
			DrawButton(g);
		}
	}

	protected internal virtual void DrawButton(Graphics g)
	{
		if (!mBtnNoDraw)
		{
			int num = 0;
			int num2 = 0;
			if (mIsDown && mIsOver)
			{
				_ = mDisabled;
			}
			int num3 = mTextOffsetX;
			int num4 = mTextOffsetY;
			float num5 = 1f + (float)mScaler / 50f;
			_ = mFont;
			g.SetScale(num5);
			g.mScaleOrigX = 1f;
			g.mScaleOrigY = 1f;
			g.DrawImage(AtlasResources.IMAGE_MENUSTYLEFREE_BG, mButtonOffsetX, mButtonOffsetY);
			g.DrawImage(mTopImage, mButtonOffsetX + mTopImageOffsetX, mButtonOffsetY + mTopImageOffsetY);
			if (!string.IsNullOrEmpty(mLabel))
			{
				g.DrawImage(AtlasResources.IMAGE_MENUSTYLEFREE_FG, mButtonOffsetX + num, mButtonOffsetY + 44 + num2);
				TodCommon.TodDrawString(g, mLabel, num3 + num, num4 + num2, mHiliteFont ?? mFont, mColors[1], DrawStringJustification.CenterVerticalMiddle, num5 * 0.8f);
			}
			g.SetScale(1f);
		}
	}
}
