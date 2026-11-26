// 超级机枪射手 (plant_super_gatling.py).
using System;
using System.Collections.Generic;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public static class TavelSuperGatling
{
	public const int Id = 101;

	public static SeedType Seed => (SeedType)Id;

	public const int PeaNormal = 0;

	public const int PeaSnow = 1;

	public const int PeaFire = 2;

	public const int PeaElectric = 3;

	public const int PeaPoison = 4;

	public const int PeaStone = 5;

	public const int BaseShots = 4;

	public const int MaxShots = 7;

	public const int BurstChance = 5;

	public const int BurstCount = 180;

	public const float BurstHalfAngle = 22.5f;

	public const int VolleyGap = 90;

	public const int ShotInterval = 6;

	public const int BurstPerFrame = 1;

	public const int AttackInterval = 90;

	public const int AttackAnimFrames = 65;

	public const int AttackFireFrame = 25;

	public const int BurstAnimFrames = 240;

	public const int BurstFireStart = 40;

	public const int BurstFireEnd = 220;

	public const int ElectricContactDamage = 80;

	public const int ElectricShockDamage = 120;

	public const int ElectricShockInterval = 9;

	public const int PoisonImpactDamage = 40;

	public const int PoisonDuration = 300;

	public const int PoisonTickInterval = 60;

	public const int PoisonDamagePercent = 2;

	public const int PoisonSlowPercent = 25;

	public const int StoneImpactDamage = 80;

	public const int StoneStunChance = 25;

	public const int StoneKnockbackChance = 25;

	public const int StoneExecuteChance = 2;

	public const int StoneStunDuration = 60;

	public const int StoneKnockbackDistance = 80;

	public sealed class State
	{
		public int Shots = BaseShots;

		public int LastFireFrame = -1000;

		public int VolleyKind;

		public int VolleyFires;

		public bool Burst;

		public int PendingShots;

		public int NextShotFrame;

		public int BurstRemaining;

		public int BurstIndex;

		public int BurstFireStartFrame;

		public int BurstFireEndFrame;

		public int AttackAnimUntil;

		public int NextAttackFrame;
	}

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	public static Image PeaImage(int theKind)
	{
		return theKind switch
		{
			PeaFire => Tavel.LoadImage("tavel/super_gatling/fire_pea.png"),
			PeaElectric => Tavel.LoadImage("tavel/super_gatling/electric_pea.png"),
			PeaPoison => Tavel.LoadImage("tavel/super_gatling/venom_pea.png"),
			PeaStone => Tavel.LoadImage("tavel/super_gatling/stone_pea.png"),
			_ => null,
		};
	}

	public static TavelSpec Spec()
	{
		return new TavelSpec(Id, "super_gatling")
		{
			Name = "超级机枪射手",
			Cost = 400,
			Card = "tavel/super_gatling/card.png",
			AlmanacText = "超级机枪射手会连续发射多颗随机属性豌豆。\n{SHORTLINE}\n{KEYWORD}伤害：{STAT}因子弹类型而异\n{KEYWORD}发射数量：{STAT}4-7颗\n{KEYWORD}特点：{STAT}随机发射六种豌豆\n{KEYWORD}大招：{STAT}向前方扇形区域发射180颗随机豌豆\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}400\n{SHORTLINE}\n{FLAVOR}更大的枪，更密集的火力，以及足以覆盖整片草坪的豌豆风暴。",
			DefName = "SUPER_GATLING_PEA",
			DefCost = 400,
			LaunchRate = AttackInterval,
			ReanimFile = "tavel/spanim/super_gatlingpea",
			Preview = new TavelPreview("anim_stand", 1.375f, -48f, -109f, 1.375f, -28f, -83f),
			OnPlanted = delegate(Plant p, int x, int y)
			{
				Get(p);
				if (p.mBoard != null)
				{
					Tavel.StartPacketCooldown(p.mBoard, Seed);
				}
				p.PlayBodyReanim("anim_stand", ReanimLoopType.Loop, 0, 30f);
				Tavel.Align(p, 1.375f, -28f, -83f);
			},
			OnUpdate = delegate(Plant p)
			{
				if (p.mBoard != null)
				{
					UpdateAttack(p);
				}
				// animation changes can overwrite the root transform
				Tavel.Align(p, 1.375f, -28f, -83f);
			},
			CanUltimate = (Plant p) => Get(p).BurstRemaining == 0,
			UltimateActive = (Plant p) => p.mTavel is State s && s.BurstRemaining > 0,
			StartUltimate = delegate(Plant p)
			{
				StartBurst(p, Get(p), p.mBoard.mMainCounter);
			}
		};
	}

	public static Projectile AddProjectile(Plant p, int theKind, float theOffsetY = 0f, int theRow = -1)
	{
		ProjectileType type = theKind == PeaSnow ? ProjectileType.Snowpea : ProjectileType.Pea;
		if (theRow < 0)
		{
			theRow = p.mRow;
		}
		Projectile pr = p.mBoard.AddProjectile(p.mX + 45, (int)(p.mY + 10 + theOffsetY), p.mRenderOrder - 1, theRow, type);
		pr.mDamageRangeFlags = p.GetDamageRangeFlags(PlantWeapon.Primary);
		pr.mFromPlant = p.mSeedType;
		pr.mVelY = 0f;
		TavelShot s = TavelHooks.Shot(pr);
		s.SuperGatling = true;
		if (theKind >= PeaFire)
		{
			s.PeaKind = theKind;
		}
		return pr;
	}

	private static void FireBurstShot(Plant p)
	{
		// every shot picks its own angle inside the 45-degree fan
		int kind = Tavel.Rand(PeaNormal, PeaStone);
		float angle = Tavel.RandF(-BurstHalfAngle, BurstHalfAngle);
		float vy = 3.33f * (float)Math.Tan(angle * Math.PI / 180.0);
		Projectile pr = AddProjectile(p, kind, 0f, p.mRow);
		TavelShot s = TavelHooks.Shot(pr);
		s.Fan = true;
		s.FanVelocityY = vy;
		s.FanScreenY = pr.mPosY;
	}

	public static void StartBurst(Plant p, State s, int theFrame)
	{
		s.PendingShots = 0;
		s.Burst = true;
		s.BurstRemaining = BurstCount;
		s.BurstIndex = 0;
		s.BurstFireStartFrame = theFrame + BurstFireStart;
		s.BurstFireEndFrame = theFrame + BurstFireEnd;
		s.Shots = Math.Min(MaxShots, s.Shots + 1);
		s.LastFireFrame = theFrame;
		p.PlayBodyReanim("anim_sa", ReanimLoopType.PlayOnceAndHold, 5, 30f);
		s.AttackAnimUntil = theFrame + BurstAnimFrames;
	}

	private static void UpdateFire(Plant p, State s)
	{
		int frame = p.mBoard.mMainCounter;
		if (s.PendingShots > 0 && frame >= s.NextShotFrame)
		{
			AddProjectile(p, s.VolleyKind);
			s.PendingShots--;
			s.NextShotFrame = frame + ShotInterval;
		}
		if (s.BurstRemaining > 0 && frame >= s.BurstFireStartFrame && frame < s.BurstFireEndFrame)
		{
			int n = Math.Min(BurstPerFrame, s.BurstRemaining);
			for (int i = 0; i < n; i++)
			{
				FireBurstShot(p);
				s.BurstIndex++;
				s.BurstRemaining--;
			}
			if (s.BurstRemaining == 0)
			{
				s.NextAttackFrame = frame + AttackInterval;
			}
		}
	}

	private static void UpdateAttack(Plant p)
	{
		State s = Get(p);
		int frame = p.mBoard.mMainCounter;
		UpdateFire(p, s);
		if (s.BurstRemaining == 0 && s.PendingShots == 0 && frame >= s.NextAttackFrame && (s.AttackAnimUntil == 0 || frame >= s.AttackAnimUntil) && p.FindTargetZombie(p.mRow, PlantWeapon.Primary) != null)
		{
			Fire(p);
			s.NextAttackFrame = frame + AttackInterval;
		}
		if (s.AttackAnimUntil > 0 && frame >= s.AttackAnimUntil)
		{
			p.PlayBodyReanim("anim_stand", ReanimLoopType.Loop, 10, 30f);
			s.AttackAnimUntil = 0;
		}
	}

	/// <summary>Plant.Fire for the super gatling: starts a volley (or, rarely, the burst).</summary>
	public static void Fire(Plant p)
	{
		State s = Get(p);
		int frame = p.mBoard.mMainCounter;
		if (frame - s.LastFireFrame >= VolleyGap)
		{
			s.VolleyFires = 0;
			s.Burst = Tavel.Rand(1, 100) <= BurstChance;
			s.VolleyKind = Tavel.Rand(PeaNormal, PeaStone);
			if (s.Burst)
			{
				StartBurst(p, s, frame);
			}
			else
			{
				s.PendingShots = s.Shots;
				p.PlayBodyReanim("anim_attack", ReanimLoopType.PlayOnceAndHold, 5, 30f);
				s.AttackAnimUntil = frame + AttackAnimFrames;
				s.NextShotFrame = frame + AttackFireFrame;
			}
		}
		s.LastFireFrame = frame;
		s.VolleyFires++;
	}

	// ------------------------------------------------------------------ the peas

	public static void ApplyPoison(Zombie z)
	{
		TavelZombie t = TavelHooks.ZState(z);
		t.Poisoned = true;
		t.PoisonRemaining = PoisonDuration;
		t.PoisonNextTick = PoisonTickInterval;
	}

	public static bool IsValidElectricTarget(Projectile pr, Zombie z)
	{
		return Tavel.Alive(z) && z.EffectedByDamage((uint)pr.mDamageRangeFlags);
	}

	private static bool IsValidShockTarget(Projectile pr, Zombie z)
	{
		if (!Tavel.Alive(z))
		{
			return false;
		}
		// arcs are not pea collisions: height and tunnels do not protect from them
		if (z.mZombieType == ZombieType.Balloon || (z.mZombieType == ZombieType.Digger && z.mZombiePhase == ZombiePhase.DiggerTunneling))
		{
			return true;
		}
		return z.EffectedByDamage((uint)pr.mDamageRangeFlags);
	}

	public static void DoElectricDamage(Zombie z, int theDamage, uint theFlags, bool theAshOnKill)
	{
		if (theAshOnKill && Tavel.ZombieEffectiveHealth(z) <= theDamage)
		{
			z.ApplyBurn();
		}
		else
		{
			z.TakeDamage(theDamage, theFlags);
		}
	}

	private static List<Zombie> ZombiesInElectricArea(Projectile pr, float theX, float theY, HashSet<Zombie> theExcluded)
	{
		Board b = pr.mBoard;
		int gx = b.PixelToGridXKeepOnBoard((int)theX, (int)theY);
		int gy = b.PixelToGridYKeepOnBoard((int)theX, (int)theY);
		List<Zombie> list = new List<Zombie>();
		foreach (Zombie z in b.mZombies)
		{
			if (theExcluded.Contains(z) || !IsValidShockTarget(pr, z))
			{
				continue;
			}
			int zx = b.PixelToGridXKeepOnBoard((int)z.mPosX, (int)z.mPosY);
			if (Math.Abs(zx - gx) <= 1 && Math.Abs(z.mRow - gy) <= 1)
			{
				list.Add(z);
			}
		}
		return list;
	}

	public static void ShockRandomZombie(Projectile pr)
	{
		HashSet<Zombie> shocked = new HashSet<Zombie>();
		List<Zombie> targets = ZombiesInElectricArea(pr, pr.mPosX, pr.mPosY, shocked);
		if (targets.Count == 0)
		{
			return;
		}
		Zombie target = targets[Tavel.Rand(0, targets.Count - 1)];
		while (target != null)
		{
			shocked.Add(target);
			bool conducts = Tavel.IsWearingMetalArmor(target);
			DoElectricDamage(target, ElectricShockDamage, pr.GetDamageFlags(target), theAshOnKill: true);
			if (!conducts)
			{
				break;
			}
			List<Zombie> near = ZombiesInElectricArea(pr, target.mPosX, target.mPosY, shocked);
			if (near.Count == 0)
			{
				break;
			}
			target = near[Tavel.Rand(0, near.Count - 1)];
		}
	}

	public static void StoneImpact(Projectile pr, Zombie z)
	{
		if (z == null)
		{
			pr.DoImpactCore(z);
			return;
		}
		// rolled before the ordinary impact can remove the target
		bool boss = Tavel.IsBoss(z);
		bool execute = !boss && !Tavel.IsGiantZombie(z) && Tavel.Rand(1, 100) <= StoneExecuteChance;
		bool stun = !boss && Tavel.Rand(1, 100) <= StoneStunChance;
		bool knockback = !boss && Tavel.Rand(1, 100) <= StoneKnockbackChance;
		pr.DoImpactCore(z);
		if (!Tavel.Alive(z))
		{
			return;
		}
		if (execute)
		{
			z.TakeDamage(Tavel.ZombieEffectiveHealth(z) + 1, pr.GetDamageFlags(z));
			return;
		}
		z.TakeDamage(StoneImpactDamage - 20, pr.GetDamageFlags(z));
		if (!Tavel.Alive(z))
		{
			return;
		}
		if (stun)
		{
			z.ApplyButter();
			z.mButteredCounter = StoneStunDuration;
		}
		if (knockback)
		{
			float lawnRight = pr.mBoard.GridToPixelX(9, z.mRow) + 80;
			if (z.mPosX < lawnRight)
			{
				z.mPosX = Math.Min(lawnRight, z.mPosX + StoneKnockbackDistance);
				z.mX = (int)z.mPosX;
			}
		}
	}

	public static void PoisonImpact(Projectile pr, Zombie z)
	{
		// Zomboss can close his head during the impact: poison him first
		bool poisonBoss = z != null && Tavel.IsBoss(z) && Tavel.Alive(z);
		if (poisonBoss)
		{
			ApplyPoison(z);
		}
		pr.DoImpactCore(z);
		if (Tavel.Alive(z))
		{
			z.TakeDamage(PoisonImpactDamage - 20, pr.GetDamageFlags(z));
			if (!poisonBoss && Tavel.Alive(z))
			{
				ApplyPoison(z);
			}
		}
	}
}
