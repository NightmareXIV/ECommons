/*namespace ECommons.Singletons;
public static class LazySingleton
{
		/// <summary>
		/// Accesses a singleton instance of specified type. If it wasn't initialized yet, initializes it. Twice as slow as <see cref="Singleton.Instance"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Instance<T>()
		{
				if (SingletonT<T>.Instance == null)
				{
						SingletonManager.InitializeForType(typeof(T));
				}
				return SingletonT<T>.Instance!;
		}
}
*/