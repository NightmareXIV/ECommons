/*
 While the concept is maybe good, I decided that I do not want to do something like this yet.
 */

/*using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECommons.Singletons;
/// <summary>
/// Very fast singleton system. Access is very fast, the same as regular static instance. Types that implement IDisposable will be disposed upon calling <see cref="ECommonsMain.Dispose"/>. Every singleton must have public or non-public parameterless constructor.
/// </summary>
public class SingletonManager
{
		private static List<IDisposable> Disposables = [];
		private static List<Type> SingletonHolders = [];
		/// <summary>
		/// Initializes singleton instance for a single specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		public static void InitializeForType(Type type)
		{
				try
				{
						var singletonT = typeof(SingletonT<>).MakeGenericType(type);
						if (singletonT.GetFields(BindingFlags.Static | BindingFlags.NonPublic)[0]!.GetValue(null) != null)
						{
								throw new InvalidOperationException($"Singleton for type {type} is already initialized.");
						}
						var obj = Activator.CreateInstance(type, true);
						singletonT.GetFields(BindingFlags.Static | BindingFlags.NonPublic)[0]!.SetValue(null, obj);
						PluginLog.Debug($"Assigned singleton instance {type.FullName}");
						if (obj is IDisposable disposable)
						{
								Disposables.Insert(0, disposable);
								PluginLog.Debug($"→Will be auto-disposed");
						}
						SingletonHolders.Add(singletonT);
				}
				catch (Exception ex)
				{
						PluginLog.Error($"Failed to initialize singleton for {type.FullName}. Make sure a parameterless constructor is available.");
						ex.Log();
				}
		}

		/// <summary>
		/// Initializes all types from your plugin's assembly that implement <see cref="ISingleton"/>. Call this in your plugin's constructor.
		/// </summary>
		public static void Initialize()
		{
				InitializeForAssembly(ECommonsMain.Instance.GetType().Assembly, typeof(ISingleton));
		}

		/// <summary>
		/// Initializes all types from specified <paramref name="assembly"/> that implement <see cref="ISingleton"/>. 
		/// <br></br>Call this in secondary assemblies if you're using any. 
		/// </summary>
		/// <param name="assembly"></param>
		public static void InitializeForAssembly(Assembly assembly) => InitializeForAssembly(assembly, typeof(ISingleton));

		/// <summary>
		/// Initializes all types from specified <paramref name="assembly"/> that implement <paramref name="interface"/> as singletons. 
		/// <br/>Call this in secondary assemblies if you're using any. 
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="interface"></param>
		public static void InitializeForAssembly(Assembly assembly, Type @interface)
		{
				var types = new List<Type>();
				foreach (var x in assembly.GetTypes())
				{
						if (x.IsInterface || x.IsAbstract) continue;
						if (@interface.IsAssignableFrom(x))
						{
								types.Add(x);
						}
				}
				foreach (var x in types.OrderBy(x => x.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0))
				{
						InitializeForType(x);
				}
		}

		internal static void Dispose()
		{
				foreach (var x in SingletonManager.Disposables)
				{
						PluginLog.Debug($"Disposing {x.GetType().FullName}");
						GenericHelpers.Safe(() => x.Dispose());
				}
				foreach (var x in SingletonManager.SingletonHolders)
				{
						GenericHelpers.Safe(() => x.GetFields(BindingFlags.Static | BindingFlags.NonPublic)[0]!.SetValue(null, null));
				}
				Disposables = null!;
				SingletonHolders = null!;
		}
}
*/