using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShultzeResponse
{
    public class ResponseData
    {
        public Int32 OptionID { get; set; }
        public Int32 Priority { get; set; }
        public Int32 ThemeID { get; set; }
        public Int32 Session { get; set; }

        public ResponseData(DataRow source)
        {
            //throw new NotImplementedException();
            OptionID = (Int32)source["OPTION_ID"];
            Priority = (Int32)source["PRIORITY"];
            ThemeID = (Int32)source["THEME_ID"];
            Session = (Int32)source["SESSION"];
        }

    }
}