using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System.Collections.Generic;
using System.Linq;

namespace HypnotoadUi.IPC;

public class LocalPlayer
{
    public ulong LocalContentId { get; init; } = 0;
    public string Name { get; init; } = "";
    public uint HomeWorld { get; init; } = 0;
    public bool BroadCastEnabled { get; set; }  = true;
    public int Affinity { get; set; } = 0;

    public unsafe string GetWorldName()
    {
        return WorldHelper.Instance()->AllWorlds[(ushort)HomeWorld].NameString;
    }
}

public static class LocalPlayerCollector
{
    public static List<LocalPlayer> localPlayers = new List<LocalPlayer>();

    public static void Add(ulong localContentId, string name, uint HomeWorld, int affinity)
    {
        var t = localPlayers.FirstOrDefault(n => n.LocalContentId == localContentId && n.Name == name);
        if (t is null)
        {
            localPlayers.Add(new LocalPlayer() { LocalContentId = localContentId, Name = name, HomeWorld = HomeWorld });
            Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCAdd, new List<string>()
            {
                Api.ClientState.LocalPlayer.Name.TextValue,
                Api.ClientState.LocalPlayer.HomeWorld.ValueNullable?.RowId.ToString(),
                "0"
            });
        }
    }

    public static void BroadCastEnabled(bool enabled)
    {
        var t = localPlayers.Where(n => n.LocalContentId == Api.ClientState.LocalContentId);
        if (t is null)
            return;
        t.First().BroadCastEnabled = enabled;
    }

    public static void Remove(ulong localContentId, string name)
    {
        var t = localPlayers.FirstOrDefault(n => n.LocalContentId == localContentId && n.Name == name);
        if (t is null)
            return;
        localPlayers.Remove(t);
    }
}
