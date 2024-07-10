using System;
using System.Numerics;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using HypnotoadUi.Functions;
using HypnotoadUi.IPC;
using HypnotoadUi.Misc;
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
        /*********************************************************/
        /***                   Graphic Settings                ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Graphic Settings", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Set Low except me"))
                GameConfig.SetLowExceptMe();
            ImGui.SameLine();
            if (ImGui.Button("Set Low Locally"))
                IPCProvider.SetGfxLowAction(true);
            ImGui.SameLine();
            if (ImGui.Button("Reset"))
            {
                GameConfig.Reset();
                IPCProvider.SetGfxLowAction(false);
            }
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
        }

        /*********************************************************/
        /***                      Party Menu                   ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Party", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Invite to party"))
                Party.InviteToParty();
            ImGui.SameLine();
            if (ImGui.Button("Gimme the lead"))
                Party.GimmePartyLead();
            ImGui.SameLine();
            if (ImGui.Button("Enter House"))
                Party.EnterHouse();
            if (ImGui.Button("Teleport"))
                Party.Teleport();
            if (ImGui.Button("Follow Me"))
                Party.FollowMe();
            ImGui.SameLine();
            if (ImGui.Button("Stop Follow"))
                Party.StopFollow();

        }

        /*********************************************************/
        /***                      Misc  Menu                   ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Misc", ImGuiTreeNodeFlags.DefaultOpen))
        {
            var MultiboxingEnabled = plugin.Configuration.MultiboxingEnabled;
            if (ImGui.Checkbox("Enable Multiboxing", ref MultiboxingEnabled))
            {
                Multiboxing.RemoveHandle();
                plugin.Configuration.MultiboxingEnabled = MultiboxingEnabled;
                plugin.Configuration.Save();
            }
        }
    }
}
