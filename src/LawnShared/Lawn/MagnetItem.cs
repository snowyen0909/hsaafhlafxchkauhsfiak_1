using Sexy;

namespace Lawn;

public class MagnetItem
{
	public float mPosX;

	public float mPosY;

	public float mDestOffsetX;

	public float mDestOffsetY;

	public MagnetItemType mItemType;

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncFloat(ref mPosX);
		theContext.SyncFloat(ref mPosY);
		theContext.SyncFloat(ref mDestOffsetX);
		theContext.SyncFloat(ref mDestOffsetY);
		theContext.SyncEnum(ref mItemType, theContext.aMagnetItemTypeSaver, SaveFileVersion.Initial, MagnetItemType.None);
		if (theContext.mVersion < SaveFileVersion.NewLayout600)
		{
			mPosX -= 100f;
		}
	}

	public void Reset()
	{
		mPosX = (mPosY = (mDestOffsetX = (mDestOffsetY = 0f)));
		mItemType = MagnetItemType.None;
	}

	public bool SaveToFile(Buffer b)
	{
		b.WriteFloat(mDestOffsetX);
		b.WriteFloat(mDestOffsetY);
		b.WriteLong((int)mItemType);
		b.WriteFloat(mPosX);
		b.WriteFloat(mPosY);
		return true;
	}

	public bool LoadFromFile(Buffer b)
	{
		mDestOffsetX = b.ReadFloat();
		mDestOffsetY = b.ReadFloat();
		mItemType = (MagnetItemType)b.ReadLong();
		mPosX = b.ReadFloat();
		mPosY = b.ReadFloat();
		return true;
	}
}
