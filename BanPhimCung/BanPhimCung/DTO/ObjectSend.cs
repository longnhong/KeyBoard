using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanPhimCung.DTO
{
    public class ObjectSend
    {
        public string action { get; set; }
        public string ticket_id { get; set; }
        public string state { get; set; }
        public string cnum { get; set; }
        public string counter_id { get; set; }
        public int mtime { get; set; }
        public string service_id { get; set; }
        public ExtraSend extra { get; set; }
        public ObjectSend(string action, string ticket_id, string state, string service_id,string counterID, string cnum,int mtime, string platform, bool record_transaction)
        {
            this.action = action;
            this.service_id = service_id;
            this.state = state;
            this.cnum = cnum;
            this.mtime = mtime;
            this.ticket_id = ticket_id;
            ExtraSend extra = new ExtraSend(platform, record_transaction);
            this.extra = extra;
            this.counter_id = counterID;
        }
        public ObjectSend() { }

    }
    public class ExtraSend
    {
        public bool record_transaction { get; set; }
        public string platform { get; set; }
        public ExtraSend(string platform, bool record_transaction)
        {
            this.platform = platform;
            this.record_transaction = record_transaction;
        }
    }


    public class ViewInfoHome
    {
        public string TicketID { get; set; }
        
        public string Cnum { get; set; }
        public string HangKH { get; set; }
        public string ServiceName { get; set; }
        
        public int MtimeDelay { get; set; }
        public ViewInfoHome(string TicketID, string Cnum, string ServiceName, int MtimeDelay, string hangKH)
        {
            this.TicketID = TicketID;
            this.Cnum = Cnum;
            this.ServiceName = ServiceName;
            this.MtimeDelay = MtimeDelay;
            this.HangKH = hangKH; 
        }

        public ViewInfoHome() { }
    }
}
