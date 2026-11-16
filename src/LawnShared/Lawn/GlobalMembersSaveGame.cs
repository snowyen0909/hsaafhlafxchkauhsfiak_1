using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

internal static class GlobalMembersSaveGame
{
	public class SaveGameContext
	{
		public EnumSaver<SeedType> aSeedTypeSaver = new SeedTypeSaver();

		public EnumSaver<ZombieType> aZombieTypeSaver = new EnumSaver<ZombieType>(ZombieType.Invalid);

		public EnumSaver<BackgroundType> aBackgroundTypeSaver = new EnumSaver<BackgroundType>(BackgroundType.Num1Day);

		public EnumSaver<GridSquareType> aGridSquareTypeSaver = new EnumSaver<GridSquareType>(GridSquareType.None);

		public EnumSaver<AdviceType> aAdviceTypeSaver = new EnumSaver<AdviceType>(AdviceType.None);

		public EnumSaver<ChallengeState> aChallengeStateSaver = new EnumSaver<ChallengeState>(ChallengeState.Normal);

		public EnumSaver<CoinMotion> aCoinMotionSaver = new EnumSaver<CoinMotion>(CoinMotion.FromSky);

		public EnumSaver<GardenType> aGardenTypeSaver = new EnumSaver<GardenType>(GardenType.Main);

		public EnumSaver<PottedPlant.FacingDirection> aFacingDirectionSaver = new EnumSaver<PottedPlant.FacingDirection>(PottedPlant.FacingDirection.Left);

		public EnumSaver<DrawVariation> aDrawVariationSaver = new EnumSaver<DrawVariation>(DrawVariation.Normal);

		public EnumSaver<PottedPlantAge> aPottedPlantAgeSaver = new EnumSaver<PottedPlantAge>(PottedPlantAge.Sprout);

		public EnumSaver<PottedPlantNeed> aPottedPlantNeedSaver = new EnumSaver<PottedPlantNeed>(PottedPlantNeed.None);

		public EnumSaver<CoinType> aCoinTypeSaver = new EnumSaver<CoinType>(CoinType.None);

		public EnumSaver<CursorType> aCursorTypeSaver = new EnumSaver<CursorType>(CursorType.Normal);

		public EnumSaver<GridItemType> aGridItemTypeSaver = new EnumSaver<GridItemType>(GridItemType.None);

		public EnumSaver<GridItemState> aGridItemStateSaver = new EnumSaver<GridItemState>(GridItemState.Normal);

		public EnumSaver<ScaryPotType> aScaryPotTypeSaver = new EnumSaver<ScaryPotType>(ScaryPotType.None);

		public EnumSaver<LawnMowerState> aLawnMowerStateSaver = new EnumSaver<LawnMowerState>(LawnMowerState.Ready);

		public EnumSaver<LawnMowerType> aLawnMowerTypeSaver = new EnumSaver<LawnMowerType>(LawnMowerType.Lawn);

		public EnumSaver<MowerHeight> aMowerHeightSaver = new EnumSaver<MowerHeight>(MowerHeight.Land);

		public EnumSaver<TutorialState> aTutorialStateSaver = new EnumSaver<TutorialState>(TutorialState.Off);

		public EnumSaver<ParticleEffect> aParticleEffectSaver = new EnumSaver<ParticleEffect>(ParticleEffect.None);

		public EnumSaver<ReanimationType> aReanimationTypeSaver = new EnumSaver<ReanimationType>(ReanimationType.None);

		public EnumSaver<ReanimLoopType> aReanimLoopTypeSaver = new EnumSaver<ReanimLoopType>(ReanimLoopType.Loop);

		public EnumSaver<FilterEffectType> aFilterEffectTypeSaver = new EnumSaver<FilterEffectType>(FilterEffectType.None);

		public EnumSaver<EffectType> aEffectTypeSaver = new EnumSaver<EffectType>(EffectType.Other);

		public EnumSaver<PlantState> aPlantStateSaver = new EnumSaver<PlantState>(PlantState.Notready);

		public EnumSaver<PlantOnBungeeState> aPlantOnBungeeStateSaver = new EnumSaver<PlantOnBungeeState>(PlantOnBungeeState.NotOnBungee);

		public EnumSaver<MagnetItemType> aMagnetItemTypeSaver = new EnumSaver<MagnetItemType>(MagnetItemType.None);

		public EnumSaver<ProjectileMotion> aProjectileMotionSaver = new EnumSaver<ProjectileMotion>(ProjectileMotion.Straight);

		public EnumSaver<ProjectileType> aProjectileTypeSaver = new EnumSaver<ProjectileType>(ProjectileType.Pea);

		public EnumSaver<ZombiePhase> aZombiePhaseSaver = new EnumSaver<ZombiePhase>(ZombiePhase.ZombieNormal);

		public EnumSaver<ZombieHeight> aZombieHeightSaver = new EnumSaver<ZombieHeight>(ZombieHeight.ZombieNormal);

		public EnumSaver<HelmType> aHelmTypeSaver = new EnumSaver<HelmType>(HelmType.None);

		public EnumSaver<ShieldType> aShieldTypeSaver = new EnumSaver<ShieldType>(ShieldType.None);

		public EnumSaver<PlantRowType> aPlantRowTypeSaver = new EnumSaver<PlantRowType>(PlantRowType.Normal);

		public EnumSaver<TrailType> aTrailTypeSaver = new EnumSaver<TrailType>(TrailType.None);

		public EnumSaver<DynamicTachieWidget.TouchRewards> aTouchRewardsSaver = new EnumSaver<DynamicTachieWidget.TouchRewards>(DynamicTachieWidget.TouchRewards.Invalid);

		public EnumSaver<MailType> aMailTypeSaver = new EnumSaver<MailType>(MailType.Invalid);

		public BufferNew mBuffer;

		public bool mReading;

		public SaveFileVersion mVersion;

		public int mVersionType;

		private void InitializeSaver<T>(EnumSaver<T> saver) where T : struct, Enum
		{
			if (mReading)
			{
				saver.Sync(this);
				return;
			}
			saver.RegistAllByDefault();
			saver.Sync(this);
		}

		public void SyncBool(ref bool theBool, SaveFileVersion version = SaveFileVersion.Initial, bool defaultValue = false)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theBool = defaultValue;
					return;
				}
				bool flag = mBuffer.ReadBoolean();
				theBool = flag;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteBoolean(theBool);
			}
		}

		public void SyncEnum<T>(ref T theEnum, EnumSaver<T> saver, SaveFileVersion version = SaveFileVersion.Initial, T defaultValue = default(T)) where T : struct, Enum
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theEnum = defaultValue;
					return;
				}
				T val = saver.Convert(mBuffer.ReadInt32LE());
				theEnum = val;
			}
			else if (mVersion >= version)
			{
				int value = saver.Convert(theEnum);
				mBuffer.WriteInt32LE(value);
			}
		}

		public void SyncDateTime(ref DateTime theDateTime, SaveFileVersion version = SaveFileVersion.Initial, DateTime defaultValue = default(DateTime))
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theDateTime = defaultValue;
					return;
				}
				long ticks = mBuffer.ReadInt64LE();
				theDateTime = new DateTime(ticks);
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteInt64LE(theDateTime.Ticks);
			}
		}

		public void SyncTRect(ref TRect theTRect, SaveFileVersion version = SaveFileVersion.Initial, TRect defaultValue = default(TRect))
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theTRect = defaultValue;
					return;
				}
				int mX = mBuffer.ReadInt32LE();
				int mY = mBuffer.ReadInt32LE();
				int mWidth = mBuffer.ReadInt32LE();
				int mHeight = mBuffer.ReadInt32LE();
				theTRect.mX = mX;
				theTRect.mY = mY;
				theTRect.mWidth = mWidth;
				theTRect.mHeight = mHeight;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteInt32LE(theTRect.mX);
				mBuffer.WriteInt32LE(theTRect.mY);
				mBuffer.WriteInt32LE(theTRect.mWidth);
				mBuffer.WriteInt32LE(theTRect.mHeight);
			}
		}

		public void SyncSexyColor(ref SexyColor theColor, SaveFileVersion version = SaveFileVersion.Initial, SexyColor defaultValue = default(SexyColor))
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theColor = defaultValue;
					return;
				}
				int mRed = mBuffer.ReadInt32LE();
				int mGreen = mBuffer.ReadInt32LE();
				int mBlue = mBuffer.ReadInt32LE();
				int mAlpha = mBuffer.ReadInt32LE();
				theColor.mRed = mRed;
				theColor.mGreen = mGreen;
				theColor.mBlue = mBlue;
				theColor.mAlpha = mAlpha;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteInt32LE(theColor.mRed);
				mBuffer.WriteInt32LE(theColor.mGreen);
				mBuffer.WriteInt32LE(theColor.mBlue);
				mBuffer.WriteInt32LE(theColor.mAlpha);
			}
		}

		public void SyncSexyTransform2D(ref SexyTransform2D theTransform, SaveFileVersion version = SaveFileVersion.Initial, SexyTransform2D defaultValue = default(SexyTransform2D))
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theTransform = defaultValue;
					return;
				}
				theTransform = new SexyTransform2D(new Matrix
				{
					M11 = mBuffer.ReadFloat32LE(),
					M21 = mBuffer.ReadFloat32LE(),
					M31 = mBuffer.ReadFloat32LE(),
					M41 = mBuffer.ReadFloat32LE(),
					M12 = mBuffer.ReadFloat32LE(),
					M22 = mBuffer.ReadFloat32LE(),
					M32 = mBuffer.ReadFloat32LE(),
					M42 = mBuffer.ReadFloat32LE(),
					M13 = mBuffer.ReadFloat32LE(),
					M23 = mBuffer.ReadFloat32LE(),
					M33 = mBuffer.ReadFloat32LE(),
					M43 = mBuffer.ReadFloat32LE(),
					M14 = mBuffer.ReadFloat32LE(),
					M24 = mBuffer.ReadFloat32LE(),
					M34 = mBuffer.ReadFloat32LE(),
					M44 = mBuffer.ReadFloat32LE()
				});
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M11);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M21);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M31);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M41);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M12);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M22);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M32);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M42);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M13);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M23);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M33);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M43);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M14);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M24);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M34);
				mBuffer.WriteFloat32LE(theTransform.mMatrix.M44);
			}
		}

		public void SyncMatrix(ref Matrix matrix, SaveFileVersion version = SaveFileVersion.Initial, Matrix defaultValue = default(Matrix))
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					matrix = defaultValue;
					return;
				}
				float m = mBuffer.ReadFloat32LE();
				float m2 = mBuffer.ReadFloat32LE();
				float m3 = mBuffer.ReadFloat32LE();
				float m4 = mBuffer.ReadFloat32LE();
				float m5 = mBuffer.ReadFloat32LE();
				float m6 = mBuffer.ReadFloat32LE();
				float m7 = mBuffer.ReadFloat32LE();
				float m8 = mBuffer.ReadFloat32LE();
				float m9 = mBuffer.ReadFloat32LE();
				float m10 = mBuffer.ReadFloat32LE();
				float m11 = mBuffer.ReadFloat32LE();
				float m12 = mBuffer.ReadFloat32LE();
				float m13 = mBuffer.ReadFloat32LE();
				float m14 = mBuffer.ReadFloat32LE();
				float m15 = mBuffer.ReadFloat32LE();
				float m16 = mBuffer.ReadFloat32LE();
				matrix.M11 = m;
				matrix.M21 = m2;
				matrix.M31 = m3;
				matrix.M41 = m4;
				matrix.M12 = m5;
				matrix.M22 = m6;
				matrix.M32 = m7;
				matrix.M42 = m8;
				matrix.M13 = m9;
				matrix.M23 = m10;
				matrix.M33 = m11;
				matrix.M43 = m12;
				matrix.M14 = m13;
				matrix.M24 = m14;
				matrix.M34 = m15;
				matrix.M44 = m16;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteFloat32LE(matrix.M11);
				mBuffer.WriteFloat32LE(matrix.M21);
				mBuffer.WriteFloat32LE(matrix.M31);
				mBuffer.WriteFloat32LE(matrix.M41);
				mBuffer.WriteFloat32LE(matrix.M12);
				mBuffer.WriteFloat32LE(matrix.M22);
				mBuffer.WriteFloat32LE(matrix.M32);
				mBuffer.WriteFloat32LE(matrix.M42);
				mBuffer.WriteFloat32LE(matrix.M13);
				mBuffer.WriteFloat32LE(matrix.M23);
				mBuffer.WriteFloat32LE(matrix.M33);
				mBuffer.WriteFloat32LE(matrix.M43);
				mBuffer.WriteFloat32LE(matrix.M14);
				mBuffer.WriteFloat32LE(matrix.M24);
				mBuffer.WriteFloat32LE(matrix.M34);
				mBuffer.WriteFloat32LE(matrix.M44);
			}
		}

		public void SyncSexyVector2(ref SexyVector2 theVector, SaveFileVersion version = SaveFileVersion.Initial, SexyVector2 defaultValue = default(SexyVector2))
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theVector = defaultValue;
					return;
				}
				float x = mBuffer.ReadFloat32LE();
				float y = mBuffer.ReadFloat32LE();
				theVector.x = x;
				theVector.y = y;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteFloat32LE(theVector.x);
				mBuffer.WriteFloat32LE(theVector.y);
			}
		}

		public void SyncListLength(ref int theInt, int theMaxCount = 32767, SaveFileVersion version = SaveFileVersion.Initial, int defaultValue = 0)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theInt = defaultValue;
					return;
				}
				int num = mBuffer.ReadInt32LE();
				if (num != 1437666)
				{
					throw new Exception("List count magic mismatches!");
				}
				int num2 = mBuffer.ReadInt32LE();
				if (num2 > theMaxCount)
				{
					throw new Exception($"List count {num2} can not be bigger than {theMaxCount}");
				}
				num = mBuffer.ReadInt32LE();
				if (num != 1437666)
				{
					throw new Exception("List count magic mismatches!");
				}
				theInt = num2;
			}
			else if (mVersion >= version)
			{
				if (theInt > theMaxCount)
				{
					throw new Exception($"List count {theInt} can not be bigger than {theMaxCount}");
				}
				mBuffer.WriteInt32LE(1437666);
				mBuffer.WriteInt32LE(theInt);
				mBuffer.WriteInt32LE(1437666);
			}
		}

		public void SyncFloat(ref float theFloat, SaveFileVersion version = SaveFileVersion.Initial, float defaultValue = 0f)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theFloat = defaultValue;
					return;
				}
				float num = mBuffer.ReadFloat32LE();
				theFloat = num;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteFloat32LE(theFloat);
			}
		}

		public void SyncDouble(ref double theDouble, SaveFileVersion version = SaveFileVersion.Initial, double defaultValue = 0.0)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theDouble = defaultValue;
					return;
				}
				double num = mBuffer.ReadFloat64LE();
				theDouble = num;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteFloat64LE(theDouble);
			}
		}

		public void SyncString(ref string theStr, SaveFileVersion version = SaveFileVersion.Initial, string defaultValue = "")
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theStr = defaultValue;
					return;
				}
				string text = mBuffer.ReadString();
				theStr = text;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteString(theStr);
			}
		}

		public void SyncByte(ref byte theByte, SaveFileVersion version = SaveFileVersion.Initial, byte defaultValue = 0)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theByte = defaultValue;
					return;
				}
				byte b = mBuffer.ReadUInt8();
				theByte = b;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteUInt8(theByte);
			}
		}

		public void SyncShort(ref short theShort, SaveFileVersion version = SaveFileVersion.Initial, short defaultValue = 0)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theShort = defaultValue;
					return;
				}
				short num = mBuffer.ReadInt16LE();
				theShort = num;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteInt16LE(theShort);
			}
		}

		public void SyncInt(ref int theInt, SaveFileVersion version = SaveFileVersion.Initial, int defaultValue = 0)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theInt = defaultValue;
					return;
				}
				int num = mBuffer.ReadInt32LE();
				theInt = num;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteInt32LE(theInt);
			}
		}

		public void SyncSaveFileVersion(ref SaveFileVersion theInt, SaveFileVersion version = SaveFileVersion.Initial, SaveFileVersion defaultValue = SaveFileVersion.Initial)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theInt = defaultValue;
					return;
				}
				int num = mBuffer.ReadInt32LE();
				theInt = (SaveFileVersion)num;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteInt32LE((int)theInt);
			}
		}

		public void SyncUint(ref uint theUint, SaveFileVersion version = SaveFileVersion.Initial, uint defaultValue = 0u)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theUint = defaultValue;
					return;
				}
				uint num = mBuffer.ReadUInt32LE();
				theUint = num;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteUInt32LE(theUint);
			}
		}

		public void SyncLong(ref long theLong, SaveFileVersion version = SaveFileVersion.Initial, long defaultValue = 0L)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theLong = defaultValue;
					return;
				}
				long num = mBuffer.ReadInt64LE();
				theLong = num;
			}
			else if (mVersion >= version)
			{
				mBuffer.WriteInt64LE(theLong);
			}
		}

		public void SyncImage(ref Image theImage, SaveFileVersion version = SaveFileVersion.Initial, Image defaultValue = null)
		{
			if (mReading)
			{
				if (mVersion < version)
				{
					theImage = defaultValue;
					return;
				}
				byte b = mBuffer.ReadUInt8();
				string value = mBuffer.ReadString();
				int theId;
				switch (b)
				{
				case 2:
					theImage = null;
					return;
				case 1:
					theId = (int)Enum.Parse<AtlasResources.AtlasImageId>(value);
					break;
				default:
					theId = (int)Enum.Parse<Resources.ResourceId>(value);
					break;
				}
				Image imageInAtlasById = AtlasResources.GetImageInAtlasById(theId);
				theImage = imageInAtlasById;
			}
			else if (mVersion >= version)
			{
				int idByImageInAtlas = AtlasResources.GetIdByImageInAtlas(theImage);
				byte value2;
				string value3;
				if (theImage == null)
				{
					value2 = 2;
					value3 = null;
				}
				else if (idByImageInAtlas >= 10000)
				{
					value2 = 1;
					value3 = Enum.GetName((AtlasResources.AtlasImageId)idByImageInAtlas);
				}
				else
				{
					value2 = 0;
					value3 = Enum.GetName((Resources.ResourceId)idByImageInAtlas);
				}
				mBuffer.WriteUInt8(value2);
				mBuffer.WriteString(value3);
			}
		}

		public void InitializePlayerEnum()
		{
			if (mVersion >= SaveFileVersion.Initial)
			{
				InitializeSaver(aSeedTypeSaver);
				InitializeSaver(aGardenTypeSaver);
				InitializeSaver(aFacingDirectionSaver);
				InitializeSaver(aDrawVariationSaver);
				InitializeSaver(aPottedPlantAgeSaver);
				InitializeSaver(aPottedPlantNeedSaver);
				if (mVersion >= SaveFileVersion.FumeshroomDynamic)
				{
					InitializeSaver(aTouchRewardsSaver);
				}
				if (mVersion >= SaveFileVersion.MailboxState)
				{
					InitializeSaver(aCoinTypeSaver);
					InitializeSaver(aMailTypeSaver);
				}
			}
		}

		public void InitializeGameEnum()
		{
			if (mVersion >= SaveFileVersion.Initial)
			{
				InitializeSaver(aSeedTypeSaver);
				InitializeSaver(aZombieTypeSaver);
				InitializeSaver(aBackgroundTypeSaver);
				InitializeSaver(aGridSquareTypeSaver);
				InitializeSaver(aAdviceTypeSaver);
				InitializeSaver(aChallengeStateSaver);
				InitializeSaver(aCoinMotionSaver);
				InitializeSaver(aGardenTypeSaver);
				InitializeSaver(aFacingDirectionSaver);
				InitializeSaver(aDrawVariationSaver);
				InitializeSaver(aPottedPlantAgeSaver);
				InitializeSaver(aPottedPlantNeedSaver);
				InitializeSaver(aCoinTypeSaver);
				InitializeSaver(aCursorTypeSaver);
				InitializeSaver(aGridItemTypeSaver);
				InitializeSaver(aGridItemStateSaver);
				InitializeSaver(aScaryPotTypeSaver);
				InitializeSaver(aLawnMowerStateSaver);
				InitializeSaver(aLawnMowerTypeSaver);
				InitializeSaver(aMowerHeightSaver);
				InitializeSaver(aTutorialStateSaver);
				InitializeSaver(aParticleEffectSaver);
				InitializeSaver(aReanimationTypeSaver);
				InitializeSaver(aReanimLoopTypeSaver);
				InitializeSaver(aFilterEffectTypeSaver);
				InitializeSaver(aEffectTypeSaver);
				InitializeSaver(aPlantStateSaver);
				InitializeSaver(aPlantOnBungeeStateSaver);
				InitializeSaver(aMagnetItemTypeSaver);
				InitializeSaver(aProjectileMotionSaver);
				InitializeSaver(aProjectileTypeSaver);
				InitializeSaver(aZombiePhaseSaver);
				InitializeSaver(aZombieHeightSaver);
				InitializeSaver(aHelmTypeSaver);
				InitializeSaver(aShieldTypeSaver);
				InitializeSaver(aPlantRowTypeSaver);
				InitializeSaver(aTrailTypeSaver);
			}
		}
	}

	public class EnumSaver<T> where T : struct, Enum
	{
		private const int DEFAULT_CAPACITY = 128;

		private readonly Dictionary<T, int> _enumToIntDic = new Dictionary<T, int>(128);

		private readonly List<T> _intToEnumDic = new List<T>(128);

		private readonly T _defaultValueEnum;

		public EnumSaver(T defaultValueEnum)
		{
			_defaultValueEnum = defaultValueEnum;
		}

		public void Sync(SaveGameContext theContext)
		{
			if (theContext.mReading)
			{
				int num = theContext.mBuffer.ReadInt32LE();
				for (int i = 0; i < num; i++)
				{
					string value = theContext.mBuffer.ReadString();
					T e;
					try
					{
						e = ConvertName(value);
					}
					catch (Exception)
					{
						e = _defaultValueEnum;
					}
					Regist(e, force: true);
				}
			}
			else
			{
				theContext.mBuffer.WriteInt32LE(_intToEnumDic.Count);
				for (int j = 0; j < _intToEnumDic.Count; j++)
				{
					T e2 = _intToEnumDic[j];
					string value2 = ConvertName(e2);
					theContext.mBuffer.WriteString(value2);
				}
			}
		}

		public virtual string ConvertName(T e)
		{
			return Enum.GetName(e);
		}

		public virtual T ConvertName(string value)
		{
			return Enum.Parse<T>(value);
		}

		public int Convert(T e)
		{
			if (_enumToIntDic.TryGetValue(e, out var value))
			{
				return value;
			}
			return -1;
		}

		public T Convert(int value)
		{
			if (value >= 0 && value < _intToEnumDic.Count)
			{
				return _intToEnumDic[value];
			}
			return _defaultValueEnum;
		}

		public bool Regist(T e, bool force)
		{
			if (!force && _enumToIntDic.ContainsKey(e))
			{
				return false;
			}
			_enumToIntDic.TryAdd(e, _intToEnumDic.Count);
			_intToEnumDic.Add(e);
			return true;
		}

		public virtual void RegistAllByDefault()
		{
			T[] values = Enum.GetValues<T>();
			for (int i = 0; i < values.Length; i++)
			{
				Regist(values[i], force: false);
			}
		}
	}

	internal class SeedTypeSaver : EnumSaver<SeedType>
	{
		public SeedTypeSaver()
			: base(SeedType.None)
		{
		}

		public override string ConvertName(SeedType e)
		{
			if (e == SeedType.None)
			{
				return base.ConvertName(e);
			}
			if ((e & SeedType.MindControlledCardFlag) != 0)
			{
				ZombieType value = (ZombieType)(e & (SeedType)(-1073741825));
				return "-" + Enum.GetName(value);
			}
			if ((e & SeedType.ZombieCardFlag) != 0)
			{
				ZombieType value2 = (ZombieType)(e & (SeedType)(-536870913));
				return "+" + Enum.GetName(value2);
			}
			return base.ConvertName(e);
		}

		public override SeedType ConvertName(string value)
		{
			if (value.StartsWith('-'))
			{
				ReadOnlySpan<char> readOnlySpan = value;
				ref ReadOnlySpan<char> reference = ref readOnlySpan;
				ZombieType zombieType = Enum.Parse<ZombieType>(reference.Slice(1, reference.Length - 1));
				return (SeedType)(zombieType | (ZombieType)0x40000000);
			}
			if (value.StartsWith('+'))
			{
				ReadOnlySpan<char> readOnlySpan = value;
				ref ReadOnlySpan<char> reference = ref readOnlySpan;
				ZombieType zombieType2 = Enum.Parse<ZombieType>(reference.Slice(1, reference.Length - 1));
				return (SeedType)(zombieType2 | (ZombieType)0x20000000);
			}
			return base.ConvertName(value);
		}

		public override void RegistAllByDefault()
		{
			base.RegistAllByDefault();
			ZombieType[] values = Enum.GetValues<ZombieType>();
			for (int i = 0; i < values.Length; i++)
			{
				Regist((SeedType)(values[i] | (ZombieType)0x20000000), force: false);
			}
			for (int j = 0; j < values.Length; j++)
			{
				Regist((SeedType)(values[j] | (ZombieType)0x40000000), force: false);
			}
		}
	}

	public const SaveFileVersion SAVE_FILE_GAME_VERSION = SaveFileVersion.MailboxState;

	public const int SAVE_FILE_GAME_MAGIC_NUMBER = 14372508;

	public const int SAVE_FILE_PLAYER_MAGIC_NUMBER = 99816666;

	public const int SAVE_FILE_WP_RAW = 20208888;

	public const int SAVE_FILE_CHECK = 777;

	public static bool LawnCreateCrashBackup(string theFilePath)
	{
		BufferNew bufferNew = GlobalStaticVars.gSexyAppBase.ReadBufferNewFromFile(theFilePath, dontWriteToDemo: false);
		if (bufferNew == null)
		{
			return false;
		}
		return GlobalStaticVars.gSexyAppBase.WriteBufferNewToFile(theFilePath + ".crash", bufferNew);
	}

	public static bool LawnLoadGame(Board theBoard, string theFilePath)
	{
		BufferNew bufferNew = GlobalStaticVars.gSexyAppBase.ReadBufferNewFromFile(theFilePath, dontWriteToDemo: false);
		if (bufferNew == null)
		{
			return false;
		}
		SaveGameContext saveGameContext = new SaveGameContext();
		saveGameContext.mBuffer = bufferNew;
		saveGameContext.mReading = true;
		try
		{
			int theInt = 0;
			saveGameContext.SyncInt(ref theInt);
			saveGameContext.SyncSaveFileVersion(ref saveGameContext.mVersion);
			saveGameContext.SyncInt(ref saveGameContext.mVersionType);
			if (theInt != 14372508)
			{
				return false;
			}
			if (saveGameContext.mVersionType != 20208888 && saveGameContext.mVersionType != 20208888)
			{
				return false;
			}
			saveGameContext.InitializeGameEnum();
			ClearObjGame(theBoard);
			SyncGame(saveGameContext, theBoard);
			SyncObjGame(theBoard, saveGameContext);
			theBoard.mApp.mSoundManager.StopAllSounds();
			theBoard.mApp.mGameScene = GameScenes.Playing;
			return true;
		}
		catch (Exception)
		{
			ClearObjGame(theBoard);
			return false;
		}
	}

	public static bool LawnSaveGame(Board theBoard, string theFilePath)
	{
		LawnApp mApp = theBoard.mApp;
		SaveGameContext saveGameContext = new SaveGameContext();
		saveGameContext.mBuffer = new BufferNew();
		saveGameContext.mReading = false;
		int theInt = 14372508;
		saveGameContext.mVersion = SaveFileVersion.MailboxState;
		saveGameContext.mVersionType = 20208888;
		try
		{
			saveGameContext.SyncInt(ref theInt);
			saveGameContext.SyncSaveFileVersion(ref saveGameContext.mVersion);
			saveGameContext.SyncInt(ref saveGameContext.mVersionType);
			saveGameContext.InitializeGameEnum();
			SyncObjGame(theBoard, saveGameContext);
			SyncGame(saveGameContext, theBoard);
			return mApp.WriteBufferNewToFile(theFilePath, saveGameContext.mBuffer);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool LawnLoadGameOld(Board mBoard, string theFilePath)
	{
		try
		{
			Sexy.Buffer theBuffer = new Sexy.Buffer();
			GlobalStaticVars.gSexyAppBase.ReadBufferFromFile(theFilePath, ref theBuffer, dontWriteToDemo: false);
			mBoard.LoadFromFile(theBuffer);
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			EffectSystem.gEffectSystem.EffectSystemFreeAll();
			return false;
		}
		return true;
	}

	public static void ClearObjGame(Board theBoard)
	{
		theBoard.mApp.mEffectSystem.ClearObj();
		theBoard.ClearObj();
	}

	public static void SyncGame(SaveGameContext theContext, Board theBoard)
	{
		theBoard.mApp.mEffectSystem.Sync(theContext);
		theBoard.Sync(theContext);
		int theInt = 777;
		theContext.SyncInt(ref theInt);
		if (theInt != 777)
		{
			throw new Exception("Check error");
		}
	}

	private static void SyncObjGame(Board theBoard, SaveGameContext theContext)
	{
		theBoard.mApp.mEffectSystem.SyncObj(theContext);
		theBoard.SyncObj(theContext);
	}
}
