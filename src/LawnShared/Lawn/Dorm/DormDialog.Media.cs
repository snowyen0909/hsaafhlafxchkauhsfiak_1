// 宿舍 furniture sets: chairs / sofas / beds the girls sit on (drawn from the hips like the swing), the TVs
// (tap to switch on; 9 channels; girls come and sit in front to watch) and the sunflower record player
// (tap to play one of the game's tunes; the disc label spins, notes rise, girls nearby dance now and then).
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	// ------------------------------------------------------------------ chairs

	/// <summary>Where the hips go, as fractions of the facing-0 art (facing 1 is its mirror). Back views have no seats.</summary>
	private static readonly Dictionary<string, Vector2[]> ChairSeats = new Dictionary<string, Vector2[]>
	{
		["sofa2"] = new[] { new Vector2(0.27f, 0.5f), new Vector2(0.7f, 0.6f) },
		["sofa1"] = new[] { new Vector2(0.55f, 0.6f) },
		["armchair"] = new[] { new Vector2(0.5f, 0.56f) },
		["chaise"] = new[] { new Vector2(0.55f, 0.5f) },
		["bed"] = new[] { new Vector2(0.42f, 0.58f) }
	};

	private static bool IsChair(Item it) => it != null && ChairSeats.ContainsKey(it.Def.Id);

	private static bool IsTv(Item it) => it != null && (it.Def.Id == "tv" || it.Def.Id == "tv_stand");

	/// <summary>Seats of chairs (grid points spread along the piece) and of switched-on TVs (floor, in front).</summary>
	private void AddSetSeats(Item it, List<Vector2> seats)
	{
		if (IsChair(it))
		{
			if (it.Rot >= 2)
			{
				return;
			}
			int n = ChairSeats[it.Def.Id].Length;
			for (int k = 0; k < n; k++)
			{
				float t = (k + 0.5f) / n;
				seats.Add(it.A >= it.B ? new Vector2(it.I + t * it.A, it.J + it.B / 2f) : new Vector2(it.I + it.A / 2f, it.J + t * it.B));
			}
			return;
		}
		if (IsTv(it) && mTvOn.Contains(it) && it.Rot < 2)
		{
			// one cell gap in front of the screen: +j for facing 0 (front to the lower left), +i for facing 1
			int n = Math.Max(it.A, it.B) >= 2 ? 2 : 1;
			for (int k = 0; k < n; k++)
			{
				int ci = it.Rot == 0 ? it.I + k : it.I + it.A + 1;
				int cj = it.Rot == 0 ? it.J + it.B + 1 : it.J + k;
				if (ci >= 0 && cj >= 0 && ci < N && cj < N && !Blocked(ci, cj))
				{
					seats.Add(new Vector2(ci + 0.5f, cj + 0.5f));
				}
			}
		}
	}

	/// <summary>A seat outside the furniture (in front of a TV): she walks to that cell itself.</summary>
	private Point? OwnSeatCell(Item it, int k, DormActor a)
	{
		if (!IsTv(it))
		{
			return null;
		}
		List<Vector2> seats = SeatsOf(it);
		if (k >= seats.Count)
		{
			return null;
		}
		Point c = new Point((int)seats[k].X, (int)seats[k].Y);
		return CellTaken(c.X, c.Y, a) ? null : c;
	}

	/// <summary>Girls sitting on a chair piece, drawn with it (after the art, from the hips at the seat point).</summary>
	private void DrawChairSitters(Graphics g, Item it, TRect r)
	{
		if (it.Rot >= 2 || it == mDrag)
		{
			return;
		}
		if (IsLounger(it))
		{
			foreach (DormActor a in mActors.Where((DormActor t) => t.Mode == DormActor.State.Seat && t.SeatItem == it))
			{
				DrawLying(g, a, it, r);
			}
			return;
		}
		Vector2[] pts = ChairSeats[it.Def.Id];
		List<(float x, float y, DormActor a)> list = new List<(float, float, DormActor)>();
		foreach (DormActor a in mActors.Where((DormActor t) => t.Mode == DormActor.State.Seat && t.SeatItem == it))
		{
			if (a.SeatIdx >= pts.Length)
			{
				continue;
			}
			Vector2 p = pts[a.SeatIdx];
			float fx = it.Rot == 1 ? 1f - p.X : p.X;
			float hop = -(float)Math.Sin(a.Hop * Math.PI) * mCW * 0.12f;
			list.Add((r.mX + fx * r.mWidth, r.mY + p.Y * r.mHeight + hop, a));
		}
		foreach (var e in list.OrderBy((x) => x.y))
		{
			e.a.Draw(g, e.x, e.y, mCW * 1.4f, 0f, 1f, 1f);
		}
	}

	/// <summary>Pieces a girl lies down on (head on the cushion end) instead of sitting.</summary>
	private static bool IsLounger(Item it) => it != null && it.Def.Id == "chaise";

	/// <summary>
	/// Lying on the chaise: the standing pose tilted almost flat along the seat, head on the pillow by the raised
	/// back (left of the facing-0 art, right on its mirror), eyes closed and breathing like on the futon.
	/// </summary>
	private void DrawLying(Graphics g, DormActor a, Item it, TRect r)
	{
		bool headLeft = it.Rot == 0;
		float hx = r.mX + r.mWidth * (headLeft ? 0.36f : 0.64f);
		float hy = r.mY + r.mHeight * 0.37f - (float)Math.Sin(a.Hop * Math.PI) * mCW * 0.1f;
		float body = r.mWidth * 0.6f;
		float tilt = headLeft ? -78f : 78f;
		float rad = MathHelper.ToRadians(tilt);
		float footX = hx - (float)Math.Sin(rad) * body * 0.83f;
		float footY = hy + (float)Math.Cos(rad) * body * 0.83f;
		float breathe = 1f + 0.025f * (float)Math.Sin(mTime * 2f * Math.PI / 3.2f);
		a.FaceLeft = !headLeft;
		a.Draw(g, footX, footY, body, tilt, breathe, 1f);
		for (int k = 0; k < 3; k++)
		{
			float t = (mTime * 0.5f + k / 3f) % 1f;
			float size = mCW * (0.14f + 0.1f * t);
			int alpha = (int)(255 * Math.Min(1f, (1f - t) * 2f) * Math.Min(1f, t * 4f));
			DrawSprite(g, "ui/ic_z", hx + (headLeft ? -1 : 1) * t * mCW * 0.25f - size / 2f, hy - mCW * 0.3f - t * mCW * 0.5f - size / 2f, size, size, new Color(255, 255, 255, alpha));
		}
	}

	// ------------------------------------------------------------------ TV

	private readonly HashSet<Item> mTvOn = new HashSet<Item>();

	private readonly Dictionary<Item, float> mTvStart = new Dictionary<Item, float>();

	private const int TvChannels = 9;

	private const float TvChannelTime = 7f;

	private int TvChannel(Item it) => mTvStart.TryGetValue(it, out float t0) ? (int)((mTime - t0) / TvChannelTime) % TvChannels : 0;

	private void DrawTvScreen(Graphics g, Item it, TRect r, Color? tint)
	{
		if (!mTvOn.Contains(it) || it.Rot >= 2 || it == mDrag)
		{
			return;
		}
		string name = (it.Def.Id == "tv" ? "tv_scr" : "tv_stand_scr") + TvChannel(it) + "_" + it.Rot;
		DrawSprite(g, name, r.mX, r.mY, r.mWidth, r.mHeight, tint);
	}

	// ------------------------------------------------------------------ record player

	private readonly HashSet<Item> mPlaying = new HashSet<Item>();

	private static readonly (MusicTune tune, string name)[] Tunes =
	{
		(MusicTune.DayGrasswalk, "白天草地"),
		(MusicTune.NightMoongrains, "夜晚月光"),
		(MusicTune.PoolWaterygraves, "泳池"),
		(MusicTune.FogRigormormist, "迷雾"),
		(MusicTune.RoofGrazetheroof, "屋顶"),
		(MusicTune.MinigameLoonboon, "Loonboon"),
		(MusicTune.PuzzleCerebrawl, "解谜"),
		(MusicTune.ChooseYourSeeds, "选卡")
	};

	private int mTuneIdx;

	private void DrawRecordPlayer(Graphics g, Item it, TRect r, Color? tint)
	{
		if (!mPlaying.Contains(it) || it == mDrag)
		{
			return;
		}
		if (it.Rot < 2)
		{
			int f = (int)(mTime * 14f) % 9;
			DrawSprite(g, "record_player_spin" + f + "_" + it.Rot, r.mX, r.mY, r.mWidth, r.mHeight, tint);
		}
		// notes drifting up from the horn side, one after another
		for (int k = 0; k < 3; k++)
		{
			float t = (mTime * 0.45f + k / 3f) % 1f;
			float size = mCW * (k == 1 ? 0.22f : 0.3f) * (0.8f + 0.2f * t);
			int alpha = (int)(255 * Math.Min(1f, (1f - t) * 2.5f) * Math.Min(1f, t * 5f));
			float side = (it.Rot & 1) == 0 ? 1f : -1f;
			float x = r.mX + r.mWidth * (0.5f + side * (0.2f + 0.25f * t)) + (float)Math.Sin((mTime + k) * 3f) * mCW * 0.06f;
			float y = r.mY + r.mHeight * 0.15f - t * mCW * 0.9f;
			DrawSprite(g, k == 1 ? "note_b" : "note_a", x - size / 2f, y - size / 2f, size, size, new Color(255, 255, 255, alpha));
		}
	}

	private void SetDormTune(MusicTune tune)
	{
		DormPlatform.PlayTune(mLawn, tune);
	}

	// ------------------------------------------------------------------ taps

	/// <summary>Room (not arranging) taps on furniture.</summary>
	private void TapFurniture(Item it)
	{
		if (it == null)
		{
			return;
		}
		if (IsTv(it))
		{
			if (mTvOn.Remove(it))
			{
				Hint("关掉了" + it.Def.Title);
			}
			else
			{
				mTvOn.Add(it);
				mTvStart[it] = mTime - (float)mRand.Next(TvChannels) * TvChannelTime;
				Hint(it.Rot >= 2 ? "电视打开了（转过来才看得到画面）" : "电视打开了 · 大家会过来看");
			}
			DormPlatform.PlaySound(Resources.SOUND_TAP);
			return;
		}
		if (it.Def.Id == "record_player")
		{
			if (mPlaying.Remove(it))
			{
				if (mPlaying.Count == 0)
				{
					SetDormTune(MusicTune.ZenGarden);
				}
				Hint("唱片机停下来了");
			}
			else
			{
				mPlaying.Clear();
				mPlaying.Add(it);
				var tune = Tunes[mTuneIdx % Tunes.Length];
				mTuneIdx++;
				SetDormTune(tune.tune);
				Hint("唱片机播放：" + tune.name + " · 再点一下停止");
			}
			return;
		}
	}

	/// <summary>Furniture that left the room (stored, sold, room cleared): forget its TV / music state.</summary>
	private void UpdateMedia()
	{
		mTvOn.RemoveWhere((Item it) => !mItems.Contains(it));
		if (mPlaying.RemoveWhere((Item it) => !mItems.Contains(it)) > 0 && mPlaying.Count == 0)
		{
			SetDormTune(MusicTune.ZenGarden);
		}
	}

	/// <summary>Music on: an idle girl near the record player dances (her garden animation) now and then.</summary>
	private bool TryDance(DormActor a)
	{
		Item rp = mPlaying.FirstOrDefault();
		if (rp == null)
		{
			return false;
		}
		float di = a.FI - (rp.I + 0.5f), dj = a.FJ - (rp.J + 0.5f);
		if (di * di + dj * dj > 16f)
		{
			return false;
		}
		float len = a.PlayGarden(mRand.Next(2) + 1);
		a.Timer = Math.Max(2.5f, len + 0.3f);
		mHearts.Add((SX(a.FI, a.FJ), a.LastRect.mY, 0f));
		return true;
	}

#if !__IOS__
	internal void TestPlace(string id, int i, int j, int rot)
	{
		Def d = FindDef(id);
		if (d == null)
		{
			return;
		}
		if (Available(d) <= 0)
		{
			Buy(d);
		}
		if (Available(d) <= 0)
		{
			return;
		}
		Item it = new Item { Def = d, I = i, J = j, Rot = rot, Side = rot };
		if (d.Kind == Kind.Wall)
		{
			it.Rot = 0;
		}
		mItems.Add(it);
	}

	internal void TestTap(string id)
	{
		TapFurniture(mItems.FirstOrDefault((Item t) => t.Def.Id == id));
	}

	internal void TestClear() => mItems.Clear();
#endif
}
