namespace Sexy.TodLib;

public class ParticleField
{
	public ParticleFieldType mFieldType;

	public FloatParameterTrack mX = new FloatParameterTrack();

	public FloatParameterTrack mY = new FloatParameterTrack();

	public ParticleField()
	{
		mFieldType = ParticleFieldType.Invalid;
	}
}
