using System.Threading.Tasks;

namespace Sexy.TodLib;

public class ReanimatorTrack
{
	private string name;

	public ReanimatorTransform[] mTransforms;

	public short mTransformCount;

	public bool IsAttacher;

	public string mName
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
			IsAttacher = name.StartsWith("attacher__");
		}
	}

	public ReanimatorTrack(string name, int transformCount)
	{
		mName = name;
		mTransformCount = (short)transformCount;
		mTransforms = new ReanimatorTransform[mTransformCount];
	}

	public override string ToString()
	{
		return name;
	}

	public void ExtractImages()
	{
		Task[] array = new Task[16];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Task.Factory.StartNew((object? j) =>
			{
				for (int k = (int)j; k < mTransforms.Length; k += 16)
				{
					mTransforms[k].ExtractImages();
				}
			}, i);
		}
		Task.WaitAll(array);
	}
}
