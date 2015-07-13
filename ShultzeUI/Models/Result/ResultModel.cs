using ShultzeResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShultzeUI.Models.Result
{
    public class ResultModel
    {
        public IEnumerable<Theme> Themes { get; set; }
        public IEnumerable<Option> Options { get; set; }
        public Int32[,] Responses { get; set; }
    }
}