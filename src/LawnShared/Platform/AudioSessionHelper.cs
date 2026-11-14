// iOS audio session handling.
// MonoGame's MediaPlayer.GameHasControl asks MPMusicPlayerController (Apple Music). On the very first launch the
// media-library permission is still undetermined, the check returns "other music is playing", and the game kept
// its BGM off for the whole session. AVAudioSession answers the same question without any permission.
using System;
#if __IOS__
using AVFoundation;
using Foundation;
#endif

namespace LawnPlatform;

public static class AudioSessionHelper
{
#if __IOS__
	private static NSObject sInterruptionObserver;
#endif

	/// <summary>True when another app (Music, Spotify...) is already playing audio at launch.</summary>
	public static bool OtherAudioPlaying()
	{
#if __IOS__
		try
		{
			AVAudioSession session = AVAudioSession.SharedInstance();
			return session.SecondaryAudioShouldBeSilencedHint;
		}
		catch (Exception ex)
		{
			PvZActivity.Log("audio session check failed: " + ex.Message);
			return false;
		}
#else
		try
		{
			return !Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl;
		}
		catch
		{
			return false;
		}
#endif
	}

	/// <summary>Sets the session up once at startup and re-activates it after phone calls / Siri / alarms.</summary>
	public static void Configure(bool theMixWithOthers)
	{
#if __IOS__
		try
		{
			AVAudioSession session = AVAudioSession.SharedInstance();
			// Playback: sound is heard even with the ring/silent switch on (SoloAmbient/Ambient mute the game
			// on phones in silent mode, which looks like "no sound at all").
			NSError err = theMixWithOthers
				? session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.MixWithOthers)
				: session.SetCategory(AVAudioSessionCategory.Playback);
			if (err != null)
			{
				PvZActivity.Log("audio category: " + err.LocalizedDescription);
			}
			err = session.SetActive(true);
			if (err != null)
			{
				PvZActivity.Log("audio activate: " + err.LocalizedDescription);
			}
			if (sInterruptionObserver == null)
			{
				sInterruptionObserver = AVAudioSession.Notifications.ObserveInterruption((object sender, AVAudioSessionInterruptionEventArgs e) =>
				{
					if (e.InterruptionType == AVAudioSessionInterruptionType.Ended)
					{
						Reactivate();
					}
				});
			}
		}
		catch (Exception ex)
		{
			PvZActivity.Log("audio session setup failed: " + ex.Message);
		}
#endif
	}

	public static void Reactivate()
	{
#if __IOS__
		try
		{
			AVAudioSession.SharedInstance().SetActive(true);
		}
		catch (Exception ex)
		{
			PvZActivity.Log("audio session reactivate failed: " + ex.Message);
		}
#endif
	}
}
