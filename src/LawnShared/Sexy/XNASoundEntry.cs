using Microsoft.Xna.Framework.Audio;

namespace Sexy;

internal class XNASoundEntry
{
	public float mBaseVolume = 1f;

	public float mBasePan;

	public SoundEffect mSound;

	/// <summary>
	/// False for sounds that came from a ContentManager: the manager keeps them cached and hands the same object
	/// back on the next Load, so disposing them here left the next level playing a freed (silent / noisy) buffer.
	/// </summary>
	public bool mOwned;

	public void Dispose()
	{
		if (mOwned && mSound != null && !mSound.IsDisposed)
		{
			mSound.Dispose();
		}
		mSound = null;
	}
}
