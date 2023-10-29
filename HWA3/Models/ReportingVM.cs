using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HWA3.Models
{
    public class ReportingVM
    {
        public List<PopularBooksVM> popularBooks {  get; set; } = new List<PopularBooksVM>();

        public List<ArchiveDetails> ArchiveDetails { get; set; } = new List<ArchiveDetails>();

    }
}