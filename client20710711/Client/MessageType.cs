using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client20710711
{
    public enum MessageType : byte
    {
        disConnect = 0,
        msg = 1,
        mapFile = 2,
        AgvFile = 3,
        reStart = 4,
        arrived = 5,
        none=6,
        move=7
    }
}
