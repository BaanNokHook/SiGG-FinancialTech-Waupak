using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GM.Application.Web.Models
{
      public class AdminViewModel
      {
         [Display(Name = "Asof Date")]
         public string asof_date { get; set; }  

         public string type {get; set; }
      }
}