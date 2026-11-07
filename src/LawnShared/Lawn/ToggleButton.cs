using Sexy;

namespace Lawn;

public class ToggleButton : NewLawnButton
{
	public bool mIsToggled;

	public Image mToggledImage;

	public Image mNoToggledImage;

	public ToggleButton(Image theComponentImage, int theId, ButtonListener theListener)
		: base(theComponentImage, theId, theListener)
	{
	}

	public static ToggleButton MakeNewButton(int theId, ButtonListener theListener, Image theImage, Image theToggledImage)
	{
		ToggleButton toggleButton = new ToggleButton(null, theId, theListener);
		if (theImage != null)
		{
			toggleButton.mWidth = theImage.mWidth;
			toggleButton.mHeight = theImage.mHeight;
		}
		toggleButton.mButtonImage = theImage;
		toggleButton.mOverImage = theToggledImage;
		toggleButton.mDownImage = theToggledImage;
		toggleButton.mHasAlpha = true;
		toggleButton.mHasTransparencies = true;
		toggleButton.mToggledImage = theToggledImage;
		toggleButton.mNoToggledImage = theImage;
		toggleButton.mClip = false;
		toggleButton.SetToggled(toggled: false);
		return toggleButton;
	}

	public void SetToggled(bool toggled)
	{
		mIsToggled = toggled;
		mButtonImage = (toggled ? mToggledImage : mNoToggledImage);
	}
}
