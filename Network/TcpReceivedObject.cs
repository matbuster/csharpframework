using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace csharpFramework.Network
{ 
    /** \brief abstract implementation of a received tcp object
     */ 
    public abstract class TcpReceivedObject
    {
        // constant definitions
        protected const int TCP_OK = 0;
        protected const int TCP_KO = 0;
        
        /** 
         * \brief abstract implementation of Parse received function
         * \param [in] associated client socket
         * \param [in] received buffer
         * \param [in] received size
         * \return error code
         */ 
        abstract public int Process(Socket clientSocket);
    }
}
