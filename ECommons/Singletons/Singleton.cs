using ECommons.Logging;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Singletons;
public static class Singleton
{
		/// <summary>
		/// Accesses a singleton instance of specified type, if it was initialized before.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Instance<T>()
		{
				return SingletonT<T>.Instance;
		}
}
