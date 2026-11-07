namespace Sexy.TodLib;

public class FoleyTypeData
{
	public FoleyInstance[] mFoleyInstances = new FoleyInstance[24];

	public int mLastVariationPlayed;

	public FoleyTypeData()
	{
		mLastVariationPlayed = -1;
		for (int i = 0; i < 24; i++)
		{
			mFoleyInstances[i] = new FoleyInstance();
		}
	}
}
