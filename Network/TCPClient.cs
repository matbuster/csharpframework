using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace csharpFramework.Network
{
    /** \brief tcp client implementation
     */ 
	class TcpClient
	{
        private String data;

        /** \brief constructor implementation
         * \param [in]
         */ 
		public TcpClient(String data)
		{
            this.data = data;
			Thread t = new Thread(new ThreadStart(ClientThreadStart));
			t.Start();
		}

        /** \brief thread start to send data trough tcp connection */
		private void ClientThreadStart()
		{
            TcpClientObject client = new TcpClientObject();
            client.Send(this.data);
            client.Close();
		}
	}
}
