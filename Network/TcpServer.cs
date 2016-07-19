using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using csharpFramework.FileIO;

namespace csharpFramework.Network
{
    /** @brief class TCPServer
     * TCPServer is the Server class. When "StartServer" method is called
     * this Server object tries to connect to a IP Address specified on a port
     * configured. Then the server start listening for client socket requests.
     * As soon as a requestcomes in from any client then a Client Socket 
     * Listening thread will be started. That thread is responsible for client
     * communication.
     */
    public class TCPServer
    {
        // constant declaration
        public static IPAddress DEFAULT_SERVER = IPAddress.Parse("127.0.0.1");
        public static int DEFAULT_PORT = 31001;
        public static IPEndPoint DEFAULT_IP_END_POINT = new IPEndPoint(DEFAULT_SERVER, DEFAULT_PORT);

        // Local Variables Declaration.
        private TcpListener m_server = null;
        private bool m_stopServer = false;
        private bool m_stopPurging = false;
        private Thread m_serverThread = null;
        private Thread m_purgingThread = null;
        private ArrayList m_socketListenersList = null;

        private TcpReceivedObject m_bindedObject = null;

        /** @brief constructors
         * instanciate a TCPSErver on ip 127.0.0.1 : port 31001
         */ 
        public TCPServer()
        {
            Init(DEFAULT_IP_END_POINT);
        }
        public TCPServer(IPAddress serverIP)
        {
            Init(new IPEndPoint(serverIP, DEFAULT_PORT));
        }
        public TCPServer(int port)
        {
            Init(new IPEndPoint(DEFAULT_SERVER, port));
        }
        public TCPServer(IPAddress serverIP, int port)
        {
            Init(new IPEndPoint(serverIP, port));
        }
        public TCPServer(IPEndPoint ipNport)
        {
            Init(ipNport);
        }

        /**
         * Destructor implementation
         * needed for all dispose strop server when garbage collection detry object
         */
        ~TCPServer()
        {
            StopServer();
        }

        /** \brief Init method that create a server (TCP Listener) Object based on the
         * IP Address and Port information that is passed in.
         * \param [in] ipNport : IP end point using ip adresse condfiration and port
         */
        private void Init(IPEndPoint ipNport)
        {
            try
            {
                cLog.Log("[TcpServer]\t[Init]\tIp adress : " + ipNport.Address, cLog.Prio.Info);
                cLog.Log("[TcpServer]\t[Init]\tPort : " + ipNport.Port, cLog.Prio.Info);

                m_server = new TcpListener(ipNport);
                if (!Directory.Exists(TCPSocketListener.DEFAULT_FILE_STORE_LOC))
                {
                    Directory.CreateDirectory(TCPSocketListener.DEFAULT_FILE_STORE_LOC);
                }
            }
            catch (Exception e)
            {
                m_server = null;
                cLog.Log("[TcpServer]\t[Init]\tAn exception occur when initialized tcp server" + e.Message, cLog.Prio.Info);
            }
        }

        /**
         * \brief bind a tcp received object
         * \param [in] tcp received object
         */ 
        public void BindObject(TcpReceivedObject obj)
        {
            this.m_bindedObject = obj;
        }

        /** \brief Method that starts TCP/IP Server.
         */ 
        public void StartServer()
        {
            try
            {
                if (m_server != null)
                {
                    cLog.Log("[TcpServer]\t[StartServer]\tStarting server", cLog.Prio.Info);

                    // Create a ArrayList for storing SocketListeners before
                    // starting the server.
                    m_socketListenersList = new ArrayList();

                    // Start the Server and start the thread to listen client 
                    // requests.
                    m_server.Start();
                    m_serverThread = new Thread(new ThreadStart(ServerThreadStart));
                    m_serverThread.Start();

                    // Create a low priority thread that checks and deletes client
                    // SocktConnection objcts that are marked for deletion.
                    m_purgingThread = new Thread(new ThreadStart(PurgingThreadStart));
                    m_purgingThread.Priority = ThreadPriority.Lowest;
                    m_purgingThread.Start();

                    cLog.Log("[TcpServer]\t[StartServer]\tStart server successsully", cLog.Prio.Info);
                }
            }
            catch (Exception ex)
            {
                cLog.Log("[TcpServer]\t[StartServer]\tStart server raised an exception : " + ex.Message, cLog.Prio.Exception);
            }
        }

        /** \brief Method that stops the TCP/IP Server.
         */ 
        public void StopServer()
        {
            try
            {
                if (m_server != null)
                {
                    cLog.Log("[TcpServer]\t[StopServer]\tStopping server", cLog.Prio.Info);

                    // It is important to Stop the server first before doing
                    // any cleanup. If not so, clients might being added as
                    // server is running, but supporting data structures
                    // (such as m_socketListenersList) are cleared. This might
                    // cause exceptions.

                    // Stop the TCP/IP Server.
                    m_stopServer = true;
                    m_server.Stop();

                    // Wait for one second for the the thread to stop.
                    m_serverThread.Join(1000);

                    // If still alive; Get rid of the thread.
                    if (m_serverThread.IsAlive)
                    {
                        m_serverThread.Abort();
                    }
                    m_serverThread = null;

                    m_stopPurging = true;
                    m_purgingThread.Join(1000);
                    if (m_purgingThread.IsAlive)
                    {
                        m_purgingThread.Abort();
                    }
                    m_purgingThread = null;

                    // Free Server Object.
                    m_server = null;

                    // Stop All clients.
                    StopAllSocketListers();
                    cLog.Log("[TcpServer]\t[StopServer]\tStop server successsully", cLog.Prio.Info);
                }
            }
            catch (Exception ex)
            {
                cLog.Log("[TcpServer]\t[StopServer]\tStop server raised an exception : " + ex.Message, cLog.Prio.Exception);
            }
        }

        /** \brief Method that stops all clients and clears the list.
         */ 
        private void StopAllSocketListers()
        {
            try
            {
                foreach (TCPSocketListener socketListener
                             in m_socketListenersList)
                {
                    socketListener.StopSocketListener();
                }
                // Remove all elements from the list.
                m_socketListenersList.Clear();
                m_socketListenersList = null;
            }
            catch (Exception ex)
            {
                cLog.Log("[TcpServer]\t[StopAllSocketListers]\tStop all socket listeners raised an exception : " + ex.Message, cLog.Prio.Exception);
            }
        }

        /** \brief TCP/IP Server Thread that is listening for clients.
         */ 
        private void ServerThreadStart()
        {
            cLog.Log("[TcpServer]\t[ServerThreadStart]\tStart a new thread for manage tcp data", cLog.Prio.Info);

            // Client Socket variable;
            Socket clientSocket = null;
            TCPSocketListener socketListener = null;
            while (!m_stopServer)
            {
                try
                {
                    // Wait for any client requests and if there is any 
                    // request from any client accept it (Wait indefinitely).
                    clientSocket = m_server.AcceptSocket();
                    cLog.Log("[TcpServer]\t[ServerThreadStart]\tAcceptSocket", cLog.Prio.Info);

                    // Create a SocketListener object for the client.
                    if (null != this.m_bindedObject)
                    {
                        socketListener = new TCPSocketListener(clientSocket, this.m_bindedObject);
                    }
                    else
                    {
                        socketListener = new TCPSocketListener(clientSocket);
                    }

                    // Add the socket listener to an array list in a thread 
                    // safe fashon.
                    //Monitor.Enter(m_socketListenersList);
                    lock (m_socketListenersList)
                    {
                        m_socketListenersList.Add(socketListener);
                    }
                    //Monitor.Exit(m_socketListenersList);

                    // Start a communicating with the client in a different
                    // thread.
                    cLog.Log("[TcpServer]\t[ServerThreadStart]\tStartSocketListener", cLog.Prio.Info);
                    socketListener.StartSocketListener();
                }
                catch (SocketException se)
                {
                    m_stopServer = true;
                    cLog.Log("[TcpServer]\t[ServerThreadStart]\tServerThreadStart rise an exception : " + se.Message, cLog.Prio.Exception);
                }
            }

            cLog.Log("[TcpServer]\t[ServerThreadStart]\tQuit thread", cLog.Prio.Info);
        }

        /** \brief Thread method for purging Client Listeneres that are marked for
         * deletion (i.e. clients with socket connection closed). This thead
         * is a low priority thread and sleeps for 10 seconds and then check
         * for any client SocketConnection obects which are obselete and 
         * marked for deletion.
         */ 
        private void PurgingThreadStart()
        {
            try
            {
                while (!m_stopPurging)
                {
                    ArrayList deleteList = new ArrayList();

                    // Check for any clients SocketListeners that are to be
                    // deleted and put them in a separate list in a thread sage
                    // fashon.
                    //Monitor.Enter(m_socketListenersList);
                    lock (m_socketListenersList)
                    {
                        foreach (TCPSocketListener socketListener
                                     in m_socketListenersList)
                        {
                            if (socketListener.IsMarkedForDeletion())
                            {
                                deleteList.Add(socketListener);
                                socketListener.StopSocketListener();
                            }
                        }

                        // Delete all the client SocketConnection ojects which are
                        // in marked for deletion and are in the delete list.
                        for (int i = 0; i < deleteList.Count; ++i)
                        {
                            m_socketListenersList.Remove(deleteList[i]);
                        }
                    }
                    //Monitor.Exit(m_socketListenersList);

                    deleteList = null;
                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                cLog.Log("[TcpServer]\t[PurgingThreadStart]\tPurgingThreadStart rise an exception : " + ex.Message, cLog.Prio.Exception);
            }
        }
    }
}
