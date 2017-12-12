using BanPhimCung.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BanPhimCung.Ultility;


namespace BanPhimCung.Controller
{
    public class SetDataInHome
    {
        public Dictionary<string, ObjectSend> AddDataDicSendToDicRecive(Dictionary<string, ObjectSend> lstSend, Dictionary<string, ObjectSend> lstRecive)
        {
            if (lstSend.Count() > 0 && lstRecive != null)
            {
                foreach (var objSend in lstSend)
                {
                    lstRecive.Add(objSend.Key, objSend.Value);
                }

            }
            else if (lstSend.Count() > 0)
            {
                if (lstRecive == null)
                {
                    lstRecive = new Dictionary<string, ObjectSend>();
                }
                lstRecive = lstSend;
            }
            return lstRecive;
        }

        public ObjectSend GetDataFromDic(string ticketID, Dictionary<string, ObjectSend> dicSend)
        {
            if (ticketID != null && dicSend.ContainsKey(ticketID))
            {
                return dicSend[ticketID];
            }
            return null;
        }
        public Dictionary<string, Service> setDicServices(List<Service> lstService, Dictionary<string, Service> dic)
        {

            if (lstService != null)
            {
                foreach (var ser in lstService)
                {
                    if (!dic.ContainsKey(ser.Id))
                    {

                        dic[ser.Id] = ser;
                    }
                }
            }
            return dic;
        }

        public Dictionary<string, Counter> setDicCounters(List<Counter> lstCounter, Dictionary<string, Counter> dic)
        {

            if (lstCounter != null)
            {
                foreach (var ser in lstCounter)
                {
                    if (!dic.ContainsKey(ser.Id))
                    {
                        dic[ser.Id] = ser;
                    }
                }
            }
            return dic;
        }

        public Dictionary<string, ObjectSend> sortDic(IDictionary<string, ObjectSend> dic)
        {
            var list = dic.ToList();
            list.Sort((pair1, pair2) => pair1.Value.mtime.CompareTo(pair2.Value.mtime));
            return list.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }


}
