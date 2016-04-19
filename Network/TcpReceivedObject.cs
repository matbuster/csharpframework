using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csharpFramework.Network
{
    /** \brief abstract implementation of a received tcp object
     */ 
    public abstract class TcpReceivedObject
    {
        abstract public int ParseReceiveBuffer(Byte[] byteBuffer, int size);
    }
}
