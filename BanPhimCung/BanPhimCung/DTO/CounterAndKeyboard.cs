using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanPhimCung.DTO
{
    public class CounterAndKeyboard
    {
         private string counterID;
         private int addressKeyboard;
        public string CounterID
        {
            get { return counterID; }
            set { counterID = value; }
        }
        

        public int AddressKeyboard
        {
            get { return addressKeyboard; }
            set { addressKeyboard = value; }
        }
        public CounterAndKeyboard(string counterId, int addressID)
        {
            this.CounterID = counterId;
            this.AddressKeyboard = addressID;
        }
    }
}
