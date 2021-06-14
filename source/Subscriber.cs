using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNet.Stomp.Client
{
    public class Subscriber
    {
        public EventHandler<object> Handler { get; }
        public Type BodyType { get; }

        public Subscriber(EventHandler<object> handler, Type bodyType)
        {
            Handler = handler;
            BodyType = bodyType;
        }
    }
}
