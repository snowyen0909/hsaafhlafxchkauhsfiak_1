// 桑葚爆破手 (plant_ssbps.py): bombs with different effects; the ultimate lobs six at random zombies.
using System;
using System.Collections.Generic;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public List<TavelSsbps.Bomb> SsbpsBombs = new List<TavelSsbps.Bomb>();

	public int SsbpsLastUpdate = -1;
}

public static class TavelSsbps
{
	public const int Id = 104;

	public static SeedType Seed => (SeedType)Id;

	public const int SaFrames = 90;

	public const int SaFireStart = 40;

	public const int SaFireEnd = 90;

	public const int SaTargetCount = 6;

	public const float ProjectileSpeed = 3.33f;

	public const int UltimateFlightTime = 60;

	public const float UltimateArcHeight = 120f;

	public const int PoisonDuration = 600;

	public const int PoisonTickInterval = 60;

	public const int PoisonDamagePercent = 20;

	public const int PoisonSlowPercent = 30;

	public const int Normal = 0;

	public const int Poison = 1;

	public const int Flash = 2;

	public const int Big = 3;

	public sealed class Bomb
	{
		public Plant Plant;

		public int Kind;

		public float X;

		public float Y;

		public int Row;

		public Zombie Target;

		public bool Ultimate;

		public float StartX;

		public float StartY;

		public float TargetX;

		public float TargetY;

		public int FlightAge;

		public int FlightTime;

		public float Rotation;

		public bool Dead;
	}

	public sealed class State
	{
		public string Mode = "stand";

		public int NextAttack;

		public int SaFired;

		public List<Zombie> SaTargets = new List<Zombie>();
	}

	private static TavelSpec sSpec;

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	private static void Align(Plant p) => Tavel.Align(p, 1.7f, -32f, -57f);

	public static TavelSpec Spec()
	{
		sSpec = new TavelSpec(Id, "ssbps")
		{
			Name = "桑葚爆破手",
			Cost = 150,
			Card = "tavel/ssbps/card.png",
			AlmanacText = "桑葚爆破手会投掷具有不同效果的爆弹。\n{SHORTLINE}\n{KEYWORD}伤害：{STAT}大\n{KEYWORD}特点：{STAT}特殊爆炸会波及3×3范围内的僵尸，并产生不同效果\n{KEYWORD}大招：{STAT}向场上随机6只僵尸投掷爆弹\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}150\n{SHORTLINE}\n{FLAVOR}我准备了一些妙趣横生的手雷，这会产生一些妙趣横生的效果",
			DefName = "SSBPS",
			DefCost = 150,
			LaunchRate = 90,
			ReanimFile = "tavel/spanim/ssbps",
			CustomBody = true,
			Preview = new TavelPreview("anim_stand", 0.72f, -35f, -88f, 1.7f, -22f, -72f),
			OnPlanted = delegate(Plant p, int x, int y)
			{
				Get(p);
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
				if (p.mDead || p.mSquished)
				{
					Cleanup(p);
					return;
				}
				if (p.mBoard != null)
				{
					Update(p);
					UpdateBombs(p.mBoard);
				}
				Align(p);
			},
			OnDie = Cleanup,
			OnSquished = delegate(Plant p, object s)
			{
				Cleanup(p);
			},
			CanUltimate = (Plant p) => Get(p).Mode != "ultimate",
			UltimateActive = (Plant p) => p.mTavel is State s && s.Mode == "ultimate",
			StartUltimate = StartUltimate,
			DrawBoard = DrawBombs
		};
		return sSpec;
	}

	private static Image BombImage(int theKind, float theRotation)
	{
		int frame = (int)(theRotation * (8.0 / Math.PI) + 0.5) & 15;
		string name = theKind switch
		{
			Poison => "poison",
			Flash => "flash",
			Big => "big",
			_ => "normal",
		};
		return Tavel.LoadImage("tavel/ssbps_rotated/" + name + "_" + frame.ToString("00") + ".png");
	}

	private static void Cleanup(Plant p)
	{
		if (p.mTavel is State)
		{
			p.mTavel = null;
		}
		if (p.mBoard?.mTavel == null)
		{
			return;
		}
		foreach (Bomb b in p.mBoard.mTavel.SsbpsBombs)
		{
			if (b.Plant == p)
			{
				b.Dead = true;
			}
		}
	}

	private static bool ValidTarget(Zombie z)
	{
		if (!Tavel.Alive(z))
		{
			return false;
		}
		return !Tavel.IsBoss(z) || TavelDmxj.BossCanBeHit(z);
	}

	private static bool HasTarget(Plant p)
	{
		// the game's own shooter targeting first
		Zombie t = p.FindTargetZombie(p.mRow, PlantWeapon.Primary);
		if (t != null && ValidTarget(t))
		{
			return true;
		}
		// it can answer with an invulnerable Zomboss before another valid zombie: search the lane
		uint flags = (uint)p.GetDamageRangeFlags(PlantWeapon.Primary);
		TRect attack = p.GetPlantAttackRect(PlantWeapon.Primary);
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (!ValidTarget(z) || z.mMindControlled || !z.EffectedByDamage(flags))
			{
				continue;
			}
			if (!Tavel.IsBoss(z) && z.mRow != p.mRow)
			{
				continue;
			}
			if (GameConstants.GetRectOverlap(attack, z.GetZombieRect()) > 0)
			{
				return true;
			}
		}
		return false;
	}

	private static int ChooseKind()
	{
		int roll = Tavel.Rand(1, 100);
		if (roll <= 65)
		{
			return Normal;
		}
		if (roll <= 80)
		{
			return Poison;
		}
		if (roll <= 95)
		{
			return Flash;
		}
		return Big;
	}

	private static void AddBomb(Plant p, int theKind)
	{
		float x = p.mBoard.GridToPixelX(p.mPlantCol, p.mRow) + 58;
		float y = p.mBoard.GetPosYBasedOnRow(x, p.mRow) + 18;
		Tavel.State(p.mBoard).SsbpsBombs.Add(new Bomb { Plant = p, Kind = theKind, X = x, Y = y, Row = p.mRow });
	}

	private static void AddUltimateBomb(Plant p, Zombie theTarget)
	{
		float sx = p.mBoard.GridToPixelX(p.mPlantCol, p.mRow) + 58;
		float sy = p.mBoard.GetPosYBasedOnRow(sx, p.mRow) + 18;
		float tx;
		float ty;
		if (theTarget != null && ValidTarget(theTarget))
		{
			TRect r = theTarget.GetZombieRect();
			tx = r.mX + r.mWidth * 0.5f;
			ty = r.mY + r.mHeight * 0.5f;
		}
		else
		{
			int col = Tavel.Rand(1, 8);
			int row = Tavel.Rand(0, 4);
			tx = p.mBoard.GridToPixelX(col, row) + 40;
			ty = p.mBoard.GetPosYBasedOnRow(tx, row) + 18;
		}
		Tavel.State(p.mBoard).SsbpsBombs.Add(new Bomb
		{
			Plant = p,
			Kind = Normal,
			X = sx,
			Y = sy,
			Row = p.mRow,
			Target = theTarget,
			Ultimate = true,
			StartX = sx,
			StartY = sy,
			TargetX = tx,
			TargetY = ty,
			FlightTime = UltimateFlightTime
		});
	}

	private static List<Zombie> AreaTargets(Board b, float theX, float theY, int theRadius)
	{
		int gx = b.PixelToGridXKeepOnBoard((int)theX, (int)theY);
		int gy = b.PixelToGridYKeepOnBoard((int)theX, (int)theY);
		List<Zombie> list = new List<Zombie>();
		foreach (Zombie z in b.mZombies)
		{
			if (!ValidTarget(z))
			{
				continue;
			}
			if (Tavel.IsBoss(z))
			{
				// the boss has no reliable cell: use his head's hit box
				TRect r = z.GetZombieRect();
				float pad = theRadius * 80f;
				if (r.mX - pad <= theX && theX <= r.mX + r.mWidth + pad && r.mY - pad <= theY && theY <= r.mY + r.mHeight + pad)
				{
					list.Add(z);
				}
				continue;
			}
			int zx = b.PixelToGridXKeepOnBoard((int)z.mPosX, (int)z.mPosY);
			if (Math.Abs(zx - gx) <= theRadius && Math.Abs(z.mRow - gy) <= theRadius)
			{
				list.Add(z);
			}
		}
		return list;
	}

	private static void ApplyPoison(Zombie z, int theMaximumHealth)
	{
		TavelZombie t = TavelHooks.ZState(z);
		t.SsbpsPoisoned = true;
		t.SsbpsRemaining = PoisonDuration;
		t.SsbpsNextTick = PoisonTickInterval;
		t.SsbpsMaximumHealth = theMaximumHealth;
	}

	private static void Explode(Bomb s, float theX, float theY, Zombie thePrimary)
	{
		Board b = s.Plant.mBoard;
		int radius = s.Kind == Normal ? 0 : 1;
		List<Zombie> targets;
		if (thePrimary != null && ValidTarget(thePrimary))
		{
			// roof y does not map back to a cell: anchor on the zombie that was hit
			int tc = b.PixelToGridXKeepOnBoard((int)thePrimary.mPosX, (int)thePrimary.mPosY);
			int tr = thePrimary.mRow;
			targets = new List<Zombie>();
			foreach (Zombie z in b.mZombies)
			{
				if (!ValidTarget(z))
				{
					continue;
				}
				if (Tavel.IsBoss(z))
				{
					if (z == thePrimary)
					{
						targets.Add(z);
					}
					continue;
				}
				int zc = b.PixelToGridXKeepOnBoard((int)z.mPosX, (int)z.mPosY);
				if (Math.Abs(zc - tc) <= radius && Math.Abs(z.mRow - tr) <= radius)
				{
					targets.Add(z);
				}
			}
		}
		else
		{
			targets = AreaTargets(b, theX, theY, radius);
		}
		foreach (Zombie z in targets)
		{
			switch (s.Kind)
			{
			case Normal:
				z.TakeDamage(180, 0u);
				break;
			case Poison:
			{
				int max = Tavel.ZombieMaximumHealth(z);
				z.TakeDamage(360, 0u);
				if (Tavel.Alive(z))
				{
					ApplyPoison(z, max);
				}
				break;
			}
			case Flash:
				z.TakeDamage(360, 0u);
				if (Tavel.Alive(z))
				{
					z.ApplyButter();
					z.mButteredCounter = Math.Max(z.mButteredCounter, 240);
				}
				break;
			default:
				if (!Tavel.IsBoss(z) && !Tavel.IsGiantZombie(z))
				{
					z.TakeDamage(Tavel.ZombieEffectiveHealth(z) + 1, 0u);
				}
				else
				{
					z.TakeDamage(1600, 0u);
				}
				break;
			}
		}
	}

	private static void UpdateBombs(Board b)
	{
		TavelBoard bs = Tavel.State(b);
		if (bs.SsbpsLastUpdate == b.mMainCounter)
		{
			return;
		}
		bs.SsbpsLastUpdate = b.mMainCounter;
		float rightEdge = b.mWidth + 40;
		foreach (Bomb s in bs.SsbpsBombs.ToArray())
		{
			Plant p = s.Plant;
			if (s.Dead || p == null || p.mDead || p.mBoard != b)
			{
				s.Dead = true;
				continue;
			}
			if (s.Ultimate)
			{
				s.FlightAge++;
				float progress = Math.Min(1f, s.FlightAge / (float)s.FlightTime);
				s.X = s.StartX + (s.TargetX - s.StartX) * progress;
				float groundY = s.StartY + (s.TargetY - s.StartY) * progress;
				s.Y = groundY - UltimateArcHeight * 4f * progress * (1f - progress);
				if (progress >= 1f)
				{
					Explode(s, s.TargetX, s.TargetY, s.Target);
					s.Dead = true;
				}
				continue;
			}
			Zombie target = s.Target;
			if (target != null && ValidTarget(target))
			{
				TRect r = target.GetZombieRect();
				float tx = r.mX + r.mWidth * 0.5f;
				float ty = r.mY + r.mHeight * 0.5f;
				float dx = tx - s.X;
				float dy = ty - s.Y;
				float dist = (float)Math.Sqrt(dx * dx + dy * dy);
				if (dist <= ProjectileSpeed * 2f)
				{
					Explode(s, tx, ty, target);
					s.Dead = true;
					continue;
				}
				s.X += ProjectileSpeed * dx / dist;
				s.Y += ProjectileSpeed * dy / dist;
			}
			else
			{
				s.X += ProjectileSpeed;
				s.Y = b.GetPosYBasedOnRow(s.X, s.Row) + 18;
			}
			s.Rotation += 0.22f;
			if (s.X > rightEdge)
			{
				s.Dead = true;
				continue;
			}
			if (target != null)
			{
				continue;
			}
			foreach (Zombie z in b.mZombies.ToArray())
			{
				if (!ValidTarget(z) || (!Tavel.IsBoss(z) && z.mRow != s.Row))
				{
					continue;
				}
				TRect r = z.GetZombieRect();
				// horizontal contact plus the row: stable on lawns and roof slopes alike
				if (r.mX - 20 <= s.X && s.X <= r.mX + r.mWidth + 20)
				{
					Explode(s, s.X, s.Y, z);
					s.Dead = true;
					break;
				}
			}
		}
		bs.SsbpsBombs.RemoveAll((Bomb s) => s.Dead);
	}

	private static void DrawBombs(Board b, Graphics g)
	{
		if (b.mTavel == null)
		{
			return;
		}
		foreach (Bomb s in b.mTavel.SsbpsBombs)
		{
			if (s.Dead || s.Plant.mBoard != b)
			{
				continue;
			}
			Image img = BombImage(s.Kind, s.Ultimate ? 0f : s.Rotation);
			if (img != null)
			{
				g.DrawImageF(img, s.X - img.mWidth * 0.5f, s.Y + 60f * Constants.S - img.mHeight * 0.5f);
			}
		}
	}

	private static void StartUltimate(Plant p)
	{
		State s = Get(p);
		s.Mode = "ultimate";
		s.SaFired = 0;
		List<Zombie> candidates = new List<Zombie>();
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (ValidTarget(z))
			{
				candidates.Add(z);
			}
		}
		s.SaTargets = new List<Zombie>();
		for (int i = 0; i < SaTargetCount; i++)
		{
			s.SaTargets.Add(candidates.Count > 0 ? candidates[Tavel.Rand(0, candidates.Count - 1)] : null);
		}
		Tavel.PlayAnim(p, "sa", ReanimLoopType.PlayOnceAndHold, 30f);
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		float t = Tavel.Body(p) == null ? 1f : Tavel.AnimTime(p);
		int frame = p.mBoard.mMainCounter;
		switch (s.Mode)
		{
		case "attack":
			if (t >= 0.999f)
			{
				AddBomb(p, ChooseKind());
				// a whole stand cycle is the recovery after every throw
				s.Mode = "attack_recovery";
				Tavel.PlayAnim(p, "stand", ReanimLoopType.PlayOnceAndHold, 30f);
			}
			return;
		case "attack_recovery":
			if (t >= 0.999f)
			{
				s.Mode = "stand";
				s.NextAttack = frame;
				Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			}
			return;
		case "ultimate":
		{
			int animFrame = (int)(t * SaFrames + 0.5f);
			int count = s.SaTargets.Count;
			while (s.SaFired < count)
			{
				int fireFrame = SaFireStart + (SaFireEnd - SaFireStart) * s.SaFired / Math.Max(1, count);
				if (animFrame < fireFrame)
				{
					break;
				}
				AddUltimateBomb(p, s.SaTargets[s.SaFired]);
				s.SaFired++;
			}
			if (t >= 0.999f)
			{
				s.Mode = "stand";
				s.NextAttack = frame + 90;
				Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			}
			return;
		}
		}
		if (frame >= s.NextAttack && HasTarget(p))
		{
			s.Mode = "attack";
			Tavel.PlayAnim(p, "attack", ReanimLoopType.PlayOnceAndHold, 30f);
		}
	}
}
