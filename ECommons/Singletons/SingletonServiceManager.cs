using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;

namespace ECommons.Singletons;
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
