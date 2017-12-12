using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BanPhimCung.DTO;

namespace BanPhimCung.Controller
{
    public class CheckByteSend
    {
        public enum BYTE_COMMAND : int
        {
            START_COMMAND = (int)0x3A,
            START_OF_COUNTER_COMMAND = (int)0x30,
            NEXT_COMMAND = START_OF_COUNTER_COMMAND + (int)0x00,
            RECALL_COMMAND = START_OF_COUNTER_COMMAND + (int)0x01,
            DELETE_COMMAND = START_OF_COUNTER_COMMAND + (int)0x02,
            FINISH_COMMAND = START_OF_COUNTER_COMMAND + (int)0x03,
            FORWARD_COMMAND_SERVICE = START_OF_COUNTER_COMMAND + (int)0x04,
            FORWARD_COMMAND_COUNTER = START_OF_COUNTER_COMMAND + (int)0x07,
            CALLSTORE_COMMAND = START_OF_COUNTER_COMMAND + (int)0x05,
            RESTORE_COMMAND = START_OF_COUNTER_COMMAND + (int)0x06,
            END_COMMAND = (int)0x1310
        }
        public int NUM_OF_COUNTER_COMMAND = 8;
        public static ResponeByte getByteSend(byte[] resByte)
        {
            ResponeByte resByteClass = null;
            var count = resByte.Length;
            if (count > 10)
            {
                resByteClass = new ResponeByte(); 
                var dataLength = resByte[5] + resByte[6] * 256;
                var end = resByte[count - 2] + resByte[count - 1] * 256;
                int startNum = (int)BYTE_COMMAND.START_COMMAND;
                int endNum = (int)BYTE_COMMAND.END_COMMAND;
                if (startNum == resByte[0] && (count - 10) - 1 == dataLength && endNum == end)
                {
                    resByteClass.DeviceId = (int)resByte[1];
                    resByteClass.AddressKey = (int)resByte[2];
                    resByteClass.Command = (int)(resByte[3] + resByte[4] * 256);
                }
            }
            return resByteClass;
        }
    }
}
