// What the pack remembers about a board, a projectile and a zombie. Kept as fields on the game objects (Board.mTavel,
// Projectile.mTavel, Zombie.mTavel), so a pooled object handed to a new life starts clean when the game resets it.
// Each plant file adds its own fields to these partial classes.
using System.Collections.Generic;
using Sexy;

namespace Lawn;

public sealed partial class TavelBoard
{
	// the shared ultimate
	public int UltStock = 1;

	public int UltCharge;

	public bool UltSelecting;

	public int UltLastUpdate = -1;

	public GameButton NextWave;

	/// <summary>Plants whose small guard repeats their ultimate once they can.</summary>
	public List<Plant> GuardPending = new List<Plant>();

	/// <summary>Tile-radish ultimates waiting to spread.</summary>
	public List<Plant> TilePending = new List<Plant>();
}

public sealed partial class TavelShot
{
	/// <summary>Super gatling pea kind (TavelSuperGatling.Pea*), -1 for none.</summary>
	public int PeaKind = -1;

	/// <summary>Fired by the super gatling (dies at the right edge of the lawn).</summary>
	public bool SuperGatling;

	/// <summary>Fan (ultimate) shot: moves across the screen plane.</summary>
	public bool Fan;

	public float FanVelocityY;

	public float FanScreenY;

	/// <summary>Electric pea: zombies it already passed through.</summary>
	public HashSet<Zombie> ElectricHits;

	/// <summary>Fired by a mushroom guarded by a small guard: hits again for 20.</summary>
	public bool SwgRepeat;
}

public sealed partial class TavelZombie
{
	public bool Poisoned;

	public int PoisonRemaining;

	public int PoisonNextTick;

	public bool SsbpsPoisoned;

	public int SsbpsRemaining;

	public int SsbpsNextTick;

	public int SsbpsMaximumHealth;

	/// <summary>Being pulled into 伏僵塔黄's ultimate.</summary>
	public bool FjthPulled;
}
