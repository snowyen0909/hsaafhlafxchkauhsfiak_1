// 牛蒡击球手 (plant_nbjqs.py): bats zombies next to her and hits passing peas into stronger shots.
using System;
using System.Collections.Generic;
using System.Linq;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public List<TavelNbjqs.Meteor> NbjqsMeteors = new List<TavelNbjqs.Meteor>();

	public List<TavelNbjqs.SaShot> NbjqsSaShots = new List<TavelNbjqs.SaShot>();
}

public sealed partial class TavelShot
{
	/// <summary>A pea batted by 牛蒡击球手 (null: not batted).</summary>
	public TavelNbjqs.Boost Boosted;
}

public static class TavelNbjqs
{
	public const int Id = 106;

	public static SeedType Seed => (SeedType)Id;

	public const int AttackFrames = 40;

	public const int HitFrame = 20;

	public const int BoostStart = 0;

	public const int BoostEnd = 10;

	public const int MeleeDamage = 100;

	public const int UltimateGiantDamage = 600;

	public const int SaBulletDamage = 1800;

	public sealed class Boost
	{
		public int Damage;

		public string Effect;

		public int Kind;

		public int NativeDamage;
	}

	public sealed class Meteor
	{
		public int Row;

		public float X;

		public float Y;

		public float TargetX;

		public float TargetY;

		public int Age;

		public int Duration;

		public int Damage;

		public bool Dead;
	}

	public sealed class SaShot
	{
		public int Row;

		public float X;

		public float Y;

		public float VX;

		public float VY;

		public HashSet<Zombie> Hit = new HashSet<Zombie>();

		public bool Dead;
	}

	public sealed class State
	{
		public string Mode = "stand";

		public bool Ultimate;

		public Zombie Target;

		public bool HitDone;

		public HashSet<Projectile> Boosted = new HashSet<Projectile>();

		public List<Projectile> Pending = new List<Projectile>();

		public Dictionary<Projectile, float> PendingVelocities = new Dictionary<Projectile, float>();
	}

	private static TavelSpec sSpec;

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	// doubled around the export's top-centre anchor
	private static void Align(Plant p) => Tavel.Align(p, 1.728f, -133.2f, -182.9f);

	public static TavelSpec Spec()
	{
		sSpec = new TavelSpec(Id, "nbjqs")
		{
			Name = "牛蒡击球手",
			Cost = 100,
			Card = "tavel/nbjqs/card.png",
			AlmanacText = "牛蒡击球手会攻击附近的僵尸，也会强化经过身边的豌豆。\n{SHORTLINE}\n{KEYWORD}伤害：{STAT}100\n{KEYWORD}特点：{STAT}强化普通、寒冰、火焰和石块豌豆\n{KEYWORD}大招：{STAT}进入强化形态，下一次攻击可秒杀普通僵尸\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}100\n{SHORTLINE}\n{FLAVOR}只要球棒挥得够准，豌豆也能打出全垒打。",
			DefName = "NBJQS",
			DefCost = 100,
			LaunchRate = 60,
			ReanimFile = "tavel/spanim/nbjqs",
			CustomBody = true,
			Preview = new TavelPreview("anim_stand", 1.728f, -134.2f, -188.9f, 1.728f, -133.2f, -182.9f),
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
					DropState(p);
					return;
				}
				if (p.mBoard != null)
				{
					Update(p);
				}
				Align(p);
			},
			OnDie = DropState,
			OnSquished = delegate(Plant p, object s)
			{
				if (s is State st)
				{
					ReleaseAll(st);
				}
			},
			// arming her may interrupt any swing; no second stock while already armed
			CanUltimate = (Plant p) => !Get(p).Ultimate,
			UltimateActive = (Plant p) => p.mTavel is State s && s.Ultimate,
			StartUltimate = StartUltimate,
			UpdateBoard = delegate(Board b)
			{
				UpdateMeteors(b);
				UpdateSaShots(b);
			},
			DrawBoard = DrawEffects
		};
		return sSpec;
	}

	private static void DropState(Plant p)
	{
		if (p.mTavel is State s)
		{
			ReleaseAll(s);
			p.mTavel = null;
		}
	}

	private static Zombie FindMeleeTarget(Plant p)
	{
		if (p.mBoard == null)
		{
			return null;
		}
		float left = p.mX - 40;
		float right = p.mX + 120;
		Zombie best = null;
		float bestD = 1000000f;
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (!Tavel.Alive(z) || z.mMindControlled || z.mRow != p.mRow)
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float cx = r.mX + r.mWidth * 0.5f;
			if (left <= cx && cx <= right)
			{
				float d = Math.Abs(cx - (p.mX + 40));
				if (d < bestD)
				{
					best = z;
					bestD = d;
				}
			}
		}
		return best;
	}

	/// <summary>The kind of pea she can bat (-1: she cannot).</summary>
	private static int PeaKind(Projectile pr)
	{
		int custom = pr.mTavel?.PeaKind ?? -1;
		if (custom == TavelSuperGatling.PeaElectric || custom == TavelSuperGatling.PeaPoison)
		{
			return -1;
		}
		if (custom == TavelSuperGatling.PeaFire || custom == TavelSuperGatling.PeaStone)
		{
			return custom;
		}
		return pr.mProjectileType switch
		{
			ProjectileType.Pea => TavelSuperGatling.PeaNormal,
			ProjectileType.Snowpea => TavelSuperGatling.PeaSnow,
			ProjectileType.Fireball => TavelSuperGatling.PeaFire,
			_ => -1,
		};
	}

	private static int OriginalDamage(int theKind)
	{
		if (theKind == TavelSuperGatling.PeaFire)
		{
			return 40;
		}
		if (theKind == TavelSuperGatling.PeaStone)
		{
			return TavelSuperGatling.StoneImpactDamage;
		}
		return 20;
	}

	private static bool InCell(Plant p, Projectile pr)
	{
		if (pr.mDead || pr.mRow != p.mRow)
		{
			return false;
		}
		TRect r = pr.GetProjectileRect();
		return r.mX + r.mWidth >= p.mX && r.mX <= p.mX + 80 && r.mY + r.mHeight >= p.mY && r.mY <= p.mY + 100;
	}

	private static List<Projectile> Eligible(Plant p)
	{
		return p.mBoard.mProjectiles.Where((Projectile pr) => PeaKind(pr) >= 0 && InCell(p, pr)).ToList();
	}

	private static void Queue(Plant p, State s)
	{
		s.Pending.RemoveAll((Projectile pr) => pr.mDead);
		HashSet<Projectile> queued = new HashSet<Projectile>(s.Pending);
		queued.UnionWith(s.Boosted);
		foreach (Projectile pr in Eligible(p))
		{
			if (queued.Contains(pr))
			{
				continue;
			}
			s.Pending.Add(pr);
			s.PendingVelocities[pr] = pr.mVelX;
			pr.mVelX = 0f;
			queued.Add(pr);
		}
	}

	private static void Release(State s, Projectile pr)
	{
		if (!pr.mDead)
		{
			pr.mVelX = s.PendingVelocities.TryGetValue(pr, out float v) ? v : 3.33f;
		}
		s.PendingVelocities.Remove(pr);
	}

	private static void ReleaseAll(State s)
	{
		foreach (Projectile pr in s.Pending)
		{
			Release(s, pr);
		}
		s.Pending = new List<Projectile>();
		s.PendingVelocities = new Dictionary<Projectile, float>();
	}

	private static Zombie NearestInRow(Board b, int theRow, float theStartX)
	{
		Zombie target = null;
		float bestX = 1000000f;
		foreach (Zombie z in b.mZombies)
		{
			if (!Tavel.Alive(z) || z.mMindControlled || z.mRow != theRow || (Tavel.IsBoss(z) && !TavelDmxj.BossCanBeHit(z)))
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float cx = r.mX + r.mWidth * 0.5f;
			if (cx >= theStartX && cx < bestX)
			{
				target = z;
				bestX = cx;
			}
		}
		return target;
	}

	private static void AddMeteor(Projectile pr, int theDamage)
	{
		Zombie target = NearestInRow(pr.mBoard, pr.mRow, pr.mPosX);
		if (target == null)
		{
			return;
		}
		TRect r = target.GetZombieRect();
		float tx = r.mX + r.mWidth * 0.5f;
		float ty = r.mY + r.mHeight * 0.5f;
		Tavel.State(pr.mBoard).NbjqsMeteors.Add(new Meteor { Row = pr.mRow, X = tx - 150f, Y = ty - 260f, TargetX = tx, TargetY = ty, Duration = 36, Damage = theDamage });
	}

	private static void AddSaShot(Plant p, float theAngle)
	{
		float a = theAngle * (float)Math.PI / 180f;
		Tavel.State(p.mBoard).NbjqsSaShots.Add(new SaShot { Row = p.mRow, X = p.mX + 65, Y = p.mY + 35, VX = (float)Math.Cos(a) * 4f, VY = (float)Math.Sin(a) * 4f });
	}

	private static void BoostProjectile(Plant p, Projectile pr, bool theUltimate)
	{
		int kind = PeaKind(pr);
		if (kind < 0)
		{
			return;
		}
		int damage = OriginalDamage(kind);
		if (theUltimate)
		{
			pr.Die();
			foreach (float angle in new[] { 0f, -15f, 15f })
			{
				AddSaShot(p, angle);
			}
			return;
		}
		int nativeDamage = pr.mProjectileType == ProjectileType.Fireball ? 40 : 20;
		int roll = Tavel.Rand(1, 100);
		if (roll <= 50)
		{
			TavelHooks.Shot(pr).Boosted = new Boost { Damage = damage, Effect = "knockback", Kind = kind, NativeDamage = nativeDamage };
			return;
		}
		if (roll <= 95)
		{
			TavelHooks.Shot(pr).Boosted = new Boost { Damage = damage, Effect = "stun", Kind = kind, NativeDamage = nativeDamage };
			Zombie target = NearestInRow(pr.mBoard, pr.mRow, pr.mPosX);
			float tx;
			float vOffset;
			if (target == null)
			{
				tx = pr.mBoard.mWidth;
				vOffset = 0f;
			}
			else
			{
				tx = target.ZombieTargetLeadX(50f) - 30f;
				vOffset = target.GetZombieRect().mY - pr.mPosY;
			}
			float distance = Math.Max(40f, tx - pr.mPosX);
			// the game's pult arc: land at the predicted zombie after 120 updates
			float flight = 120f;
			pr.mMotionType = ProjectileMotion.Lobbed;
			pr.mVelX = distance / flight;
			pr.mVelY = 0f;
			pr.mVelZ = -7f + vOffset / flight;
			pr.mAccZ = 0.115f;
			return;
		}
		AddMeteor(pr, damage * 5);
		pr.Die();
	}

	/// <summary>Projectile.DoImpact of a batted pea (true: handled).</summary>
	public static bool Impact(Projectile pr, TavelShot s, Zombie z)
	{
		Boost boosted = s.Boosted;
		if (boosted == null)
		{
			return false;
		}
		s.Boosted = null;
		if (z == null)
		{
			pr.DoImpactCore(z);
			return true;
		}
		if (Tavel.IsBoss(z) && !TavelDmxj.BossCanBeHit(z))
		{
			// stays batted: it may hit Zomboss once his head comes down
			s.Boosted = boosted;
			return true;
		}
		int baseDamage = boosted.Damage * 2;
		bool stoneExecute = false;
		bool stoneStun = false;
		bool stoneKnockback = false;
		if (boosted.Kind == TavelSuperGatling.PeaStone)
		{
			bool boss = Tavel.IsBoss(z);
			stoneExecute = !boss && !Tavel.IsGiantZombie(z) && Tavel.Rand(1, 100) <= TavelSuperGatling.StoneExecuteChance;
			stoneStun = !boss && Tavel.Rand(1, 100) <= TavelSuperGatling.StoneStunChance;
			stoneKnockback = !boss && Tavel.Rand(1, 100) <= TavelSuperGatling.StoneKnockbackChance;
		}
		pr.DoImpactCore(z);
		if (!Tavel.Alive(z))
		{
			return true;
		}
		if (stoneExecute)
		{
			z.TakeDamage(Tavel.ZombieEffectiveHealth(z) + 1, pr.GetDamageFlags(z));
			return true;
		}
		z.TakeDamage(Math.Max(0, baseDamage - boosted.NativeDamage), pr.GetDamageFlags(z));
		if (!Tavel.Alive(z))
		{
			return true;
		}
		if (stoneStun)
		{
			z.ApplyButter();
			z.mButteredCounter = Math.Max(z.mButteredCounter, TavelSuperGatling.StoneStunDuration);
		}
		float lawnRight = pr.mBoard.GridToPixelX(8, z.mRow) + 80;
		if (stoneKnockback && z.mPosX < lawnRight)
		{
			z.mPosX = Math.Min(lawnRight, z.mPosX + TavelSuperGatling.StoneKnockbackDistance);
			z.mX = (int)z.mPosX;
		}
		if (boosted.Effect == "knockback")
		{
			if (!Tavel.IsBoss(z) && z.mPosX < lawnRight)
			{
				z.mPosX = Math.Min(lawnRight, z.mPosX + 80f);
				z.mX = (int)z.mPosX;
			}
		}
		else if (!Tavel.IsBoss(z))
		{
			z.ApplyButter();
			z.mButteredCounter = Math.Max(z.mButteredCounter, 120);
		}
		return true;
	}

	private static void UpdateMeteors(Board b)
	{
		if (b.mTavel == null || b.mTavel.NbjqsMeteors.Count == 0)
		{
			return;
		}
		foreach (Meteor m in b.mTavel.NbjqsMeteors)
		{
			if (m.Dead)
			{
				continue;
			}
			m.Age++;
			float progress = Math.Min(1f, m.Age / (float)m.Duration);
			m.X += (m.TargetX - m.X) * 0.13f;
			m.Y += (m.TargetY - m.Y) * 0.13f;
			if (progress < 1f)
			{
				continue;
			}
			foreach (Zombie z in b.mZombies.ToArray())
			{
				if (!Tavel.Alive(z) || z.mRow != m.Row || (Tavel.IsBoss(z) && !TavelDmxj.BossCanBeHit(z)))
				{
					continue;
				}
				TRect r = z.GetZombieRect();
				if (Math.Abs(r.mX + r.mWidth * 0.5f - m.TargetX) <= 40f)
				{
					z.TakeDamage(m.Damage, 0u);
					if (Tavel.Alive(z) && !Tavel.IsBoss(z))
					{
						z.ApplyButter();
						z.mButteredCounter = 240;
					}
				}
			}
			m.Dead = true;
		}
		b.mTavel.NbjqsMeteors.RemoveAll((Meteor m) => m.Dead);
	}

	private static void UpdateSaShots(Board b)
	{
		if (b.mTavel == null || b.mTavel.NbjqsSaShots.Count == 0)
		{
			return;
		}
		foreach (SaShot s in b.mTavel.NbjqsSaShots)
		{
			if (s.Dead)
			{
				continue;
			}
			s.X += s.VX;
			s.Y += s.VY;
			foreach (Zombie z in b.mZombies.ToArray())
			{
				if (s.Hit.Contains(z) || !Tavel.Alive(z) || (Tavel.IsBoss(z) && !TavelDmxj.BossCanBeHit(z)))
				{
					continue;
				}
				TRect r = z.GetZombieRect();
				if (r.mX - 20 <= s.X && s.X <= r.mX + r.mWidth + 20 && r.mY - 20 <= s.Y && s.Y <= r.mY + r.mHeight + 20)
				{
					s.Hit.Add(z);
					if (Tavel.IsBoss(z) || Tavel.IsGiantZombie(z))
					{
						z.TakeDamage(SaBulletDamage, 0u);
					}
					else
					{
						z.TakeDamage(Tavel.ZombieEffectiveHealth(z) + 1, 0u);
					}
				}
			}
			if (s.X > b.mWidth + 80 || s.Y < -100f || s.Y > b.mHeight + 100)
			{
				s.Dead = true;
			}
		}
		b.mTavel.NbjqsSaShots.RemoveAll((SaShot s) => s.Dead);
	}

	private static void DrawEffects(Board b, Graphics g)
	{
		if (b.mTavel == null)
		{
			return;
		}
		Image meteor = b.mTavel.NbjqsMeteors.Count > 0 ? Tavel.LoadImage("tavel/nbjqs/bullet_fall.png") : null;
		if (meteor != null)
		{
			foreach (Meteor m in b.mTavel.NbjqsMeteors)
			{
				if (!m.Dead)
				{
					g.DrawImageF(meteor, m.X - meteor.mWidth * 0.5f, m.Y - meteor.mHeight * 0.5f);
				}
			}
		}
		Image bullet = b.mTavel.NbjqsSaShots.Count > 0 ? Tavel.LoadImage("tavel/nbjqs/sa-bullet.png") : null;
		if (bullet != null)
		{
			// the picture has a long trail: scale it so the head is about a melon's size
			float k = 60f / Math.Max(1f, bullet.mHeight);
			foreach (SaShot s in b.mTavel.NbjqsSaShots)
			{
				if (!s.Dead)
				{
					TodCommon.TodDrawImageScaledF(g, bullet, s.X - bullet.mWidth * k * 0.5f, s.Y - bullet.mHeight * k * 0.5f, k, k);
				}
			}
		}
	}

	private static void StartUltimate(Plant p)
	{
		State s = Get(p);
		// cancel what she is doing; peas held for the interrupted swing move on
		ReleaseAll(s);
		s.Ultimate = true;
		s.Mode = "stand";
		s.Target = null;
		s.HitDone = false;
		s.Boosted = new HashSet<Projectile>();
		Tavel.PlayAnim(p, "sa-stand", ReanimLoopType.Loop, 30f);
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		float t = Tavel.Body(p) == null ? 1f : Tavel.AnimTime(p);
		int animFrame = (int)(t * AttackFrames + 0.5f);
		string attack = s.Ultimate ? "sa-attack" : "attack";
		if (s.Mode == "attack_melee")
		{
			// peas arriving during a swing wait for the batting swing
			Queue(p, s);
			if (!s.HitDone && animFrame >= HitFrame)
			{
				Zombie z = s.Target;
				if (Tavel.Alive(z))
				{
					if (s.Ultimate)
					{
						if (Tavel.IsBoss(z) || Tavel.IsGiantZombie(z))
						{
							z.TakeDamage(UltimateGiantDamage, 0u);
						}
						else
						{
							z.TakeDamage(Tavel.ZombieEffectiveHealth(z) + 1, 0u);
						}
						s.Ultimate = false;
					}
					else
					{
						z.TakeDamage(MeleeDamage, 0u);
					}
				}
				s.HitDone = true;
			}
			if (t >= 0.999f)
			{
				if (s.Pending.Count > 0)
				{
					s.Mode = "attack_boost";
					s.Target = null;
					s.Boosted = new HashSet<Projectile>();
					Tavel.PlayAnim(p, s.Ultimate ? "sa-attack" : "attack", ReanimLoopType.PlayOnceAndHold, 30f);
					return;
				}
				s.Mode = "stand";
				s.Target = null;
				Tavel.PlayAnim(p, s.Ultimate ? "sa-stand" : "stand", ReanimLoopType.Loop, 30f);
			}
			return;
		}
		if (s.Mode == "attack_boost")
		{
			// a single swing: only peas there during frames 0-10 are batted
			if (animFrame <= BoostEnd)
			{
				Queue(p, s);
			}
			if (BoostStart <= animFrame && animFrame <= BoostEnd)
			{
				foreach (Projectile pr in s.Pending.ToArray())
				{
					if (pr.mDead || s.Boosted.Contains(pr))
					{
						continue;
					}
					s.Boosted.Add(pr);
					Release(s, pr);
					BoostProjectile(p, pr, s.Ultimate);
					s.Pending.Remove(pr);
				}
			}
			if (t >= 0.999f)
			{
				s.Ultimate = false;
				ReleaseAll(s);
				s.Mode = "stand";
				s.Boosted = new HashSet<Projectile>();
				Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			}
			return;
		}
		if (Eligible(p).Count > 0)
		{
			s.Mode = "attack_boost";
			s.Boosted = new HashSet<Projectile>();
			Queue(p, s);
			Tavel.PlayAnim(p, attack, ReanimLoopType.PlayOnceAndHold, 30f);
			return;
		}
		Zombie target = FindMeleeTarget(p);
		if (target != null)
		{
			s.Mode = "attack_melee";
			s.Target = target;
			s.HitDone = false;
			Tavel.PlayAnim(p, attack, ReanimLoopType.PlayOnceAndHold, 30f);
		}
	}
}
