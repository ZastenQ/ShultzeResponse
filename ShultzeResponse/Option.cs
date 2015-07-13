using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShultzeResponse
{
    public class Option
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }

        public Option(DataRow source)
        {
            //throw new NotImplementedException();
            ID = (Int32)source["ID"];
            Name = (String)source["NAME"];
        }
    }
}
