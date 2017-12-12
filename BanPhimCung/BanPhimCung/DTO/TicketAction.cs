using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanPhimCung.DTO
{
    public class TicketAction
    {
        public string Action { get; set; }
        public Ticket Ticket { get; set; }
        public Extra Extra { get; set; }
    }
}
public class Extra
{
    public Customer Customer { get; set; }
    public string Kiosk_id { get; set; }
    public string Lang { get; set; }
    //public string Priority { get; set; }
}
public class Customer
{
    public string code { get; set; }
    public string service_id { get; set; }
    public string id { get; set; }
    public int mtime { get; set; }
    public string state { get; set; }
    public string branch_id { get; set; }
    public string cnum { get; set; }

}
