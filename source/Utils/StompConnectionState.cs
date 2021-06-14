using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNet.Stomp.Client.Utils
{
    public enum StompConnectionState
    {
        Open,
        Closed,
        Reconnecting,
    }
}
