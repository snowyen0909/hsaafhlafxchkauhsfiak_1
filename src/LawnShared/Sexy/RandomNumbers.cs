using System;
using MersenneTwister;

namespace Sexy;

public static class RandomNumbers
{
	private static Random r;

	public static int NextNumber()
	{
		if (r == null)
		{
			Seed();
		}
		return r.Next();
	}

	public static int NextNumber(int ceiling)
	{
		if (r == null)
		{
			Seed();
		}
		return r.Next(ceiling);
	}

	public static float NextNumber(float ceiling)
	{
		if (r == null)
		{
			Seed();
		}
		return (float)r.NextDouble() * ceiling;
	}

	public static void Seed()
	{
		r = MTRandom.Create(MTEdition.CokOpt_19937);
	}

	public static void Seed(int seed)
	{
		r = MTRandom.Create(seed, MTEdition.CokOpt_19937);
	}
}
