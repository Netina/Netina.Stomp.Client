using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netina.Stomp.Client.Utils
{
    public enum StompConnectionState
    {
        Open,
        Closed,
        Reconnecting,
    }
}
