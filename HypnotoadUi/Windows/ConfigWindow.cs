using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using HypnotoadUi.Formations;
using HypnotoadUi.IPC;
using ImGuiNET;

namespace HypnotoadUi.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;

    public ConfigWindow(HypnotoadUi plugin) : base(
        "Config Window",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(350, 300);
        this.SizeCondition = ImGuiCond.Always;
        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }


    public override void Draw()
    {
        if (ImGui.Button("test"))
        {
            FormationEntry fE = new FormationEntry()
            {
                RelativePosition = new Vector3() { X = 0.027093465f, Y = 9.536743E-07f, Z = 1.6012704f },
                RelativeRotation = 0.06902254f
            };

            IPCProvider.MoveToAction(FormationCalculation.RelativeToAbsolute(fE, Api.ClientState.LocalPlayer).Key.ToString() + ";" +
                    (FormationCalculation.RelativeToAbsolute(fE, Api.ClientState.LocalPlayer).Value + 3.1415927f).ToString());


        }
    }
}
