// Where the game calls into the pack (port of nineplants/hooks.py and the plant-independent parts of ui.py).
// Each game method that the PC pack hooked starts with "if (Tavel.Loaded) { TavelHooks.X(this, ...); return; }" and its
// own body moved to XCore; the hook here calls XCore where the PC hook called "orig".
using System;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public static class TavelHooks
{
	// ------------------------------------------------------------------ plants

	/// <summary>PlantInitialize ends with an internal UpdateReanim before the pack can pick the Spine animation.</summary>
	private static int sInitializing;

	public static void PlantInitialize(Plant p, int theGridX, int theGridY, SeedType theSeed, SeedType theImitater)
	{
		// the Plant object may be a recycled one
		p.mTavel = null;
		TavelSpec spec = Tavel.Get(theSeed);
		if (spec == null)
		{
			p.PlantInitializeCore(theGridX, theGridY, theSeed, theImitater);
			return;
		}
		PlantDefinition def = GameConstants.gPlantDefs[spec.SeedId];
		ReanimationType customReanim = def.mReanimationType;
		if (spec.CustomBody)
		{
			// the initialiser needs a valid native body; the Spine body is made by OnPlanted afterwards
			def.mReanimationType = ReanimationType.Peashooter;
		}
		sInitializing++;
		try
		{
			p.PlantInitializeCore(theGridX, theGridY, spec.NativeSeed != SeedType.None ? spec.NativeSeed : theSeed, theImitater);
		}
		finally
		{
			def.mReanimationType = customReanim;
			sInitializing = Math.Max(0, sInitializing - 1);
		}
		spec.OnPlanted?.Invoke(p, theGridX, theGridY);
	}

	public static void PlantUpdateReanim(Plant p)
	{
		TavelSpec spec = Tavel.GetFor(p);
		if (spec == null)
		{
			p.UpdateReanimCore();
			return;
		}
		if (sInitializing > 0 && Tavel.Get(p.mSeedType) != null)
		{
			return;
		}
		if (spec.CustomBody)
		{
			// the bodies are standalone Spine files: their own controllers pick the animations
			Tavel.Body(p)?.Update();
			return;
		}
		p.UpdateReanimCore();
	}

	public static void PlantUpdate(Plant p)
	{
		TavelSpec spec = Tavel.GetFor(p);
		p.UpdateCore();
		spec?.OnUpdate?.Invoke(p);
	}

	public static void PlantDraw(Plant p, Graphics g)
	{
		TavelSpec spec = Tavel.Get(p.mSeedType);
		if (spec == null)
		{
			p.DrawCore(g);
			return;
		}
		if (p.mSquished && spec.DrawSquished != null)
		{
			// flattened by a gargantuar: fades away over the last second, as the game's own
			Tavel.Tinted(g, new Color(255, 255, 255, (int)(255f * Math.Min(1f, p.mDisappearCountdown / 100f))), delegate
			{
				spec.DrawSquished(p, g);
			});
			return;
		}
		Action draw = spec.Draw != null ? delegate
		{
			spec.Draw(p, g);
		} : (Action)delegate
		{
			p.DrawCore(g);
		};
		TavelBoard ult = p.mBoard == null ? null : Tavel.State(p.mBoard);
		if (ult == null || !ult.UltSelecting || !Tavel.CanUseUltimate(p))
		{
			draw();
			return;
		}
		// plants that can take the armed ultimate pulse
		float pulse = ((float)Math.Sin(p.mBoard.mMainCounter * Math.PI / 45.0) + 1f) * 0.5f;
		int bright = (int)(205f + 50f * pulse);
		Tavel.Tinted(g, new Color(bright, bright, bright, 255), draw);
	}

	public static void PlantDie(Plant p)
	{
		TavelSpec spec = Tavel.GetFor(p);
		spec?.OnDie?.Invoke(p);
		p.DieCore();
	}

	public static void PlantSquish(Plant p)
	{
		if (Tavel.IsCellProtected(p))
		{
			return;
		}
		TavelSpec spec = Tavel.GetFor(p);
		if (spec?.SquishOverride != null)
		{
			spec.SquishOverride(p);
			return;
		}
		spec = Tavel.Get(p.mSeedType);
		if (spec != null)
		{
			object state = p.mTavel;
			p.mTavel = null;
			if (state is TavelFjth.State fs)
			{
				foreach (TavelFjth.Pull item in fs.UltimateTargets)
				{
					if (item.Zombie.mTavel != null)
					{
						item.Zombie.mTavel.FjthPulled = false;
					}
				}
			}
			spec.OnSquished?.Invoke(p, state);
		}
		p.SquishCore();
	}

	public static void PlantMouseDown(Plant p, int x, int y, int theClickCount)
	{
		TavelSpec spec = Tavel.Get(p.mSeedType);
		if (spec != null && theClickCount >= 0 && p.mBoard != null && p.mApp.mGameScene == GameScenes.Playing && p.mBoard.mCursorObject.mCursorType == CursorType.Normal)
		{
			TavelBoard ult = Tavel.State(p.mBoard);
			if (ult.UltSelecting && ult.UltStock > 0 && Tavel.StartUltimate(p))
			{
				ult.UltStock--;
				ult.UltSelecting = false;
				return;
			}
			if (spec.OnClick != null && !ult.UltSelecting)
			{
				spec.OnClick(p);
				return;
			}
		}
		p.MouseDownCore(x, y, theClickCount);
	}

	public static void PlantKillAllPlantsNearDoom(Plant p)
	{
		if (!Tavel.IsCellProtected(p))
		{
			p.KillAllPlantsNearDoomCore();
		}
	}

	public static void PlantFire(Plant p, Zombie theTarget, int theRow, PlantWeapon theWeapon)
	{
		if (p.mBoard != null && (p.mSeedType == SeedType.Puffshroom || p.mSeedType == SeedType.Fumeshroom || p.mSeedType == SeedType.Scaredyshroom) && TavelSwg.FindSmallAt(p.mBoard, p.mPlantCol, p.mRow) != null)
		{
			int before = p.mBoard.mProjectiles.Count;
			var known = new System.Collections.Generic.HashSet<Projectile>(p.mBoard.mProjectiles);
			p.FireCore(theTarget, theRow, theWeapon);
			foreach (Projectile pr in p.mBoard.mProjectiles)
			{
				if (!known.Contains(pr))
				{
					Shot(pr).SwgRepeat = true;
				}
			}
			return;
		}
		if (p.mSeedType == TavelSuperGatling.Seed)
		{
			TavelSuperGatling.Fire(p);
			return;
		}
		p.FireCore(theTarget, theRow, theWeapon);
	}

	public static Zombie PlantFindTargetZombie(Plant p, int theRow, PlantWeapon theWeapon)
	{
		if (p.mBoard != null && (p.mSeedType == SeedType.Puffshroom || p.mSeedType == SeedType.Fumeshroom || p.mSeedType == SeedType.Scaredyshroom) && TavelSwg.FindSmallAt(p.mBoard, p.mPlantCol, p.mRow) != null)
		{
			// guarded mushrooms see the whole row ahead
			foreach (Zombie z in p.mBoard.mZombies)
			{
				if (Tavel.Alive(z) && z.mRow == p.mRow && z.mPosX >= p.mX)
				{
					return z;
				}
			}
		}
		return p.FindTargetZombieCore(theRow, theWeapon);
	}

	public static void PlantDoRowAreaDamage(Plant p, int theDamage, uint theFlags)
	{
		if (p.mSeedType == SeedType.Fumeshroom && p.mBoard != null && TavelSwg.FindSmallAt(p.mBoard, p.mPlantCol, p.mRow) != null)
		{
			uint range = (uint)p.GetDamageRangeFlags(PlantWeapon.Primary);
			foreach (Zombie z in p.mBoard.mZombies.ToArray())
			{
				if (Tavel.Alive(z) && z.mRow == p.mRow && z.mPosX >= p.mX && z.EffectedByDamage(range))
				{
					z.TakeDamage(theDamage * 2, theFlags);
				}
			}
			return;
		}
		p.DoRowAreaDamageCore(theDamage, theFlags);
	}

	// ------------------------------------------------------------------ board

	public static PlantingReason BoardCanPlantAt(Board b, int theGridX, int theGridY, SeedType theType, bool theIsMove)
	{
		if (theType == TavelCzlb.Seed)
		{
			if (theGridX < 0 || theGridX >= 9 || theGridY < 0 || theGridY >= 6)
			{
				return PlantingReason.NotHere;
			}
			return TavelCzlb.CellTaken(b, theGridX, theGridY) ? PlantingReason.NotHere : PlantingReason.Ok;
		}
		if (Tavel.IsCustom(theType))
		{
			return b.CanPlantAtCore(theGridX, theGridY, SeedType.Peashooter, theIsMove);
		}
		return b.CanPlantAtCore(theGridX, theGridY, theType, theIsMove);
	}

	public static PlantingReason ChallengeCanPlantAt(Challenge c, int theGridX, int theGridY, SeedType theType)
	{
		if (theType == TavelCzlb.Seed)
		{
			if (c.mBoard == null)
			{
				return PlantingReason.NotHere;
			}
			return TavelCzlb.CellTaken(c.mBoard, theGridX, theGridY) ? PlantingReason.NotHere : PlantingReason.Ok;
		}
		return c.CanPlantAtCore(theGridX, theGridY, Tavel.IsCustom(theType) ? SeedType.Peashooter : theType);
	}

	public static void BoardKillAllPlantsInRadius(Board b, int theX, int theY, int theRadius)
	{
		var keep = new System.Collections.Generic.List<(Plant, int)>();
		foreach (Plant p in b.mPlants)
		{
			if (!p.mDead && Math.Abs(p.mX - theX) <= theRadius && Math.Abs(p.mY - theY) <= theRadius && Tavel.IsCellProtected(p))
			{
				keep.Add((p, p.mPlantHealth));
				p.mPlantHealth = int.MaxValue;
			}
		}
		try
		{
			b.KillAllPlantsInRadiusCore(theX, theY, theRadius);
		}
		finally
		{
			foreach ((Plant p, int h) in keep)
			{
				if (!p.mDead)
				{
					p.mPlantHealth = h;
				}
			}
		}
	}

	public static void BoardUpdateGame(Board b)
	{
		b.UpdateGameCore();
		if (Tavel.UltimateUiAllowed(b))
		{
			Tavel.UpdateUltimateStock(b);
		}
		else if (b.mTavel != null)
		{
			b.mTavel.UltSelecting = false;
		}
		// projectiles, summons and tiles outlive the plant that made them
		foreach (TavelSpec s in Tavel.Plants)
		{
			s.UpdateBoard?.Invoke(b);
		}
	}

	/// <summary>Returns true when the pack took the tap (下一波 / ultimate button).</summary>
	public static bool BoardMouseDown(Board b, int x, int y, int theClickCount)
	{
		if (theClickCount >= 0 && Tavel.CanShowNextWaveButton(b))
		{
			GameButton button = Tavel.NextWaveButton(b);
			if (new TRect(button.mX, button.mY, button.mWidth, button.mHeight).Contains(x, y))
			{
				Tavel.TriggerNextWave(b);
				return true;
			}
		}
		if (theClickCount >= 0 && Tavel.UltimateUiAllowed(b) && Tavel.UltimateRect(b).Contains(x, y))
		{
			TavelBoard s = Tavel.State(b);
			if (s.UltStock > 0)
			{
				s.UltSelecting = !s.UltSelecting;
			}
			return true;
		}
		if (theClickCount < 0 && b.mTavel != null)
		{
			b.mTavel.UltSelecting = false;
		}
		return false;
	}

	/// <summary>After the board drew itself: the pack's board effects (lawn coordinates), then its buttons.</summary>
	public static void BoardDraw(Board b, Graphics g)
	{
		bool camera = b.mCameraEnabled && b.mCamera != null;
		if (camera)
		{
			b.mCamera.ApplyTransform(g);
		}
		foreach (TavelSpec s in Tavel.Plants)
		{
			s.DrawBoardUnder?.Invoke(b, g);
		}
		foreach (TavelSpec s in Tavel.Plants)
		{
			s.DrawBoard?.Invoke(b, g);
		}
		if (camera)
		{
			b.mCamera.ResetTransform(g);
		}
		Tavel.DrawUltimateButton(b, g);
		Tavel.DrawNextWaveButton(b, g);
	}

	/// <summary>After the lawn is drawn, under everything standing on it: tiles, formations, whirlpools.</summary>
	public static void BoardDrawBackdrop(Board b, Graphics g)
	{
		if (b.mTavel == null)
		{
			return;
		}
		TavelCzlb.DrawTiles(b, g);
		TavelQgxlj.DrawFormations(b, g);
		TavelNjspg.DrawBackdrop(b, g);
	}

	// ------------------------------------------------------------------ projectiles

	public static TavelShot Shot(Projectile p) => p.mTavel ??= new TavelShot();

	public static void ProjectileDoImpact(Projectile pr, Zombie z)
	{
		// a straight shot hitting a quantum zombie hits it and flies on
		bool pierce = TavelDmdf.BeginPierce(pr, z);
		try
		{
			ProjectileDoImpactInner(pr, z);
		}
		finally
		{
			if (pierce)
			{
				TavelDmdf.EndPierce();
			}
		}
	}

	public static void ProjectileDie(Projectile pr)
	{
		if (TavelDmdf.KeepAlive(pr))
		{
			return;
		}
		pr.DieCore();
	}

	public static Zombie ProjectileFindCollisionTarget(Projectile pr)
	{
		System.Collections.Generic.List<Zombie> hidden = TavelDmdf.HideForSearch(pr);
		if (hidden == null)
		{
			return pr.FindCollisionTargetCore();
		}
		try
		{
			return pr.FindCollisionTargetCore();
		}
		finally
		{
			foreach (Zombie z in hidden)
			{
				z.mDead = false;
			}
		}
	}

	private static void ProjectileDoImpactInner(Projectile pr, Zombie z)
	{
		TavelShot s = pr.mTavel;
		if (z == null)
		{
			Plant target = null;
			try
			{
				target = pr.FindCollisionTargetPlant();
			}
			catch
			{
			}
			if (target != null && Tavel.IsCellProtected(target))
			{
				pr.Die();
				return;
			}
		}
		if (s == null)
		{
			pr.DoImpactCore(z);
			return;
		}
		if (s.SwgRepeat)
		{
			s.SwgRepeat = false;
			pr.DoImpactCore(z);
			if (Tavel.Alive(z))
			{
				z.TakeDamage(20, pr.GetDamageFlags(z));
			}
			return;
		}
		if (TavelExtraImpacts(pr, s, z))
		{
			return;
		}
		switch (s.PeaKind)
		{
		case TavelSuperGatling.PeaFire:
			pr.DoImpactCore(z);
			if (Tavel.Alive(z))
			{
				// the game's pea deals 20: fire peas deal 40
				z.TakeDamage(20, pr.GetDamageFlags(z));
			}
			return;
		case TavelSuperGatling.PeaStone:
			TavelSuperGatling.StoneImpact(pr, z);
			return;
		case TavelSuperGatling.PeaPoison:
			TavelSuperGatling.PoisonImpact(pr, z);
			return;
		}
		pr.DoImpactCore(z);
	}

	/// <summary>Impacts of the other plants' projectiles (batter's boosted shots, cone trainer's spikes, ...).</summary>
	private static bool TavelExtraImpacts(Projectile pr, TavelShot s, Zombie z)
	{
		return TavelNbjqs.Impact(pr, s, z) || TavelQgxlj.Impact(pr, s, z);
	}

	public static void ProjectileUpdate(Projectile pr)
	{
		TavelShot s = pr.mTavel;
		if (s == null)
		{
			pr.UpdateCore();
			return;
		}
		if (s.Fan && !pr.mDead)
		{
			// the game's straight-pea code must only see a row-centred y; the fan's y is put back afterwards
			int safeY = pr.mBoard.GridToPixelY(0, pr.mRow) + 10;
			pr.mPosY = safeY;
			pr.mY = safeY;
			pr.mVelY = 0f;
		}
		pr.UpdateCore();
		if (s.Fan && !pr.mDead)
		{
			s.FanScreenY += s.FanVelocityY;
			pr.mPosY = s.FanScreenY;
			pr.mY = (int)pr.mPosY;
			Reanimation fire = pr.mApp.ReanimationTryToGet(pr.mProjectileReanimID);
			fire?.SetPosition(pr.mPosX, pr.mPosY);
		}
		int rightEdge = s.Fan ? pr.mBoard.mWidth + 40 : pr.mBoard.GridToPixelX(9, pr.mRow);
		if (s.SuperGatling && !pr.mDead && pr.mPosX > rightEdge)
		{
			pr.Die();
		}
		if (pr.mDead)
		{
			pr.mTavel = null;
			return;
		}
		if (s.PeaKind == TavelSuperGatling.PeaElectric && pr.mProjectileAge > 0 && pr.mProjectileAge % TavelSuperGatling.ElectricShockInterval == 0)
		{
			TavelSuperGatling.ShockRandomZombie(pr);
		}
	}

	public static void ProjectileCheckForCollision(Projectile pr)
	{
		TavelShot s = pr.mTavel;
		if (s == null)
		{
			pr.CheckForCollisionCore();
			return;
		}
		if (s.Fan)
		{
			TRect r = pr.GetProjectileRect();
			foreach (Zombie z in pr.mBoard.mZombies.ToArray())
			{
				if (TavelSuperGatling.IsValidElectricTarget(pr, z) && Tavel.RectsOverlap(r, z.GetZombieRect()))
				{
					pr.DoImpact(z);
					return;
				}
			}
			return;
		}
		if (s.PeaKind != TavelSuperGatling.PeaElectric)
		{
			pr.CheckForCollisionCore();
			return;
		}
		s.ElectricHits ??= new System.Collections.Generic.HashSet<Zombie>();
		TRect rect = pr.GetProjectileRect();
		foreach (Zombie z in pr.mBoard.mZombies.ToArray())
		{
			if (s.ElectricHits.Contains(z) || !TavelSuperGatling.IsValidElectricTarget(pr, z))
			{
				continue;
			}
			if (Tavel.RectsOverlap(rect, z.GetZombieRect()))
			{
				s.ElectricHits.Add(z);
				TavelSuperGatling.DoElectricDamage(z, TavelSuperGatling.ElectricContactDamage, pr.GetDamageFlags(z), theAshOnKill: false);
			}
		}
	}

	public static void ProjectileCheckForHighGround(Projectile pr)
	{
		// fan shots travel in screen space and pass over roof slopes
		if (pr.mTavel != null && pr.mTavel.Fan)
		{
			return;
		}
		pr.CheckForHighGroundCore();
	}

	public static bool ProjectilePeaAboutToHitTorchwood(Projectile pr)
	{
		if (pr.mTavel != null && pr.mTavel.PeaKind >= TavelSuperGatling.PeaFire)
		{
			return false;
		}
		return pr.PeaAboutToHitTorchwoodCore();
	}

	public static void ProjectileDraw(Projectile pr, Graphics g)
	{
		int kind = pr.mTavel?.PeaKind ?? -1;
		if (kind < TavelSuperGatling.PeaFire)
		{
			pr.DrawCore(g);
			return;
		}
		// the game's own pea drawing, with the picture swapped
		Image native = AtlasResources.IMAGE_PROJECTILEPEA;
		ProjectileType nativeType = pr.mProjectileType;
		AtlasResources.IMAGE_PROJECTILEPEA = TavelSuperGatling.PeaImage(kind) ?? native;
		if (kind == TavelSuperGatling.PeaFire)
		{
			pr.mProjectileType = ProjectileType.Pea;
		}
		try
		{
			pr.DrawCore(g);
		}
		finally
		{
			pr.mProjectileType = nativeType;
			AtlasResources.IMAGE_PROJECTILEPEA = native;
		}
	}

	// ------------------------------------------------------------------ zombies

	public static TavelZombie ZState(Zombie z) => z.mTavel ??= new TavelZombie();

	public static void ZombieUpdate(Zombie z)
	{
		TavelZombie t = z.mTavel;
		int summon = z.mSummonCounter;
		if (t == null || (!t.Poisoned && !t.SsbpsPoisoned))
		{
			z.UpdateCore();
			TavelExtraZombieUpdate(z, summon);
			return;
		}
		if (z.mDead || z.IsDeadOrDying())
		{
			t.Poisoned = t.SsbpsPoisoned = false;
			z.UpdateCore();
			return;
		}
		float oldX = z.mPosX;
		float oldY = z.mPosY;
		z.UpdateCore();
		TavelExtraZombieUpdate(z, summon);
		if (z.mDead || z.IsDeadOrDying())
		{
			t.Poisoned = t.SsbpsPoisoned = false;
			return;
		}
		// both poisons scale this tick's movement on top of the game's own chill
		float scale = 1f;
		if (t.Poisoned)
		{
			scale *= (100f - TavelSuperGatling.PoisonSlowPercent) / 100f;
		}
		if (t.SsbpsPoisoned)
		{
			scale *= (100f - TavelSsbps.PoisonSlowPercent) / 100f;
		}
		z.mPosX = oldX + (z.mPosX - oldX) * scale;
		z.mPosY = oldY + (z.mPosY - oldY) * scale;
		if (t.Poisoned)
		{
			t.PoisonRemaining--;
			t.PoisonNextTick--;
			if (t.PoisonNextTick <= 0)
			{
				z.TakeDamage(Math.Max(1, (Tavel.ZombieMaximumHealth(z) * TavelSuperGatling.PoisonDamagePercent + 99) / 100), 0u);
				t.PoisonNextTick = TavelSuperGatling.PoisonTickInterval;
			}
			if (t.PoisonRemaining <= 0 || !Tavel.Alive(z))
			{
				t.Poisoned = false;
			}
		}
		if (t.SsbpsPoisoned && Tavel.Alive(z))
		{
			t.SsbpsRemaining--;
			t.SsbpsNextTick--;
			if (t.SsbpsNextTick <= 0)
			{
				z.TakeDamage(Math.Max(1, (t.SsbpsMaximumHealth * TavelSsbps.PoisonDamagePercent + 99) / 100), 0u);
				t.SsbpsNextTick = TavelSsbps.PoisonTickInterval;
			}
			if (t.SsbpsRemaining <= 0 || !Tavel.Alive(z))
			{
				t.SsbpsPoisoned = false;
			}
		}
	}

	/// <summary>Per-zombie effects of the other plants (dark matter's quantum mark ...).</summary>
	private static void TavelExtraZombieUpdate(Zombie z, int theSummonBefore)
	{
		if (z.mTavel != null)
		{
			TavelDmdf.AfterZombieUpdate(z, theSummonBefore);
		}
	}

	public static void ZombieDraw(Zombie z, Graphics g)
	{
		TavelZombie t = z.mTavel;
		if (t == null)
		{
			z.DrawCore(g);
			return;
		}
		if (t.Poisoned || t.SsbpsPoisoned)
		{
			Tavel.Tinted(g, new Color(170, 70, 210, 255), delegate
			{
				z.DrawCore(g);
			});
			return;
		}
		if (TavelTclc.IsBurning(z))
		{
			Tavel.Tinted(g, new Color(255, 150, 90, 255), delegate
			{
				z.DrawCore(g);
			});
			return;
		}
		if (TavelDmdf.DrawZombie(z, g))
		{
			return;
		}
		z.DrawCore(g);
	}

	public static void ZombieDrawBossPart(Zombie z, Graphics g, BossPart thePart)
	{
		TavelZombie t = z.mTavel;
		if (t == null || (!t.Poisoned && !t.SsbpsPoisoned))
		{
			z.DrawBossPartCore(g, thePart);
			return;
		}
		Tavel.Tinted(g, new Color(170, 70, 210, 255), delegate
		{
			z.DrawBossPartCore(g, thePart);
		});
	}

	public static void ZombieEatPlant(Zombie z, Plant p)
	{
		if (Tavel.IsCellProtected(p))
		{
			return;
		}
		// 贪吃龙草 hovers out of reach while breathing fire; zombies snap at the air and stay in the flames
		if (p.mSeedType == TavelTclc.Seed && TavelTclc.IsAirborne(p))
		{
			return;
		}
		z.EatPlantCore(p);
	}

	public static void ZombieSquishAllInSquare(Zombie z, int theX, int theY, ZombieAttackType theType)
	{
		if (z.mTavel != null && z.mTavel.FjthPulled)
		{
			return;
		}
		if ((z.mZombieType == ZombieType.RobotTitan || z.mZombieType == ZombieType.RedeyeRobotTitan) && z.mBoard != null)
		{
			foreach (Plant p in z.mBoard.mPlants)
			{
				if (p.mDead || p.mSeedType != TavelFjth.Seed || p.mPlantCol != theX || p.mRow != theY)
				{
					continue;
				}
				if (TavelFjth.SwallowTitan(p, z))
				{
					return;
				}
				break;
			}
		}
		z.SquishAllInSquareCore(theX, theY, theType);
	}

	public static void ZombieUpdateZamboni(Zombie z)
	{
		if (z.mTavel == null || !z.mTavel.FjthPulled)
		{
			z.UpdateZamboniCore();
			return;
		}
		// keep the zamboni's own update, but not the ice made from its artificial position
		Board b = z.mBoard;
		int row = z.mRow;
		int iceMin = b.mIceMinX[row];
		int iceTimer = b.mIceTimer[row];
		z.UpdateZamboniCore();
		b.mIceMinX[row] = iceMin;
		b.mIceTimer[row] = iceTimer;
	}

	public static Plant ZombieFindPlantTarget(Zombie z, ZombieAttackType theType)
	{
		if (z.mTavel != null && z.mTavel.FjthPulled)
		{
			return null;
		}
		Plant target = z.FindPlantTargetCore(theType);
		if (target?.mBoard == null)
		{
			return target;
		}
		// a small guard in the cell is eaten first
		Plant guard = TavelSwg.FindSmallAt(target.mBoard, target.mPlantCol, target.mRow);
		return guard != null && guard != target ? guard : target;
	}

	/// <summary>Pole vaulters and pogo zombies treat 守卫菇 as a tall-nut.</summary>
	public static void ZombieRunWithSwgAsTallnut(Zombie z, Action theUpdate)
	{
		Board b = z.mBoard;
		var changed = new System.Collections.Generic.List<Plant>();
		if (b != null)
		{
			foreach (Plant p in b.mPlants)
			{
				if (!p.mDead && p.mSeedType == TavelSwg.Seed)
				{
					changed.Add(p);
					p.mSeedType = SeedType.Tallnut;
				}
			}
		}
		try
		{
			theUpdate();
		}
		finally
		{
			foreach (Plant p in changed)
			{
				p.mSeedType = TavelSwg.Seed;
			}
		}
	}

	public static bool LawnMowerSkips(Zombie z) => z.mTavel != null && z.mTavel.FjthPulled;
}
