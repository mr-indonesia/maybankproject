using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace App.Core.Models
{
    public class ResponseModel<T>
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
