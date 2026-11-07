using System;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed class LevelInfoPanel : PulloutPanel, ButtonListener
{
	private sealed class InfoBody : Widget
	{
		public readonly LevelInfoRichText Text;

		private readonly Font mTitleFont = Resources.FONT_DWARVENTODCRAFT15;

		public InfoBody(LawnApp app, int columns)
		{
			mMouseVisible = false;
			ClipInput = true;
			Text = new LevelInfoRichText(app.mReanimatorCache, Resources.FONT_DWARVENTODCRAFT12, columns);
			AddWidget(Text);
		}

		public override void Resize(int x, int y, int width, int height)
		{
			base.Resize(x, y, width, height);
			int num = mTitleFont.GetHeight() - mTitleFont.GetAscentPadding() + 10;
			Text.Resize(0, num, width, Math.Max(0, height - num));
		}

		public override void Draw(Graphics g)
		{
			string theString = TodStringFile.TodStringTranslate("[OVERRIDE_TITLE]");
			g.SetFont(mTitleFont);
			g.SetColor(SexyColor.White);
			g.DrawString(theString, (mWidth - mTitleFont.StringWidth(theString)) / 2, 0);
		}
	}

	private readonly LawnApp mApp;

	private readonly LevelInfoRichText mText;

	private readonly LawnStoneButton mToggle;

	private bool mRawValues;

	public bool HasOverrides => !string.IsNullOrEmpty(mText.Text);

	public LevelInfoPanel(LawnApp app, int columns = 1)
		: base(AtlasResources.IMAGE_DIALOG_HEADER, 15, new InfoBody(app, columns), new Widget
		{
			mMouseVisible = false
		})
	{
		mApp = app;
		mText = ((InfoBody)Content).Text;
		mToggle = GameButton.MakeButton(0, this, "[OVERRIDE_SHOW_RAW_VALUES]");
		Footer.AddWidget(mToggle);
		RefreshText();
	}

	public override void Resize(int x, int y, int width, int height)
	{
		base.Resize(x, y, width, height);
		int num = AtlasResources.IMAGE_BUTTON_LEFT.mHeight;
		int num2 = Math.Min(width, 240);
		mToggle.Resize((Footer.mWidth - num2) / 2, (Footer.mHeight - num) / 2, num2, num);
	}

	private void RefreshText()
	{
		mText.SetLevel(mApp, mRawValues);
		mToggle.SetLabel(mRawValues ? "[OVERRIDE_SHOW_RELATIVE_VALUES]" : "[OVERRIDE_SHOW_RAW_VALUES]");
		mToggle.mInverted = mRawValues;
		MarkDirty();
	}

	public void ButtonDepress(int id)
	{
		mRawValues = !mRawValues;
		RefreshText();
	}

	public void ButtonPress(int id)
	{
	}

	public void ButtonPress(int id, int count)
	{
	}

	public void ButtonDownTick(int id)
	{
	}

	public void ButtonMouseEnter(int id)
	{
	}

	public void ButtonMouseLeave(int id)
	{
	}

	public void ButtonMouseMove(int id, int x, int y)
	{
	}
}
