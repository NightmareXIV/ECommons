/*using System;

namespace ECommons.Singletons;
/// <summary>
/// You may use this attribute to specify singleton creation order via <see cref="Singleton.InitializeForAssembly(System.Reflection.Assembly, Type)"/> method. By default, all singletons have priority=0. <br></br>Creation order is negative to positive (-1, then 0, then 1). <br></br>Disposal order is always reverse of the creation order.
/// </summary>
/// <param name="priority"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PriorityAttribute(int priority) : Attribute
{
		public int Priority => priority;
}
*/