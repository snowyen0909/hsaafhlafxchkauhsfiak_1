using Microsoft.Xna.Framework;
using Sexy.TodLib;

namespace Lawn;

public class Wind
{
	public int mAge;

	public WindType mType;

	public int mDisappearCountdown;

	public bool mDead;

	public Vector2 mCurrentWind;

	public void Update()
	{
		mAge++;
		mDisappearCountdown--;
		if (mDisappearCountdown < 0)
		{
			mDead = true;
		}
		else if (mType == WindType.Blover)
		{
			mCurrentWind.X = TodCommon.TodAnimateCurveFloat(0, 300, mAge, 30f, 10f, TodCurves.EaseSinWave);
			mCurrentWind.Y = 0f;
		}
	}

	public Wind(WindType windType)
	{
		mType = windType;
		mAge = 0;
		mDead = false;
		mCurrentWind = Vector2.Zero;
		if (windType == WindType.Blover)
		{
			mDisappearCountdown = 300;
		}
	}

	public void PrepareForReuse()
	{
	}
}
