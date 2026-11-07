namespace Sexy;

public class IMECompatibleWidget : Widget
{
	public bool mIMEEnabled;

	public int mOldY;

	public virtual TRect mIMEHotArea => new TRect(mX, mY, mWidth, mHeight);
}
