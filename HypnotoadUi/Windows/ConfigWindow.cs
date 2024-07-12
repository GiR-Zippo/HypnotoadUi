using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
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
    }
}
