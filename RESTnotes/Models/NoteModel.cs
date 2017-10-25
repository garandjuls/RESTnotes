using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace RESTnotes.Models
{

    public class Notes
    {
       public IList<NoteModel> notes = new List<NoteModel>();
    }


    public class NoteModel
    {
        public int id { get; set; }
        
        public string body { get; set; }

        public NoteModel() {
            id = 0;
            body = "";
        }
        public NoteModel(int pid)
        {
            id = pid;
            body = "";
        }
        public NoteModel(int pid, string pbody)
        {
            id = pid;
            body = pbody;
        }

    }
}