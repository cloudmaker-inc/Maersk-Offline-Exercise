using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;

        /// <summary>
        ///     Initializes the class of type <see cref="SortController"/>.
        /// </summary>
        /// <param name="sortJobProcessor">The sorting job processor.</param>
        public SortController(
            ISortJobProcessor sortJobProcessor)
        {
            _sortJobProcessor = sortJobProcessor;
        }

        [HttpPost("run")]
        [Obsolete("This executes the sort job asynchronously. Use the asynchronous 'EnqueueJob' instead.")]
        public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        {
            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);

            return Ok(completedJob);
        }

        [HttpPost("jobs/enqueue")]
        public async Task<ActionResult<SortJob>> EnqueueJob(int[] values)
        {
            try
            {
                var pendingJob = new SortJob(
                    id: Guid.NewGuid(),
                    status: SortJobStatus.Pending,
                    duration: null,
                    input: values,
                    output: null);

                var jobStatus = await this._sortJobProcessor.EnqueueJob(pendingJob);

                return Ok(jobStatus);
            }
            catch (Exception ex)
            {
                return Problem($"Failed to enqueue the sorting request due to exception: {ex.Message}");
            }
        }

        [HttpGet("jobs")]
        public async Task<ActionResult<SortJob[]>> GetJobs()
        {
            try
            {
                var jobs = await Task.Run(() => this._sortJobProcessor.GetEnqueuedJobs());

                if (jobs.Any())
                {
                    return Ok(jobs);
                }

                return NotFound($"No jobs in the in-memory storage.");
            }
            catch (Exception ex)
            {
                return Problem($"Failed to get list of the enqueued jobs due to exception: {ex.Message}");
            }
        }

        [HttpGet("jobs/{jobId}")]
        public async Task<ActionResult<SortJob>> GetJob(Guid jobId)
        {
            try
            {
                var job = await Task.Run(() => this._sortJobProcessor.GetEnqueuedJob(jobId));

                if (job != null)
                {
                    return Ok(job);
                }

                return NotFound($"Job with Id {jobId} not found in the in-memory storage");
            }
            catch (Exception ex)
            {
                return Problem($"Failed to get job with Id {jobId} due to exception: {ex.Message}");
            }
        }
    }
}
