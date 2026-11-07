namespace Sexy;

public class UNPACK_INFO
{
	public Image mpImage;

	public int mX;

	public int mY;

	public int mWidth;

	public int mHeight;

	public int mRows;

	public int mCols;

	public AnimType mAnimType;

	public int mFrameDelay;

	public int mBeginDelay;

	public int mEndDelay;

	public float? mInvScale;

	public UNPACK_INFO(Image pImage, int theX, int theY, int theWidth, int theHeight, int theRows, int theCols, AnimType theAnimType, int theFrameDelay, int theBeginDelay, int theEndDelay, float? theInvScale = null)
	{
		mpImage = pImage;
		mX = theX;
		mY = theY;
		mWidth = theWidth;
		mHeight = theHeight;
		mRows = theRows;
		mCols = theCols;
		mAnimType = theAnimType;
		mFrameDelay = theFrameDelay;
		mBeginDelay = theBeginDelay;
		mEndDelay = theEndDelay;
		mInvScale = theInvScale;
	}
}
