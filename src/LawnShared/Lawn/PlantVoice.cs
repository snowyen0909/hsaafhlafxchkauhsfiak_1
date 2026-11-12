using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NVorbis;
using Sexy;

namespace Lawn;

public static class PlantVoice
{
	public enum VoiceType
	{
		Plant1,
		Plant2,
		Garden1,
		Garden2,
		Garden2_2,
		Sleep1,
		Sleep2,
		Power,
		Attack,
		SkillCancel,
		Mending,
		Die,
		NumVoiceTypes,
		Special_Title_En,
		Special_Title_Cn,
		Special_Title_Jp,
		Special_Garlic_Mending,
		Special_Ninja_Appear,
		Special_Ninja_Die,
		Special_Squash_Attack,
		Special_Squash_Water,
		Special_Squash_Caidan,
		Special_Talisman_Wuguiyuncai,
		Special_Talisman_Tianshuihengliu,
		Special_Talisman_Fengyin,
		Special_Zorrose_Attack1,
		Special_Zorrose_Attack2,
		Special_Zorrose_Enable,
		Special_Zorrose_Disable,
		Special_Gatlingpea_Attack1,
		Special_Gatlingpea_Attack2,
		Special_Fumeshroom_Touch_Enter_1,
		Special_Fumeshroom_Touch_Enter_2,
		Special_Fumeshroom_Touch1,
		Special_Fumeshroom_Touch2_1,
		Special_Fumeshroom_Touch2_2,
		Special_Fumeshroom_Touch3_1,
		Special_Fumeshroom_Touch3_2,
		NumSpecialTypes
	}

	private static uint?[,] cachedSoundEffects = new uint?[(int)SeedType.SeedTypeCount, 12];

	private static uint?[] cachedSpecialSoundEffects = new uint?[38];

	public static void Initialize()
	{
		for (SeedType seedType = SeedType.Peashooter; seedType < SeedType.SeedTypeCount; seedType++)
		{
			Prepare(seedType, VoiceType.Plant1);
			Prepare(seedType, VoiceType.Plant2);
			Prepare(seedType, VoiceType.Garden1);
			Prepare(seedType, VoiceType.Garden2);
		}
	}

	public static void Play(SeedType seedType, SeedType imitatorType, VoiceType purpose)
	{
		SoundManager mSoundManager = GlobalStaticVars.gSexyAppBase.mSoundManager;
		if (purpose > VoiceType.NumVoiceTypes)
		{
			uint? num = cachedSpecialSoundEffects[(int)purpose];
			uint? num2;
			if (num.HasValue)
			{
				num2 = cachedSpecialSoundEffects[(int)purpose].Value;
			}
			else
			{
				num2 = Prepare(seedType, purpose, isSpecial: true);
				if (!num2.HasValue)
				{
					return;
				}
			}
			mSoundManager.GetSoundInstance(num2.Value)?.Play(looping: false);
		}
		else
		{
			if (seedType < SeedType.Peashooter || (int)seedType >= cachedSoundEffects.Length)
			{
				return;
			}
			uint? num = cachedSoundEffects[(int)seedType, (int)purpose];
			uint? num2;
			if (num.HasValue)
			{
				num2 = cachedSoundEffects[(int)seedType, (int)purpose].Value;
			}
			else
			{
				num2 = Prepare(seedType, purpose);
				if (!num2.HasValue)
				{
					return;
				}
			}
			mSoundManager.GetSoundInstance(num2.Value)?.Play(looping: false);
		}
	}

	public static uint? Prepare(SeedType seedType, VoiceType purpose, bool isSpecial = false)
	{
		if (!(GlobalStaticVars.gSexyAppBase.mSoundManager is XNASoundManager))
		{
			throw new NotImplementedException("Only XNASoundManager is supported");
		}
		XNASoundManager xNASoundManager = GlobalStaticVars.gSexyAppBase.mSoundManager as XNASoundManager;
		string name = ((!isSpecial) ? Path.Join("Content\\voices", seedType.ToString().ToLower() + "-" + purpose.ToString().ToLower().Replace("_", "-") + ".ogg") : Path.Join("Content\\voices", purpose.ToString().ToLower().Replace("special_", "")
			.Replace("_", "-") + ".ogg"));
		Stream stream;
		try
		{
			stream = TitleContainer.OpenStream(name);
		}
		catch (FileNotFoundException)
		{
			return null;
		}
		int freeSoundId = xNASoundManager.GetFreeSoundId();
		if (freeSoundId < 0)
		{
			throw new InvalidOperationException("No free sound id");
		}
		xNASoundManager.LoadSound((uint)freeSoundId, LoadSound(stream));
		if (isSpecial)
		{
			return cachedSpecialSoundEffects[(int)purpose] = (uint)freeSoundId;
		}
		return cachedSoundEffects[(int)seedType, (int)purpose] = (uint)freeSoundId;
	}

	private static SoundEffect LoadSound(Stream stream)
	{
		int channels;
		int sampleRate;
		short[] array2;
		int length;
		using (VorbisReader vorbisReader = new VorbisReader(stream))
		{
			channels = vorbisReader.Channels;
			sampleRate = vorbisReader.SampleRate;
			float[] array = new float[channels * sampleRate];
			array2 = new short[1048576];
			TimeSpan zero = TimeSpan.Zero;
			int num = 0;
			if (Vector.IsHardwareAccelerated)
			{
				int num2;
				while ((num2 = vorbisReader.ReadSamples(array, 0, array.Length)) > 0)
				{
					if (num + num2 >= 1048576)
					{
						Array.Resize(ref array2, (int)((double)(array2.Length + num2) * 1.5));
					}
					int count = Vector<float>.Count;
					int num3 = num2 % (count * 2);
					for (int i = 0; i < num2 - num3; i += count * 2)
					{
						Vector<float> value = new Vector<float>(array, i);
						Vector<float> value2 = new Vector<float>(array, i + count);
						value *= 32768f;
						value2 *= 32768f;
						Vector<int> low = Vector.ConvertToInt32(value);
						Vector<int> high = Vector.ConvertToInt32(value2);
						Vector.Narrow(low, high).CopyTo(array2, num + i);
					}
					for (int j = num2 - num3; j < num2; j++)
					{
						array2[num + j] = (short)(array[j] * 32768f);
					}
					num += num2;
				}
			}
			else
			{
				int num2;
				while ((num2 = vorbisReader.ReadSamples(array, 0, array.Length)) > 0)
				{
					if (num + num2 >= 1048576)
					{
						Array.Resize(ref array2, (int)((double)(array2.Length + num2) * 1.5));
					}
					for (int k = 0; k < num2; k++)
					{
						array2[num + k] = (short)(array[k] * 32768f);
					}
					num += num2;
				}
			}
			length = num;
		}
		return new SoundEffect(MemoryMarshal.AsBytes(array2.AsSpan(0, length)).ToArray(), sampleRate, (channels != 2) ? AudioChannels.Mono : AudioChannels.Stereo);
	}
}
