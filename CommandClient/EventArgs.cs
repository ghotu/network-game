using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Project.CommandClient
{
    public delegate void CommandReceivedEventHandler(object sender , CommandEventArgs e);

    public delegate void CommandSentEventHandler(object sender , EventArgs e);

    public delegate void CommandSendingFailedEventHandler(object sender , EventArgs e);

    public class CommandEventArgs : EventArgs
    {
        private Command command;
        public Command Command
        {
            get { return command; }
        }
        public CommandEventArgs(Command cmd)
        {
            this.command = cmd;
        }
    }
    public delegate void ServerDisconnectedEventHandler(object sender , ServerEventArgs e);
    public class ServerEventArgs : EventArgs
    {
        private Socket socket;
        public IPAddress IP
        {
            get { return ( (IPEndPoint)this.socket.RemoteEndPoint ).Address; }
        }
        public int Port
        {
            get { return ( (IPEndPoint)this.socket.RemoteEndPoint ).Port; }
        }
        public ServerEventArgs(Socket clientSocket)
        {
            this.socket = clientSocket;
        }
    }

    public delegate void DisconnectedEventHandler(object sender , EventArgs e);

    public delegate void ConnectingSuccessedEventHandler(object sender , EventArgs e);

    public delegate void ConnectingFailedEventHandler(object sender , EventArgs e);

    public delegate void NetworkDeadEventHandler(object sender , EventArgs e);

    public delegate void NetworkAlivedEventHandler(object sender , EventArgs e);

}
