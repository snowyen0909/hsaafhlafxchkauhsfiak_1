namespace Sexy.TodLib;

public class ReanimationParams
{
	public ReanimationType mReanimationType;

	public string mReanimFileName;

	public string mOldFilename;

	public int mReanimParamFlags;

	public bool mIsSpine;

	public readonly float mSpineScale;

	public ReanimationParams(ReanimationType aReanimationType, string aReanimFilename)
		: this(aReanimationType, aReanimFilename, 0)
	{
	}

	public ReanimationParams(ReanimationType aReanimationType, string aReanimFilename, int aReanimparamFlags)
		: this(aReanimationType, aReanimFilename, 0, isSpine: false)
	{
	}

	public ReanimationParams(ReanimationType aReanimationType, string aReanimFilename, int aReanimparamFlags, bool isSpine, string aOldFilename = null, float aSpineScale = 16f)
	{
		mReanimationType = aReanimationType;
		mReanimFileName = aReanimFilename;
		mReanimParamFlags = aReanimparamFlags;
		mIsSpine = isSpine;
		mOldFilename = aOldFilename;
		mSpineScale = aSpineScale;
	}
}
