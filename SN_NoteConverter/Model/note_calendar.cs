//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SN_NoteConverter.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class note_calendar
    {
        public int id { get; set; }
        public System.DateTime date { get; set; }
        public int type { get; set; }
        public string description { get; set; }
        public string group_maid { get; set; }
        public string group_weekend { get; set; }
        public int max_leave { get; set; }
        public string rec_by { get; set; }
    }
}