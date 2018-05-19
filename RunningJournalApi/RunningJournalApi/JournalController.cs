using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RunningJournalApi
{
    public class JournalController : ApiController
    {
        public static readonly IList<JournalEntryModel> Entries = new List<JournalEntryModel>();

        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new JournalModel
            {
                Entries = Entries.ToArray()
            });
        }

        public HttpResponseMessage Post(JournalEntryModel journalEntry)
        {
            Entries.Add(journalEntry);
            return Request.CreateResponse();
        }
    }

    public class JournalEntryModel
    {
        public DateTimeOffset Time { get; set; }

        public int Distance { get; set; }

        public TimeSpan Duration { get; set; }
    }

    public class JournalModel
    {
        public JournalEntryModel[] Entries { get; set; }
    }
}