using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;
using MonoGame.IMEHelper;
using Sexy.TodLib;

namespace Sexy;

internal class EditWidget : IMECompatibleWidget
{
	public class WidthCheck
	{
		public Font mFont;

		public int mWidth;
	}

	public enum Colors
	{
		Bkg,
		Outline,
		Text,
		Hilite,
		HiliteText,
		NUM_COLORS
	}

	internal static int[,] gEditWidgetColors = new int[5, 3]
	{
		{ 0, 0, 0 },
		{ 0, 0, 0 },
		{ 0, 0, 0 },
		{ 0, 0, 0 },
		{ 255, 255, 255 }
	};

	public bool mEditing;

	public bool mAcceptsEmptyText;

	public string mTitle;

	public string mDescription;

	private bool callbackDone;

	private bool inputCancelled;

	public int mId;

	public string mString;

	public string mPasswordDisplayString;

	public Font mFont;

	public LinkedList<WidthCheck> mWidthCheckList = new LinkedList<WidthCheck>();

	public EditListener mEditListener;

	public bool mShowingCursor;

	public bool mDrawSelOverride;

	public bool mHadDoubleClick;

	public int mCursorPos;

	public int mHilitePos;

	public int mBlinkAcc;

	public int mBlinkDelay;

	public int mLeftPos;

	public int mMaxChars;

	public int mMaxPixels;

	public SexyChar mPasswordChar;

	public string mUndoString;

	public int mUndoCursor;

	public int mUndoHilitePos;

	public int mLastModifyIdx;

	public override TRect mIMEHotArea
	{
		get
		{
			int num = mX;
			int num2 = mY;
			for (WidgetContainer widgetContainer = mParent; widgetContainer != null; widgetContainer = widgetContainer.mParent)
			{
				num += widgetContainer.mX;
				num2 += widgetContainer.mY;
			}
			return new TRect(num, num2, mWidth, mHeight);
		}
	}

	public EditWidget(int theId, EditListener theEditListener, string title, string description)
	{
		mId = theId;
		mEditListener = theEditListener;
		mFont = null;
		mHadDoubleClick = false;
		mHilitePos = -1;
		mLastModifyIdx = -1;
		mLeftPos = 0;
		mUndoCursor = 0;
		mUndoHilitePos = 0;
		mLastModifyIdx = 0;
		mBlinkAcc = 0;
		mCursorPos = 0;
		mShowingCursor = false;
		mDrawSelOverride = false;
		mMaxChars = -1;
		mMaxPixels = -1;
		mPasswordChar = new SexyChar('\0');
		mBlinkDelay = 40;
		mTitle = TodStringFile.TodStringTranslate(title);
		mDescription = TodStringFile.TodStringTranslate(description);
		mEditing = false;
		mAcceptsEmptyText = false;
		mFont = new XNAFont();
		SetColors(gEditWidgetColors, 5);
		mString = "";
	}

	protected virtual void ProcessKey(KeyCode theKey, SexyChar theChar)
	{
		bool flag = mWidgetManager.mKeyDown[16];
		bool flag2 = mWidgetManager.mKeyDown[17];
		if (theKey == KeyCode.Shift || theKey == KeyCode.Control)
		{
			return;
		}
		bool flag3 = false;
		bool flag4 = !flag;
		if (flag && mHilitePos == -1)
		{
			mHilitePos = mCursorPos;
		}
		string text = new string(mString);
		int num = mCursorPos;
		int num2 = mHilitePos;
		if (theChar == 3u || theChar == 24u)
		{
			if (mHilitePos != -1 && mHilitePos != mCursorPos)
			{
				_ = mCursorPos;
				_ = mHilitePos;
				throw new NotImplementedException();
			}
		}
		else
		{
			if (theChar == 22u)
			{
				throw new NotImplementedException();
			}
			if (theChar == 26u)
			{
				mLastModifyIdx = -1;
				string text2 = mString;
				int num3 = mCursorPos;
				int num4 = mHilitePos;
				mString = mUndoString;
				mCursorPos = mUndoCursor;
				mHilitePos = mUndoHilitePos;
				mUndoString = text2;
				mUndoCursor = num3;
				mUndoHilitePos = num4;
				flag4 = false;
			}
			else
			{
				switch (theKey)
				{
				case KeyCode.Left:
					if (flag2)
					{
						while (mCursorPos > 0 && !IsPartOfWord(mString[mCursorPos - 1]))
						{
							mCursorPos--;
						}
						while (mCursorPos > 0 && IsPartOfWord(mString[mCursorPos - 1]))
						{
							mCursorPos--;
						}
					}
					else if (flag || mHilitePos == -1)
					{
						mCursorPos--;
					}
					else
					{
						mCursorPos = Math.Min(mCursorPos, mHilitePos);
					}
					break;
				case KeyCode.Right:
					if (flag2)
					{
						while (mCursorPos < mString.length() - 1 && IsPartOfWord(mString[mCursorPos + 1]))
						{
							mCursorPos++;
						}
						while (mCursorPos < mString.length() - 1 && !IsPartOfWord(mString[mCursorPos + 1]))
						{
							mCursorPos++;
						}
					}
					if (flag || mHilitePos == -1)
					{
						mCursorPos++;
					}
					else
					{
						mCursorPos = Math.Max(mCursorPos, mHilitePos);
					}
					break;
				case KeyCode.Back:
					if (mString.length() <= 0)
					{
						break;
					}
					if (mHilitePos != -1 && mHilitePos != mCursorPos)
					{
						mString = mString.Substring(0, Math.Min(mCursorPos, mHilitePos)) + mString.Substring(Math.Max(mCursorPos, mHilitePos));
						mCursorPos = Math.Min(mCursorPos, mHilitePos);
						mHilitePos = -1;
						flag3 = true;
						break;
					}
					if (mCursorPos > 0)
					{
						mString = mString.Substring(0, mCursorPos - 1) + mString.Substring(mCursorPos);
					}
					else
					{
						mString = mString.Substring(mCursorPos);
					}
					mCursorPos--;
					mHilitePos = -1;
					if (mCursorPos != mLastModifyIdx)
					{
						flag3 = true;
					}
					mLastModifyIdx = mCursorPos - 1;
					break;
				case KeyCode.Delete:
					if (mString.length() <= 0)
					{
						break;
					}
					if (mHilitePos != -1 && mHilitePos != mCursorPos)
					{
						mString = mString.Substring(0, Math.Min(mCursorPos, mHilitePos)) + mString.Substring(Math.Max(mCursorPos, mHilitePos));
						mCursorPos = Math.Min(mCursorPos, mHilitePos);
						mHilitePos = -1;
						flag3 = true;
						break;
					}
					if (mCursorPos < mString.length())
					{
						mString = mString.Substring(0, mCursorPos) + mString.Substring(mCursorPos + 1);
					}
					if (mCursorPos != mLastModifyIdx)
					{
						flag3 = true;
					}
					mLastModifyIdx = mCursorPos;
					break;
				case KeyCode.Home:
					mCursorPos = 0;
					break;
				case KeyCode.End:
					mCursorPos = mString.length();
					break;
				case KeyCode.Return:
					mEditListener.EditWidgetText(mId, mString);
					break;
				default:
				{
					string theString = theChar.ToString();
					uint num5 = (char)theChar;
					uint num6 = 127u;
					num6 = 255u;
					if (num5 >= 32 && num5 <= num6 && mFont.StringWidth(theString) > 0 && mEditListener.AllowChar(mId, new SexyChar(theChar)))
					{
						if (mHilitePos != -1 && mHilitePos != mCursorPos)
						{
							mString = mString.Substring(0, Math.Min(mCursorPos, mHilitePos)) + theChar.ToString() + mString.Substring(Math.Max(mCursorPos, mHilitePos));
							mCursorPos = Math.Min(mCursorPos, mHilitePos);
							mHilitePos = -1;
							flag3 = true;
						}
						else
						{
							mString = mString.Substring(0, mCursorPos) + theChar.ToString() + mString.Substring(mCursorPos);
							if (mCursorPos != mLastModifyIdx + 1)
							{
								flag3 = true;
							}
							mLastModifyIdx = mCursorPos;
							mHilitePos = -1;
						}
						mCursorPos++;
						FocusCursor(bigJump: false);
					}
					else
					{
						flag4 = false;
					}
					break;
				}
				}
			}
		}
		if (mMaxChars != -1 && mString.length() > mMaxChars)
		{
			mString = mString.Substring(0, mMaxChars);
		}
		EnforceMaxPixels();
		if (mCursorPos < 0)
		{
			mCursorPos = 0;
		}
		else if (mCursorPos > mString.length())
		{
			mCursorPos = mString.length();
		}
		if (num != mCursorPos)
		{
			mBlinkAcc = 0;
			mShowingCursor = true;
		}
		FocusCursor(bigJump: true);
		if (flag4 || mHilitePos == mCursorPos)
		{
			mHilitePos = -1;
		}
		if (!mEditListener.AllowText(mId, ref mString))
		{
			mString = text;
			mCursorPos = num;
			mHilitePos = num2;
		}
		else if (flag3)
		{
			mUndoString = text;
			mUndoCursor = num;
			mUndoHilitePos = num2;
		}
		MarkDirty();
	}

	public virtual void FocusCursor(bool bigJump)
	{
		while (mCursorPos < mLeftPos)
		{
			if (bigJump)
			{
				mLeftPos = Math.Max(0, mLeftPos - 10);
			}
			else
			{
				mLeftPos = Math.Max(0, mLeftPos - 1);
			}
			MarkDirty();
		}
		if (mFont == null)
		{
			return;
		}
		string displayString = GetDisplayString();
		while (mWidth - 8 > 0 && mFont.StringWidth(displayString.Substring(0, mCursorPos)) - mFont.StringWidth(displayString.Substring(0, mLeftPos)) >= mWidth - 8)
		{
			if (bigJump)
			{
				mLeftPos = Math.Min(mLeftPos + 10, mString.length() - 1);
			}
			else
			{
				mLeftPos = Math.Min(mLeftPos + 1, mString.length() - 1);
			}
			MarkDirty();
		}
	}

	protected string GetDisplayString()
	{
		if (mPasswordChar.value_type == '\0')
		{
			return mString;
		}
		if (mPasswordDisplayString.Length != mString.Length)
		{
			string text = mPasswordDisplayString;
			string text2 = mString.Length.ToString();
			SexyChar sexyChar = mPasswordChar;
			mPasswordDisplayString = text + text2 + sexyChar.ToString();
		}
		return mPasswordDisplayString;
	}

	protected virtual void HiliteWord()
	{
		string displayString = GetDisplayString();
		if (mCursorPos < displayString.length())
		{
			mHilitePos = mCursorPos;
			while (mHilitePos > 0 && IsPartOfWord(displayString[mHilitePos - 1]))
			{
				mHilitePos--;
			}
			while (mCursorPos < displayString.length() - 1 && IsPartOfWord(displayString[mCursorPos + 1]))
			{
				mCursorPos++;
			}
			if (mCursorPos < displayString.length())
			{
				mCursorPos++;
			}
		}
	}

	public void EnforceMaxPixels()
	{
		if (mMaxPixels <= 0 && mWidthCheckList.Count == 0)
		{
			return;
		}
		if (mWidthCheckList.Count == 0)
		{
			while (mFont.StringWidth(mString) > mMaxPixels)
			{
				mString = mString.Substring(0, mString.length() - 1);
			}
			return;
		}
		foreach (WidthCheck mWidthCheck in mWidthCheckList)
		{
			int num = mWidthCheck.mWidth;
			if (num <= 0)
			{
				num = mMaxPixels;
				if (num <= 0)
				{
					continue;
				}
			}
			while (mWidthCheck.mFont.StringWidth(mString) > num)
			{
				mString = mString.Substring(0, mString.length() - 1);
			}
		}
	}

	public virtual bool IsPartOfWord(char theChar)
	{
		if ((theChar < 'A' || theChar > 'Z') && (theChar < 'a' || theChar > 'z') && (theChar < '0' || theChar > '9') && (theChar < 'À' || theChar > 'ÿ'))
		{
			return theChar == '_';
		}
		return true;
	}

	public virtual void SetFont(Font theFont)
	{
		mFont.Dispose();
		mFont = theFont.Duplicate();
	}

	public virtual void SetText(string theText, bool leftPosToZero = true)
	{
		mString = theText;
		mCursorPos = mString.length();
		mHilitePos = 0;
		if (leftPosToZero)
		{
			mLeftPos = 0;
		}
		else
		{
			FocusCursor(bigJump: true);
		}
		MarkDirty();
	}

	public override void Resize(TRect frame)
	{
		base.Resize(frame);
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		RehupBounds();
	}

	public override void Draw(Graphics g)
	{
		string displayString = GetDisplayString();
		g.SetColor(mColors[0]);
		g.FillRect(0, 0, mWidth, mHeight);
		for (int i = 0; i < 2; i++)
		{
			Graphics graphics = g.Create();
			graphics.SetFont(mFont);
			if (i == 1)
			{
				int num = mFont.StringWidth(displayString.Substring(0, mCursorPos)) - mFont.StringWidth(displayString.Substring(0, mLeftPos));
				int val = num + 2;
				if (mHilitePos != -1 && mCursorPos != mHilitePos)
				{
					val = mFont.StringWidth(displayString.Substring(0, mHilitePos)) - mFont.StringWidth(displayString.Substring(0, mLeftPos));
				}
				if (!mShowingCursor)
				{
					num += 2;
				}
				num = Math.Min(Math.Max(0, num), mWidth - 8);
				val = Math.Min(Math.Max(0, val), mWidth - 8);
				graphics.ClipRect(4 + Math.Min(num, val), (mHeight - mFont.GetHeight()) / 2, Math.Abs(val - num), mFont.GetHeight());
			}
			else
			{
				graphics.ClipRect(4, 0, mWidth - 8, mHeight);
			}
			bool flag = mHasFocus || mDrawSelOverride;
			if ((i == 1) & flag)
			{
				graphics.SetColor(mColors[3]);
				graphics.FillRect(0, 0, mWidth, mHeight);
			}
			if (i == 0 || !flag)
			{
				graphics.SetColor(mColors[2]);
			}
			else
			{
				graphics.SetColor(mColors[4]);
			}
			graphics.DrawString(displayString.Substring(mLeftPos), 4, 0);
			graphics?.Dispose();
		}
		g.SetColor(mColors[1]);
		g.DrawRect(0, 0, mWidth - 1, mHeight - 1);
	}

	public override void Update()
	{
		base.Update();
		if (mHasFocus && ++mBlinkAcc > mBlinkDelay)
		{
			MarkDirty();
			mBlinkAcc = 0;
			mShowingCursor = !mShowingCursor;
		}
	}

	public override bool WantsFocus()
	{
		return true;
	}

	public override void GotFocus()
	{
		base.GotFocus();
		IMEHandler mIMEHandler = mWidgetManager.mIMEHandler;
		mWidgetManager.mIMEHotWidget = this;
		mShowingCursor = true;
		mBlinkAcc = 0;
		MarkDirty();
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		base.MouseDown(x, y, theBtnNum, theClickCount);
		mHilitePos = -1;
		mCursorPos = GetCharAt(x, y);
		if (theClickCount > 1)
		{
			mHadDoubleClick = true;
			HiliteWord();
		}
		MarkDirty();
		FocusCursor(bigJump: false);
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
		base.MouseUp(x, y, theBtnNum, theClickCount);
		if (mHilitePos == mCursorPos)
		{
			mHilitePos = -1;
		}
		if (mHadDoubleClick)
		{
			mHilitePos = -1;
			mCursorPos = GetCharAt(x, y);
			mHadDoubleClick = false;
			HiliteWord();
		}
		MarkDirty();
	}

	public override void MouseDrag(int x, int y)
	{
		base.MouseDrag(x, y);
		if (mHilitePos == -1)
		{
			mHilitePos = mCursorPos;
		}
		mCursorPos = GetCharAt(x, y);
		MarkDirty();
		FocusCursor(bigJump: false);
	}

	public override void MouseEnter()
	{
		base.MouseEnter();
	}

	public override void MouseLeave()
	{
		base.MouseLeave();
	}

	public virtual int GetCharAt(int x, int y)
	{
		int result = 0;
		string displayString = GetDisplayString();
		for (int i = mLeftPos; i < displayString.length(); i++)
		{
			string theString = displayString.Substring(mLeftPos, i - mLeftPos);
			string theString2 = displayString.Substring(mLeftPos, i - mLeftPos + 1);
			int num = mFont.StringWidth(theString);
			int num2 = mFont.StringWidth(theString2);
			if (x >= (num + num2) / 2 + 5)
			{
				result = i + 1;
			}
		}
		return result;
	}

	public override void KeyChar(SexyChar theChar)
	{
		ProcessKey(KeyCode.Unknown, new SexyChar(theChar));
		base.KeyChar(theChar);
	}

	private void KeyboardCallback(IAsyncResult result)
	{
		string text = Guide.EndShowKeyboardInput(result);
		inputCancelled = text == null;
		if (!inputCancelled)
		{
			mString = text;
		}
		callbackDone = true;
	}

	public override void KeyDown(KeyCode theKey)
	{
		if (((uint)theKey < 65u || (uint)theKey >= 90u) && mEditListener.AllowKey(mId, theKey))
		{
			ProcessKey(theKey, new SexyChar('\0'));
		}
		base.KeyDown(theKey);
	}

	private void DoKeyboardCallback()
	{
		callbackDone = false;
		if (mString == null)
		{
			LostFocus();
			EditingEnded(mString);
		}
		else
		{
			EditingEnded(mString);
		}
	}

	public override void LostFocus()
	{
		base.LostFocus();
		mShowingCursor = false;
		mWidgetManager.mIMEHotWidget = null;
		MarkDirty();
	}

	public virtual void RehupBounds()
	{
	}

	public virtual void EditingEnded(string theString)
	{
		mEditing = false;
		mString = theString;
		mEditListener.EditWidgetText(mId + (inputCancelled ? 1000 : 0), mString);
	}

	public virtual bool ShouldChangeCharacters(int theRangeStart, int theRangeLength, string theReplacementChars)
	{
		return true;
	}

	public virtual bool ShouldClear()
	{
		bool flag = mEditListener.ShouldClear();
		if (flag)
		{
			mString = "";
		}
		return flag;
	}

	public override void Dispose()
	{
		mFont = null;
	}
}
