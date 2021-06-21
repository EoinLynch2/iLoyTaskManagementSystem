using iLoyTaskManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

        [Route("TmsTask/Delete/{id}"), HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var tmsTask = _context.TmsTask.Find(id);
            _context.TmsTask.Remove(tmsTask);
            _context.SaveChanges();
            return Ok();
        }

        [Route("TmsTask/Add/{parentTaskId?}"), HttpPost] //if parent taskId is specified, the TmsTask object will be a subtask assigned to the parentTask of associated id
        public IHttpActionResult Post([FromBody] TmsTask tmsTask, int? parentTaskId = null)
        {
            if (tmsTask.State == null)
                return BadRequest("The state must exist");

            bool isParentTaskExists = false;
            if (parentTaskId != null)
            {
                isParentTaskExists = IsTaskExists(parentTaskId);

                if (isParentTaskExists)
                    tmsTask.ParentTmsTaskId = (int)parentTaskId;
                else
                    return BadRequest("The specified parent task does not exist");
            }

            try
            {
                _context.Set<TmsTask>().Add(tmsTask);
                _context.SaveChanges();
                if (isParentTaskExists)
                {
                    UpdateParentTaskStatus((int)parentTaskId);
                }
            }
            catch (DbUpdateException updateException)
            {
                //Log exception //Don't give detailed error message in production...
                return BadRequest("Update Exception: " + updateException.ToString());
            }
            return Ok();
        }

        [Route("TmsTask/Update/{id}"), HttpPut]
        public IHttpActionResult Update(int id, [FromBody] TmsTask tmsTask)
        {
            var entity = _context.TmsTask.FirstOrDefault(t => t.TmsTaskId == id);
            entity.TaskName = tmsTask.TaskName;
            entity.Description = tmsTask.Description;
            entity.StartDate = tmsTask.StartDate;
            entity.FinishDate = tmsTask.FinishDate;
            entity.State = tmsTask.State;
            _context.SaveChanges();

            if (entity.ParentTmsTaskId > 0)
                UpdateParentTaskStatus((int)entity.ParentTmsTaskId);
            else
            {
                int numOfSubtasksOfThisTask = _context.TmsTask
                    .Where(t => t.ParentTmsTaskId == id).Count();
                if (numOfSubtasksOfThisTask > 0)
                    //If a parents task status is manually overridden, we will call update the task status to ensure correctness.
                    UpdateParentTaskStatus(id);
            }


            return Ok();
        }

        [Route("GetInProgressTasks/")]
        public HttpResponseMessage GetInprogressTasksForDate(/*DateTimeOffset date*/)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            var taskList = _context.TmsTask;
            ///*.Where(t => t.State == "InProgress")*/.ToList();
            //List<String> myTestList = new List<string> { "This", "Is", "Test" };
            string csv = "";
            foreach(TmsTask tmsTask in taskList)
            {
                csv += "TaskName:" + tmsTask.TaskName + ",";
                csv += "Description:" + tmsTask.Description + ",";
                csv += "StartDate:" + tmsTask.StartDate + ",";
                csv += "FinishDate:" + tmsTask.FinishDate + ",";
                csv += "State:" + tmsTask.State + ",";
            }


            writer.Write(csv);
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Export.csv" };
            return result;

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
                int totalInProgressSubtasks = _context.TmsTask
                    .Where(t => t.ParentTmsTaskId == ParentTaskId && t.State == "InProgress").Count();
                if(totalInProgressSubtasks > 0)
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