using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanPhimCung.DTO
{
    public class Initial
    {
        public Initial()
        {
            Counters = new List<Counter>();
            Stats = new List<Stat>();
            Tickets = new Dictionary<string, Ticket>();
        }
        public List<Counter> Counters { get; set; }
        //public Counter CurrentCounter { get; set; }
        //public int Now { get; set; }
        public List<Stat> Stats { get; set; }
        //public List<Ticket> Tickets { get; set; }
        public Dictionary<string, Ticket> Tickets { get; set; }
        //public User User { get; set; }

        public List<Service> Services { get; set; }
    }

    public class Service
    {
        public string code { get; set; }
        public string Id { get; set; }
        public Lang l10n { get; set; }
    }
    public class Lang
    {
        public string En { get; set; }
        public string Es { get; set; }
        public string Sp { get; set; }
        public string Vi { get; set; }
    }
    public class Counter
    {
        public string Id { get; set; }
        public int MTime { get; set; }
        public int DTime { get; set; }
        public string Code { get; set; }
        public string BranchId { get; set; }
        public string Name { get; set; }
        public int CNum { get; set; }
        public int DevAddress { get; set; }
        public string[] Services { get; set; }
    }

    public class Stat
    {
        public string UserId { get; set; }
        public string ServiceId { get; set; }
        public string State { get; set; }
        public int Count { get; set; }
        public int STime { get; set; }
    }

    public class Ticket
    {
        public string Id { get; set; }
        [DisplayName("Time Đợi")]
        public int MTime { get; set; }
        public int DTime { get; set; }
        public int CTime { get; set; }
        public string TransactionId { get; set; }
        public string BranchId { get; set; }
        public string KioskId { get; set; }
        public string Service_Id { get; set; }
        public string Counter_Id { get; set; }
        public string UserId { get; set; }
        public Customer Customer { get; set; }
        public string[] Services { get; set; }
        [DisplayName("STT")]
        public string CNum { get; set; }
        public string Lang { get; set; }
        public int CCount { get; set; }
        public bool Online { get; set; }
        public int NWaiting { get; set; }
        Track[] Tracks { get; set; }
        public string State { get; set; }
    }

    public class Track
    {
        public string State { get; set; }
        public int MTime { get; set; }
        public string[] Services { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public int MTime { get; set; }
        public int DTime { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string BranchId { get; set; }
        public string Origin { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }

    }

    public class Customer
    {
        public string Id { get; set; }
        public int Mtime { get; set; }
        public int Dtime { get; set; }
        public string Code { get; set; }
    }

    public class InitialCreate
    {
        public Ticket ticket { get; set; }
    }
}

