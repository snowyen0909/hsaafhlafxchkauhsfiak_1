using System;
using System.Collections.Generic;
using System.Text.Json;
using Sexy;

namespace Lawn;

internal static class LocalMailbox
{
	private static readonly (int mLevel, MailType mType)[] mLetters = new (int, MailType)[7]
	{
		(0, MailType.HelpZombieNote),
		(10, MailType.ZombieNote1),
		(20, MailType.ZombieNote2),
		(30, MailType.ZombieNote3),
		(40, MailType.ZombieNote4),
		(50, MailType.FinalZombieNote),
		(51, MailType.CreditsZombieNote)
	};

	internal static void DeliverLocalMail(PlayerInfo thePlayerInfo)
	{
		(int, MailType)[] array = mLetters;
		for (int i = 0; i < array.Length; i++)
		{
			var (num, aType) = array[i];
			if ((thePlayerInfo.mFinishedAdventure > 0 || thePlayerInfo.GetLevel() >= num) && !thePlayerInfo.mMail.Exists((LocalMail aMail) => aMail.mType == aType))
			{
				thePlayerInfo.mMail.Add(new LocalMail(aType, default, false));
			}
		}
	}

	internal static void DeliverAdventureReward(PlayerInfo thePlayerInfo)
	{
		thePlayerInfo.mMail.Add(new LocalMail(MailType.Text, JsonSerializer.SerializeToElement(new
		{
			TitleKey = "[MAIL_ADVENTURE_GIFT_TITLE]",
			BodyKey = "[MAIL_ADVENTURE_GIFT_BODY]"
		}), true, (CoinType.Award10PottedPlants, 1)));
	}

	internal static void Sync(GlobalMembersSaveGame.SaveGameContext theContext, List<LocalMail> theMail)
	{
		if (theContext.mReading)
		{
			theMail.Clear();
		}
		if (theContext.mVersion < SaveFileVersion.MailboxState)
		{
			return;
		}
		int theInt = theMail.Count;
		theContext.SyncListLength(ref theInt);
		for (int i = 0; i < theInt; i++)
		{
			if (theContext.mReading)
			{
				theMail.Add(new LocalMail());
			}
			theMail[i].Sync(theContext);
		}
	}

	internal static int GetUnseenCount(PlayerInfo thePlayerInfo)
	{
		if (thePlayerInfo == null)
		{
			return 0;
		}
		int num = 0;
		foreach (LocalMail item in thePlayerInfo.mMail)
		{
			if (!item.mSeen && item.IsSupported())
			{
				num++;
			}
		}
		return num;
	}

	internal static bool MarkSeen(PlayerInfo thePlayerInfo)
	{
		List<LocalMail> list = thePlayerInfo.mMail.FindAll((LocalMail aLetter) => !aLetter.mSeen && aLetter.IsSupported());
		if (list.Count == 0)
		{
			return true;
		}
		foreach (LocalMail item in list)
		{
			item.mSeen = true;
		}
		if (thePlayerInfo.SaveDetails())
		{
			return true;
		}
		foreach (LocalMail item2 in list)
		{
			item2.mSeen = false;
		}
		return false;
	}

	internal static bool SetRead(PlayerInfo thePlayerInfo, LocalMail theMail, bool theRead)
	{
		if (!thePlayerInfo.mMail.Contains(theMail) || (theRead && theMail.HasUnclaimedRewards()) || (!theRead && theMail.mDeleteOnlyAfterRead))
		{
			return false;
		}
		int mReadOrder = theMail.mReadOrder;
		bool mSeen = theMail.mSeen;
		int num = 0;
		if (theRead)
		{
			foreach (LocalMail item in thePlayerInfo.mMail)
			{
				num = Math.Max(num, item.mReadOrder);
			}
			num++;
		}
		theMail.mReadOrder = num;
		theMail.mSeen = true;
		if (thePlayerInfo.SaveDetails())
		{
			return true;
		}
		theMail.mReadOrder = mReadOrder;
		theMail.mSeen = mSeen;
		return false;
	}

	internal static bool Delete(PlayerInfo thePlayerInfo, LocalMail theMail)
	{
		int num = thePlayerInfo.mMail.IndexOf(theMail);
		if (num < 0 || !theMail.mDeleteOnlyAfterRead || theMail.mReadOrder == 0 || theMail.HasUnclaimedRewards())
		{
			return false;
		}
		thePlayerInfo.mMail.RemoveAt(num);
		if (thePlayerInfo.SaveDetails())
		{
			return true;
		}
		thePlayerInfo.mMail.Insert(num, theMail);
		return false;
	}

	internal static bool ClaimReward(PlayerInfo thePlayerInfo, LocalMail theMail, int theIndex, out PottedPlant[] thePlants, out int theRequiredSpace)
	{
		thePlants = Array.Empty<PottedPlant>();
		theRequiredSpace = 0;
		if (!thePlayerInfo.mMail.Contains(theMail) || (uint)theIndex >= (uint)theMail.mRewards.Length)
		{
			return false;
		}
		LocalMailReward localMailReward = theMail.mRewards[theIndex];
		int coinValue = Coin.GetCoinValue(localMailReward.mType);
		if (localMailReward.mClaimed || localMailReward.mCount <= 0)
		{
			return false;
		}
		int mCoins = thePlayerInfo.mCoins;
		int num = thePlayerInfo.mPurchases[26];
		int mNumPottedPlants = thePlayerInfo.mNumPottedPlants;
		PottedPlant[] array = Array.Empty<PottedPlant>();
		checked
		{
			if (coinValue > 0)
			{
				thePlayerInfo.AddCoins(coinValue * localMailReward.mCount);
			}
			else if (localMailReward.mType == CoinType.Chocolate)
			{
				thePlayerInfo.mPurchases[26] = Math.Max(1000, num) + localMailReward.mCount;
			}
			else
			{
				if (localMailReward.mType != CoinType.PresentPlant && localMailReward.mType != CoinType.Award10PottedPlants)
				{
					return false;
				}
				bool flag = localMailReward.mType == CoinType.Award10PottedPlants;
				int num2 = localMailReward.mCount * ((!flag) ? 1 : 10);
				ZenGarden mZenGarden = GlobalStaticVars.gLawnApp.mZenGarden;
				if (mZenGarden.IsZenGardenFull(theIncludeDroppedPresents: false, num2))
				{
					theRequiredSpace = num2;
					return false;
				}
				array = new PottedPlant[num2];
				Array.Copy(thePlayerInfo.mPottedPlant, mNumPottedPlants, array, 0, num2);
				thePlants = new PottedPlant[num2];
				SeedType[] array2 = new SeedType[num2];
				unchecked
				{
					for (int i = 0; i < num2; i++)
					{
						array2[i] = (flag ? mZenGarden.PickRandomSeedTypeWithNewPlants() : mZenGarden.PickRandomSeedType());
					}
					if (flag)
					{
						int[] array3 = new int[54];
						for (int j = 0; j < mNumPottedPlants; j++)
						{
							SeedType mSeedType = thePlayerInfo.mPottedPlant[j].mSeedType;
							if ((uint)mSeedType < (uint)array3.Length)
							{
								array3[(int)mSeedType]++;
							}
						}
						for (int k = 0; k < num2; k += 10)
						{
							int num3 = -1;
							for (int l = k; l < k + 10; l++)
							{
								if (array3[(int)array2[l]] == 0)
								{
									num3 = l;
								}
							}
							if (num3 == -1)
							{
								SeedType seedType = SeedType.Imitater;
								for (SeedType seedType2 = SeedType.Peashooter; seedType2 <= SeedType.Imitater; seedType2++)
								{
									if (seedType2 != SeedType.Flowerpot && seedType2 != SeedType.Cobcannon && array3[(int)seedType2] == 0)
									{
										seedType = seedType2;
										break;
									}
								}
								array2[k] = seedType;
							}
							else
							{
								ref SeedType reference = ref array2[k];
								ref SeedType reference2 = ref array2[num3];
								SeedType seedType3 = array2[num3];
								SeedType seedType4 = array2[k];
								reference = seedType3;
								reference2 = seedType4;
							}
							for (int m = k; m < k + 10; m++)
							{
								array3[(int)array2[m]]++;
							}
						}
					}
					for (int n = 0; n < num2; n++)
					{
						PottedPlant pottedPlant = new PottedPlant();
						pottedPlant.InitializePottedPlant(array2[n]);
						thePlayerInfo.mPottedPlant[mNumPottedPlants + n] = pottedPlant;
						mZenGarden.AddPottedPlant(pottedPlant);
						thePlants[n] = pottedPlant;
					}
				}
			}
			localMailReward.mClaimed = true;
			if (thePlayerInfo.SaveDetails())
			{
				return true;
			}
			thePlayerInfo.mCoins = mCoins;
			thePlayerInfo.mPurchases[26] = num;
			thePlayerInfo.mNumPottedPlants = mNumPottedPlants;
			Array.Copy(array, 0, thePlayerInfo.mPottedPlant, mNumPottedPlants, array.Length);
			localMailReward.mClaimed = false;
			thePlants = Array.Empty<PottedPlant>();
			return false;
		}
	}
}
