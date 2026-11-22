// 宿舍 floors: 1F is free, 2F is bought once ($30,000). Each floor has its own walls, floor, furniture and up to 4
// girls; the warehouse is shared. Only the floor on screen is live (mItems, mChosen, the actors); the others are kept as
// their saved layout text and girl list. Switch with the round 上楼 / 下楼 button (not while arranging).
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	private sealed class FloorState
	{
		/// <summary>Saved layout in the one-line-per-piece text (see Serialize).</summary>
		public string Layout = "@wall wall_sakura\n@floor floor_tatami";

		public List<string> Girls = new List<string>();
	}

	/// <summary>Unlocked floors, 1F first. The entry of the floor on screen is refreshed by SyncFloor before use.</summary>
	private readonly List<FloorState> mFloors = new List<FloorState>();

	/// <summary>Floor on screen (0 = 1F).</summary>
	private int mFloor;

	/// <summary>Price of the second floor, in coins ($30,000).</summary>
	private const int SecondFloorPrice = 3000;

	private const int MaxFloors = 2;

	private bool mUnlockAsk;

	private TRect mUnlockPanel, mUnlockOk, mUnlockNo;

	/// <summary>Fade when changing floors (1 → 0).</summary>
	private float mFloorFade;

	private static string FloorName(int k) => (k + 1) + "F";

	private bool HasSecondFloor => mFloors.Count > 1;

	private void SyncFloor()
	{
		while (mFloors.Count <= mFloor)
		{
			mFloors.Add(new FloorState());
		}
		mFloors[mFloor].Layout = mSnapshot ?? Serialize();
		mFloors[mFloor].Girls = new List<string>(mChosen);
	}

	/// <summary>Pieces of this kind standing on the floors that are not on screen.</summary>
	private int PlacedElsewhere(Def def)
	{
		int n = 0;
		for (int k = 0; k < mFloors.Count; k++)
		{
			if (k == mFloor)
			{
				continue;
			}
			foreach (string line in mFloors[k].Layout.Split('\n'))
			{
				int sp = line.IndexOf(' ');
				if (sp > 0 && line[0] != '@' && line[0] != '#' && line.Substring(0, sp) == def.Id)
				{
					n++;
				}
			}
		}
		return n;
	}

	/// <summary>Which floor a girl lives on, or -1.</summary>
	private int FloorIndexOf(string skel)
	{
		if (mChosen.Contains(skel))
		{
			return mFloor;
		}
		for (int k = 0; k < mFloors.Count; k++)
		{
			if (k != mFloor && mFloors[k].Girls.Contains(skel))
			{
				return k;
			}
		}
		return -1;
	}

	private int GirlsOn(int k) => k == mFloor ? mChosen.Count : mFloors[k].Girls.Count;

	private int GirlsInDorm => Enumerable.Range(0, mFloors.Count).Sum(GirlsOn);

	/// <summary>Moves a girl to floor <paramref name="target"/> (-1 = out of the dorm). False when that floor is full.</summary>
	private bool MoveGirl(string skel, int target)
	{
		int from = FloorIndexOf(skel);
		if (from == target)
		{
			return true;
		}
		if (target >= 0 && GirlsOn(target) >= MaxGirls)
		{
			return false;
		}
		if (from == mFloor)
		{
			ToggleGirl(skel);
		}
		else if (from >= 0)
		{
			mFloors[from].Girls.Remove(skel);
		}
		if (target == mFloor)
		{
			ToggleGirl(skel);
		}
		else if (target >= 0)
		{
			mFloors[target].Girls.Add(skel);
		}
		WriteSave();
		return true;
	}

	private void SwitchFloor(int k)
	{
		if (k == mFloor || k < 0 || k >= mFloors.Count || mEdit)
		{
			return;
		}
		SyncFloor();
		foreach (DormActor a in mActors)
		{
			a.Dispose();
		}
		mActors.Clear();
		mHeld = null;
		mPress = null;
		mSel = null;
		mFloor = k;
		Deserialize(mFloors[k].Layout);
		mSnapshot = Serialize();
		mChosen.Clear();
		mChosen.AddRange(mFloors[k].Girls);
		foreach (string skel in mChosen)
		{
			AddActor(skel);
		}
		PlaceActorsInitially();
		mFloorFade = 1f;
		DormPlatform.PlaySound(Resources.SOUND_TAP);
		Hint("来到了 " + FloorName(k) + (mChosen.Count == 0 ? " · 在「宿舍管理」让植物娘住进来" : ""));
	}

	private void FloorButton(bool up)
	{
		if (up && !HasSecondFloor)
		{
			mUnlockAsk = true;
			return;
		}
		SwitchFloor(up ? mFloor + 1 : mFloor - 1);
	}

	// ------------------------------------------------------------------ unlock dialog

	private void LayoutUnlock()
	{
		int pw = 380, ph = 230;
		mUnlockPanel = new TRect((mWidth - pw) / 2, (mHeight - ph) / 2, pw, ph);
		int bw = 140, bh = 44, by = mUnlockPanel.mY + ph - bh - 22;
		mUnlockNo = new TRect(mUnlockPanel.mX + pw / 2 - bw - 10, by, bw, bh);
		mUnlockOk = new TRect(mUnlockPanel.mX + pw / 2 + 10, by, bw, bh);
	}

	private void UnlockClick(int x, int y)
	{
		LayoutUnlock();
		if (In(mUnlockOk, x, y))
		{
			if (!DormPlatform.SpendCoins(mLawn, SecondFloorPrice))
			{
				Hint("金币不够 · 解锁二楼需要 " + LawnApp.GetMoneyString(SecondFloorPrice));
				mUnlockAsk = false;
				return;
			}
			SyncFloor();
			mFloors.Add(new FloorState());
			WriteSave();
			DormPlatform.PlaySound(Resources.SOUND_COIN);
			mUnlockAsk = false;
			SwitchFloor(1);
			Hint("二楼解锁了！在「宿舍管理」让植物娘住进 2F");
		}
		else if (In(mUnlockNo, x, y) || !In(mUnlockPanel, x, y))
		{
			mUnlockAsk = false;
		}
	}

	private void DrawUnlock(Graphics g)
	{
		LayoutUnlock();
		g.SetColor(new Color(40, 25, 15, 140));
		g.FillRect(0, 0, mWidth, mHeight);
		TRect p = mUnlockPanel;
		Draw9(g, "ui/panel", p, 26);
		Flowers(g, p);
		string title = "解锁二楼";
		int tw = mFont.StringWidth(title) + 60;
		TRect tr = new TRect(p.mX + (p.mWidth - tw) / 2, p.mY - 20, tw, 44);
		Draw9(g, "ui/pill_dark", tr, 20);
		TextCentered(g, mFont, title, tr, Color.White, -2);
		DrawSprite(g, "ui2/floor_up", p.mX + 24, p.mY + 40, 90, 90);
		int tx = p.mX + 130;
		int coins = DormPlatform.Coins(mLawn);
		Text(g, mFont, "多一层房间：", tx, p.mY + 40, InkBrown);
		Text(g, mFont, "再住 " + MaxGirls + " 位植物娘", tx, p.mY + 40 + mFont.GetHeight() + 2, InkBrown);
		Text(g, mFont, "价格  " + LawnApp.GetMoneyString(SecondFloorPrice), tx, p.mY + 98, new Color(170, 110, 20));
		Text(g, mFont, "金币  " + LawnApp.GetMoneyString(coins), tx, p.mY + 98 + mFont.GetHeight() + 4, coins >= SecondFloorPrice ? InkBrown : InkWarn);
		Draw9(g, "ui/btn", mUnlockNo, 22);
		TextCentered(g, mFont, "取消", mUnlockNo, InkBrown, -2);
		bool can = coins >= SecondFloorPrice;
		Draw9(g, can ? "ui/btn_on" : "ui/btn_off", mUnlockOk, 22);
		TextCentered(g, mFont, "解锁", mUnlockOk, can ? InkBrown : InkGrey, -2);
	}

	private void DrawFloorFade(Graphics g, float dt)
	{
		if (mFloorFade <= 0f)
		{
			return;
		}
		g.SetColor(new Color(40, 28, 18, (int)(200 * mFloorFade)));
		g.FillRect(0, 0, mWidth, mHeight);
	}

	private void UpdateFloors(float dt)
	{
		mFloorFade = Math.Max(0f, mFloorFade - dt * 2.5f);
	}

#if !__IOS__
	internal void TestUnlockFloor()
	{
		mUnlockAsk = true;
	}

	internal void TestUnlockOk()
	{
		LayoutUnlock();
		UnlockClick(mUnlockOk.mX + 5, mUnlockOk.mY + 5);
	}

	internal void TestSwitchFloor(int k) => SwitchFloor(k);

	internal void TestMoveGirl(string skel, int floor) => MoveGirl(skel, floor);
#endif
}
