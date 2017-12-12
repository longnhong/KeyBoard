using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BanPhimCung.Command;
using BanPhimCung.Connect_Socket;
using BanPhimCung.Controller;
using BanPhimCung.DTO;
using BanPhimCung.Ultility;
using System.IO.Ports;

namespace BanPhimCung
{
    class Program
    {
        static string comName = "";
        static MRW_ModBus modBus = null;
        static WriteLog log = null;
        //static Input input = null;
        static MRW_SerialPort serialPort = null;

        static MRW_Socket socket = null;
        static SetServiceCounter setSrcVcs = null;

        static string branchCode = "";
        static Dictionary<string, Service> dicService = null;
        static Dictionary<string, Counter> dicCounter = null;
        static Dictionary<int, CounterAndKeyboard> dicCounterKeyboard = null;
        static ResponeByte resByte = null;


        #region Init
        private static void Init()
        {
            InitClass();
            InitSerialPort(comName); // mở port nhận và truyền data phím cứng
            InitSocket(branchCode);
        }
        private static void InitClass()
        {
            modBus = new MRW_ModBus();
            log = new WriteLog();
            setSrcVcs = new SetServiceCounter();
            dicCounter = new Dictionary<string, Counter>();
            dicService = new Dictionary<string, Service>();
            dicCounterKeyboard = new Dictionary<int, CounterAndKeyboard>();
        }
        private static void InitSerialPort(string comName)
        {
            serialPort = new MRW_SerialPort(new System.IO.Ports.SerialPort(comName.ToUpper(), 19200));
            serialPort.openPort();
            serialPort.DataReceived += serialPortDataReceived;
        }
        private static void InitSocket(string branchCode1)
        {
            branchCode = branchCode1;
            socket = new MRW_Socket();
            socket.OpendSocket();
            //socket.MrwOpenSocket += InitKeyBoardSend;
            socket.DataReceived += ReciveDataSocket;
            // Khi data gửi từ socket 
        }
        #endregion

        private static int InputAddressDevice(List<Counter> lstCounter)
        {
            Console.WriteLine("===============================");
            Console.WriteLine("| ID | Ten counter ");
            foreach (var item in lstCounter.OrderBy(m => m.CNum))
            {
                Console.WriteLine("|" + item.CNum + "  |  " + item.Code + "");
            }
            Console.WriteLine("===============================");
            Console.Write("Nhap so thiet bi: ");
            int numDevice = 0;
            string value = Console.ReadLine();
            int.TryParse(value, out numDevice);
            return numDevice;
        }

        private static void InitKeyBoardSend(Dictionary<string, Counter> lstCounter, Dictionary<string, Service> lstServices)
        {
            dicCounter = lstCounter;
            dicService = lstServices;

            List<Counter> lstCounters = lstCounter.Values.ToList();
            //var numDevice = InputAddressDevice(lstCounters);
            int address = 249;
            int numCounter = 1;
            string counterID = "";
            do
            {
                counterID = dicCounter.Values.FirstOrDefault(m => m.CNum == numCounter).Id;
                if (!counterID.Equals(""))
                {
                    dicCounterKeyboard.Add(address, new CounterAndKeyboard(counterID, address));
                    foreach (var counter in lstCounters)
                    {
                        serialPort.SendData(setSrcVcs.SetService(address, dicService.Select(m => m.Value.code).ToList()));
                        serialPort.SendData(setSrcVcs.SetCounter(address, counter.Id, lstCounters.Select(m => m.Name).ToList()));
                    }
                }
            } while (counterID.Equals(""));

            //for (int i = 0; i < numDevice; i++)
            //{
            //    Console.WriteLine("Thiet bi so: " + i + 1);


            //    bool isCheck = false;
            //    int address = -1;
            //    int numCounter = -1;
            //    do
            //    {
            //        Console.Write("Nhap dia chi: ");
            //        isCheck = int.TryParse(Console.ReadLine(), out address);
            //    }while(!isCheck);
            //    isCheck = false;
            //    do
            //    {
            //        Console.Write("Chon counter so: ");
            //        isCheck = int.TryParse(Console.ReadLine(), out numCounter);
            //    } while (!isCheck);

            //    string counterID = "";
            //    do
            //    {
            //        counterID = dicCounter.Values.FirstOrDefault(m => m.CNum == numCounter).Id;
            //        if (!counterID.Equals(""))
            //        {
            //            dicCounterKeyboard.Add(address, new CounterAndKeyboard(counterID, address));
            //            foreach (var counter in lstCounters)
            //            {
            //                serialPort.SendData(setSrcVcs.SetService(address, dicService.Select(m => m.Value.code).OrderBy(s=>s).ToList()));
            //                serialPort.SendData(setSrcVcs.SetCounter(address, counter.Id, lstCounters.Select(m => m.Name).OrderBy(c => c).ToList()));
            //            }
            //        }
            //    } while (counterID.Equals(""));

            //}
            log.sendLog("Set services and counter success!");

        }
        private static int getIndexService(string serviceID, string counterID)
        {
            if (counterID != null)
            {
                if (dicCounter.ContainsKey(counterID) && dicService.ContainsKey(serviceID))
                {
                    //var services = dicCounter[counterID].Services;
                    var index = Array.FindIndex(dicService.Values.ToArray(), m => m.Id.Contains(serviceID));
                    return index;
                }
            }
            return -1;
        }
        private static void InitDic(Dictionary<string, Service> dicSer, Dictionary<string, Counter> dicCount)
        {
            dicService = dicSer;
            dicCounter = dicCount;
        }
        private static void ReciveDataSocket(EventSocketSendProgram eventData)
        {
            int command = 0;
            switch (eventData.Action)
            {
                case ActionTicket.INITIAL:
                    if (dicCounter.Count() == 0 && dicService.Count() == 0)
                    {
                        InitKeyBoardSend(eventData.DicCounter, eventData.DicService);
                    }
                    return;
                case ActionTicket.ACTION_CREATE:// không làm gì
                    break;
                case ActionTicket.ACTION_CALL:
                    command = (int)CheckByteSend.BYTE_COMMAND.NEXT_COMMAND;
                    break;
                case ActionTicket.ACTION_RECALL:
                    command = (int)CheckByteSend.BYTE_COMMAND.RECALL_COMMAND;
                    break;
                case ActionTicket.ACTION_RESTORE:
                    command = (int)CheckByteSend.BYTE_COMMAND.RESTORE_COMMAND;
                    break;
                case ActionTicket.ACTION_MOVE:
                    command = (int)CheckByteSend.BYTE_COMMAND.FORWARD_COMMAND_COUNTER;
                    command = (int)CheckByteSend.BYTE_COMMAND.FORWARD_COMMAND_SERVICE;
                    break;
                case ActionTicket.ACTION_CANCEL:
                    command = (int)CheckByteSend.BYTE_COMMAND.DELETE_COMMAND;
                    break;
                case ActionTicket.ACTION_FINISH:
                    command = (int)CheckByteSend.BYTE_COMMAND.FINISH_COMMAND;
                    break;
                default:
                    //bug
                    break;
            }

            if (eventData.ObjSend != null)
            {
                string counterID = eventData.ObjSend.counter_id;
                int indexService = getIndexService(eventData.ObjSend.service_id, counterID);
                if (indexService != -1)
                {
                    string data = eventData.ObjSend.cnum;
                    var address = dicCounterKeyboard.FirstOrDefault(d => d.Value.CounterID == counterID).Key;
                    if (address > 0)  {
                        var byteRes = modBus.BuildText(ActionTicket.DEVICE_ID, address, command, data, indexService);
                        serialPort.SendData(byteRes);
                    }
                    else  {
                        //bug
                    }
                    
                }  else  {
                    //truyền lỗi xuống port
                }
            }
            resByte = null;
        }

        private static void serialPortDataReceived(byte[] data)
        {
            // nhận data từ bàn phím xử lý gọi lên socket
            resByte = CheckByteSend.getByteSend(data);
            if (resByte != null)
            {
                string idCounter = "";
                if (dicCounterKeyboard.ContainsKey(resByte.AddressKey))
                {
                    idCounter = dicCounterKeyboard[resByte.AddressKey].CounterID;

                    string action = "";
                    switch (resByte.Command)
                    {
                        case (int)CheckByteSend.BYTE_COMMAND.NEXT_COMMAND:
                            action = ActionTicket.ACTION_CALL; break;
                        case (int)CheckByteSend.BYTE_COMMAND.RECALL_COMMAND:
                            action = ActionTicket.ACTION_RECALL; break;
                        case (int)CheckByteSend.BYTE_COMMAND.DELETE_COMMAND:
                            action = ActionTicket.ACTION_CANCEL; break;
                        case (int)CheckByteSend.BYTE_COMMAND.FINISH_COMMAND:
                            action = ActionTicket.ACTION_FINISH; break;
                        case (int)CheckByteSend.BYTE_COMMAND.FORWARD_COMMAND_SERVICE:
                            action = action = ActionTicket.ACTION_MOVE;
                            break;
                        case (int)CheckByteSend.BYTE_COMMAND.FORWARD_COMMAND_COUNTER:
                            action = action = ActionTicket.ACTION_MOVE;
                            break;
                        case (int)CheckByteSend.BYTE_COMMAND.CALLSTORE_COMMAND:
                            action = ActionTicket.ACTION_MOVE; break;
                        case (int)CheckByteSend.BYTE_COMMAND.RESTORE_COMMAND:
                            action = ActionTicket.ACTION_ALL_RESTORE;
                            break;
                        default:
                            //ERRor
                            break;
                    }
                    if (!"".Equals(action) && !"".Equals(idCounter))
                    {
                        socket.SendFromAction(action, idCounter);
                    }
                    else
                    {
                        //serialPort.SendData();//error
                    }
                }
                else
                {
                    Console.WriteLine("Khong ton tai dia chi nay!");
                }

            }
        }
        static void Main(string[] args)
        {
            //InputParam();

            comName = GetCom(comName);

            if (comName.Length > 0)
            {
                Init();
                try
                {
                    serialPort.openPort();

                    while (true)
                    {
                    }
                }
                catch (Exception ex)
                {
                    log.sendLog("exception " + ex);
                }
            }
            return;
        }
        private static string GetCom(string comName)
        {
            if (comName.Equals(""))
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Count() == 1)
                {
                    comName = ports[0];
                }
                else
                {
                    log.sendLog("missing com port");
                }
            }
            return comName.ToUpper();
        }

        static void InputParam()
        {
            Console.WriteLine("Nhap cong Com: ");
            comName = Console.ReadLine();

            Console.WriteLine("Nhap BrandCode: ");
            branchCode = Console.ReadLine();

        }
    }
    public class KeyAddressDevice : EventArgs
    {
        public delegate void ReceivedEventHandler(KeyAddressDevice addressDevice);
        public event ReceivedEventHandler DataReceivedFromKeyboard;

        private int deviceId;
        private int addressDevice;
        private string idCounter;
        private int commandFromKeyboard;

        public int CommandFromKeyboard
        {
            get { return commandFromKeyboard; }
            set { commandFromKeyboard = value; }
        }

        public string IdCounter
        {
            get { return idCounter; }
            set { idCounter = value; }
        }
        public int DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }
        public int AddressDevice
        {
            get { return addressDevice; }
            set { addressDevice = value; }
        }

        public KeyAddressDevice(int deviceID, int address, string counterID, int commandFromKeyboard)
        {
            this.DeviceId = deviceId;
            this.AddressDevice = address;
            this.IdCounter = counterID;
            this.CommandFromKeyboard = commandFromKeyboard;
        }
        public KeyAddressDevice() { }
    }
}
