using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation.NeoTaskManager;
public partial class TaskManager
{
		/// <summary>
		/// Provides temporary dedicated storage for tasks where tasks are put for future use.
		/// </summary>
		public List<TaskManagerTask> Stack { get; init; } = [];

		/// <summary>
		/// Whether stack mode is active. When active, newly enqueued and inserted tasks go into stack instead of main queue.
		/// </summary>
		public bool IsStackActive { get; private set; } = false;

		/// <summary>
		/// Enables stack mode. Euqueue and Insert calls will go into the stack instead of queue after this call.
		/// </summary>
		public void BeginStack()
		{
				if (DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Stack mode begins");
				Stack.Clear();
				IsStackActive = true;
		}

		/// <summary>
		/// Enqueues the whole stack of tasks into the end of primary queue, disables stack mode and clears the stack afterwards.
		/// </summary>
		public void EnqueueStack()
		{
				if (DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Enqueueing stack with {Stack.Count} tasks");
				IsStackActive = false;
				this.EnqueueMulti([.. Stack]);
				Stack.Clear();
		}

		/// <summary>
		/// Inserts the whole stack of tasks into the beginning of primary queue, disables stack mode and clears the stack afterwards.
		/// </summary>
		public void InsertStack()
		{
				if (DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Inserting stack with {Stack.Count} tasks");
				IsStackActive = false;
				this.InsertMulti([.. Stack]);
				Stack.Clear();
		}

		/// <summary>
		/// Disables stack mode and clears the stack.
		/// </summary>
		public void DiscardStack()
		{
				if (DefaultConfiguration.ShowDebug == true) PluginLog.Debug($"Discarding stack with {Stack.Count} tasks");
				IsStackActive = false;
				Stack.Clear();
		}
}
