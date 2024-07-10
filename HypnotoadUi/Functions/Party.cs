using HypnotoadUi.IPC;
using System;
using System.Collections.Generic;

namespace HypnotoadUi.Functions;

public static class Party
{
    public static void InviteToParty()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyInviteAccept, new List<string>());
        foreach (var player in LocalPlayerCollector.localPlayers)
        {
            if (player.GamobjectId == Api.ClientState.LocalPlayer.GameObjectId)
                continue;
            IPCProvider.PartyInviteAction(player.Name + ";" + Convert.ToUInt16(player.HomeWorld).ToString());
        }
    }

    public static void GimmePartyLead()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyPromote, new List<string>()
                {
                    Api.ClientState.LocalPlayer.Name.TextValue,
                    Api.ClientState.LocalPlayer.HomeWorld.Id.ToString(),
                });
    }

    public static void EnterHouse()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyEnterHouse, new List<string>());
    }

    public static void Teleport()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyTeleport, new System.Collections.Generic.List<string>());
        IPCProvider.PartyTeleportAction(true);
    }

    public static void FollowMe()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyFollow, new System.Collections.Generic.List<string>()
                {
                    (true).ToString(),
                    Api.ClientState.LocalPlayer.Name.TextValue,
                    Api.ClientState.LocalPlayer.HomeWorld.Id.ToString(),
                });
    }

    public static void StopFollow()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyFollow, new System.Collections.Generic.List<string>()
                {
                    (false).ToString()
                });
    }
}
