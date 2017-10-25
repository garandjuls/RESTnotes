using System;
using System.IO;
using System.Net;
using System.Web.Http;

namespace RESTnotes.Controllers
{
    public class notesController : ApiController
    {
        /// <summary>
        /// Json in, Json out
        /// 
        /// </summary>
        /// 

        private string sfilepath = AppDomain.CurrentDomain.BaseDirectory + "\\notes.db";
        private string sfilter = ""; // used for filtering by supplied value

        // GET api/values
        /// <summary>
        /// Get all items.
        /// </summary>
        /// <returns></returns>
        public Models.Notes Get()
        {
            return Get(0); // 0=get all
        }

        /// <summary>
        /// get by search value
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Models.Notes Get(string query)
        {
            if (query != null)
                sfilter = query;
            return Get(0); // 0=get all
        }

        /// <summary>
        ///  if query and id are passed then just do id and ignore query
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public Models.Notes Get(int id, string query) {
            if (query != null)
                sfilter = query;
            return Get(id);
        }

        /// <summary>
        /// Get by id. if id=0 get all. if filter not empty then apply filter 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Models.Notes Get(int id)
        {
            // get all - open text file, loop over every item
            Models.Notes thenotes = new Models.Notes();

            fileCheck(sfilepath);
            int lineno = 0;
            using (StreamReader sr = new StreamReader(sfilepath))
            {
                String aline = "";
                if (id > 0) 
                {  // skip to specific item
                    while ((aline = sr.ReadLine()) != null)
                    {
                        lineno++;
                        if (lineno < id)
                        {
                            continue;
                        }
                        else
                            break;
                    }
                    if (aline == null)
                        aline = "";
                    if (aline.Length > 0)
                    {
                        Models.NoteModel anote = new Models.NoteModel(lineno, aline);
                        thenotes.notes.Add(anote);
                    }

                }

                else { // pid=0 or invalid or blank
                    // read all
                    while ((aline = sr.ReadLine()) != null)
                    {
                        lineno++;
                        if (aline.Length > 0)
                        {
                            if (sfilter.Length == 0 || (sfilter.Length > 0 && aline.ToLower().Contains(sfilter.ToLower())))
                            {
                                Models.NoteModel anote = new Models.NoteModel(lineno, aline);
                                thenotes.notes.Add(anote);
                            }
                        }
                        if (id > 0) // onlywant 1
                            break;
                    }
                }
                sr.Dispose();
            }
            if (thenotes.notes.Count == 0)
            {
                if (id > 0)
                {
                    Models.NoteModel anote = new Models.NoteModel(id, "Note not found");
                    thenotes.notes.Add(anote);

                }
                else {
                    Models.NoteModel anote = new Models.NoteModel(-1, "Notes are empty");
                    thenotes.notes.Add(anote);
                }
            }
            return thenotes;

        }



        // POST api/values
        /// <summary>
        /// Save to data store. Return New ID
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Models.NoteModel Post([FromBody]dynamic value)
        {
            Models.NoteModel thenote = new Models.NoteModel();

            string bod = (string)value.body;
            if (bod.Trim().Length > 0)
            {
                fileCheck(sfilepath);
                using (StreamWriter sw = File.AppendText(sfilepath))
                {
                    sw.WriteLine(bod );
                    sw.Dispose();
                }
                // now get the count
                int linect = 0;
                using (StreamReader sr = new StreamReader(sfilepath))
                {
                    string aline = "";
                    while ((aline = sr.ReadLine()) != null)
                    {
                        linect++;
                    }
                    sr.Dispose();
                }

                thenote = new Models.NoteModel(linect);
                thenote.body = bod;
            }
            else
            {
                thenote = new Models.NoteModel(-1);
                thenote.body = "Empty or invalid note";
            }
            return thenote;
        }

        // PUT api/values/5
        /// <summary>
        /// Update datastore
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public Models.NoteModel Put([FromBody]dynamic value)
        {
            Models.NoteModel thenote;
            int pid = 0;
            string sid = (string)value.id;
            if (sid.Length > 0)
                int.TryParse(sid, out pid);

            if (pid > 0)
            {
                string newvalue = (string)value.body;
                string[] oldlines = File.ReadAllLines(sfilepath);
                // update the new line
                oldlines[pid - 1] = newvalue;
                File.WriteAllLines(sfilepath, oldlines);
                thenote = new Models.NoteModel(pid);
                thenote.body = newvalue;
            }
            else
            {
                thenote = new Models.NoteModel(-1);
                thenote.body = "Empty or invalid id";
            }
            return thenote;
        }

        // DELETE api/values/5
        /// <summary>
        /// Indicate deleted in datastore
        /// </summary>
        /// <param name="id"></param>
        public Models.NoteModel Delete(int id)
        {
            Models.NoteModel thenote;

            if (id > 0)
            {
                string[] oldlines = File.ReadAllLines(sfilepath);
                // update the new line
                oldlines[id - 1] = "";
                File.WriteAllLines(sfilepath, oldlines);
                thenote = new Models.NoteModel(id);
                thenote.body = "Deleted";
            }
            else
            {
                thenote = new Models.NoteModel(-1);
                thenote.body = "Empty or invalid id";
            }
            return thenote;

        }

        /// <summary>
        /// make sure weather json or query string is passed in, it goes into json format
        /// </summary>
        /// <returns></returns>
        private dynamic xqueryToJson() {            
            string jq = WebUtility.UrlDecode(Request.RequestUri.Query);
            if (jq.StartsWith("?") && jq.Length > 1)
                jq = jq.Substring(1);

            if (jq.StartsWith("{")) // JSON
            {
                dynamic JsonInput = Newtonsoft.Json.JsonConvert.DeserializeObject(jq);
                return JsonInput;
            }
            else
            {
                // parse
                string[] argv = jq.Split('&');
                string pvalue = "";
                foreach (string ctarg in argv)
                {
                    string[] ct = ctarg.Split('=');
                    if (ct[0].ToLower()=="query")
                    {
                        pvalue = ct[1];
                    }
                }
                Models.NoteModel thenote = new Models.NoteModel();
                thenote.body = pvalue;
                dynamic JsonInput = Newtonsoft.Json.JsonConvert.SerializeObject(thenote);
                JsonInput = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonInput);
                return JsonInput;
            }
        }


        /// <summary>
        /// create database if needed
        /// </summary>
        /// <param name="ppath"></param>
        private void fileCheck(string ppath)
        {
            if (!System.IO.File.Exists(ppath))
            {
                System.IO.File.Create(ppath);
            }
        }
    }
}