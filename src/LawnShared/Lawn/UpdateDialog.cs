using System;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

public class UpdateDialog : LawnDialog
{
	private sealed class RichTextDocument : Widget
	{
		private readonly RichTextLayout _layout;

		public RichTextDocument(RichTextLayout layout)
		{
			_layout = layout;
			mMouseVisible = false;
		}

		public override void Draw(Graphics g)
		{
			if (mWidth <= 0 || mHeight <= 0 || g.mClipRect.mWidth <= 0 || g.mClipRect.mHeight <= 0)
			{
				return;
			}
			g.HardwareClip();
			try
			{
				_layout.Draw(Graphics.spriteBatch, new Vector2(g.mTransX, g.mTransY), Color.White);
			}
			finally
			{
				g.EndHardwareClip();
			}
		}
	}

	private readonly RichTextLayout _richTextLayout = new RichTextLayout
	{
		VerticalSpacing = 4
	};

	private readonly ScrollWidget _scrollWidget;

	private readonly RichTextDocument _document;

	private int ContentTop => mContentInsets.mTop + mBackgroundInsets.mTop + (int)Constants.InvertAndScale(4f) + Constants.DIALOG_HEADER_OFFSET + mHeaderFont.GetHeight() + mSpaceAfterHeader;

	static UpdateDialog()
	{
		RichTextDefaults.ImageResolver = (string p) =>
		{
			Image imageInAtlasById = AtlasResources.GetImageInAtlasById(AtlasResources.GetAtlasIdByStringId(p));
			return new TextureFragment(imageInAtlasById.Texture, new Rectangle(imageInAtlasById.mS, imageInAtlasById.mT, imageInAtlasById.mWidth, imageInAtlasById.mHeight));
		};
	}

	public UpdateDialog(LawnApp theApp, string richTextContent, Font font)
		: base(theApp, null, 65, isModal: false, "植物娘大战僵尸 更新公告", string.Empty, "[BUTTON_OK]", 3)
	{
		_richTextLayout.Font = (font as StashFont).mFonts[0];
		_richTextLayout.Text = richTextContent;
		_document = new RichTextDocument(_richTextLayout);
		_scrollWidget = new ScrollWidget
		{
			ClipInput = true
		};
		_scrollWidget.AddWidget(_document);
		_scrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		AddWidget(_scrollWidget);
		_richTextLayout.Width = GetTextWidth(700);
		int num = AtlasResources.IMAGE_DIALOG_BOTTOMLEFT.mHeight;
		int val = ContentTop + _richTextLayout.Size.Y + num;
		int val2 = Constants.DIALOG_HEADER_OFFSET + AtlasResources.IMAGE_DIALOG_TOPLEFT.mHeight + num;
		int val3 = Constants.BOARD_HEIGHT - (int)Constants.InvertAndScaleUI(40f);
		Resize(mX, mY, 700, Math.Min(val3, Math.Max(val2, val)));
	}

	private int GetContentWidth(int dialogWidth)
	{
		return Math.Max(1, dialogWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight - (int)Constants.InvertAndScale(4f));
	}

	private int GetTextWidth(int dialogWidth)
	{
		return Math.Max(1, GetContentWidth(dialogWidth) - AtlasResources.IMAGE_SCROLL_INDICATOR.mWidth - 4);
	}

	public override void Resize(int x, int y, int width, int height)
	{
		bool flag = width != mWidth || height != mHeight;
		base.Resize(x, y, width, height);
		if (flag)
		{
			int theX = mBackgroundInsets.mLeft + mContentInsets.mLeft + (int)Constants.InvertAndScale(2f);
			int num = mLawnYesButton.mY - (int)Constants.InvertAndScaleUI(10f);
			_scrollWidget.Resize(theX, ContentTop, GetContentWidth(width), Math.Max(1, num - ContentTop));
			int textWidth = GetTextWidth(width);
			_richTextLayout.Width = textWidth;
			_document.Resize(_document.mX, _document.mY, textWidth, _richTextLayout.Size.Y);
			_scrollWidget.ClientSizeChanged();
			_scrollWidget.SetScrollOffset(new CGPoint(0f, Math.Clamp(_scrollWidget.GetScrollOffset().y, Math.Min(0, _scrollWidget.mHeight - _document.mHeight), 0f)), animated: false);
		}
	}

	public override void Dispose()
	{
		RemoveWidget(_scrollWidget);
		_scrollWidget.Dispose();
		base.Dispose();
	}
}
