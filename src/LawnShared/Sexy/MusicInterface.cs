namespace Sexy;

public abstract class MusicInterface
{
	internal const bool kMusicLoop = true;

	internal const bool kMusicNoLoop = false;

	internal const float kMusicNoFade = 0f;

	public static bool USER_MUSIC_PLAYING;

	public bool isStopped;

	protected float mDefaultFadeInSeconds;

	protected float mDefaultFadeOutSeconds;

	public MusicInterface()
	{
	}

	public virtual void Dispose()
	{
	}

	public virtual void Enable(bool enable)
	{
	}

	public virtual bool LoadMusic(int theSongId, string theFileName)
	{
		return false;
	}

	public virtual void PlayMusic(int theSongId)
	{
		PlayMusic(theSongId, mDefaultFadeOutSeconds, mDefaultFadeInSeconds, loop: true);
	}

	public virtual void PlayMusic(int theSongId, bool loop)
	{
		PlayMusic(theSongId, mDefaultFadeOutSeconds, mDefaultFadeInSeconds, loop);
	}

	public virtual void PlayMusic(int theSongId, float fadeInSeconds)
	{
		PlayMusic(theSongId, mDefaultFadeOutSeconds, fadeInSeconds, loop: true);
	}

	public virtual void PlayMusic(int theSongId, float fadeInSeconds, bool loop)
	{
		PlayMusic(theSongId, mDefaultFadeOutSeconds, fadeInSeconds, loop);
	}

	public virtual void PlayMusic(int theSongId, float fadeOutSeconds, float fadeInSeconds)
	{
		PlayMusic(theSongId, fadeOutSeconds, fadeInSeconds, loop: true);
	}

	public virtual void PlayMusic(int theSongId, float fadeOutSeconds, float fadeInSeconds, bool loop)
	{
		PlayMusic(theSongId, 0, fadeOutSeconds, fadeInSeconds, loop);
	}

	public virtual void PlayMusic(int theSongId, int offset)
	{
		PlayMusic(theSongId, offset, mDefaultFadeOutSeconds, mDefaultFadeInSeconds, loop: true);
	}

	public virtual void PlayMusic(int theSongId, int offset, bool loop)
	{
		PlayMusic(theSongId, offset, mDefaultFadeOutSeconds, mDefaultFadeInSeconds, loop);
	}

	public virtual void PlayMusic(int theSongId, int offset, float fadeInSeconds)
	{
		PlayMusic(theSongId, offset, mDefaultFadeOutSeconds, fadeInSeconds, loop: true);
	}

	public virtual void PlayMusic(int theSongId, int offset, float fadeInSeconds, bool loop)
	{
		PlayMusic(theSongId, offset, mDefaultFadeOutSeconds, fadeInSeconds, loop);
	}

	public virtual void PlayMusic(int theSongId, int offset, float fadeOutSeconds, float fadeInSeconds)
	{
		PlayMusic(theSongId, offset, fadeOutSeconds, fadeInSeconds, loop: true);
	}

	public virtual void PlayMusic(int theSongid, int offset, float fadeOutSeconds, float fadeinSeconds, bool loop)
	{
	}

	public virtual void StopMusic()
	{
		StopMusic(mDefaultFadeOutSeconds);
	}

	public virtual void StopMusic(float fadeOutSeconds)
	{
	}

	public virtual void StopMusic(int theSongId)
	{
		StopMusic();
	}

	public virtual void StopAllMusic()
	{
		StopMusic();
	}

	public virtual void PauseMusic()
	{
	}

	public virtual void PauseMusic(int theSongId)
	{
		PauseMusic();
	}

	public virtual void PauseAllMusic()
	{
		PauseMusic();
	}

	public virtual void ResumeMusic()
	{
	}

	public virtual void ResumeMusic(int theSongId)
	{
		ResumeMusic();
	}

	public virtual void ResumeAllMusic()
	{
		ResumeMusic();
	}

	public virtual void UnloadMusic(int theSongId)
	{
	}

	public virtual void UnloadAllMusic()
	{
	}

	public virtual void SetDefaultFadeOut(float fadeOutSeconds)
	{
		mDefaultFadeOutSeconds = fadeOutSeconds;
	}

	public virtual void SetDefaultFadeIn(float fadeInSeconds)
	{
		mDefaultFadeInSeconds = fadeInSeconds;
	}

	public virtual bool IsPlaying(int theSongId)
	{
		return false;
	}

	public virtual bool IsPausing(int theSongId)
	{
		return false;
	}

	public virtual bool HasMusic(int theSongId)
	{
		return false;
	}

	public virtual bool IsStream(int theSongId)
	{
		return false;
	}

	public virtual uint GetMusicOrder(int theSongId)
	{
		return uint.MaxValue;
	}

	public virtual int GetMusicBPM(int theSongId)
	{
		return 0;
	}

	public virtual void SetMusicBPM(int theSongId, int theBpm)
	{
	}

	public virtual int GetMusicSpeed(int theSongId)
	{
		return 0;
	}

	public virtual void SetMusicSpeed(int theSongId, int theSpeed)
	{
	}

	public virtual void SetMusicTrackVolume(int theSongId, int theTrack, float theVolume)
	{
	}

	public virtual void PlayMusicFromOrder(int theSongId, int theOrderOffset, double theVolume, bool loop)
	{
		PlayMusic(theSongId, loop);
		SetSongVolume(theSongId, theVolume);
	}

	public virtual void SetMusicOrderPosition(int theSongId, int theOrder)
	{
	}

	public virtual void SetMusicDecodePosition(int theSongId, int thePosition)
	{
	}

	public virtual void SetSongVolume(int theSongId, double theVolume)
	{
	}

	public virtual void SetSongMaxVolume(int theSongId, double theMaxVolume)
	{
	}

	public virtual void SetMusicAmplify(int theSongId, double theAmp)
	{
	}

	public virtual void SetVolume(float theVolume)
	{
	}

	public abstract float GetVolume();

	public virtual void Update()
	{
	}

	public virtual int GetFreeMusicId()
	{
		return -1;
	}
}
