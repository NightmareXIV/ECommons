using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using ECommons;
using System.Reflection;
using System.Linq;

namespace ECommons.Singletons;
/// <summary>
/// Simple singleton service manager. Create a static class and fill it up with fields/properties of your singleton services, then call <see cref="Initialize"/> with your static class type whenever you want. Any uninitialized instances will be initialized upon <see cref="Initialize"/> call, and anything that implements <see cref="IDisposable"/> will be disposed together with <see cref="ECommonsMain.Dispose"/> call.
/// </summary>
public static class SingletonServiceManager
{
		internal static List<Type> Types = [];

		internal static void DisposeAll()
		{
				foreach(var x in Types)
				{
						foreach (var t in x.GetFieldPropertyUnions(ReflectionHelper.AllFlags).Reverse())
						{
								var value = t.GetValue(null);
								if (value is IDisposable disposable)
								{
										try
										{
												PluginLog.Debug($"Disposing singleton instance of {t.UnionType.FullName}");
												disposable.Dispose();
										}
										catch (Exception e)
										{
												e.Log();
										}
								}
								t.SetValue(null, null);
						}
				}
				Types = null!;
		}

		public static void Initialize(Type staticType)
		{
				Types.Add(staticType);
				foreach (var x in staticType.GetFieldPropertyUnions(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
				{
						var value = x.GetValue(null);
						if(value == null)
						{
								try
								{
										PluginLog.Debug($"Creating singleton instance of {x.UnionType.FullName}");
										x.SetValue(null, Activator.CreateInstance(x.UnionType, true));
								}
								catch(Exception e)
								{
										e.Log();
								}
						}
				}
		}
}
