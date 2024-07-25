using HypnotoadUi.IPC;
using System.Collections.Generic;

namespace HypnotoadUi.Functions;

public static class Chat
{

    public static void HandleBroadcastCommands(List<string> message)
    {
        if (message[1].Equals(@"/logout"))
            IPCProvider.CharacterLogoutAction();
        else if (message[1].Equals(@"/shutdown"))
            IPCProvider.GameShutdownAction();
        else
            IPCProvider.SendChatAction(message[1]);
    }

}
