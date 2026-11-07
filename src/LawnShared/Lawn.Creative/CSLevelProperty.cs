using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Graphics;
using Sexy;

namespace Lawn.Creative;

public class CSLevelProperty : IComponent, IDisposable
{
	public List<ZombieType> mAllowedZombies;

	public BackgroundType mBackground;

	public string mChineseName;

	public bool mEasyUpgrade;

	public string mEnglishName;

	public int mIcon;

	public Image mIconImage;

	public int mInitPlantColumn;

	public int mMaxPlainZombiesPerFlag;

	public MusicTune? mMusicType;

	public string mName;

	public int mNumWaves;

	public bool mSpawnFlagZombie;

	public int mStartingSun;

	public int mStartingTime;

	public int mStartingWave;

	public int mWavesPerFlag;

	public int Version { get; init; }

	public CSLevelProperty()
	{
		mName = null;
		mChineseName = null;
		mEnglishName = null;
		mIcon = -1;
		mIconImage = null;
		mBackground = BackgroundType.Num1Day;
		mInitPlantColumn = 0;
		mEasyUpgrade = false;
		mNumWaves = 40;
		mWavesPerFlag = 10;
		mStartingSun = 50;
		mStartingWave = 0;
		mStartingTime = 1800;
		mAllowedZombies = null;
		mSpawnFlagZombie = true;
		mMaxPlainZombiesPerFlag = 8;
		mMusicType = null;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Name", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mName = value.GetString();
		}
		if (root.TryGetProperty("ChineseName", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mChineseName = value2.GetString();
		}
		if (root.TryGetProperty("EnglishName", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mEnglishName = value3.GetString();
		}
		if (root.TryGetProperty("Icon", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			mIcon = value4.GetInt32();
		}
		if (root.TryGetProperty("IconImage", out var value5) && value5.ValueKind != JsonValueKind.Null)
		{
			byte[] bytesFromBase = value5.GetBytesFromBase64();
			Texture2D theTexture;
			using (Stream stream = new MemoryStream(bytesFromBase))
			{
				theTexture = Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
			}
			mIconImage = new Image(theTexture);
		}
		if (root.TryGetProperty("Background", out var value6) && value6.ValueKind != JsonValueKind.Null)
		{
			mBackground = BackgroundTypeConvertor.ToEnum(value6.GetInt32(), Version);
		}
		if (root.TryGetProperty("InitPlantColumn", out var value7) && value7.ValueKind != JsonValueKind.Null)
		{
			mInitPlantColumn = value7.GetInt32();
		}
		if (root.TryGetProperty("EasyUpgrade", out var value8) && value8.ValueKind != JsonValueKind.Null)
		{
			mEasyUpgrade = value8.GetBoolean();
		}
		if (root.TryGetProperty("NumWaves", out var value9) && value9.ValueKind != JsonValueKind.Null)
		{
			mNumWaves = value9.GetInt32();
		}
		if (root.TryGetProperty("WavesPerFlag", out var value10) && value10.ValueKind != JsonValueKind.Null)
		{
			mWavesPerFlag = value10.GetInt32();
		}
		if (root.TryGetProperty("StartingWave", out var value11) && value11.ValueKind != JsonValueKind.Null)
		{
			mStartingWave = value11.GetInt32();
		}
		if (root.TryGetProperty("StartingSun", out var value12) && value12.ValueKind != JsonValueKind.Null)
		{
			mStartingSun = value12.GetInt32();
		}
		if (root.TryGetProperty("StartingTime", out var value13) && value13.ValueKind != JsonValueKind.Null)
		{
			mStartingTime = value13.GetInt32();
		}
		if (root.TryGetProperty("AllowedZombies", out var value14) && value14.ValueKind != JsonValueKind.Null)
		{
			mAllowedZombies = new List<ZombieType>();
			IComponent.PrepareOneIntListFromString(value14, mAllowedZombies, (JsonElement item) => ZombieTypeConvertor.ToEnum(item.GetInt32(), Version));
		}
		if (root.TryGetProperty("SpawnFlagZombie", out var value15) && value15.ValueKind != JsonValueKind.Null)
		{
			mSpawnFlagZombie = value15.GetBoolean();
		}
		if (root.TryGetProperty("MaxPlainZombiesPerFlag", out var value16) && value16.ValueKind != JsonValueKind.Null)
		{
			mMaxPlainZombiesPerFlag = value16.GetInt32();
		}
		if (root.TryGetProperty("MusicType", out var value17) && value17.ValueKind != JsonValueKind.Null)
		{
			int @int = value17.GetInt32();
			if (@int < 0)
			{
				mMusicType = null;
			}
			else
			{
				mMusicType = MusicTuneConvertor.ToEnum(@int, Version);
			}
		}
	}

	public void Check(CreativeLevel level)
	{
		if (mInitPlantColumn < 0 || mInitPlantColumn > Constants.GRIDSIZEX)
		{
			throw new Exception($"InitPlantColumn({mInitPlantColumn}) out of range [0,{Constants.GRIDSIZEX}]");
		}
		if (mNumWaves < 0)
		{
			throw new Exception($"NumWaves({mNumWaves}) can not be smaller than 0");
		}
		if (mWavesPerFlag < 0)
		{
			throw new Exception($"WavesPerFlag({mWavesPerFlag}) can not be smaller than 0");
		}
		if (mStartingWave < 0)
		{
			throw new Exception($"StartingWave({mStartingWave}) can not be smaller than 0");
		}
		if (mStartingTime < 0)
		{
			throw new Exception($"StartingTime({mStartingTime}) can not be smaller than 0");
		}
	}

	public void Dispose()
	{
		if (mIconImage != null)
		{
			mIconImage.Dispose();
			mIconImage = null;
		}
	}
}
