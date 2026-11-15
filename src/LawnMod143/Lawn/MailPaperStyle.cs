using System;
using Sexy;

namespace Lawn;

internal sealed class MailPaperStyle
{
	internal readonly string mResourceGroup;

	internal readonly string mImageId;

	internal readonly int mOffsetX;

	internal readonly TRect mContentBounds;

	internal readonly TRect mTitleBounds;

	internal readonly SexyColor mInk;

	private static readonly MailPaperStyle mZombie = new MailPaperStyle("DelayLoad_ZombieNote", "IMAGE_ZOMBIE_NOTE", 0, new TRect(20, 25, 614, 380), new TRect(52, 28, 548, 40), new SexyColor(63, 42, 25));

	private static readonly MailPaperStyle mCardstock = new MailPaperStyle("DelayLoad_Mailbox", "IMAGE_MAIL_BACKGROUND_CARDSTOCK", 1, new TRect(64, 70, 502, 308), new TRect(55, 5, 519, 50), new SexyColor(63, 42, 25));

	internal MailPaperStyle(string theResourceGroup, string theImageId, int theOffsetX, TRect theContentBounds, TRect theTitleBounds, SexyColor theInk)
	{
		mResourceGroup = theResourceGroup;
		mImageId = theImageId;
		mOffsetX = theOffsetX;
		mContentBounds = theContentBounds;
		mTitleBounds = theTitleBounds;
		mInk = theInk;
	}

	internal static MailPaperStyle GetPaperStyle(MailPaperType theType)
	{
		return theType switch
		{
			MailPaperType.Zombie => mZombie, 
			MailPaperType.Cardstock => mCardstock, 
			_ => throw new ArgumentOutOfRangeException("theType"), 
		};
	}
}
