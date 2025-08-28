using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models
{
    public partial class SingleObjectValue
    {
        public object Value { get; set; }
    }

    public partial class DualObjectValue
    {
        public object Value1 { get; set; }
        public object Value2 { get; set; }
    }

    public partial class ValueLable
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }

    public partial class ErrorMsg
    {
        public string Error { get; set; }
        public string Module { get; set; }
    }

    public partial class AddReturn
    {
        public bool Result { get;set; }
        public List<ErrorMsg> Errors { get; set; }
    }
}
