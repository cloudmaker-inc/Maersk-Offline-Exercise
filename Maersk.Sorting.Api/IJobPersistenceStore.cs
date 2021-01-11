using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface IJobPersistenceStore
    {
        /// <summary>
        ///     Gets the list of persisted sorting jobs.
        /// </summary>
        /// <returns>The list of persisted jobs.</returns>
        IEnumerable<SortJob> GetPersistedSortJob();

        /// <summary>
        ///     Persists the sorting job in in-memory object.
        /// </summary>
        /// <param name="sortJob">The sort object.</param>
        /// <param name="isUpdateOperation">The value which tells us whether this method is used for adding or updating jobs.</param>
        /// <param name="updatingJobId">The job id of the updating job.</param>
        /// <returns>The persisted sorting job.</returns>
        Task<SortJob> PersistSortJob(
            SortJob sortJob,
            bool isUpdateOperation = false,
            Guid updatingJobId = default);
    }
}
