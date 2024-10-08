using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using HypnotoadUi.Formations;
using HypnotoadUi.IPC;
using ImGuiNET;

namespace HypnotoadUi.Windows;

public class BtBFormation : Window, IDisposable
{
    private Configuration configuration;

    public BtBFormation(HypnotoadUi plugin) : base(
        "BtB formation importer",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(350, 300);
        this.SizeCondition = ImGuiCond.Always;
        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }


    private FileDialogManager fileDialogManager = null;
    string loadedFilePath = "";

    static List<string> comboData = new List<string>();
    static string current_item = "";
    static string error_msg = "";
    public override void Draw()
    {
        if (ImGui.Button("Open BtB Config"))
        {
            if (fileDialogManager == null)
            {
                fileDialogManager = new FileDialogManager();
                Api.PluginInterface.UiBuilder.Draw += fileDialogManager.Draw;
                fileDialogManager.OpenFileDialog("Select File...", "json File{.json}", (b, files) =>
                {
                    if (files.Count != 1) return;
                    loadedFilePath = files[0];
                    comboData = FormationFactory.ReadBtBFormationNames(loadedFilePath);
                }, 1, loadedFilePath, true);
                fileDialogManager = null;
            }
        }

        if (ImGui.BeginCombo("##combo", current_item))
        {
            for (int n = 0; n < comboData.Count; n++)
            {
                bool is_selected = (current_item == comboData[n]);
                if (ImGui.Selectable(comboData[n], is_selected))
                    current_item = comboData[n];
                if (is_selected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        if (ImGui.Button("Read Formation"))
        {
            if (loadedFilePath != "" && current_item != "")
            {
                if (configuration.FormationsList.Exists(n => n.Name.Equals(current_item)))
                {
                    error_msg = "Formation already exists.";
                    ImGui.OpenPopup("ErrorPopUp");
                }
                else
                {
                    FormationsData fData = FormationFactory.ConvertBtBFormation(loadedFilePath, current_item);
                    if (fData != null)
                    {
                        configuration.FormationsList.Add(fData);
                        FormationFactory.CheckMissingCIDs(loadedFilePath, current_item, configuration);
                        configuration.Save();
                    }
                }
            }
        }



        if (ImGui.BeginPopupModal("ErrorPopUp"))
        {
            ImGui.Text(error_msg);
            if (ImGui.Button("Okay"))
                ImGui.CloseCurrentPopup();
            ImGui.EndPopup();
        }

    }
}
