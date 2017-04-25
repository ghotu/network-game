using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;

namespace Project.CommandClient
{
    public class CMDClient
    {
        private Socket clientSocket;
        private NetworkStream networkStream;
        private BackgroundWorker bwReceiver;
        private IPEndPoint serverEP;
        private string networkName;

        public bool Connected
        {
            get
            {
                if (this.clientSocket != null)
                    return this.clientSocket.Connected;
                else
                    return false;
            }
        }
        public IPAddress ServerIP
        {
            get
            {
                if (this.Connected)
                    return this.serverEP.Address;
                else
                    return IPAddress.None;
            }
        }
        public int ServerPort
        {
            get
            {
                if (this.Connected)
                    return this.serverEP.Port;
                else
                    return -1;
            }
        }
        public IPAddress IP
        {
            get
            {
                if (this.Connected)
                    return ((IPEndPoint)this.clientSocket.LocalEndPoint).Address;
                else
                    return IPAddress.None;
            }
        }
        public int Port
        {
            get
            {
                if (this.Connected)
                    return ((IPEndPoint)this.clientSocket.LocalEndPoint).Port;
                else
                    return -1;
            }
        }
        public string NetworkName
        {
            get { return networkName; }
            set { networkName = value; }
        }

        public string Password { get; set; }

        #region Contsructors
        public CMDClient(IPEndPoint server, string netName)
        {
            this.serverEP = server;
            this.networkName = netName;
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += new System.Net.NetworkInformation.NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        public CMDClient(IPAddress serverIP, int port, string netName)
        {
            this.serverEP = new IPEndPoint(serverIP, port);
            this.networkName = netName;
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += new System.Net.NetworkInformation.NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        #endregion

        #region Private Methods

        private void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable)
            {
                this.OnNetworkDead(new EventArgs());
                this.OnDisconnectedFromServer(new EventArgs());
            }
            else
                this.OnNetworkAlived(new EventArgs());
        }

        private void StartReceive(object sender, DoWorkEventArgs e)
        {
            while (this.clientSocket.Connected)
            {
                //Read the command's Type.
                byte[] buffer = new byte[4];
                int readBytes = this.networkStream.Read(buffer, 0, 4);
                if (readBytes == 0)
                    break;
                CommandType cmdType = (CommandType)(BitConverter.ToInt32(buffer, 0));

                //Read the command's sender ip size.
                buffer = new byte[4];
                readBytes = this.networkStream.Read(buffer, 0, 4);
                if (readBytes == 0)
                    break;
                int senderIPSize = BitConverter.ToInt32(buffer, 0);

                //Read the command's sender ip.
                buffer = new byte[senderIPSize];
                readBytes = this.networkStream.Read(buffer, 0, senderIPSize);
                if (readBytes == 0)
                    break;
                IPAddress senderIP = IPAddress.Parse(System.Text.Encoding.ASCII.GetString(buffer));
                //IPAddress senderIP = IPAddress.Parse("127.0.0.1");
                //Read the command's sender name size.
                buffer = new byte[4];
                readBytes = this.networkStream.Read(buffer, 0, 4);
                if (readBytes == 0)
                    break;
                int senderNameSize = BitConverter.ToInt32(buffer, 0);

                //Read the command's sender name.
                buffer = new byte[senderNameSize];
                readBytes = this.networkStream.Read(buffer, 0, senderNameSize);
                if (readBytes == 0)
                    break;
                string senderName = System.Text.Encoding.Unicode.GetString(buffer);

                //Read the command's target size.
                string cmdTarget = "";
                buffer = new byte[4];
                readBytes = this.networkStream.Read(buffer, 0, 4);
                if (readBytes == 0)
                    break;
                int ipSize = BitConverter.ToInt32(buffer, 0);

                //Read the command's target.
                buffer = new byte[ipSize];
                readBytes = this.networkStream.Read(buffer, 0, ipSize);
                if (readBytes == 0)
                    break;
                cmdTarget = System.Text.Encoding.ASCII.GetString(buffer);

                //Read the command's MetaData size.
                string cmdMetaData = "";
                buffer = new byte[4];
                readBytes = this.networkStream.Read(buffer, 0, 4);
                if (readBytes == 0)
                    break;
                int metaDataSize = BitConverter.ToInt32(buffer, 0);

                //Read the command's Meta data.
                buffer = new byte[metaDataSize];
                readBytes = this.networkStream.Read(buffer, 0, metaDataSize);
                if (readBytes == 0)
                    break;
                cmdMetaData = System.Text.Encoding.Unicode.GetString(buffer);

                Command cmd = new Command(cmdType, IPAddress.Parse(cmdTarget), cmdMetaData);
                cmd.SenderIP = senderIP;
                cmd.SenderName = senderName;
                this.OnCommandReceived(new CommandEventArgs(cmd));
            }
            this.OnServerDisconnected(new ServerEventArgs(this.clientSocket));
            this.Disconnect();
        }

        private void bwSender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null && ((bool)e.Result))
                this.OnCommandSent(new EventArgs());
            else
                this.OnCommandFailed(new EventArgs());

            ((BackgroundWorker)sender).Dispose();
            GC.Collect();
        }

        private void bwSender_DoWork(object sender, DoWorkEventArgs e)
        {
            Command cmd = (Command)e.Argument;
            e.Result = this.SendCommandToServer(cmd);
        }

        //This Semaphor is to protect the critical section from concurrent access of sender threads.
        System.Threading.Semaphore semaphor = new System.Threading.Semaphore(1, 1);
        private bool SendCommandToServer(Command cmd)
        {
            try
            {
                semaphor.WaitOne();
                if (cmd.MetaData == null || cmd.MetaData == "")
                    this.SetMetaDataIfIsNull(cmd);
                //CommandType
                byte[] buffer = new byte[4];
                buffer = BitConverter.GetBytes((int)cmd.CommandType);
                this.networkStream.Write(buffer, 0, 4);
                this.networkStream.Flush();
                //Command Target
                byte[] ipBuffer = Encoding.ASCII.GetBytes(cmd.Target.ToString());
                buffer = new byte[4];
                buffer = BitConverter.GetBytes(ipBuffer.Length);
                this.networkStream.Write(buffer, 0, 4);
                this.networkStream.Flush();
                this.networkStream.Write(ipBuffer, 0, ipBuffer.Length);
                this.networkStream.Flush();
                //Command MetaData
                byte[] metaBuffer = Encoding.Unicode.GetBytes(cmd.MetaData);
                buffer = new byte[4];
                buffer = BitConverter.GetBytes(metaBuffer.Length);
                this.networkStream.Write(buffer, 0, 4);
                this.networkStream.Flush();
                this.networkStream.Write(metaBuffer, 0, metaBuffer.Length);
                this.networkStream.Flush();

                semaphor.Release();
                return true;
            }
            catch
            {
                semaphor.Release();
                return false;
            }

        }

        private void SetMetaDataIfIsNull(Command cmd)
        {
            switch (cmd.CommandType)
            {
                case (CommandType.ClientLoginInform):
                    cmd.MetaData = this.IP.ToString() + ":" + this.networkName;
                    break;
                case (CommandType.PCLockWithTimer):
                case (CommandType.PCLogOFFWithTimer):
                case (CommandType.PCRestartWithTimer):
                case (CommandType.PCShutDownWithTimer):
                case (CommandType.UserExitWithTimer):
                    cmd.MetaData = "60000";
                    break;
                default:
                    cmd.MetaData = "\n";
                    break;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Connect the current instance of command client to the server.This method throws ServerNotFoundException on failur.Run this method and handle the 'ConnectingSuccessed' and 'ConnectingFailed' to get the connection state.
        /// </summary>
        public void ConnectToServer()
        {
            BackgroundWorker bwConnector = new BackgroundWorker();
            bwConnector.DoWork += new DoWorkEventHandler(bwConnector_DoWork);
            bwConnector.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwConnector_RunWorkerCompleted);
            bwConnector.RunWorkerAsync();
        }

        private void bwConnector_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!((bool)e.Result))
                this.OnConnectingFailed(new EventArgs());
            else
                this.OnConnectingSuccessed(new EventArgs());

            ((BackgroundWorker)sender).Dispose();
        }

        private void bwConnector_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.clientSocket.Connect(this.serverEP);
                e.Result = true;
                this.networkStream = new NetworkStream(this.clientSocket);
                this.bwReceiver = new BackgroundWorker();
                this.bwReceiver.WorkerSupportsCancellation = true;
                this.bwReceiver.DoWork += new DoWorkEventHandler(StartReceive);
                this.bwReceiver.RunWorkerAsync();

                //Inform to all clients that this client is now online.
                Command informToAllCMD = new Command(CommandType.ClientLoginInform, IPAddress.Broadcast, this.IP.ToString() + ":" + this.networkName + ":" + this.Password);
                this.SendCommand(informToAllCMD);
            }
            catch
            {
                e.Result = false;
            }
        }
        public void SendCommand(Command cmd)
        {
            if (this.clientSocket != null && this.clientSocket.Connected)
            {
                BackgroundWorker bwSender = new BackgroundWorker();
                bwSender.DoWork += new DoWorkEventHandler(bwSender_DoWork);
                bwSender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSender_RunWorkerCompleted);
                bwSender.WorkerSupportsCancellation = true;
                bwSender.RunWorkerAsync(cmd);
            }
            else
                this.OnCommandFailed(new EventArgs());
        }

        public bool Disconnect()
        {
            if (this.clientSocket != null && this.clientSocket.Connected)
            {
                try
                {
                    this.clientSocket.Shutdown(SocketShutdown.Both);
                    this.clientSocket.Close();
                    this.bwReceiver.CancelAsync();
                    this.OnDisconnectedFromServer(new EventArgs());
                    return true;
                }
                catch
                {
                    return false;
                }

            }
            else
                return true;
        }
        #endregion

        #region Events
        public event CommandReceivedEventHandler CommandReceived;
        protected virtual void OnCommandReceived(CommandEventArgs e)
        {
            if (CommandReceived != null)
            {
                Control target = CommandReceived.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(CommandReceived, new object[] { this, e });
                else
                    CommandReceived(this, e);
            }
        }
        public event CommandSentEventHandler CommandSent;
        protected virtual void OnCommandSent(EventArgs e)
        {
            if (CommandSent != null)
            {
                Control target = CommandSent.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(CommandSent, new object[] { this, e });
                else
                    CommandSent(this, e);
            }
        }

        public event CommandSendingFailedEventHandler CommandFailed;
        protected virtual void OnCommandFailed(EventArgs e)
        {
            if (CommandFailed != null)
            {
                Control target = CommandFailed.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(CommandFailed, new object[] { this, e });
                else
                    CommandFailed(this, e);
            }
        }

        public event ServerDisconnectedEventHandler ServerDisconnected;
        protected virtual void OnServerDisconnected(ServerEventArgs e)
        {
            if (ServerDisconnected != null)
            {
                Control target = ServerDisconnected.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(ServerDisconnected, new object[] { this, e });
                else
                    ServerDisconnected(this, e);
            }
        }

        public event DisconnectedEventHandler DisconnectedFromServer;
        protected virtual void OnDisconnectedFromServer(EventArgs e)
        {
            if (DisconnectedFromServer != null)
            {
                Control target = DisconnectedFromServer.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(DisconnectedFromServer, new object[] { this, e });
                else
                    DisconnectedFromServer(this, e);
            }
        }

        public event ConnectingSuccessedEventHandler ConnectingSuccessed;
        protected virtual void OnConnectingSuccessed(EventArgs e)
        {
            if (ConnectingSuccessed != null)
            {
                Control target = ConnectingSuccessed.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(ConnectingSuccessed, new object[] { this, e });
                else
                    ConnectingSuccessed(this, e);
            }
        }

        public event ConnectingFailedEventHandler ConnectingFailed;
        protected virtual void OnConnectingFailed(EventArgs e)
        {
            if (ConnectingFailed != null)
            {
                Control target = ConnectingFailed.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(ConnectingFailed, new object[] { this, e });
                else
                    ConnectingFailed(this, e);
            }
        }

        public event NetworkDeadEventHandler NetworkDead;
        protected virtual void OnNetworkDead(EventArgs e)
        {
            if (NetworkDead != null)
            {
                Control target = NetworkDead.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(NetworkDead, new object[] { this, e });
                else
                    NetworkDead(this, e);
            }
        }

        public event NetworkAlivedEventHandler NetworkAlived;
        protected virtual void OnNetworkAlived(EventArgs e)
        {
            if (NetworkAlived != null)
            {
                Control target = NetworkAlived.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(NetworkAlived, new object[] { this, e });
                else
                    NetworkAlived(this, e);
            }
        }

        #endregion
    }
}
