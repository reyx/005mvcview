using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reyx.Lib.Mvc.Admin
{
    class DecimalField : Field
    {
        public int MaxDigits { get; set; }
        public int DecimalPlaces { get; set; }
    }
}
