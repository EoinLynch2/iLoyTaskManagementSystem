using iLoyTaskManagementSystem.Models;
using iLoyTaskManagementSystem.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Moq;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.Results;

namespace iLoyTaskManagementSystem.Tests.Controllers
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TmsTaskTest
    {
        public TmsTaskTest()
        {
            //
            // TODO: Add constructor logic here
            //

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        /*[TestMethod]
        public void Get_returns_list_of_tmsTasks()
        {
            TmsTask t1 = new TmsTask { TmsTaskId = 1, TaskName = "Plan", Description = "Plan the sprint", State = "InProgress" };
            TmsTask t2 = new TmsTask { TmsTaskId = 2, TaskName = "Do", Description = "Implementation phase", State = "InProgress" };
            TmsTask t3 = new TmsTask { TmsTaskId = 3, TaskName = "Test", Description = "Customer testing", State = "InProgress" };
            TmsTask t4 = new TmsTask { TmsTaskId = 4, TaskName = "Review", Description = "Review the project", State = "Planned" };

            var data = new List<TmsTask> { t1, t2, t3, t4 };//.AsQueryable();

            var mockSet = DbSetMock.CreateFrom(data);
            var mockContext = new Mock<TmsTaskContext>();
            mockContext.Setup(c => c.TmsTasks).Returns(mockSet.Object);

            var tmsTaskController = new TmsTaskController(mockContext.Object);

            var tmsTasks = tmsTaskController.Get();
            Assert.IsTrue(tmsTasks.Contains(t1));
            Assert.IsTrue(tmsTasks.Contains(t2));
            Assert.IsTrue(tmsTasks.Contains(t3));
            Assert.IsTrue(tmsTasks.Contains(t4));
        }*/

       /*[TestMethod]
        public void TmsTaskAdd_saves_a_tms_task_via_context()
        {
            var mockSet = new Mock<DbSet<TmsTask>>();

            var mockContext = new Mock<TmsTaskContext>();
            mockContext.Setup(m => m.TmsTasks).Returns(mockSet.Object);

            /*var tmsTaskController = new TmsTaskController(mockContext.Object);
            TmsTask tmsTask = new TmsTask { 
                TmsTaskId = 1, 
                TaskName = "Newly Created Task", 
                Description = "This task should be added in unit test" };
            tmsTaskController.Post(tmsTask);

            mockSet.Verify(m => m.Add(It.IsAny<TmsTask>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }*/

        [TestMethod]
        public void TmsTaskUpdate_contains_valid_id_and_body()
        {
            var tmsTaskController = new TmsTaskController();
            TmsTask t1 = new TmsTask { TaskName = "Plan", Description = "Plan the sprint", State = "InProgress" };
            IHttpActionResult invalidActionResult = tmsTaskController.Update(-11, t1);
            IHttpActionResult validActionResult = tmsTaskController.Update(21, t1);

            Assert.IsInstanceOfType(invalidActionResult, typeof(BadRequestResult));
            Assert.IsInstanceOfType(validActionResult, typeof(OkResult));
        }

        [TestMethod]
        public void Task_add_throws_correct_responses()
        {
            var tmsTaskController = new TmsTaskController();
            TmsTask noStateTask = new TmsTask { TaskName = "TestTask", Description = "This task has no state"};
            TmsTask t1 = new TmsTask { TaskName = "MyNewTask", Description = "Sample task", State = "InProgress"};
            IHttpActionResult noStateActionResult = tmsTaskController.Post(noStateTask);
            IHttpActionResult nonExistingParentResult = tmsTaskController.Post(t1, 10000);
            IHttpActionResult addedSuccessfullyResult = tmsTaskController.Post(t1);

            Assert.IsInstanceOfType(noStateActionResult, typeof(BadRequestResult));
            Assert.IsInstanceOfType(nonExistingParentResult, typeof(BadRequestResult));
            Assert.IsInstanceOfType(addedSuccessfullyResult, typeof(OkResult));
        }

        [TestMethod]
        public void Task_delete_throws_correct_responses()
        {
            var tmsTaskController = new TmsTaskController();
            int nonExistingId = 1111;
            int existingId = 1;
            IHttpActionResult nonExistingTaskDeleteResult = tmsTaskController.Delete(nonExistingId);
            //IHttpActionResult existingTaksDeleteResult = tmsTaskController.Delete(1); don't actually delete from live database - proof of concept.

            Assert.IsInstanceOfType(nonExistingTaskDeleteResult, typeof(BadRequestResult));
        }
    }
}
