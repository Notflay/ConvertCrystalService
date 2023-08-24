using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STR_GP_CONVERT_DOCUMENT
{
    public class SBODataField
    {
        public int IDFIla { get; set; }
        public int SubIDFIla { get; set; }
        public SAPbobsCOM.BoFieldTypes DataType { get; set; }
        public string Value { get; set; }
    }
}
