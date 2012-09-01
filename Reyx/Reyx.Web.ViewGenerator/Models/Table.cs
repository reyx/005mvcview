using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Reyx.Web.ViewGenerator.Models
{
    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public string ClassName
        {
            get
            {
                return HumanName.Replace(" ", "");
            }
        }
        public string HumanName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                string[] names = Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string name in names)
                {
                    sb.Append(" " + name[0].ToString().ToUpper() + name.Substring(1).ToLower());
                }

                return sb.ToString().Substring(1);
            }
        }
    }
}