using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using Project.CommandServer;

namespace Project.CommandServer
{
    public class ClientManager
    {
        public IPAddress IP
        {
            get
            {
                if (this.socket != null)
                    return ((IPEndPoint)this.socket.RemoteEndPoint).Address;
                else
                    return IPAddress.None;
            }
        }
        public int Port
        {
            get
            {
                if (this.socket != null)
                    return ((IPEndPoint)this.socket.RemoteEndPoint).Port;
                else
                    return -1;
            }
        }
        public bool Connected
        {
            get
            {
                if (this.socket != null)
                    return this.socket.Connected;
                else
                    return false;
            }
        }

        private Socket socket;
        private string clientName;
        public string ClientName
        {
            get { return this.clientName; }
            set { this.clientName = value; }
        }
        NetworkStream networkStream;
        private BackgroundWorker bwReceiver;

        #region Constructor
        public ClientManager(Socket clientSocket)
        {
            this.socket = clientSocket;
            this.networkStream = new NetworkStream(this.socket);
            this.bwReceiver = new BackgroundWorker();
            this.bwReceiver.DoWork += new DoWorkEventHandler(StartReceive);
            this.bwReceiver.RunWorkerAsync();
        }
        #endregion

        #region Private Methods
        private void StartReceive(object sender, DoWorkEventArgs e)
        {
            while (this.socket.Connected)
            {

                byte[] buffer = new byte[4];
                int readBytes = this.networkStream.Read(buffer, 0, 4);
                if (readBytes == 0)
                    break;
                CommandType cmdType = (CommandType)(BitConverter.ToInt32(buffer, 0));

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
                cmd.SenderIP = this.IP;
                if (cmd.CommandType == CommandType.ClientLoginInform)
                    cmd.SenderName = cmd.MetaData.Split(new char[] { ':' })[1];
                else
                    cmd.SenderName = this.ClientName;
                this.OnCommandReceived(new CommandEventArgs(cmd));
            }
            this.OnDisconnected(new ClientEventArgs(this.socket));
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
            e.Result = this.SendCommandToClient(cmd);
        }


        System.Threading.Semaphore semaphor = new System.Threading.Semaphore(1, 1);
        private bool SendCommandToClient(Command cmd)
        {

            try
            {
                semaphor.WaitOne();
                //Type
                byte[] buffer = new byte[4];
                buffer = BitConverter.GetBytes((int)cmd.CommandType);
                this.networkStream.Write(buffer, 0, 4);
                this.networkStream.Flush();

                //Sender IP
                byte[] senderIPBuffer = Encoding.ASCII.GetBytes(cmd.SenderIP.ToString());
                buffer = new byte[4];
                buffer = BitConverter.GetBytes(senderIPBuffer.Length);
                this.networkStream.Write(buffer, 0, 4);
                this.networkStream.Flush();
                this.networkStream.Write(senderIPBuffer, 0, senderIPBuffer.Length);
                this.networkStream.Flush();

                //Sender Name
                byte[] senderNameBuffer = Encoding.Unicode.GetBytes(cmd.SenderName.ToString());
                buffer = new byte[4];
                buffer = BitConverter.GetBytes(senderNameBuffer.Length);
                this.networkStream.Write(buffer, 0, 4);
                this.networkStream.Flush();
                this.networkStream.Write(senderNameBuffer, 0, senderNameBuffer.Length);
                this.networkStream.Flush();

                //Target
                byte[] ipBuffer = Encoding.ASCII.GetBytes(cmd.Target.ToString());
                buffer = new byte[4];
                buffer = BitConverter.GetBytes(ipBuffer.Length);
                this.networkStream.Write(buffer, 0, 4);
                this.networkStream.Flush();
                this.networkStream.Write(ipBuffer, 0, ipBuffer.Length);
                this.networkStream.Flush();

                //Meta Data.
                if (cmd.MetaData == null || cmd.MetaData == "")
                    cmd.MetaData = "\n";

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
        #endregion

        #region Public Methods
        public void SendCommand(Command cmd)
        {
            if (this.socket != null && this.socket.Connected)
            {
                BackgroundWorker bwSender = new BackgroundWorker();
                bwSender.DoWork += new DoWorkEventHandler(bwSender_DoWork);
                bwSender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSender_RunWorkerCompleted);
                bwSender.RunWorkerAsync(cmd);
            }
            else
                this.OnCommandFailed(new EventArgs());
        }

        public bool Disconnect()
        {
            if (this.socket != null && this.socket.Connected)
            {
                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
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
                CommandReceived(this, e);
        }

        public event CommandSentEventHandler CommandSent;
        protected virtual void OnCommandSent(EventArgs e)
        {
            if (CommandSent != null)
                CommandSent(this, e);
        }

        public event CommandSendingFailedEventHandler CommandFailed;
        protected virtual void OnCommandFailed(EventArgs e)
        {
            if (CommandFailed != null)
                CommandFailed(this, e);
        }
        public event DisconnectedEventHandler Disconnected;
        protected virtual void OnDisconnected(ClientEventArgs e)
        {
            if (Disconnected != null)
                Disconnected(this, e);
        }

        #endregion
    }
}
