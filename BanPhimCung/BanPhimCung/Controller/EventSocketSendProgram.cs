using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BanPhimCung.DTO;

namespace BanPhimCung.Controller
{
    public class EventSocketSendProgram : EventArgs
    {
        private string action;

        private Dictionary<string, Service> dicService ;

        private Dictionary<string, Counter> dicCounter ;

        private ObjectSend objSend ;

        public EventSocketSendProgram(string action, Dictionary<string, Service> lstService, Dictionary<string, Counter> lstCounter, ObjectSend objSend)
        {
            this.DicCounter = lstCounter;
            this.DicService = lstService;
            this.ObjSend = objSend;
            this.Action = action;
        }

        public Dictionary<string, Counter> DicCounter
        {
            get { return dicCounter; }
            set { dicCounter = value; }
        }
        public Dictionary<string, Service> DicService
        {
            get { return dicService; }
            set { dicService = value; }
        }

        public ObjectSend ObjSend
        {
            get { return objSend; }
            set { objSend = value; }
        }

        public string Action
        {
            get { return action; }
            set { action = value; }
        }
    }
}
