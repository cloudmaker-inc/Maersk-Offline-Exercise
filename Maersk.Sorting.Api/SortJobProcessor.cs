using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    /// <summary>
    ///     Represents the instance of the sorting job processor.
    /// </summary>
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly IJobPersistenceStore jobPersistenceStore;
        private readonly ILogger<SortJobProcessor> _logger;

        /// <summary>
        ///     Initializes the class of type <see cref="SortJobProcessor"/>.
        /// </summary>
        /// <param name="jobPersistenceStore">The in-memory sorting job persistence storage service.</param>
        /// <param name="logger">The logger service.</param>
        public SortJobProcessor(
            IJobPersistenceStore jobPersistenceStore,
            ILogger<SortJobProcessor> logger)
        {
            this.jobPersistenceStore = jobPersistenceStore;
            _logger = logger;
        }

        /// <summary>
        ///     Enqueues the sorting job request coming from the API.
        /// </summary>
        /// <param name="job">The sorting job.</param>
        /// <returns>The response of the sort job.</returns>
        public async Task<SortJob> EnqueueJob(SortJob job)
        {
            await this.jobPersistenceStore.PersistSortJob(job);

            await Task.Factory.StartNew(
                () => this.Process(job));

            return job;
        }

        /// <summary>
        ///     Gets the enqueued job from job persistence store.
        /// </summary>
        /// <param name="jobId">The jobId.</param>
        /// <returns>The requested sorting job.</returns>
        public SortJob GetEnqueuedJob(Guid jobId)
        {
            return this.GetSortedJobsInternal(jobId).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the list of enqueued jobs from the job persistence store.s
        /// </summary>
        /// <returns>The list of sorting jobs.</returns>
        public IEnumerable<SortJob> GetEnqueuedJobs()
        {
            return this.GetSortedJobsInternal();
        }

        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(5000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            var updatedJob = new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output);

            return await this.jobPersistenceStore.PersistSortJob(updatedJob, true, updatedJob.Id);
        }

        private IEnumerable<SortJob> GetSortedJobsInternal(
            Guid jobId = default)
        {
            if(jobId == Guid.Empty)
            {
                return this.jobPersistenceStore.GetPersistedSortJob();
            }

            return this.jobPersistenceStore.GetPersistedSortJob().Where(
                job => job.Id.Equals(jobId));
        }
    }
}
