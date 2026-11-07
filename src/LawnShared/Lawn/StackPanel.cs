using System.Linq;
using Sexy;

namespace Lawn;

public class StackPanel : Widget
{
	public readonly bool mIsVertical;

	public LayoutFlags mAlignment;

	public bool mSupressRefresh = true;

	public StackPanel(bool isVertical)
	{
		mIsVertical = isVertical;
	}

	public override void Draw(Graphics g)
	{
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		int num = 0;
		int num2 = 0;
		if (mIsVertical)
		{
			foreach (Widget mWidget in mWidgets)
			{
				mWidget.Resize(num, num2, mWidget.mWidth, mWidget.mHeight);
				num2 += mWidget.mHeight;
			}
		}
		else
		{
			foreach (Widget mWidget2 in mWidgets)
			{
				mWidget2.Resize(num, num2, mWidget2.mWidth, mWidget2.mHeight);
				num += mWidget2.mWidth;
			}
		}
		mX = theX;
		mY = theY;
	}

	public void RefreshSize()
	{
		if (mIsVertical)
		{
			mHeight = mWidgets.Select((Widget c) => c.mHeight).Sum();
			mWidth = mWidgets.Select((Widget c) => c.mWidth).Max();
		}
		else
		{
			mHeight = mWidgets.Select((Widget c) => c.mHeight).Max();
			mWidth = mWidgets.Select((Widget c) => c.mWidth).Sum();
		}
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		mSupressRefresh = false;
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		mSupressRefresh = true;
	}

	public override void AddWidget(Widget theWidget)
	{
		base.AddWidget(theWidget);
		if (!mSupressRefresh)
		{
			RefreshSize();
		}
	}

	public override void RemoveWidget(Widget theWidget)
	{
		base.RemoveWidget(theWidget);
		if (!mSupressRefresh)
		{
			RefreshSize();
		}
	}
}
