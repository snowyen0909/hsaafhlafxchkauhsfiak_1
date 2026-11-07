namespace Sexy;

internal class ReusableObjectHelper<T> : ReusableObjectHelperCommon<T> where T : IReusable
{
	public override void ResetOnPush(T obj)
	{
		obj.Reset();
	}
}
