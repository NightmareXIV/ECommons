using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using ECommons;

namespace ECommons.Singletons;
/// <summary>
/// Simple singleton service manager. Extend this class and fill it up with fields/properties of your singleton services, then create an instance of it whenever you want. Any uninitialized instances will be initialized upon constructor call, and anything that implements <see cref="IDisposable"/> will be disposed together with <see cref="ECommonsMain.Dispose"/> call.
/// </summary>
public abstract class SingletonServiceManager
{
		internal static List<SingletonServiceManager> RegisteredManagers = [];

		internal static void DisposeAll()
		{
				foreach(var x in RegisteredManagers)
				{
						x.Dispose();
				}
				RegisteredManagers = null!;
		}

		public SingletonServiceManager()
		{
				RegisteredManagers.Add(this);
				foreach (var x in this.GetType().GetFieldPropertyUnions(ReflectionHelper.AllFlags))
				{
						var value = x.GetValue(this);
						if(value == null)
						{
								try
								{
										PluginLog.Debug($"Creating singleton instance of {x.UnionType.FullName}");
										x.SetValue(this, Activator.CreateInstance(x.UnionType, true));
								}
								catch(Exception e)
								{
										e.Log();
								}
						}
				}
		}

		private void Dispose()
		{
				foreach (var x in this.GetType().GetFieldPropertyUnions(ReflectionHelper.AllFlags))
				{
						var value = x.GetValue(this);
						if(value is IDisposable disposable)
						{
								try
								{
										PluginLog.Debug($"Disposing singleton instance of {x.UnionType.FullName}");
										disposable.Dispose();
								}
								catch(Exception e)
								{
										e.Log();
								}
						}
				}
		}
}
