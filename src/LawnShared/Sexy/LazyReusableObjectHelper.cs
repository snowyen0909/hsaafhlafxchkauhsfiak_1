namespace Sexy;

internal class LazyReusableObjectHelper<T> : ReusableObjectHelperCommon<T> where T : IReusable
{
	public override void ResetOnGet(T obj)
	{
		obj.Reset();
	}
}
