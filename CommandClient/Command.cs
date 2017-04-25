using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Project.CommandClient
{
    public class Command
    {
        private IPAddress senderIP;
        public IPAddress SenderIP
        {
            get { return senderIP; }
            set { senderIP = value; }
        }

        private string senderName;
        public string SenderName
        {
            get { return senderName; }
            set { senderName = value; }
        }
        
        private CommandType cmdType;
        public CommandType CommandType
        {
            get { return cmdType; }
            set { cmdType = value; }
        }

        private IPAddress target;
        public IPAddress Target
        {
            get { return target; }
            set { target = value; }
        }
        private string commandBody;
        public string MetaData
        {
            get { return commandBody; }
            set { commandBody = value; }
        }
        public Command(CommandType type , IPAddress targetMachine , string metaData)
        {
            this.cmdType = type;
            this.target = targetMachine;
            this.commandBody = metaData;
        }

        public Command(CommandType type , IPAddress targetMachine)
        {
            this.cmdType = type;
            this.target = targetMachine;
            this.commandBody = "";
        }
    }
}
