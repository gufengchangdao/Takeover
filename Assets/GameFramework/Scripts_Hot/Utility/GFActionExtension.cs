using System;
using GameFramework.AOT;

public static class GFActionExtension
{
    /// <summary>
    /// Safe invocation of all delegates stored in the passed <see cref="Action"/> delegate.
    /// </summary>
    public static void InvokeSafe(this Action action)
    {
        if (action == null)
            return;

        foreach (Action a in action.GetInvocationList())
        {
            try
            {
                a.Invoke();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    public static void InvokeSafe<T>(this Action<T> action, T arg)
    {
        if (action == null)
            return;

        foreach (Action<T> a in action.GetInvocationList())
        {
            try
            {
                a.Invoke(arg);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    public static void InvokeSafe<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        if (action == null)
            return;
        foreach (Action<T1, T2> a in action.GetInvocationList())
        {
            try
            {
                a.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    public static void InvokeSafe<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        if (action == null)
            return;
        foreach (Action<T1, T2, T3> a in action.GetInvocationList())
        {
            try
            {
                a.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}