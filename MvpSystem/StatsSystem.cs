using System;
using System.Linq;
using System.Reflection;

namespace MvpSystem;

internal static class StatsSystem
{
    internal static void TryIncrementMVPs(object playerLike)
    {
        if (playerLike == null) return;

        try
        {
            var providedType = playerLike.GetType();
            if (!string.Equals(providedType.Name, "Player", StringComparison.Ordinal)) return;

            var ext = FindSuitableStaticIncrementMethodForFirstParam(providedType);
            if (ext == null) return;
            var args = new[] { playerLike, "MVPs", 1L };
            ext.Invoke(null, args);
        }
        catch
        {
            // swallow exceptions - no-op if not available
        }
    }

    private static MethodInfo FindSuitableStaticIncrementMethodForFirstParam(Type firstParamType)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(x => x != null).ToArray();
            }
            catch
            {
                continue;
            }

            foreach (var type in types)
            {
                if (type == null) continue;
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(m => string.Equals(m.Name, "IncrementStat", StringComparison.Ordinal));

                foreach (var m in methods)
                {
                    var parms = m.GetParameters();
                    if (parms.Length >= 1 && parms[0].ParameterType.IsAssignableFrom(firstParamType))
                        return m;
                }
            }
        }


        return null;
    }
}