using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HWA3.Models
{
    public class StudentBookVM
    {
        public PagedList<student> students { get; set; } 
        public PagedList<book> books { get; set; } 

    }
}