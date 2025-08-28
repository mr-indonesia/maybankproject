using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models
{
    public class CompanyModel
    {
        public partial class CompanyInfo
        {
            public string CompanyName {  get; set; }
            public string EpType {  get; set; }
            public string EpCode { get; set; }
            public string ParentType {  get; set; }
            public string ParentCode {  get; set; }
            public string Ext {  get; set; }
            public string AreaCode {  get; set; }
            public string AreaName { get; set; }
            public string GroupDealerCode {  get; set; }
            public string GroupDealerName { get;set; }
            public string CurrencyCode {  get; set; }
            public bool IsDealer { get; set; }
            public bool IsGroupDealer {  set; get; }
            public bool IsArea {  get; set; }
            public string Value { get; set; }
            public string Label {  get; set; }
        }
    }
}
