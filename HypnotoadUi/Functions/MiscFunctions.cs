using Dalamud.Game.ClientState.Objects.SubKinds;
using HypnotoadUi.IPC;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HypnotoadUi.Functions;

public static class MiscFunctions
{
    [DllImport("user32.dll")]
    public static extern int SetWindowText(IntPtr hWnd, string text);

    public static void EnableBCForClient(LocalPlayer player)
    {
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCEnabled, new List<string>()
        {
            player.LocalContentId.ToString(),
            player.BroadCastEnabled.ToString()
        });
    }
}
