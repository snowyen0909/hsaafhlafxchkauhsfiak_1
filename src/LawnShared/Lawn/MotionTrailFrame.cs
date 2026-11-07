using Sexy;

namespace Lawn;

public class MotionTrailFrame
{
	public float mPosX;

	public float mPosY;

	public float mAnimTime;

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncFloat(ref mPosX);
		theContext.SyncFloat(ref mPosY);
		theContext.SyncFloat(ref mAnimTime);
	}

	public void SaveToFile(Buffer b)
	{
		b.WriteFloat(mPosX);
		b.WriteFloat(mPosY);
		b.WriteFloat(mAnimTime);
	}

	public void LoadFromFile(Buffer b)
	{
		mPosX = b.ReadFloat();
		mPosY = b.ReadFloat();
		mAnimTime = b.ReadFloat();
	}
}
