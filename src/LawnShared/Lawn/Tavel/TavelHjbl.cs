// 黄金蓓蕾 (plant_hjbl.py): blooms, gives 375 sun, and is gone.
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public static class TavelHjbl
{
	public const int Id = 108;

	public static SeedType Seed => (SeedType)Id;

	public sealed class State
	{
		public bool SunCreated;
	}

	private static TavelSpec sSpec;

	public static State Get(Plant p) => p.mTavel as State ?? (State)(p.mTavel = new State());

	private static void Align(Plant p) => Tavel.Align(p, 1.65f, -98f, -210f);

	public static TavelSpec Spec()
	{
		sSpec = new TavelSpec(Id, "hjbl")
		{
			Name = "黄金蓓蕾",
			Cost = 0,
			RefreshTime = 1800,
			StartsReady = true,
			Card = "tavel/hjbl/card.png",
			AlmanacText = "黄金蓓蕾会在绽放后产生大量阳光。\n{SHORTLINE}\n{KEYWORD}特点：{STAT}种植后产生375阳光，然后消失\n{NORMAL}\n{KEYWORD}冷却时间：{STAT}30秒\n{KEYWORD}阳光：{STAT}0\n{SHORTLINE}\n{FLAVOR}金色的花蕾里，藏着一整个晴天。",
			OnAlmanac = delegate(Plant p)
			{
				Get(p).SunCreated = true;
				Tavel.PlayAnim(p, "stand", ReanimLoopType.Loop, 30f);
			},
			DefName = "HJBL",
			DefCost = 0,
			DefRefresh = 1800,
			ReanimFile = "tavel/spanim/hjbl",
			CustomBody = true,
			Preview = new TavelPreview("anim_stand", 0.46f, -28f, -80f, 1.65f, -98f, -210f),
			OnPlanted = delegate(Plant p, int x, int y)
			{
				Get(p);
				if (p.mBoard != null)
				{
					Tavel.StartPacketCooldown(p.mBoard, Seed);
				}
				Tavel.DropNativeBody(p);
				Reanimation body = Tavel.CreateBody(p, sSpec.Reanim, 2, null);
				body.PlayReanim("anim_attack", ReanimLoopType.PlayOnceAndHold, 0, 30f);
				Align(p);
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
				Align(p);
			},
			OnDie = delegate(Plant p)
			{
				p.mTavel = null;
			}
		};
		return sSpec;
	}

	private static void Update(Plant p)
	{
		State s = Get(p);
		if (s.SunCreated || (Tavel.Body(p) != null && Tavel.AnimTime(p) < 0.999f))
		{
			return;
		}
		s.SunCreated = true;
		// fifteen ordinary suns (375), with the game's own collecting
		for (int i = 0; i < 15; i++)
		{
			p.mBoard.AddCoin(p.mX + 10 + i % 5 * 12, p.mY + 10 + i / 5 * 8, CoinType.Sun, CoinMotion.FromPlant);
		}
		p.Die();
	}
}
