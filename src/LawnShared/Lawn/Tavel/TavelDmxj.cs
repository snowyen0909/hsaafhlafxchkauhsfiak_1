// 电鳗香蕉 (plant_dmxj.py): a shot that goes out and comes back.
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public sealed partial class TavelBoard
{
	public List<TavelDmxj.Shot> DmxjShots = new List<TavelDmxj.Shot>();

	public int DmxjLastUpdate = -1;
}

public static class TavelDmxj
{
	public const int Id = 103;

	public static SeedType Seed => (SeedType)Id;

	public const int AttackFrames = 40;

	public const float NormalSpeed = 5f;

	public const float UltimateSpeed = 2.67f;

	public const int NormalDamage = 80;

	public const int UltimateDamage = 1800;

	public const int NormalRangeGrids = 6;

	public sealed class Shot
	{
		public Plant Plant;

		public float X;

		public float Y;

		public int Row;

		public int Direction = 1;

		public bool Ultimate;

		public float Speed;

		public float TurnX;

		public HashSet<Zombie> HitOut = new HashSet<Zombie>();

		public HashSet<Zombie> HitBack = new HashSet<Zombie>();

		public bool Dead;
	}

	public sealed class State
	{
		public string Mode = "stand";

		public Shot Shot;
	}

	private static TavelSpec sSpec;

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	private static void Align(Plant p) => Tavel.Align(p, 1.716f, -28f, -60f);

	public static TavelSpec Spec()
	{
		sSpec = new TavelSpec(Id, "dmxj")
		{
			Name = "电鳗香蕉",
			Cost = 125,
			Card = "tavel/dmxj/card.png",
			AlmanacText = "电鳗香蕉会发动往返攻击。\n{SHORTLINE}\n{KEYWORD}伤害：{STAT}中等\n{KEYWORD}特点：{STAT}穿透敌人，子弹到尽头会折返\n{KEYWORD}大招：{STAT}秒杀前方中小型僵尸，对大型僵尸造成高额伤害。击败僵尸有几率刷新大招\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}短\n{KEYWORD}阳光：{STAT}125\n{SHORTLINE}\n{FLAVOR}这条围巾戴着有点麻麻的，是保暖效果太好了吗。",
			DefName = "DMXJ",
			DefCost = 125,
			LaunchRate = 90,
			ReanimFile = "tavel/spanim/dmxj",
			CustomBody = true,
			Preview = new TavelPreview("anim_stand", 0.66f, -36f, -84f, 1.716f, -28f, -60f),
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
					UpdateShots(p.mBoard);
				}
				Align(p);
			},
			OnDie = Cleanup,
			OnSquished = delegate(Plant p, object s)
			{
				Cleanup(p);
			},
			CanUltimate = (Plant p) => Get(p).Mode != "ultimate_anim",
			UltimateActive = (Plant p) => p.mTavel is State s && s.Mode == "ultimate_anim",
			StartUltimate = delegate(Plant p)
			{
				State s = Get(p);
				if (s.Shot != null)
				{
					s.Shot.Dead = true;
					s.Shot = null;
				}
				s.Mode = "ultimate_anim";
				Tavel.PlayAnim(p, "sa", ReanimLoopType.PlayOnceAndHold, 30f);
			},
			DrawBoard = DrawShots
		};
		return sSpec;
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
		foreach (Shot s in p.mBoard.mTavel.DmxjShots)
		{
			if (s.Plant == p)
			{
				s.Dead = true;
			}
		}
	}

	/// <summary>Zomboss is only hit while his head is down spitting.</summary>
	public static bool BossCanBeHit(Zombie z)
	{
		return z.mZombiePhase == ZombiePhase.BossHeadIdleBeforeSpit || z.mZombiePhase == ZombiePhase.BossHeadIdleAfterSpit || z.mZombiePhase == ZombiePhase.BossHeadSpit;
	}

	private static bool HasTarget(Plant p)
	{
		float startX = p.mBoard.GridToPixelX(p.mPlantCol, p.mRow) + 45;
		float maxX = startX + NormalRangeGrids * 80f;
		foreach (Zombie z in p.mBoard.mZombies)
		{
			if (!Tavel.Alive(z))
			{
				continue;
			}
			if (Tavel.IsBoss(z))
			{
				if (!BossCanBeHit(z))
				{
					continue;
				}
			}
			else if (z.mRow != p.mRow)
			{
				continue;
			}
			TRect r = z.GetZombieRect();
			float cx = r.mX + r.mWidth * 0.5f;
			if (startX <= cx && cx <= maxX)
			{
				return true;
			}
		}
		return false;
	}

	private static Shot AddShot(Plant p, bool theUltimate)
	{
		int gx = p.mBoard.GridToPixelX(p.mPlantCol, p.mRow);
		float x = gx + 55;
		float y = p.mBoard.GetPosYBasedOnRow(x, p.mRow) + (theUltimate ? 10 : 50);
		float rightEdge = p.mBoard.GridToPixelX(9, 0) + 80;
		Shot shot = new Shot
		{
			Plant = p,
			X = x,
			Y = y,
			Row = p.mRow,
			Ultimate = theUltimate,
			Speed = theUltimate ? UltimateSpeed : NormalSpeed,
			TurnX = theUltimate ? rightEdge : Math.Min(rightEdge, x + NormalRangeGrids * 80f)
		};
		Tavel.State(p.mBoard).DmxjShots.Add(shot);
		if (!theUltimate)
		{
			Get(p).Shot = shot;
		}
		return shot;
	}

	private static void Damage(Shot s, Zombie z)
	{
		bool wasAlive = Tavel.Alive(z);
		if (s.Ultimate)
		{
			if (Tavel.IsBoss(z) || Tavel.IsGiantZombie(z))
			{
				TavelSuperGatling.DoElectricDamage(z, UltimateDamage, 0u, theAshOnKill: true);
			}
			else
			{
				// a true execute: strip every defensive layer, then the charred death
				if (z.mHasShield || z.mShieldType != ShieldType.None)
				{
					z.DropShield(16u);
					z.mShieldHealth = 0;
				}
				if (z.mHasHelm || z.mHelmType != HelmType.None)
				{
					z.DropHelm(16u);
					z.mHelmHealth = 0;
				}
				z.mBodyHealth = Math.Min(z.mBodyHealth, 1799);
				z.ApplyBurn();
			}
			if (wasAlive && !Tavel.Alive(z) && Tavel.Rand(1, 100) <= 18)
			{
				Tavel.AddUltimateStock(z.mBoard, 1);
			}
			return;
		}
		TavelSuperGatling.DoElectricDamage(z, NormalDamage, 0u, theAshOnKill: true);
	}

	private static void UpdateShots(Board b)
	{
		TavelBoard bs = Tavel.State(b);
		if (bs.DmxjLastUpdate == b.mMainCounter)
		{
			return;
		}
		bs.DmxjLastUpdate = b.mMainCounter;
		float rightEdge = b.GridToPixelX(9, 0) + 80;
		foreach (Shot s in bs.DmxjShots.ToArray())
		{
			Plant p = s.Plant;
			if (s.Dead || p == null || p.mDead || p.mBoard != b)
			{
				s.Dead = true;
				continue;
			}
			s.X += s.Speed * s.Direction;
			s.Y = b.GetPosYBasedOnRow(s.X, s.Row) + (s.Ultimate ? 10 : 50);
			float turnX = s.Ultimate ? rightEdge : s.TurnX;
			if (s.Direction > 0 && s.X >= turnX)
			{
				if (s.Ultimate)
				{
					s.Dead = true;
					continue;
				}
				s.X = turnX;
				s.Direction = -1;
			}
			HashSet<Zombie> hit = s.Direction > 0 ? s.HitOut : s.HitBack;
			foreach (Zombie z in b.mZombies.ToArray())
			{
				if (!Tavel.Alive(z) || hit.Contains(z))
				{
					continue;
				}
				bool boss = Tavel.IsBoss(z);
				if (boss)
				{
					if (!BossCanBeHit(z))
					{
						continue;
					}
				}
				else if (z.mRow != s.Row)
				{
					continue;
				}
				TRect r = z.GetZombieRect();
				bool ignoresHeight = s.Ultimate && (z.mZombieType == ZombieType.Balloon || (z.mZombieType == ZombieType.Digger && z.mZombiePhase == ZombiePhase.DiggerTunneling));
				bool horizontal = r.mX - 35 <= s.X && s.X <= r.mX + r.mWidth + 35;
				if (horizontal && (boss || ignoresHeight || (r.mY - 25 <= s.Y && s.Y <= r.mY + r.mHeight + 25)))
				{
					hit.Add(z);
					Damage(s, z);
				}
			}
			if (!s.Ultimate && s.Direction < 0)
			{
				float home = b.GridToPixelX(p.mPlantCol, p.mRow) + 45;
				if (s.X <= home)
				{
					s.Dead = true;
					State st = Get(p);
					st.Shot = null;
					st.Mode = "reverse";
					Tavel.PlayAnim(p, "attack", ReanimLoopType.PlayOnceAndHold, 0f);
					Reanimation body = Tavel.Body(p);
					if (body != null)
					{
						body.mAnimTime = 1f;
					}
				}
			}
		}
		bs.DmxjShots.RemoveAll((Shot s) => s.Dead);
	}

	private static void UpdateReverse(Plant p, State s)
	{
		Reanimation body = Tavel.Body(p);
		if (body == null)
		{
			s.Mode = "stand";
			return;
		}
		body.mAnimTime = Math.Max(0f, body.mAnimTime - 1f / AttackFrames);
		if (body.mAnimTime <= 0.001f)
		{
			s.Mode = "stand";
			Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
		}
	}

	private static void DrawShots(Board b, Graphics g)
	{
		if (b.mTavel == null)
		{
			return;
		}
		foreach (Shot s in b.mTavel.DmxjShots)
		{
			if (s.Dead || s.Plant.mBoard != b)
			{
				continue;
			}
			Image img = Tavel.LoadImage(s.Ultimate ? "tavel/dmxj/sa.png" : "tavel/dmxj/shot.png");
			if (img == null)
			{
				continue;
			}
			float scale = s.Ultimate ? 0.336f : 0.108f;
			float sx = s.Direction > 0 ? scale : -scale;
			float dx = sx < 0f ? s.X + img.mWidth * scale * 0.5f : s.X - img.mWidth * scale * 0.5f;
			float dy = s.Y - img.mHeight * scale * 0.5f;
			if (s.Ultimate)
			{
				Tavel.Tinted(g, new Color(255, 255, 255, 180), delegate
				{
					TodCommon.TodDrawImageScaledF(g, img, dx, dy, sx, scale);
				});
			}
			else
			{
				TodCommon.TodDrawImageScaledF(g, img, dx, dy, sx, scale);
			}
		}
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		bool animDone = Tavel.Body(p) == null || Tavel.AnimTime(p) >= 0.999f;
		switch (s.Mode)
		{
		case "attack_anim":
			if (animDone)
			{
				s.Shot = AddShot(p, theUltimate: false);
				s.Mode = "rest";
				Tavel.PlayAnim(p, "stand2", ReanimLoopType.Loop, 30f);
			}
			return;
		case "rest":
			return;
		case "reverse":
			UpdateReverse(p, s);
			return;
		case "ultimate_anim":
			if (animDone)
			{
				AddShot(p, theUltimate: true);
				s.Mode = "stand";
				Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			}
			return;
		}
		if (s.Mode == "stand" && HasTarget(p))
		{
			s.Mode = "attack_anim";
			Tavel.PlayAnim(p, "attack", ReanimLoopType.PlayOnceAndHold, 30f);
		}
	}
}
