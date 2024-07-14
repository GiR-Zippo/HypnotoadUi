using HypnotoadUi.IPC;
using System.Collections.Generic;


namespace HypnotoadUi.Functions;

public static class GameConfig
{
    public static void SetLowExceptMe()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.SetGfx, new List<string>() { true.ToString() });
    }

    public static void Reset()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.SetGfx, new List<string>() { false.ToString() });
    }
}
