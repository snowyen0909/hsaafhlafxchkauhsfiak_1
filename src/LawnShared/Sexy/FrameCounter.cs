using System.Collections.Generic;
using System.Linq;

namespace Sexy;

public class FrameCounter
{
	public const int MAXIMUM_SAMPLES = 100;

	private Queue<float> _sampleBuffer = new Queue<float>();

	public long TotalFrames { get; private set; }

	public float TotalSeconds { get; private set; }

	public float AverageFramesPerSecond { get; private set; }

	public float CurrentFramesPerSecond { get; private set; }

	public bool Update(float deltaTime)
	{
		CurrentFramesPerSecond = 1f / deltaTime;
		_sampleBuffer.Enqueue(CurrentFramesPerSecond);
		if (_sampleBuffer.Count > 100)
		{
			_sampleBuffer.Dequeue();
			AverageFramesPerSecond = _sampleBuffer.Average((float i) => i);
		}
		else
		{
			AverageFramesPerSecond = CurrentFramesPerSecond;
		}
		TotalFrames++;
		TotalSeconds += deltaTime;
		return true;
	}
}
