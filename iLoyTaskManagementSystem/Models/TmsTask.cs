using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace iLoyTaskManagementSystem.Models
{
    public class TmsTask
    {
        public int TmsTaskId { get; set;}
        [StringLength(200)]
        [Index(IsUnique=true)]
        public string TaskName{ get; set; }
        public string  Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset FinishDate { get; set; }
        public string State { get; set; }


        public virtual int? ParentTmsTaskId { get; set; }
    }
}