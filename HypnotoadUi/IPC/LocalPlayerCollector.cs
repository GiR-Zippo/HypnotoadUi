using System.Collections.Generic;
using System.Linq;

namespace HypnotoadUi.IPC;

public class LocalPlayer
{
    public ulong LocalContentId { get; init; } = 0;
    public string Name { get; init; } = "";
    public uint HomeWorld { get; init; } = 0;
}

public static class LocalPlayerCollector
{
    public static List<LocalPlayer> localPlayers = new List<LocalPlayer>();

    public static void Add(ulong localContentId, string name, uint HomeWorld)
    {
        var t = localPlayers.FirstOrDefault(n => n.LocalContentId == localContentId && n.Name == name);
        if (t is null)
        {
            localPlayers.Add(new LocalPlayer() { LocalContentId = localContentId, Name = name, HomeWorld = HomeWorld });
            Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCAdd, new List<string>()
            {
                Api.ClientState.LocalPlayer.Name.TextValue,
                Api.ClientState.LocalPlayer.HomeWorld.Id.ToString()
            });
        }
    }

    public static void Remove(ulong localContentId, string name)
    {
        var t = localPlayers.FirstOrDefault(n => n.LocalContentId == localContentId && n.Name == name);
        if (t is null)
            return;
        localPlayers.Remove(t);
    }    
}
