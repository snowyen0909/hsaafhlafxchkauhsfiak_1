namespace Sexy.TodLib;

public class ParticleParams
{
	public ParticleEffect mParticleEffect;

	public string mParticleFileName;

	public ParticleParams(ParticleEffect aParticleEffect, string aParticleName)
	{
		mParticleEffect = aParticleEffect;
		mParticleFileName = aParticleName;
	}
}
