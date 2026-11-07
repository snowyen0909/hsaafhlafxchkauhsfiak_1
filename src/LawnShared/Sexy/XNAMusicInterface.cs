using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Sexy;

internal class XNAMusicInterface : MusicInterface
{
	private const float VOLUME_LIMIT = 0.5f;

	private const float MO3_VOLUME_LIMIT = 1f;

	private const int MO3_RESERVED_CHANNELS = 3;

	private readonly Song[] mSongs = new Song[XNAMusicInterfaceConstants.MAX_SONGS];

	private readonly OpenMptMo3MusicTrack[] mMo3Songs = new OpenMptMo3MusicTrack[XNAMusicInterfaceConstants.MAX_SONGS];

	private readonly float[] mSongVolumes = new float[XNAMusicInterfaceConstants.MAX_SONGS];

	private readonly float[] mSongMaxVolumes = new float[XNAMusicInterfaceConstants.MAX_SONGS];

	private readonly ContentManager mContent;

	private readonly SoundManager mSoundManager;

	private bool mDisposed;

	private bool mEnabled;

	private int mCurrentSong;

	private bool mFadeOut;

	private float mMasterVolume;

	private bool mHasPlayed;

	private float mFirstPlayVolume;

	public XNAMusicInterface(SexyAppBase theApp)
	{
		MusicInterface.USER_MUSIC_PLAYING = !Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl;
		mEnabled = false;
		mContent = theApp.mContentManager;
		mSoundManager = theApp.mSoundManager;
		mCurrentSong = -1;
		mFadeOut = false;
		for (int i = 0; i < XNAMusicInterfaceConstants.MAX_SONGS; i++)
		{
			mSongVolumes[i] = 1f;
			mSongMaxVolumes[i] = 1f;
		}
		if (!mSoundManager.TryReserveChannels(3))
		{
			throw new InvalidOperationException("Insufficient audio channel capacity for MO3 music.");
		}
	}

	public override void Dispose()
	{
		if (!mDisposed)
		{
			UnloadAllMusic();
			mSoundManager.ReleaseReservedChannels(3);
			mDisposed = true;
			base.Dispose();
		}
	}

	public override void Enable(bool enable)
	{
		mEnabled = enable;
	}

	public override bool LoadMusic(int theSongId, string theFileName)
	{
		if (!IsValidSongId(theSongId))
		{
			return false;
		}
		if (TryLoadMo3(theSongId, theFileName))
		{
			return true;
		}
		if (Path.GetExtension(theFileName).Equals(".mo3", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		mSongs[theSongId] = mContent.Load<Song>(theFileName);
		return true;
	}

	public override void PlayMusic(int theSongId, int offset, float fadeOutSeconds, float fadeInSeconds, bool loop)
	{
		LawnPlatform.PvZActivity.Log($"PlayMusic song={theSongId} mo3={(IsValidSongId(theSongId) && mMo3Songs[theSongId] != null)} song={(IsValidSongId(theSongId) && mSongs[theSongId] != null)} userMusic={MusicInterface.USER_MUSIC_PLAYING}");
		if (MusicInterface.USER_MUSIC_PLAYING || !IsValidSongId(theSongId))
		{
			return;
		}
		try
		{
			OpenMptMo3MusicTrack openMptMo3MusicTrack = mMo3Songs[theSongId];
			if (openMptMo3MusicTrack != null)
			{
				StopMediaPlayer();
				ApplyFirstPlayVolume();
				ApplySongVolume(theSongId);
				if (!openMptMo3MusicTrack.IsPlaying)
				{
					openMptMo3MusicTrack.Play(loop);
				}
				isStopped = false;
				return;
			}
			Song song = mSongs[theSongId];
			if (song != null && (theSongId != mCurrentSong || Microsoft.Xna.Framework.Media.MediaPlayer.State != MediaState.Playing))
			{
				StopAllMo3Music();
				mCurrentSong = theSongId;
				Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = loop;
				ApplyFirstPlayVolume();
				ApplySongVolume(theSongId);
				Microsoft.Xna.Framework.Media.MediaPlayer.Play(song);
				isStopped = false;
			}
		}
		catch (Exception ex)
		{
			LawnPlatform.PvZActivity.Log("music error: " + ex);
		}
	}

	public override void PlayMusicFromOrder(int theSongId, int theOrderOffset, double theVolume, bool loop)
	{
		LawnPlatform.PvZActivity.Log($"PlayMusicFromOrder song={theSongId} order={theOrderOffset} vol={theVolume} mo3={(IsValidSongId(theSongId) && mMo3Songs[theSongId] != null)} userMusic={MusicInterface.USER_MUSIC_PLAYING}");
		if (MusicInterface.USER_MUSIC_PLAYING || !IsValidSongId(theSongId))
		{
			return;
		}
		OpenMptMo3MusicTrack openMptMo3MusicTrack = mMo3Songs[theSongId];
		if (openMptMo3MusicTrack == null)
		{
			base.PlayMusicFromOrder(theSongId, theOrderOffset, theVolume, loop);
			return;
		}
		try
		{
			StopMediaPlayer();
			mSongVolumes[theSongId] = ClampVolume(theVolume);
			ApplyFirstPlayVolume();
			ApplySongVolume(theSongId);
			openMptMo3MusicTrack.PlayFromOrder(theOrderOffset, loop);
			isStopped = false;
		}
		catch (Exception ex)
		{
			LawnPlatform.PvZActivity.Log("music error: " + ex);
		}
	}

	public override void StopMusic(float fadeOutSeconds)
	{
		StopAllMusic();
	}

	public override void StopMusic(int theSongId)
	{
		if (MusicInterface.USER_MUSIC_PLAYING || !IsValidSongId(theSongId))
		{
			return;
		}
		try
		{
			if (mMo3Songs[theSongId] != null)
			{
				mMo3Songs[theSongId].Stop();
			}
			else if (theSongId == mCurrentSong)
			{
				StopMediaPlayer();
			}
		}
		catch (Exception)
		{
		}
	}

	public override void StopAllMusic()
	{
		if (MusicInterface.USER_MUSIC_PLAYING)
		{
			return;
		}
		try
		{
			StopAllMo3Music();
			StopMediaPlayer();
			isStopped = true;
		}
		catch (Exception)
		{
		}
	}

	public override void PauseMusic()
	{
		PauseAllMusic();
	}

	public override void PauseMusic(int theSongId)
	{
		if (MusicInterface.USER_MUSIC_PLAYING || !IsValidSongId(theSongId))
		{
			return;
		}
		try
		{
			if (mMo3Songs[theSongId] != null)
			{
				mMo3Songs[theSongId].Pause();
			}
			else if (theSongId == mCurrentSong)
			{
				Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
			}
		}
		catch (Exception)
		{
		}
	}

	public override void PauseAllMusic()
	{
		if (MusicInterface.USER_MUSIC_PLAYING)
		{
			return;
		}
		try
		{
			for (int i = 0; i < mMo3Songs.Length; i++)
			{
				mMo3Songs[i]?.Pause();
			}
			if (mCurrentSong >= 0)
			{
				Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
			}
		}
		catch (Exception)
		{
		}
	}

	public override void ResumeMusic()
	{
		ResumeAllMusic();
	}

	public override void ResumeMusic(int theSongId)
	{
		if (MusicInterface.USER_MUSIC_PLAYING || !IsValidSongId(theSongId))
		{
			return;
		}
		try
		{
			if (mMo3Songs[theSongId] != null)
			{
				mMo3Songs[theSongId].Resume();
			}
			else if (theSongId == mCurrentSong)
			{
				Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
			}
			isStopped = false;
		}
		catch (Exception)
		{
		}
	}

	public override void ResumeAllMusic()
	{
		if (MusicInterface.USER_MUSIC_PLAYING)
		{
			return;
		}
		try
		{
			for (int i = 0; i < mMo3Songs.Length; i++)
			{
				mMo3Songs[i]?.Resume();
			}
			if (mCurrentSong >= 0)
			{
				Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
			}
			isStopped = false;
		}
		catch (Exception)
		{
		}
	}

	public override void UnloadMusic(int theSongId)
	{
		if (IsValidSongId(theSongId))
		{
			try
			{
				StopMusic(theSongId);
			}
			catch (Exception)
			{
			}
			if (mSongs[theSongId] != null)
			{
				mSongs[theSongId].Dispose();
				mSongs[theSongId] = null;
			}
			if (mMo3Songs[theSongId] != null)
			{
				mMo3Songs[theSongId].Dispose();
				mMo3Songs[theSongId] = null;
			}
		}
	}

	public override void UnloadAllMusic()
	{
		try
		{
			StopAllMusic();
		}
		catch (Exception)
		{
		}
		for (int i = 0; i < XNAMusicInterfaceConstants.MAX_SONGS; i++)
		{
			UnloadMusic(i);
		}
	}

	public override bool IsPlaying(int theSongId)
	{
		if (!IsValidSongId(theSongId))
		{
			return false;
		}
		if (mMo3Songs[theSongId] != null)
		{
			return mMo3Songs[theSongId].IsPlaying;
		}
		bool flag = false;
		try
		{
			flag = Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Playing || Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Paused;
		}
		catch (Exception)
		{
		}
		return (mCurrentSong == theSongId) & flag;
	}

	public override bool IsPausing(int theSongId)
	{
		if (!IsValidSongId(theSongId))
		{
			return false;
		}
		if (mMo3Songs[theSongId] != null)
		{
			return mMo3Songs[theSongId].IsPaused;
		}
		if (mCurrentSong == theSongId)
		{
			return Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Paused;
		}
		return false;
	}

	public override bool HasMusic(int theSongId)
	{
		if (IsValidSongId(theSongId))
		{
			if (!(mSongs[theSongId] != null))
			{
				return mMo3Songs[theSongId] != null;
			}
			return true;
		}
		return false;
	}

	public override bool IsStream(int theSongId)
	{
		if (IsValidSongId(theSongId))
		{
			return mSongs[theSongId] != null;
		}
		return false;
	}

	public override uint GetMusicOrder(int theSongId)
	{
		return (uint)(((int?)GetMo3Song(theSongId)?.GetMusicOrder()) ?? (-1));
	}

	public override int GetMusicBPM(int theSongId)
	{
		return GetMo3Song(theSongId)?.GetMusicBPM() ?? 0;
	}

	public override void SetMusicBPM(int theSongId, int theBpm)
	{
		GetMo3Song(theSongId)?.SetMusicBPM(theBpm);
	}

	public override int GetMusicSpeed(int theSongId)
	{
		return GetMo3Song(theSongId)?.GetMusicSpeed() ?? 0;
	}

	public override void SetMusicSpeed(int theSongId, int theSpeed)
	{
		GetMo3Song(theSongId)?.SetMusicSpeed(theSpeed);
	}

	public override void SetMusicTrackVolume(int theSongId, int theTrack, float theVolume)
	{
		GetMo3Song(theSongId)?.SetTrackVolume(theTrack, theVolume);
	}

	public override void SetMusicOrderPosition(int theSongId, int theOrder)
	{
		GetMo3Song(theSongId)?.SetOrderPosition(theOrder);
	}

	public override void SetMusicDecodePosition(int theSongId, int thePosition)
	{
		GetMo3Song(theSongId)?.SetDecodePosition(thePosition);
	}

	public override void SetMusicAmplify(int theSongId, double theAmp)
	{
		GetMo3Song(theSongId)?.SetAmplify(theAmp);
	}

	public override void SetVolume(float theVolume)
	{
		mMasterVolume = ClampVolume(theVolume);
		if (!mHasPlayed)
		{
			mFirstPlayVolume = mMasterVolume;
		}
		else
		{
			ApplyAllVolumes();
		}
	}

	public override float GetVolume()
	{
		return mMasterVolume;
	}

	public override void SetSongVolume(int theSongId, double theVolume)
	{
		if (IsValidSongId(theSongId))
		{
			mSongVolumes[theSongId] = ClampVolume(theVolume);
			ApplySongVolume(theSongId);
		}
	}

	public override void SetSongMaxVolume(int theSongId, double theMaxVolume)
	{
		if (IsValidSongId(theSongId))
		{
			mSongMaxVolumes[theSongId] = ClampVolume(theMaxVolume);
			ApplySongVolume(theSongId);
		}
	}

	public override void Update()
	{
		try
		{
			if (mFadeOut)
			{
				StopAllMusic();
			}
			for (int i = 0; i < XNAMusicInterfaceConstants.MAX_SONGS; i++)
			{
				mMo3Songs[i]?.Update();
			}
		}
		catch (Exception updEx)
		{
			LawnPlatform.PvZActivity.Log("music update error: " + updEx);
		}
	}

	public override int GetFreeMusicId()
	{
		for (int i = 0; i < XNAMusicInterfaceConstants.MAX_SONGS; i++)
		{
			if (mSongs[i] == null && mMo3Songs[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	private bool TryLoadMo3(int theSongId, string theFileName)
	{
#if __IOS__
		// The interactive MO3 music stays silent on iOS; use the converted per-level songs (m4a) instead.
		return false;
#endif
		string text = (Path.GetExtension(theFileName).Equals(".mo3", StringComparison.OrdinalIgnoreCase) ? theFileName : (theFileName + ".mo3"));
		string name = (mContent.RootDirectory + "/" + text).Replace('\\', '/');
		byte[] data;
		try
		{
			using Stream stream = TitleContainer.OpenStream(name);
			using MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			data = memoryStream.ToArray();
		}
		catch (FileNotFoundException)
		{
			return false;
		}
		catch (DirectoryNotFoundException)
		{
			return false;
		}
		catch (ContentLoadException)
		{
			return false;
		}
		try
		{
			mMo3Songs[theSongId]?.Dispose();
			mMo3Songs[theSongId] = new OpenMptMo3MusicTrack(data);
			LawnPlatform.PvZActivity.Log("MO3 loaded: " + text + " (song " + theSongId + ")");
			return true;
		}
		catch (Exception ex4)
		{
			LawnPlatform.PvZActivity.Log("MO3 load failed '" + text + "': " + ex4);
			return false;
		}
	}

	private void ApplyFirstPlayVolume()
	{
		if (!mHasPlayed)
		{
			mHasPlayed = true;
			mMasterVolume = mFirstPlayVolume;
			ApplyAllVolumes();
		}
	}

	private void ApplyAllVolumes()
	{
		for (int i = 0; i < XNAMusicInterfaceConstants.MAX_SONGS; i++)
		{
			ApplySongVolume(i);
		}
	}

	private void ApplySongVolume(int theSongId)
	{
		float num = ((mMo3Songs[theSongId] == null) ? 0.5f : 1f);
		float volume = mMasterVolume * num * mSongVolumes[theSongId] * mSongMaxVolumes[theSongId];
		if (mMo3Songs[theSongId] != null)
		{
			mMo3Songs[theSongId].SetVolume(volume);
		}
		else if (mCurrentSong == theSongId)
		{
			try
			{
				Microsoft.Xna.Framework.Media.MediaPlayer.Volume = volume;
			}
			catch (Exception)
			{
			}
		}
	}

	private void StopAllMo3Music()
	{
		for (int i = 0; i < mMo3Songs.Length; i++)
		{
			mMo3Songs[i]?.Stop();
		}
	}

	private void StopMediaPlayer()
	{
		if (mCurrentSong >= 0)
		{
			Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
			mCurrentSong = -1;
		}
	}

	private OpenMptMo3MusicTrack GetMo3Song(int theSongId)
	{
		if (!IsValidSongId(theSongId))
		{
			return null;
		}
		return mMo3Songs[theSongId];
	}

	private static bool IsValidSongId(int theSongId)
	{
		if (theSongId >= 0)
		{
			return theSongId < XNAMusicInterfaceConstants.MAX_SONGS;
		}
		return false;
	}

	private static float ClampVolume(double volume)
	{
		return (float)Math.Max(0.0, Math.Min(1.0, volume));
	}
}
