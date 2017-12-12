using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanPhimCung.Ultility
{
    public class ActionTicket
    {
        public const string TICKET_ONCE = "/ticketsuper?once=";
        public const string INITIAL = "initial";
        public const string TICKET_ACTION = "ticket_action";

        public const string ACTION_CALL = "call";
        public const string ACTION_RECALL = "recall";
        public const string ACTION_MOVE = "move";
        public const string ACTION_CANCEL = "cancel";
        public const string ACTION_FINISH = "finish";
        public const string ACTION_CREATE = "create";
        public const string ACTION_RESTORE = "restore";
        public const string ACTION_ALL_RESTORE = "all_restore";
        public const string ACTION_ALL_SERVICE = "all_service";
        public const string ACTION_ALL_COUNTER = "all_counter";

        public const string STATE_WATING = "waiting";
        public const string STATE_CANCELLED = "cancelled";
        public const string STATE_SERVING = "serving";

        public const int LENGH_RANDOM = 5;
        public const string PLATFORM = "keyboard";
        public const int DEVICE_ID = 2;

    }
}
