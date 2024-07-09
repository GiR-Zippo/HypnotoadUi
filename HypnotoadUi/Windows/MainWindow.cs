using System;
using System.IO;
using System.Numerics;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using HypnotoadUi.IPC;
using ImGuiNET;

namespace HypnotoadUi.Windows;

public class MainWindow : Window, IDisposable
{
    private HypnotoadUi plugin;

    public MainWindow(HypnotoadUi plugin) : base(
        "Hypnotoad-Ui", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose()
    {

    }

    private FileDialogManager fileDialogManager = null;
    string loadedFilePath = "";
    public override void Draw()
    {
        if (ImGui.CollapsingHeader("Graphic Settings", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Set Low except me"))
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.SetGfx, new System.Collections.Generic.List<string>() { true.ToString() });
            ImGui.SameLine();
            if (ImGui.Button("Set Low Locally"))
                IPCProvider.SetGfxLowAction(true);
            ImGui.SameLine();
            if (ImGui.Button("Reset"))
            {
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.SetGfx, new System.Collections.Generic.List<string>() { false.ToString() });
                IPCProvider.SetGfxLowAction(false);
            }
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
        }
        if (ImGui.CollapsingHeader("Party", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Invite to party"))
            {
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyInviteAccept, new System.Collections.Generic.List<string>());
                foreach (var player in LocalPlayerCollector.localPlayers)
                {
                    if (player.GamobjectId == Api.ClientState.LocalPlayer.GameObjectId)
                        continue;
                    IPCProvider.PartyInviteAction(player.Name + ";" + Convert.ToUInt16(player.HomeWorld).ToString());
                }
            }
            ImGui.SameLine();
            if (ImGui.Button("Gimme the lead"))
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyPromote, new System.Collections.Generic.List<string>()
                {
                    Api.ClientState.LocalPlayer.Name.TextValue,
                    Api.ClientState.LocalPlayer.HomeWorld.Id.ToString(),
                });
            ImGui.SameLine();
            if (ImGui.Button("Enter House"))
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyEnterHouse, new System.Collections.Generic.List<string>());
            if (ImGui.Button("Teleport"))
            {
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyTeleport, new System.Collections.Generic.List<string>());
                IPCProvider.PartyTeleportAction(true);
            }
            if (ImGui.Button("Follow Me"))
            {
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyFollow, new System.Collections.Generic.List<string>()
                {
                    (true).ToString(),
                    Api.ClientState.LocalPlayer.Name.TextValue,
                    Api.ClientState.LocalPlayer.HomeWorld.Id.ToString(),
                });
            }
            ImGui.SameLine();
            if (ImGui.Button("Stop Follow"))
            {
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.PartyFollow, new System.Collections.Generic.List<string>()
                {
                    (false).ToString()
                });
            }

        }
    }
}
