using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace csharpFramework.Network
{
    public class TcpClientObject
    {
        // constant definition
        private const int TCP_OK = 0;
        private const int TCP_KO = 1;
        private const int TCP_DATA_LENGTH_MISMATH = 2;

        /** client socket*/
        private Socket clientSocket = null;
        
        /** ip adress definition*/
        private IPAddress adress = null;

        /** internal port  configuration*/
        private int port = 0;

        /** state of socket */
        private bool bSocketConnected = false;

        /** TcpClientObject default constructor
         * @brief etash a connection on localhost port 31001
         */ 
        public TcpClientObject()
        {
            try
            {
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31001));
                this.bSocketConnected = true;
            }
            catch (Exception ex)
            {
                this.bSocketConnected = false;
            }
        }

        /** TcpClientObject constructor
         * @param in ipadress associated ip adress
         * @param in port attached port
         */
        public TcpClientObject(String ipdress, int port)
        {
            try
            {
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipdress), port));
                this.bSocketConnected = true;
            }
            catch (Exception ex)
            {
                this.bSocketConnected = false;
            }
    }

        /** implementation of destructor for disposing object
         * do the close for the corresponding sockect
         */
        ~TcpClientObject()
        {
            Close();
        }

        /** send data using the tcp socket
         * @param [in] associated data to send trough the tcp connection
         * @return error code
         */ 
        public int Send(String data)
        {
            // Send the file name.
            Byte[] bData = Encoding.ASCII.GetBytes(data);

            int length = clientSocket.Send(bData);
            if (length != bData.Length)
            {
                return TCP_DATA_LENGTH_MISMATH;
            }

            return TCP_OK;
        }

        /** received data using the tcp socket
         * @return received data
         */ 
        public byte[] Received()
        {
            byte[] data = new byte[128];
            clientSocket.Receive(data);
            return data;
        }

        /** close the socket
         */
        public void Close()
        {
            if (this.bSocketConnected)
            {
                this.clientSocket.Close();
                this.bSocketConnected = false;
            }
        }
    }
}
