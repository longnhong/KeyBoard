using BanPhimCung.DTO;
using BanPhimCung.Ultility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanPhimCung.Controller
{
    public class SetDataInSocketToHome
    {
        private string platform = ActionTicket.PLATFORM;
        public ObjectSend SetDataFromTicketInitial(Ticket ticket, string action, Dictionary<string, Service> dicServices)
        {
            var idService = ticket.Services[0];
            var counterID = ticket.Counter_Id;
            if (counterID == null || counterID.Equals(""))
            {
                counterID = null;
            }
            return new ObjectSend(action, ticket.Id, ticket.State, idService, counterID, ticket.CNum, ticket.MTime, platform, true);
        }

        public ObjectSend SetDataFromTicketAction(TicketAction ticketAction, string action, Dictionary<string, Service> dicServices)
        {
            var idService = ticketAction.Extra.Customer.service_id;
            var counterID = ticketAction.Ticket.Counter_Id;
            if (counterID == null || counterID.Equals(""))
            {
                counterID = null;
            }
            //if (dicServices.ContainsKey(idService))
            //{
            //    string lang = dicServices[idService].l10n.Vi;
            //    if (lang == null)
            //    {
            //        lang = dicServices[idService].l10n.En;
            //    }
            //    idService = lang;
            //}
            return new ObjectSend(action, ticketAction.Extra.Customer.id, ticketAction.Extra.Customer.state, idService, counterID, ticketAction.Extra.Customer.cnum, ticketAction.Extra.Customer.mtime, platform, true);
        }

        public Dictionary<string, Dictionary<string, ObjectSend>> SetDataToObjectSend(Initial initial, TicketAction ticketAction, Dictionary<string, Service> dicServices)
        {
            string action = "";
            Dictionary<string, Dictionary<string, ObjectSend>> dicObjSend = new Dictionary<string, Dictionary<string, ObjectSend>>();
            Dictionary<string, ObjectSend> lstObjectWaiting = new Dictionary<string, ObjectSend>();
            Dictionary<string, ObjectSend> lstObjectCanceled = new Dictionary<string, ObjectSend>();
            Dictionary<string, ObjectSend> lstObjectServing = new Dictionary<string, ObjectSend>();

            SetDataInSocketToHome setData = new SetDataInSocketToHome();

            if (initial != null)
            {
                action = ActionTicket.INITIAL;
                foreach (var ticketDic in initial.Tickets)
                {
                    var ticket = ticketDic.Value;
                    var state = ticket.State;
                    ObjectSend objSend = setData.SetDataFromTicketInitial(ticket, action, dicServices);
                    AddList(state, lstObjectWaiting, lstObjectCanceled, lstObjectServing, objSend);
                }
            }
            else
            {
                action = ticketAction.Action;
                var state = ticketAction.Extra.Customer.state;
                ObjectSend objSend = setData.SetDataFromTicketAction(ticketAction, action, dicServices);
                AddList(state, lstObjectWaiting, lstObjectCanceled, lstObjectServing, objSend);
            }
            dicObjSend.Add(ActionTicket.STATE_WATING, lstObjectWaiting);
            dicObjSend.Add(ActionTicket.STATE_CANCELLED, lstObjectCanceled);
            dicObjSend.Add(ActionTicket.STATE_SERVING, lstObjectServing);
            return dicObjSend;
        }
        public void AddList(string state, Dictionary<string, ObjectSend> lstObjectWaiting, Dictionary<string, ObjectSend> lstObjectCanceled,
            Dictionary<string, ObjectSend> lstObjectServing, ObjectSend objSend)
        {
            switch (state)
            {
                case ActionTicket.STATE_WATING:
                    lstObjectWaiting.Add(objSend.ticket_id, objSend);
                    break;
                case ActionTicket.STATE_CANCELLED:
                    lstObjectCanceled.Add(objSend.ticket_id, objSend);
                    break;
                case ActionTicket.STATE_SERVING:
                    lstObjectServing.Add(objSend.ticket_id, objSend);
                    break;
                default:
                    break;
            }
        }

    }
}
