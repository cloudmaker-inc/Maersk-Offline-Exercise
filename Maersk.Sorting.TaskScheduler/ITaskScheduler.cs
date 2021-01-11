using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.TaskFactory
{
    public interface ITaskScheduler
    {
        Task<Task> SafelyCreateEnqueuedTask(
            Task action,
            Guid serviceIdentifier);
    }
}
