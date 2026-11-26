// 伏僵塔黄 (plant_fjth.py).
using System.Collections.Generic;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public static class TavelFjth
{
	public const int Id = 102;

	public static SeedType Seed => (SeedType)Id;

	public const int SleepTime = 1800;

	public const int AttackTotalFrames = 80;

	public const int AttackSwallowFrame = 26;

	public const int UltimateTargetCount = 8;

	public const int UltimateTotalFrames = 80;

	public const int UltimatePullFrame = 20;

	public sealed class Pull
	{
		public Zombie Zombie;

		public float StartX;

		public float StartY;
	}

	public sealed class State
	{
		public int SleepUntil;

		public int AttackUntil;

		public Zombie AttackTarget;

		public bool AttackSwallowed;

		public string Mode = "stand";

		public int UltimateStart;

		public List<Pull> UltimateTargets = new List<Pull>();

		public bool UltimateBurned;
	}

	private static TavelSpec sSpec;

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	private static void Align(Plant p) => Tavel.Align(p, 1.056f, 2f, -3f);

	public static TavelSpec Spec()
	{
		sSpec = new TavelSpec(Id, "fjth")
		{
			Name = "伏僵塔黄",
			Cost = 225,
			Card = "tavel/fjth/card.png",
			AlmanacText = "伏僵塔黄会吞噬靠近前方的僵尸，随后进入休眠。\n{SHORTLINE}\n{KEYWORD}生命：{STAT}非常高\n{KEYWORD}特点：{STAT}吞噬前方一格的僵尸，然后进入休眠\n{KEYWORD}大招：{STAT}吸引全场随机8只僵尸并立刻炼化，同时灼烧本行\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}225\n{SHORTLINE}\n{FLAVOR}镇守高塔的少女，会将闯入的敌人尽数收入法器。",
			DefName = "FJTH",
			DefCost = 225,
			LaunchRate = 60,
			ReanimFile = "tavel/spanim/fjth",
			CustomBody = true,
			Preview = new TavelPreview("anim_stand", 0.40f, -35f, -82f, 1.056f, 2f, -3f),
			OnPlanted = delegate(Plant p, int x, int y)
			{
				Get(p);
				p.mPlantHealth = 32000;
				p.mPlantMaxHealth = 32000;
				if (p.mBoard != null)
				{
					Tavel.StartPacketCooldown(p.mBoard, Seed);
				}
				Tavel.DropNativeBody(p);
				Tavel.CreateBody(p, sSpec.Reanim, 1, "stand");
				Align(p);
			},
			OnUpdate = delegate(Plant p)
			{
				if (p.mBoard != null)
				{
					Update(p);
				}
				Align(p);
			},
			CanUltimate = (Plant p) => Get(p).Mode != "ultimate",
			UltimateActive = (Plant p) => p.mTavel is State s && s.Mode == "ultimate",
			StartUltimate = delegate(Plant p)
			{
				StartUltimate(p, Get(p), p.mBoard.mMainCounter);
			}
		};
		return sSpec;
	}

	private static bool IsPulled(Zombie z) => z.mTavel != null && z.mTavel.FjthPulled;

	private static void SetPulled(Zombie z, bool theOn)
	{
		if (theOn)
		{
			TavelHooks.ZState(z).FjthPulled = true;
		}
		else if (z.mTavel != null)
		{
			z.mTavel.FjthPulled = false;
		}
	}

	/// <summary>A robot titan stepping on her while she stands: she swallows it instead.</summary>
	public static bool SwallowTitan(Plant p, Zombie z)
	{
		State s = Get(p);
		if (s.Mode == "stand" && s.SleepUntil <= p.mBoard.mMainCounter)
		{
			Tavel.PlayAnim(p, "attack", ReanimLoopType.PlayOnceAndHold, 60f);
			s.AttackTarget = z;
			s.AttackSwallowed = false;
			s.Mode = "attack";
			return true;
		}
		return false;
	}

	private static Zombie FindTarget(Plant p)
	{
		if (p.mBoard == null)
		{
			return null;
		}
		float left = p.mX + 35;
		float right = left + 100;
		Zombie best = null;
		float bestX = 1000000f;
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (z.mDead || IsPulled(z) || z.mRow != p.mRow || Tavel.IsBoss(z))
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float cx = r.mX + r.mWidth * 0.5f;
			if (left <= cx && cx <= right && cx < bestX)
			{
				best = z;
				bestX = cx;
			}
		}
		return best;
	}

	private static List<Pull> ChooseUltimateTargets(Plant p)
	{
		List<Zombie> candidates = new List<Zombie>();
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (Tavel.Alive(z) && !IsPulled(z) && !Tavel.IsBoss(z))
			{
				candidates.Add(z);
			}
		}
		List<Pull> selected = new List<Pull>();
		while (candidates.Count > 0 && selected.Count < UltimateTargetCount)
		{
			int i = Tavel.Rand(0, candidates.Count - 1);
			Zombie z = candidates[i];
			candidates.RemoveAt(i);
			selected.Add(new Pull { Zombie = z, StartX = z.mPosX, StartY = z.mPosY });
		}
		return selected;
	}

	private static void StartUltimate(Plant p, State s, int theFrame)
	{
		s.SleepUntil = 0;
		s.AttackUntil = 0;
		s.Mode = "ultimate";
		s.UltimateStart = theFrame;
		s.UltimateTargets = ChooseUltimateTargets(p);
		foreach (Pull item in s.UltimateTargets)
		{
			SetPulled(item.Zombie, theOn: true);
			if (item.Zombie.mIsEating)
			{
				item.Zombie.StopEating();
			}
		}
		s.UltimateBurned = false;
		Tavel.PlayAnim(p, "sa", ReanimLoopType.PlayOnceAndHold, 30f);
	}

	private static void UpdateUltimate(Plant p, State s)
	{
		float t = Tavel.AnimTime(p);
		float swallowTime = UltimatePullFrame / (float)UltimateTotalFrames;
		// a zombie's mPosX/mPosY is its model anchor: centre its 120-wide body on the 80-wide cell
		int gx = p.mBoard.GridToPixelX(p.mPlantCol, p.mRow);
		float tx = gx - 20;
		float ty = p.mBoard.GetPosYBasedOnRow(tx + 40f, p.mRow) - 30f;
		if (t < swallowTime)
		{
			float progress = System.Math.Max(0f, System.Math.Min(1f, t / swallowTime));
			foreach (Pull item in s.UltimateTargets)
			{
				Zombie z = item.Zombie;
				if (z.mDead)
				{
					SetPulled(z, theOn: false);
					continue;
				}
				z.mPosX = item.StartX + (tx - item.StartX) * progress;
				z.mPosY = item.StartY + (ty - item.StartY) * progress;
				z.mX = (int)z.mPosX;
				z.mY = (int)z.mPosY;
			}
			return;
		}
		if (!s.UltimateBurned)
		{
			HashSet<Zombie> swallowed = new HashSet<Zombie>();
			foreach (Pull item in s.UltimateTargets)
			{
				swallowed.Add(item.Zombie);
				SetPulled(item.Zombie, theOn: false);
				if (!item.Zombie.mDead)
				{
					item.Zombie.DieNoLoot(false);
				}
			}
			foreach (Zombie z in p.mBoard.mZombies.ToArray())
			{
				if (!z.mDead && z.mRow == p.mRow && !swallowed.Contains(z))
				{
					z.ApplyBurn();
				}
			}
			p.mBoard.DoFwoosh(p.mRow);
			p.mApp.PlayFoley(FoleyType.JalapenoIgnite);
			p.mApp.PlayFoley(FoleyType.Juicy);
			s.UltimateBurned = true;
		}
		if (Tavel.Body(p) != null && Tavel.AnimTime(p) < 0.999f)
		{
			return;
		}
		s.UltimateTargets = new List<Pull>();
		s.Mode = "stand";
		Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		int frame = p.mBoard.mMainCounter;
		if (s.Mode == "ultimate")
		{
			UpdateUltimate(p, s);
			return;
		}
		if (s.Mode == "attack")
		{
			float t = Tavel.AnimTime(p);
			float swallowTime = AttackSwallowFrame / (float)AttackTotalFrames;
			if (!s.AttackSwallowed && t >= swallowTime)
			{
				if (s.AttackTarget != null && !s.AttackTarget.mDead)
				{
					s.AttackTarget.DieNoLoot(false);
				}
				s.AttackSwallowed = true;
				s.SleepUntil = frame + SleepTime;
			}
			if (Tavel.Body(p) != null && t < 0.999f)
			{
				return;
			}
			s.AttackTarget = null;
			s.AttackSwallowed = false;
			Tavel.PlayAnim(p, "sleep", ReanimLoopType.Loop, 30f);
			s.Mode = "sleep";
			return;
		}
		if (s.SleepUntil > frame)
		{
			if (s.AttackUntil != 0 && frame >= s.AttackUntil)
			{
				Tavel.PlayAnim(p, "sleep", ReanimLoopType.Loop, 30f);
				s.AttackUntil = 0;
				s.Mode = "sleep";
			}
			return;
		}
		if (s.Mode != "stand")
		{
			Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			s.Mode = "stand";
		}
		Zombie target = FindTarget(p);
		if (target == null)
		{
			return;
		}
		Tavel.PlayAnim(p, "attack", ReanimLoopType.PlayOnceAndHold, 60f);
		s.AttackTarget = target;
		s.AttackSwallowed = false;
		s.Mode = "attack";
	}
}
