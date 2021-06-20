using iLoyTaskManagementSystem.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;

namespace iLoyTaskManagementSystem.Controllers
{
    public class TmsTaskController : ApiController
    {

        private ApplicationDbContext _context;

        public TmsTaskController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            //properly dispose of the db context object.
            _context.Dispose();
        }

        [Route("TmsTask/"), HttpGetAttribute]
        public IEnumerable<TmsTask> Get()
        {
            return _context.TmsTask;
        }

        [Route("TmsTask/{id}"), HttpGetAttribute]
        public TmsTask Get(int id)
        {
            var TmsTask = _context.TmsTask.Find(id);
            return TmsTask;
        }

        [Route("TmsTask/Add/{parentTaskId?}"), HttpPost]
        public IHttpActionResult Post([FromBody]TmsTask  tmsTask, int? parentTaskId = null)
        {
            if (tmsTask.State == null)
                return BadRequest("The state must exist");

            if(parentTaskId != null)
                if (IsTaskExists(parentTaskId))
                    tmsTask.ParentTmsTaskId = (int)parentTaskId;
                else
                    return BadRequest("The specified parent task does not exist");

            try
            {
                _context.Set<TmsTask>().Add(tmsTask);
                _context.SaveChanges();
            }
            catch (DbUpdateException updateException)
            {
                //Log exception //Don't give detailed error message in production...
                return BadRequest("Update Exception: " + updateException.ToString());
            }
            return Ok();
        }

        [Route("TmsTask/Update/{id}"), HttpPut]
        public IHttpActionResult Update(int id, [FromBody]TmsTask tmsTask)
        {
            var entity = _context.TmsTask.FirstOrDefault(t => t.TmsTaskId == id);
            entity.TaskName = tmsTask.TaskName;
            entity.Description = tmsTask.Description;
            entity.StartDate = tmsTask.StartDate;
            entity.FinishDate = tmsTask.FinishDate;
            entity.State = tmsTask.State;
            _context.SaveChanges();
            return Ok();
        }

        public void UpdateParentTaskStatus(int ParentTaskId)
        {
            string parentTaskState = "Planned";

            int totalSubtasks = _context.TmsTask
                .Where(t => t.ParentTmsTaskId == ParentTaskId)
                .Count();

            int totalCompletedSubtasks = _context.TmsTask
                .Where(t => t.ParentTmsTaskId == ParentTaskId && t.State == "Completed").Count();

            if (totalCompletedSubtasks == totalSubtasks)
            {
                parentTaskState = "Completed";
            }
            else
            {
                var InProgressSubtasks = _context.TmsTask
                    .Where(t => t.ParentTmsTaskId == ParentTaskId && t.State == "InProgress").ToList();
                parentTaskState = "InProgress";
            }

            TmsTask parentTmsTask = _context.TmsTask.Find(ParentTaskId);
            parentTmsTask.State = parentTaskState;
            _context.SaveChanges();

        }



        public bool IsTaskExists(int? taskId)
        {
            bool isTaskExists = false;
            if (_context.TmsTask.Any(t => t.TmsTaskId == taskId))
                isTaskExists = true;
            return isTaskExists;
        }

    }
}