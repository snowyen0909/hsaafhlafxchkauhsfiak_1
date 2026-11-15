using System;
using System.Collections.Generic;

namespace Lawn;

internal sealed class LocalMailboxState
{
	internal bool mShowingRead;

	internal int mIndex;

	private readonly PlayerInfo mPlayerInfo;

	private List<LocalMail> mMessages;

	internal LocalMailboxState(PlayerInfo thePlayerInfo)
	{
		mPlayerInfo = thePlayerInfo;
		RefreshMessages();
	}

	internal IReadOnlyList<LocalMail> GetMessages()
	{
		return mMessages;
	}

	internal LocalMail GetCurrentMail()
	{
		if (mMessages.Count != 0)
		{
			return mMessages[mIndex];
		}
		return null;
	}

	internal bool SwitchMail(int theDirection)
	{
		if (mMessages.Count == 0)
		{
			return false;
		}
		int num = Math.Clamp(mIndex + theDirection, 0, mMessages.Count - 1);
		if (num == mIndex)
		{
			return false;
		}
		mIndex = num;
		return true;
	}

	internal void SwitchMailbox()
	{
		mShowingRead = !mShowingRead;
		mIndex = 0;
		RefreshMessages();
	}

	internal bool ToggleCurrentRead()
	{
		LocalMail currentMail = GetCurrentMail();
		if (!((mShowingRead && currentMail != null && currentMail.mDeleteOnlyAfterRead) ? LocalMailbox.Delete(mPlayerInfo, currentMail) : LocalMailbox.SetRead(mPlayerInfo, currentMail, !mShowingRead)))
		{
			return false;
		}
		RefreshMessages();
		return true;
	}

	private void RefreshMessages()
	{
		mMessages = mPlayerInfo.mMail.FindAll((LocalMail aMail) => aMail.IsSupported() && aMail.mReadOrder != 0 == mShowingRead);
		if (mShowingRead)
		{
			mMessages.Sort((LocalMail aLeft, LocalMail aRight) => aRight.mReadOrder.CompareTo(aLeft.mReadOrder));
		}
		mIndex = Math.Max(0, Math.Min(mIndex, mMessages.Count - 1));
	}
}
