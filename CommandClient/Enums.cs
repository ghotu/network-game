using System;
using System.Collections.Generic;
using System.Text;

namespace Project.CommandClient
{
    public enum CommandType
    {
        UserExit ,
        UserExitWithTimer ,
        PCLock ,
        PCLockWithTimer ,
        PCRestart ,
        PCRestartWithTimer ,
        PCLogOFF ,
        PCLogOFFWithTimer ,
        PCShutDown ,
        PCShutDownWithTimer ,
        Message ,
        ClientLoginInform ,
        ClientLogOffInform ,
        IsNameExists,
        SendClientList,
        FreeCommand,

        LoginSuccessful,

        LoginUnsuccessful,
        NewUserJoined,
        GameAlphabet,
        StartGame,
        GameStatusUpdate
    }
}
