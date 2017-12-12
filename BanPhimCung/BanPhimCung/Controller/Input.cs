using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanPhimCung.Controller
{
    public class Input
    {
        public string Com { get; set; }
        public string BranchCode { get; set; }
        public string CounterCode { get; set; }
        public int AddressKeyBoard { get; set; }
        public string UserId { get; set; }
        public void InputParam()
        {
            Console.WriteLine("Nhap cong Com: ");
            Com = Console.ReadLine();

            Console.WriteLine("Nhap BrandCode: ");
            BranchCode = Console.ReadLine();
        }
    }
}
