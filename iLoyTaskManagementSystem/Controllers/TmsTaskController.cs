﻿using iLoyTaskManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
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

        [Route("TmsTask/{taskName}")]
        public string Get(string TaskName)
        {
            return TaskName;
        }

        [Route("TmsTask/Add")]
        public string Post([FromBody]TmsTask  tmsTask)
        {
            _context.TmsTask.Add(tmsTask);
            _context.SaveChanges();
            string name = tmsTask.Name;
            return "MyTaskName: " +  name;

        }

    }
}