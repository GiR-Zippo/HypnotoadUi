using Dalamud.Game.ClientState.Objects.SubKinds;
using HypnotoadUi.IPC;
using System.Collections.Generic;
using System.Linq;

namespace HypnotoadUi.Functions;

public static class MiscFunctions
{
    public static void EnableBCForClient(LocalPlayer player)
    {
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCEnabled, new List<string>()
        {
            player.LocalContentId.ToString(),
            player.BroadCastEnabled.ToString()
        });
    }
}
