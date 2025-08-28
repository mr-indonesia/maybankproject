using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities //Core.Entities
{
    public partial class Blog : BaseEntity
    {
        public string BlogTitle { get; set; }
        public string BlogSnippet { get; set; }
        public string Body { get; set; }
    }
}
