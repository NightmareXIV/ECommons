using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Automation
{
    internal record class TaskManagerTask
    {
        internal Func<bool?> Action;
        internal int TimeLimitMS;
        internal bool AbortOnTimeout;

        internal TaskManagerTask(Func<bool?> action, int timeLimitMS, bool abortOnTimeout)
        {
            Action = action;
            TimeLimitMS = timeLimitMS;
            AbortOnTimeout = abortOnTimeout;
        }
    }
}
