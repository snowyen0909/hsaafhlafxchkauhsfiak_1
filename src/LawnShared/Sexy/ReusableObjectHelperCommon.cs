using System.Collections.Generic;

namespace Sexy;

internal class ReusableObjectHelperCommon<T> where T : IReusable
{
	protected static Stack<T> unusedInstances;

	protected static ReusableObjectHelperCommon<T> singleton;

	protected ReusableObjectHelperCommon()
	{
	}

	public static T GetNewNullable()
	{
		if (unusedInstances.Count != 0)
		{
			T val = unusedInstances.Pop();
			singleton.ResetOnGet(val);
			return val;
		}
		_ = (object)default(T);
		return default;
	}

	public static void PushOnToReuseStack(T obj)
	{
		singleton.ResetOnPush(obj);
		unusedInstances.Push(obj);
	}

	public virtual void ResetOnGet(T obj)
	{
	}

	public virtual void ResetOnPush(T obj)
	{
	}

	public static void Initialize(int count)
	{
		unusedInstances = new Stack<T>(count);
		singleton = new ReusableObjectHelperCommon<T>();
	}

	public static void Initialize()
	{
		unusedInstances = new Stack<T>();
		singleton = new ReusableObjectHelperCommon<T>();
	}

	public static void Dispose()
	{
		unusedInstances = null;
	}
}
