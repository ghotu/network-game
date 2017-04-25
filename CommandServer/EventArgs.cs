using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Project.CommandServer
{
    public delegate void CommandReceivedEventHandler(object sender, CommandEventArgs e);

    public delegate void CommandSentEventHandler(object sender, EventArgs e);

    public delegate void CommandSendingFailedEventHandler(object sender, EventArgs e);

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
    public delegate void DisconnectedEventHandler(object sender, ClientEventArgs e);
    public class ClientEventArgs : EventArgs
    {
        private Socket socket;
        public IPAddress IP
        {
            get { return ((IPEndPoint)this.socket.RemoteEndPoint).Address; }
        }
        public int Port
        {
            get { return ((IPEndPoint)this.socket.RemoteEndPoint).Port; }
        }
        public ClientEventArgs(Socket clientManagerSocket)
        {
            this.socket = clientManagerSocket;
        }
    }
}
