using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client20710711
{
    class IMessage
    {

        private MessageType type;
        public string Message { get; set; }
        public IMessage(MessageType type, string Message)
        {
            this.type = type;
            this.Message=Message;
        }
    }
}
