using System.Collections.Generic;
using System.Linq;

namespace HypnotoadUi.IPC;

public class LocalPlayer
{
    public ulong GamobjectId { get; init; } = 0;
    public string Name { get; init; } = "";
    public uint HomeWorld { get; init; } = 0;
}

public static class LocalPlayerCollector
{
    public static List<LocalPlayer> localPlayers = new List<LocalPlayer>();

    public static void Add(ulong gamobjectId, string name, uint HomeWorld)
    {
        var t = localPlayers.FirstOrDefault(n => n.GamobjectId == gamobjectId && n.Name == name);
        if (t is null)
        {
            localPlayers.Add(new LocalPlayer() { GamobjectId = gamobjectId, Name = name, HomeWorld = HomeWorld });
            Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.BCAdd, new List<string>()
            {
                Api.ClientState.LocalPlayer.Name.TextValue,
                Api.ClientState.LocalPlayer.HomeWorld.Id.ToString()
            });
        }
    }

    public static void Remove(ulong gamobjectId, string name)
    {
        var t = localPlayers.FirstOrDefault(n => n.GamobjectId == gamobjectId && n.Name == name);
        if (t is null)
            return;
        localPlayers.Remove(t);
    }    
}
