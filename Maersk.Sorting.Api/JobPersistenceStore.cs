using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class JobPersistenceStore : IJobPersistenceStore
    {
        private static IDictionary<Guid, SortJob> storedData = new Dictionary<Guid, SortJob>();

        /// <summary>
        ///     Gets the list of persisted sorting jobs.
        /// </summary>
        /// <returns>The list of persisted jobs.</returns>
        public IEnumerable<SortJob> GetPersistedSortJob()
        {
            return storedData.Values;
        }

        /// <summary>
        ///     Persists the sorting job in in-memory object.
        /// </summary>
        /// <param name="sortJob">The sort object.</param>
        /// <param name="isUpdateOperation">The value which tells us whether this method is used for adding or updating jobs.</param>
        /// <param name="updatingJobId">The job id of the updating job.</param>
        /// <returns>The persisted sorting job.</returns>
        public async Task<SortJob> PersistSortJob(
            SortJob sortJob,
            bool isUpdateOperation = false,
            Guid updatingJobId = default)
        {
            if(isUpdateOperation)
            {
                return await this.UpdatePersistedJob(sortJob, updatingJobId);
            }

            storedData.Add(sortJob.Id, sortJob);

            return sortJob;
        }

        private async Task<SortJob> UpdatePersistedJob(SortJob sortJob, Guid jobId)
        {
            if (storedData.ContainsKey(jobId))
            {
                storedData.Remove(jobId);
                storedData.Add(jobId, sortJob);
            }

            return await Task.Run(() => sortJob);
        }
    }
}
