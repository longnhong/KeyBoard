using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BanPhimCung.Command
{
    public class MRW_SerialPort
    {
        WriteLog log = new WriteLog();
        private string comName = "";


        public enum Status
        {
            Opened,
            Closed,
            Opening,
            Closing,
            CloseError,
            OpenError,
            SendError,
            ReceivedEror
        }

        private SerialPort serialPort = null;

        public delegate void StatusEventHandler(SerialPort serialPort, Status status);

        public delegate void ReceivedEventHandler(byte[] data);

        public event StatusEventHandler StatusChanged;

        public event ReceivedEventHandler DataReceived;

        private bool isRead = false;

        public MRW_SerialPort(SerialPort serialPort)
        {
            Init(serialPort);
        }

        public MRW_SerialPort(string Port)
        {
            Init(new SerialPort(Port));
        }

        public void Init(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            serialPort.DataReceived += serialPortDataReceived;
        }

        public void serialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);
            DataReceived(ReadByteInPort());
        }

        private byte[] ReadByteInPort()
        {
            //Thread.Sleep(1000);

            byte[] r = new byte[serialPort.BytesToRead];
            try
            {
                serialPort.Read(r, 0, serialPort.BytesToRead);
            }
            catch (Exception ex)
            {
                log.sendLog("Exception read byte in port: " + ex.ToString());
            }
            finally
            {
                if (serialPort.BytesToRead != 0) serialPort.DiscardInBuffer();
            }

            return r;
        }
        public string[] PortName { get { return SerialPort.GetPortNames(); } }
        public bool openPort()
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    log.sendLog("opening Port " + comName);
                    serialPort.Open();
                    log.sendLog("opened Port" + comName);
                    return true;
                }
                catch (Exception e)
                {
                    log.sendLog("Open port: " + e);
                    return false;
                }
            }
            return true;
        }

        public void SendData(byte[] data)
        {
            if (openPort())
            {
                try
                {
                    serialPort.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
