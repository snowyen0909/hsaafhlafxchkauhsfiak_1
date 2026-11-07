namespace Lawn;

public enum DamageFlags
{
	BypassesShield = 1,
	HitsShieldAndBody = 2,
	Freeze = 4,
	DoesntCauseFlash = 8,
	DoesntLeaveBody = 0x10,
	Spike = 0x20
}
