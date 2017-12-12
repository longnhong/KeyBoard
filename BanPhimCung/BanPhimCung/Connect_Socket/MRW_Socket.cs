using BanPhimCung.Command;
using BanPhimCung.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;
using BanPhimCung.Controller;
using BanPhimCung.Ultility;
using Newtonsoft.Json;
namespace BanPhimCung.Connect_Socket
{
    public class MRW_Socket
    {

        WriteLog log = new WriteLog();

        private SetDataInSocketToHome setDataFromSocket = null;
        private SetDataInHome setDataInHome = null;
   

        /////////////LIST DATAOBJECT///////////////
        Dictionary<string, Service> dicAllServices = null;
        Dictionary<string, Counter> dicAllCounters = null;

        Dictionary<string, ObjectSend> dicWating = null;
        Dictionary<string, ObjectSend> dicCancelled = null;
        Dictionary<string, ObjectSend> dicServing = null;

        /////////////Web Socket///////////////
        private WebSocket webSocket;

        public delegate void ReceivedEventHandler(EventSocketSendProgram objectData);
        public event ReceivedEventHandler DataReceived;


        public MRW_Socket()
        {
            InitDicList();
            Init_WebSocket("AMAST02");
        }
        private void InitDicList()
        {
            setDataFromSocket = new SetDataInSocketToHome();
            setDataInHome = new SetDataInHome();

            dicWating = new Dictionary<string, ObjectSend>();
            dicCancelled = new Dictionary<string, ObjectSend>();
            dicServing = new Dictionary<string, ObjectSend>();

            dicAllServices = new Dictionary<string, Service>();
            dicAllCounters = new Dictionary<string, Counter>();
        }
        private void Init_WebSocket(string branchCode)
        {
            string url = "ws://mqserver:3000/room/actor/join?branch_code=AMAST02&actor_type=superbox&user_id=usr_zyGMn75ag7EG04KZ2xDE&reconnect_count=0";
            webSocket = new WebSocket(url);
            webSocket.Opened += websocket_Opened;
            webSocket.Error += websocket_Error;
            webSocket.Closed += websocket_Closed;
            webSocket.MessageReceived += websocket_MessageReceived;
        }

        public void OpendSocket()
        {
            webSocket.Open();
            Thread.Sleep(2000);
        }

        public void websocket_Opened(object sender, EventArgs e)
        {
            log.sendLog("Opened Socket");
        }
        public void websocket_Error(object sender, EventArgs e)
        {
            log.sendLog("Error Socket");
        }
        public void websocket_Closed(object sender, EventArgs e)
        {
            log.sendLog("Close Socket");
            Thread.Sleep(3000);
            if (webSocket != null && webSocket.State == WebSocketState.Closed)
            {
                webSocket.Open();
            }
        }
        public void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Received(e.Message.TrimEnd('\0'));
        }
        public void Received(string str)
        {
            var handle = str.Split(' ')[0].Trim('/');
            var data = str.Remove(0, str.IndexOf(' '));

            log.sendLog("handle " + handle);
            ObjectSend objSend = null;

            switch (handle) {
                case ActionTicket.INITIAL:
                    if (dicAllCounters.Count() == 0) {
                        string action1 = ActionTicket.INITIAL;
                        Initial dataUser = JsonConvert.DeserializeObject<Initial>(data);
                        addDicSrcCounter(dataUser.Services, dataUser.Counters);
                        
                        objSend = setDataToObjectSend(dataUser, null, ActionTicket.INITIAL);
                        EventSocketSendProgram even = new EventSocketSendProgram(action1, dicAllServices, dicAllCounters, objSend);
                        
                        Thread.Sleep(2000);
                        DataReceived(even);
                    }
                    break;
                case ActionTicket.TICKET_ACTION:
                    var tkAc = JsonConvert.DeserializeObject<TicketAction>(data);
                    string action = tkAc.Action;
                    if (ActionTicket.ACTION_CREATE.Equals(action) || ActionTicket.ACTION_CANCEL.Equals(action)) {
                        InitialCreate dataz = JsonConvert.DeserializeObject<InitialCreate>(data);
                        Initial tickets = new Initial();
                        tickets.Tickets.Add(dataz.ticket.Id, dataz.ticket);
                        objSend = setDataToObjectSend(tickets, null, action);
                    } else {
                        objSend = setDataToObjectSend(null, tkAc, tkAc.Action);
                    }

                    if (!ActionTicket.ACTION_CREATE.Equals(action)) {
                        EventSocketSendProgram even1 = new EventSocketSendProgram(action, null, null, objSend);
                        DataReceived(even1);
                    }
                    break;
                default:
                    break;
            }
        }

        private ObjectSend setDataToObjectSend(Initial initial, TicketAction ticketAction, string action)
        {
            ObjectSend objSend = null;
            if (initial != null)
            {
                foreach (var ticketDic in initial.Tickets)
                {
                    var ticket = ticketDic.Value;
                    string state = ticket.State;
                    objSend = setDataFromSocket.SetDataFromTicketInitial(ticket, ActionTicket.INITIAL, dicAllServices);

                    if (ActionTicket.ACTION_CREATE.Equals(action) || ActionTicket.ACTION_CANCEL.Equals(action))
                    {
                        subList(action, objSend, objSend.ticket_id);
                    }
                    else
                    {
                        addList(state, objSend);
                    }

                }
            }
            else
            {
                var ticketID = ticketAction.Ticket.Id;
                if (action.Equals(ActionTicket.ACTION_CREATE) || action.Equals(ActionTicket.ACTION_CANCEL)) {
                    objSend = setDataFromSocket.SetDataFromTicketAction(ticketAction, ticketAction.Action, dicAllServices);
                }
                objSend = GetObjSendByAction(action,ticketID);
                subList(action, objSend, ticketID);
            }
            sortDic();
            return objSend;
        }

        private ObjectSend GetObjSendByAction(string action, string ticketID){
            ObjectSend objSend = null;
            if( ActionTicket.ACTION_CALL.Equals(action)){
                objSend = dicWating[ticketID];
            }
            else if (!ActionTicket.ACTION_RESTORE.Equals(action)) {
                objSend = dicServing[ticketID];
            }
            return objSend;
        }
        private void addDicSrcCounter(List<Service> lstServices, List<Counter> lstCounters)
        {
            dicAllServices = setDataInHome.setDicServices(lstServices, dicAllServices);
            dicAllCounters = setDataInHome.setDicCounters(lstCounters, dicAllCounters);
        }

        private void addList(string state, ObjectSend objSend)
        {
            switch (state)
            {
                case ActionTicket.STATE_WATING:
                    addDataDicAndList(objSend, dicWating);
                    break;
                case ActionTicket.STATE_CANCELLED:
                    addDataDicAndList(objSend, dicCancelled);
                    break;
                case ActionTicket.STATE_SERVING:
                    addDataServing(objSend, dicServing);
                    break;
                default:
                    break;
            }
        }
        private void addDataDicAndList(ObjectSend objSend, Dictionary<string, ObjectSend> dic) {
            var key = objSend.ticket_id;
            if (dic.ContainsKey(key)) {
                dic.Remove(key);
            }
            dic.Add(key, objSend);
        }
        private void addDataServing(ObjectSend objSend, Dictionary<string, ObjectSend> dicServing) {
            if (!dicServing.ContainsKey(objSend.ticket_id)) {
                dicServing.Add(objSend.ticket_id, objSend);
            }
            

        }

        private void sortDic() {
            dicWating = setDataInHome.sortDic(dicWating);
            dicCancelled = setDataInHome.sortDic(dicCancelled);
        }

        private void subList(string action, ObjectSend objSend, string ticketID) {
            ObjectSend objRes = null;
            switch (action) {
                case ActionTicket.ACTION_CREATE:
                    addList(objSend.state, objSend);
                    break;
                case ActionTicket.ACTION_CALL:
                    objRes = setDataInHome.GetDataFromDic(ticketID,dicWating);
                    if (objRes != null) {
                        objRes.state = ActionTicket.STATE_SERVING;
                        dicServing.Add(ticketID, objRes);
                        dicWating.Remove(ticketID);
                    }
                    break;
                case ActionTicket.ACTION_RECALL:
                    break;
                case ActionTicket.ACTION_CANCEL:
                    objRes = setDataInHome.GetDataFromDic(ticketID, dicServing);
                    if (objRes != null) {
                        dicServing.Remove(objRes.ticket_id);
                        objRes.state = ActionTicket.STATE_CANCELLED;
                        dicCancelled.Add(objRes.ticket_id, objRes);
                    }
                    break;
                case ActionTicket.ACTION_FINISH:
                   objRes = setDataInHome.GetDataFromDic(ticketID, dicServing);
                   if (objRes != null) {
                       dicServing.Remove(ticketID);
                   }
                    break;
                case ActionTicket.ACTION_MOVE:
                    break;
                case ActionTicket.ACTION_RESTORE:
                    objRes = setDataInHome.GetDataFromDic(ticketID, dicCancelled);
                    if (objRes != null)
                    {
                        dicCancelled.Remove(ticketID);
                    }
                    objRes.state = ActionTicket.STATE_WATING;
                    dicWating.Add(ticketID,objRes);
                    break;
                default:
                    break;
            }
        }


        #region CONVERT OBJECT TO JSON
        public ObjectSend SendFromAction(string action, string idCounter)
        {
            var objSend = getObject(action, idCounter);
            string json = ConvertObjectToJson(objSend);
            if (!json.Equals(""))
            {
                string data = ActionTicket.TICKET_ONCE + GenRandomString() + " " + json;
                webSocket.Send(data);
                return objSend;
            }
            return null;

        }
        private string GenRandomString()
        {
            return MRW_Convert.RandomString(ActionTicket.LENGH_RANDOM);
        }

        private ObjectSend getObject(string action, string counterID) {
            ObjectSend objSend = null;
            switch (action) {
                case ActionTicket.ACTION_CREATE:
                    break;
                case ActionTicket.ACTION_CALL:
                    objSend = getObjSendToDic(counterID, dicAllCounters, dicWating);
                    break;
                case ActionTicket.ACTION_RECALL:
                    objSend = getObjSendToDic(counterID, dicAllCounters, dicServing);
                    break;
                case ActionTicket.ACTION_CANCEL:
                    objSend = getObjSendToDic(counterID, dicAllCounters, dicServing);
                    break;
                case ActionTicket.ACTION_MOVE:
                    break;
                case ActionTicket.ACTION_FINISH:
                    objSend = getObjSendToDic(counterID, dicAllCounters, dicServing);
                    break;
                case ActionTicket.ACTION_RESTORE:
                    objSend = getObjSendToDic(counterID, dicAllCounters, dicCancelled);
                    break;
            }
            if (objSend != null) {
                objSend.action = action;
            }
            return objSend;
        }

        private ObjectSend getObjSendToDic(string counterID, Dictionary<string, Counter> dicCounters, Dictionary<string, ObjectSend> dicObject) {
            ObjectSend objSend = null;
            if (dicObject != null && dicObject.Count() > 0) {
                List<string> lstService = null;
                if (dicCounters.ContainsKey(counterID)) {
                    lstService = dicCounters[counterID].Services.ToList();
                }
                var lst = from user in dicObject.Values where lstService.Contains(user.service_id) select user;
                if (lst != null && lst.Count() > 0) {
                    objSend = lst.OrderBy(m => m.mtime).First();
                    if (objSend != null) {
                        objSend.counter_id = counterID;
                    }
                }
            }
            return objSend;
        }


        private string ConvertObjectToJson(ObjectSend objSend) {
            return (objSend == null) ? "" : MRW_Convert.ConvertObjectToJson(objSend);
        }
        #endregion
    }
}
