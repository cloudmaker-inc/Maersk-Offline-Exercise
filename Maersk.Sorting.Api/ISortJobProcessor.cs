using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface ISortJobProcessor
    {
        Task<SortJob> Process(SortJob job);

        /// <summary>
        ///     Enqueues the sorting job request coming from the API.
        /// </summary>
        /// <param name="job">The sorting job.</param>
        /// <returns>The response of the sort job.</returns>
        Task<SortJob> EnqueueJob(SortJob job);

        /// <summary>
        ///     Gets the enqueued job from job persistence store.
        /// </summary>
        /// <param name="jobId">The jobId.</param>
        /// <returns>The requested sorting job.</returns>
        SortJob GetEnqueuedJob(Guid jobId);

        /// <summary>
        ///     Gets the list of enqueued jobs from the job persistence store.s
        /// </summary>
        /// <returns>The list of sorting jobs.</returns>
        IEnumerable<SortJob> GetEnqueuedJobs();
    }
}