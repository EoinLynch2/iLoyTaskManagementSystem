using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iLoyTaskManagementSystem.Models
{
    public class State
    {
        public int StateId { get; set; }
        [StringLength(200)]
        [Index(IsUnique = true)]
        public string StateName { get; set; }
    }
}