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

        /// <summary>
        /// Returns all tasks in database.
        /// </summary>
        /// <returns></returns>
        [Route("TmsTask/"), HttpGetAttribute]
        public IEnumerable<TmsTask> Get()
        {
            return _context.TmsTask;
        }


        /// <summary>
        /// Returns a Task that matches a provided id. 
        /// Returns a Bad Request(400) if no TmsTask exists for id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("TmsTask/{id}"), HttpGetAttribute]
        public TmsTask Get(int id)
        {
            var TmsTask = _context.TmsTask.Find(id);
            return TmsTask;
        }

        /// <summary>
        /// Deletes a task that matches a provided id. 
        /// Returns Ok(200) status code if delete successful.
        /// Returns a Bad Request(400) if no TmsTask exists for id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("TmsTask/Delete/{id}"), HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var tmsTask = _context.TmsTask.Find(id);
            if (tmsTask == null)
                return BadRequest("No task found with that id");
            _context.TmsTask.Remove(tmsTask);
            _context.SaveChanges();
            return Ok();
        }


        /// <summary>
        /// Adds a new TmsTask. If parentID is provided, the TmsTask provided will be added as a subtask of the task associated with ParentId. 
        /// Returns Ok(200) status code if added successfully.
        /// Returns Bad Request(400) status code if State is not provided.
        /// </summary>
        /// <param name="tmsTask"></param>
        /// <param name="parentTaskId"> </param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates a TmsTask based on a provided id and TmsTask data to be updated. 
        /// Returns Ok(200) status code if update successful. 
        /// Returns Bad Request(400) status code if no TmsTask exists for id.
        /// </summary>
        /// <param name="id"> the id provided</param>
        /// <param name="tmsTask"></param>
        /// <returns></returns>
        [Route("TmsTask/Update/{id}"), HttpPut]
        public IHttpActionResult Update(int id, [FromBody] TmsTask tmsTask)
        {
            var lookupTask = _context.TmsTask.Find(id);
            if (id <= 0)
                return BadRequest();
            else if (lookupTask == null)
                return BadRequest("No task exists for this id");
            //TODO: add lookup, throw error if not exist


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


        /// <summary>
        /// Returns a downloadable csv file for all InProgress TmsTasks that started before a given date.
        /// Date parameter should in format : GetInProgressTasks?date=2021-06-21T05:04:18.070Z so timezone is not lost.
        /// Returns Bad Request(400) if date not provided.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [Route("GetInProgressTasks/{date?}")]
        public HttpResponseMessage GetInprogressTasksForDate(DateTimeOffset date)
        {
            if (date == null)
            {
                HttpError err = new HttpError("A date must be provided");
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            //Url param should be passed like: GetInProgressTasks?date=2021-06-21T05:04:18.070Z

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            var taskList = _context.TmsTask
                .Where(t => t.StartDate <= date && t.State == "InProgress")
                .ToList();
        
            string csv = "";
            foreach(TmsTask tmsTask in taskList)
            {
                csv += "TaskName:" + tmsTask.TaskName + ",";
                csv += "Description:" + tmsTask.Description + ",";
                csv += "StartDate:" + tmsTask.StartDate + ",";
                csv += "FinishDate:" + tmsTask.FinishDate + ",";
                csv += "State:" + tmsTask.State + ",\n";
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


        /// <summary>
        /// Will update status of parent task id based on following rules
        /// Completed, if all subtasks have state Completed
        /// InProgress, if there is at least one subtask with state inProgress
        /// Planned in all other cases.
        /// </summary>
        /// <param name="ParentTaskId"></param>
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

        /// <summary>
        /// Returns true if tasks exists based on id.
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool IsTaskExists(int? taskId)
        {
            bool isTaskExists = false;
            if (_context.TmsTask.Any(t => t.TmsTaskId == taskId))
                isTaskExists = true;
            return isTaskExists;
        }

    }
}