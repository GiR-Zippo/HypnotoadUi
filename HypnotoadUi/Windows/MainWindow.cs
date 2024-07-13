using System;
using System.Numerics;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using HypnotoadUi.Formations;
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


    public override void OnOpen()
    {
        plugin.Configuration.MainWindowVisible = true;
    }

    public override void OnClose()
    {
        plugin.Configuration.MainWindowVisible = false;
    }

    private FileDialogManager fileDialogManager = null;
    string loadedFilePath = "";
    FormationsData selected_formation = null;

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
            var AllowMultiBox = plugin.Configuration.AllowMultiBox;
            if (ImGui.Checkbox("Enable Multiboxing", ref AllowMultiBox))
            {
                Multiboxing.RemoveHandle();
                plugin.Configuration.AllowMultiBox = AllowMultiBox;
                plugin.Configuration.Save();
            }
            ImGui.SameLine();
            if (ImGui.Button("Import BtBFormation"))
            {
                plugin.ToggleDrawBtBUI();
            }
        }

        /*********************************************************/
        /***                   Formations  Menu                ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Formations", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.BeginCombo("##combo", selected_formation != null? selected_formation.Name : ""))
            {
                var comboData = plugin.Configuration.FormationsList;
                for (int n = 0; n < plugin.Configuration.FormationsList.Count; n++)
                {
                    bool is_selected = (selected_formation == comboData[n]);
                    if (ImGui.Selectable(comboData[n].Name, is_selected))
                        selected_formation = comboData[n];
                    if (is_selected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            if (ImGui.Button("Load"))
            {
                if (selected_formation != null)
                    FormationFactory.LoadFormation(selected_formation);
            }
            ImGui.SameLine();
            if (ImGui.Button("STOP"))
            {
                FormationFactory.StopFormation();
            }
        }
    }
}
