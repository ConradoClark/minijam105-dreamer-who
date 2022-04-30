using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class FrameVariables<T>
{
    private static readonly Dictionary<string, T> Variables = new Dictionary<string, T>();

    public static (T Variable, Action ClearAction) Get(string variable, Func<T> getter)
    {
        if (Variables.ContainsKey(variable)) return ( Variables[variable], Clear);
        return (Variables[variable] = getter(), Clear);
    }

    public static void Clear()
    {
        Variables.Clear();
    }
}
