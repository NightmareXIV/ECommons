using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Singletons;
/// <summary>
/// Very fast singleton system. Access is very fast, the same as regular static instance. Types that implement IDisposable will be disposed upon calling <see cref="ECommonsMain.Dispose"/>. Every singleton must have public or non-public parameterless constructor.
/// </summary>
public class SingletonManager
{
		internal static List<IDisposable> Disposables = [];
		/// <summary>
		/// Initializes singleton instance for a single specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		public static void InitializeForType(Type type)
		{
				try
				{
						var singletonT = typeof(SingletonT<>).MakeGenericType(type);
						if (singletonT.GetField(nameof(SingletonT<object>.Instance), BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null) != null)
						{
								throw new InvalidOperationException($"Singleton for type {type} is already initialized.");
						}
						var obj = Activator.CreateInstance(type, true);
						singletonT.GetField(nameof(SingletonT<object>.Instance), BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, obj);
						PluginLog.Debug($"Assigned singleton instance {type.FullName}");
						if (obj is IDisposable disposable)
						{
								Disposables.Insert(0, disposable);
								PluginLog.Debug($"→Will be auto-disposed");
						}
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
}
