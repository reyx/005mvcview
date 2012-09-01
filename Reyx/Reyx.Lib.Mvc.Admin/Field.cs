using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reyx.Lib.Mvc.Admin
{
    public class Field
    {
        public bool Null { get; set; }
        public bool Blank { get; set; }
        public Dictionary<string, string> Choices { get; set; }
        public string DbColumn { get; set; }
        public bool DbIndex { get; set; }
        public string DbTableSpace { get; set; }
        public string Default { get; set; }
        public bool Editable { get; set; }
        public Dictionary<int, string> ErrorMessages { get; set; }
        public string HelpText { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Unique { get; set; }
        public bool UniqueForDate { get; set; }
        public bool UniqueForMonth { get; set; }
        public bool UniqueForYear { get; set; }
        public string VerboseName { get; set; }
        // Validators {get;set;}
        public string Name { get; set; }
        public string MaxLength { get; set; }
        public string Rel { get; set; }
        public string Serialize { get; set; }
        
        public string AutoCreated { get; set; }

        public Field (
            bool Null = false,
             bool Blank = false,
             Dictionary<string, string> Choices = null,
             string DbColumn = "",
             bool DbIndex = false,
             string DbTableSpace = "",
             string Default = "",
             bool Editable = true,
             Dictionary<int, string> ErrorMessages = null,
             string HelpText = "",
             bool PrimaryKey = false,
             bool Unique = false,
             bool UniqueForDate = false,
             bool UniqueForMonth = false,
             bool UniqueForYear = false,
             string VerboseName = "",
             //,idators {get;set;}
             string Name = "",
             string MaxLength = "",
             string Rel = "",
             string Serialize = ""
        )
        {
            this.Name = Name;
            this.VerboseName = VerboseName;
            this.PrimaryKey = PrimaryKey;
            this.MaxLength = MaxLength;
            this.Unique = Unique;
            this.Blank = Blank;
            this.Null = Null;
            this.DbIndex = DbIndex;
            this.Rel = Rel;
            this.Default = Default;
            this.Editable = Editable;
            this.Serialize = Serialize;
            this.UniqueForDate = UniqueForDate;
            this.UniqueForMonth = UniqueForMonth;
            this.UniqueForYear = UniqueForYear;
            this.HelpText = HelpText;
            this.DbColumn = DbColumn;
            this.DbTableSpace = DbTableSpace;
            this.AutoCreated = AutoCreated;
        }
    }
}
