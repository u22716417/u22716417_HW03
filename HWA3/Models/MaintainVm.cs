using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HWA3.Models
{
    public class MaintainVm
    {
        public PagedList<author> authorList;

        public PagedList<type> typeList;

        public PagedList<borrow> borrowsList;
    }
}