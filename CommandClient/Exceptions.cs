using System;

namespace Project.CommandClient
{
    public class ServerNotFoundException : Exception
    {
        public ServerNotFoundException(string message): base(message)
        { }
    }
}
