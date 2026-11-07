using System.Collections.Generic;

namespace Sexy;

public class PreModalInfo
{
	public Widget mBaseModalWidget;

	public Widget mPrevBaseModalWidget;

	public Widget mPrevFocusWidget;

	public FlagsMod mPrevBelowModalFlagsMod;

	private static Stack<PreModalInfo> unusedObjects = new Stack<PreModalInfo>();

	public void PrepareForReuse()
	{
		Reset();
		unusedObjects.Push(this);
	}

	public static PreModalInfo GetNewPreModalInfo()
	{
		if (unusedObjects.Count > 0)
		{
			return unusedObjects.Pop();
		}
		return new PreModalInfo();
	}

	private void Reset()
	{
		mBaseModalWidget = null;
		mPrevBaseModalWidget = null;
		mPrevFocusWidget = null;
		mPrevBelowModalFlagsMod = default;
	}

	private PreModalInfo()
	{
		Reset();
	}
}
