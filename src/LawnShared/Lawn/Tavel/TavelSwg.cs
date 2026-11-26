// 守卫菇 and the small guard it summons (plant_swg.py). On the lawn the small guard is a pumpkin (so it shares a cell
// with another plant) that the pack adopts.
using System.Collections.Generic;
using System.Linq;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public static class TavelSwg
{
	public const int Id = 109;

	public const int SmallId = 110;

	public static SeedType Seed => (SeedType)Id;

	public static SeedType SmallSeed => (SeedType)SmallId;

	public const int SummonInterval = 600;

	public const int UltimateMaxTargets = 5;

	public const int UltimateHeavyDamage = 800;

	public const int SmallHealth = 600;

	public const int SmallHeavyDamage = 400;

	public sealed class State
	{
		public string Mode = "stand";

		public int NextSummon;
	}

	public sealed class SmallState
	{
		public string Mode = "stand";

		public Zombie Target;
	}

	private static TavelSpec sSpec;

	private static TavelSpec sSmallSpec;

	public static State Get(Plant p)
	{
		if (p.mTavel is State s)
		{
			return s;
		}
		s = new State { NextSummon = (p.mBoard?.mMainCounter ?? 0) + SummonInterval };
		p.mTavel = s;
		return s;
	}

	public static SmallState GetSmall(Plant p) => p.mTavel as SmallState ?? (SmallState)(p.mTavel = new SmallState());

	public static bool IsSmall(Plant p) => p != null && (p.mSeedType == SmallSeed || p.mTavel is SmallState);

	public static Plant FindSmallAt(Board b, int theX, int theY)
	{
		if (b == null)
		{
			return null;
		}
		foreach (Plant p in b.mPlants)
		{
			if (!p.mDead && !p.mSquished && p.mPlantCol == theX && p.mRow == theY && IsSmall(p))
			{
				return p;
			}
		}
		return null;
	}

	public static IEnumerable<TavelSpec> Specs()
	{
		sSpec = new TavelSpec(Id, "swg")
		{
			Name = "守卫菇",
			Cost = 100,
			Card = "tavel/swg/card.png",
			AlmanacText = "守卫菇会召唤小守卫菇保护附近的植物。\n{SHORTLINE}\n{KEYWORD}生命：{STAT}3000\n{KEYWORD}特点：{STAT}高大植物，可阻挡跳跃僵尸\n{KEYWORD}小守卫菇：{STAT}保护并强化同格植物\n{KEYWORD}大招：{STAT}吞噬前方3×3范围的僵尸，并补满周围守卫\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}100\n{SHORTLINE}\n{FLAVOR}守护不只是挡在前面，也是让身边的伙伴变得更强。",
			DefName = "SWG",
			DefCost = 100,
			ReanimFile = "tavel/spanim/swg",
			CustomBody = true,
			Preview = new TavelPreview("anim_stand", 0.46f, -35f, -82f, 1.35f, -25f, -20f),
			OnPlanted = delegate(Plant p, int x, int y)
			{
				Get(p);
				p.mPlantHealth = 3000;
				p.mPlantMaxHealth = 3000;
				if (p.mBoard != null)
				{
					Tavel.StartPacketCooldown(p.mBoard, Seed);
				}
				Tavel.DropNativeBody(p);
				CreateBody(p, theSmall: false);
			},
			OnUpdate = delegate(Plant p)
			{
				if (p.mDead || p.mSquished)
				{
					p.mTavel = null;
					return;
				}
				if (p.mBoard != null)
				{
					Update(p);
				}
				Tavel.Align(p, 1.35f, -25f, -20f);
			},
			OnDie = delegate(Plant p)
			{
				p.mTavel = null;
			},
			CanUltimate = (Plant p) => Get(p).Mode != "ultimate",
			UltimateActive = (Plant p) => p.mTavel is State s && s.Mode == "ultimate",
			StartUltimate = StartUltimate,
			UpdateBoard = UpdateSmallRepeats,
			SaveBoard = SaveBoard,
			LoadBoard = LoadBoard,
			// a small guard repeats the ultimate of the plant it protects
			OnAnyUltimate = QueueSmallRepeat
		};
		sSmallSpec = new TavelSpec(SmallId, "swg_small")
		{
			Selectable = false,
			Conveyor = false,
			DefName = "SWG_SMALL",
			DefCost = 0,
			DefRefresh = 0,
			ReanimFile = "tavel/spanim/swg_small",
			CustomBody = true,
			NativeSeed = SeedType.Pumpkinshell,
			Claims = (Plant p) => p.mTavel is SmallState,
			OnPlanted = delegate(Plant p, int x, int y)
			{
				GetSmall(p);
				p.mPlantHealth = SmallHealth;
				p.mPlantMaxHealth = SmallHealth;
				Tavel.DropNativeBody(p);
				CreateBody(p, theSmall: true);
			},
			OnUpdate = delegate(Plant p)
			{
				if (p.mDead || p.mSquished)
				{
					p.mTavel = null;
					return;
				}
				if (p.mBoard != null)
				{
					UpdateSmall(p);
				}
				Tavel.Align(p, 1.44f, -26f, -18f);
			},
			OnDie = delegate(Plant p)
			{
				p.mTavel = null;
			},
			SquishOverride = delegate(Plant p)
			{
				// the small guard is a pumpkin underneath: never show the pumpkin's collapse
				if (p.mTavel is SmallState)
				{
					p.mTavel = null;
					Tavel.Body(p)?.ReanimationDie();
				}
				p.mPlantHealth = 0;
				p.Die();
			}
		};
		return new[] { sSpec, sSmallSpec };
	}

	private static void CreateBody(Plant p, bool theSmall)
	{
		Tavel.CreateBody(p, theSmall ? sSmallSpec.Reanim : sSpec.Reanim, theSmall ? 20 : 2, "stand");
		if (theSmall)
		{
			Tavel.Align(p, 1.44f, -26f, -18f);
		}
		else
		{
			Tavel.Align(p, 1.35f, -25f, -20f);
		}
	}

	/// <summary>A pumpkin becomes a small guard (summoned, or read back from a save).</summary>
	private static void ConvertPumpkin(Plant p)
	{
		p.mPlantHealth = SmallHealth;
		p.mPlantMaxHealth = SmallHealth;
		GetSmall(p);
		Tavel.Body(p)?.ReanimationDie();
		p.mBodyReanimID = null;
		CreateBody(p, theSmall: true);
	}

	private static void QueueSmallRepeat(Plant p, bool thePropagateTiles, string theSource)
	{
		if ((theSource != "manual" && theSource != "tile") || p?.mBoard == null || FindSmallAt(p.mBoard, p.mPlantCol, p.mRow) == null)
		{
			return;
		}
		TavelBoard s = Tavel.State(p.mBoard);
		if (!s.GuardPending.Contains(p))
		{
			s.GuardPending.Add(p);
		}
	}

	private static void UpdateSmallRepeats(Board b)
	{
		if (b.mTavel == null || b.mTavel.GuardPending.Count == 0)
		{
			return;
		}
		List<Plant> remaining = new List<Plant>();
		foreach (Plant p in b.mTavel.GuardPending)
		{
			if (p == null || p.mDead || p.mBoard != b)
			{
				continue;
			}
			if (Tavel.CanUseUltimate(p))
			{
				Tavel.StartUltimate(p, thePropagateTiles: false, "guard_repeat");
			}
			else
			{
				remaining.Add(p);
			}
		}
		b.mTavel.GuardPending = remaining;
	}

	private static string SaveBoard(Board b)
	{
		return string.Join(";", b.mPlants.Where((Plant p) => !p.mDead && IsSmall(p)).Select((Plant p) => p.mPlantCol + "," + p.mRow));
	}

	private static void LoadBoard(Board b, string theText)
	{
		HashSet<(int, int)> cells = new HashSet<(int, int)>();
		foreach (string cell in (theText ?? "").Split(';'))
		{
			string[] pair = cell.Split(',');
			if (pair.Length == 2 && int.TryParse(pair[0], out int x) && int.TryParse(pair[1], out int y))
			{
				cells.Add((x, y));
			}
		}
		foreach (Plant p in b.mPlants.ToArray())
		{
			if (!p.mDead && p.mSeedType == SeedType.Pumpkinshell && cells.Contains((p.mPlantCol, p.mRow)))
			{
				ConvertPumpkin(p);
			}
		}
	}

	private static (List<(int, int)> Planted, List<(int, int)> Empty) CandidateCells(Plant p)
	{
		Board b = p.mBoard;
		List<(int, int)> planted = new List<(int, int)>();
		List<(int, int)> empty = new List<(int, int)>();
		int[] dxs = { -1, 0, 1, -1, 1, -1, 0, 1 };
		int[] dys = { -1, -1, -1, 0, 0, 1, 1, 1 };
		for (int i = 0; i < 8; i++)
		{
			int gx = p.mPlantCol + dxs[i];
			int gy = p.mRow + dys[i];
			if (gx < 0 || gx > 8 || gy < 0 || gy >= 6 || FindSmallAt(b, gx, gy) != null)
			{
				continue;
			}
			bool has = b.mPlants.Any((Plant o) => !o.mDead && !IsSmall(o) && o.mPlantCol == gx && o.mRow == gy);
			(has ? planted : empty).Add((gx, gy));
		}
		return (planted, empty);
	}

	private static Plant Summon(Plant p, int theX, int theY)
	{
		if (FindSmallAt(p.mBoard, theX, theY) != null)
		{
			return null;
		}
		Plant small = p.mBoard.AddPlant(theX, theY, SeedType.Pumpkinshell, SeedType.None);
		if (small != null)
		{
			ConvertPumpkin(small);
		}
		return small;
	}

	private static List<Zombie> FindTargets(Plant p, int theMax)
	{
		List<Zombie> list = new List<Zombie>();
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (!Tavel.Alive(z))
			{
				continue;
			}
			int col = p.mBoard.PixelToGridXKeepOnBoard((int)z.mPosX, (int)z.mPosY);
			if (p.mPlantCol + 1 <= col && col <= p.mPlantCol + 3 && System.Math.Abs(z.mRow - p.mRow) <= 1)
			{
				list.Add(z);
				if (list.Count >= theMax)
				{
					break;
				}
			}
		}
		return list;
	}

	private static void StartUltimate(Plant p)
	{
		State s = Get(p);
		s.Mode = "ultimate";
		foreach (Zombie z in FindTargets(p, UltimateMaxTargets))
		{
			if (Tavel.IsBoss(z) || Tavel.IsGiantZombie(z))
			{
				z.TakeDamage(UltimateHeavyDamage, 0u);
			}
			else
			{
				z.TakeDamage(Tavel.ZombieEffectiveHealth(z) + 1, 0u);
			}
		}
		Tavel.PlayAnim(p, "sa", ReanimLoopType.PlayOnceAndHold, 30f);
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		if (s.Mode == "ultimate")
		{
			if (Tavel.AnimTime(p) < 0.999f)
			{
				return;
			}
			var (planted, empty) = CandidateCells(p);
			foreach ((int x, int y) in planted.Concat(empty))
			{
				Summon(p, x, y);
			}
			s.Mode = "stand";
			s.NextSummon = p.mBoard.mMainCounter + SummonInterval;
			Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			return;
		}
		if (p.mBoard.mMainCounter < s.NextSummon)
		{
			return;
		}
		var (pl, em) = CandidateCells(p);
		List<(int, int)> candidates = pl.Count > 0 ? pl : em;
		if (candidates.Count > 0)
		{
			(int cx, int cy) = candidates[Tavel.Rand(0, candidates.Count - 1)];
			Summon(p, cx, cy);
		}
		s.NextSummon = p.mBoard.mMainCounter + SummonInterval;
	}

	private static Zombie SmallFindTarget(Plant p)
	{
		int tx = p.mPlantCol + 1;
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (!Tavel.Alive(z) || z.mRow != p.mRow)
			{
				continue;
			}
			int col = p.mBoard.PixelToGridXKeepOnBoard((int)z.mPosX, (int)z.mPosY);
			if (p.mPlantCol <= col && col <= tx)
			{
				return z;
			}
		}
		return null;
	}

	private static void UpdateSmall(Plant p)
	{
		SmallState s = GetSmall(p);
		if (s.Mode == "attack")
		{
			if (Tavel.AnimTime(p) < 0.999f)
			{
				return;
			}
			Zombie t = s.Target;
			if (Tavel.Alive(t))
			{
				if (Tavel.IsBoss(t) || Tavel.IsGiantZombie(t))
				{
					t.TakeDamage(SmallHeavyDamage, 0u);
				}
				else
				{
					t.TakeDamage(Tavel.ZombieEffectiveHealth(t) + 1, 0u);
				}
			}
			s.Mode = "stand";
			s.Target = null;
			Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			return;
		}
		Zombie target = SmallFindTarget(p);
		if (target != null)
		{
			s.Mode = "attack";
			s.Target = target;
			Tavel.PlayAnim(p, "attack", ReanimLoopType.PlayOnceAndHold, 30f);
		}
	}
}
