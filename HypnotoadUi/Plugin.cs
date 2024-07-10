using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HypnotoadUi.Windows;
using HypnotoadUi.IPC;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace HypnotoadUi;
public class HypnotoadUi : IDalamudPlugin
{
    public string Name => "HypnotoadUi";
    private const string CommandName = "/ht";

    private static IDalamudPluginInterface PluginInterface { get; set; }
    private static ICommandManager CommandManager { get; set; }

    public Configuration Configuration { get; init; }
    public WindowSystem WindowSystem = new("HypnotoadUi");

    public Api api { get; set; }

    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    private ulong GoId { get; set; } = 0;
    private string PlayerName { get; set; } = "";

    public HypnotoadUi(IDalamudPluginInterface pluginInterface, IChatGui chatGui, IDataManager data, ICommandManager commandManager, IClientState clientState, IPartyList partyList)
    {
        api = pluginInterface.Create<Api>();

        PluginInterface = pluginInterface;
        CommandManager = commandManager;

        this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.Configuration.Initialize(PluginInterface);

        //Init the IPC - Receiver
        IPCProvider.Initialize();

        Broadcaster.Initialize();

        Api.ClientState.Login += OnLogin;
        Api.ClientState.Logout += OnLogout;

        //Build and register the Windows
        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += UiBuilder_OpenConfigUi;
        PluginInterface.UiBuilder.OpenMainUi += UiBuilder_OpenMainUi;

        MainWindow.IsOpen = true;
        OnLogin();
    }

    private void UiBuilder_OpenMainUi()
    {
        MainWindow.IsOpen = true;
    }

    private void UiBuilder_OpenConfigUi()
    {
        ConfigWindow.IsOpen = true;
    }

    public void Dispose()
    {
        Api.ClientState.Login -= OnLogin;
        Api.ClientState.Logout -= OnLogout;

        OnLogout();

        Broadcaster.Dispose();
        IPCProvider.Dispose();

        this.WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        if (Configuration != null)
            Configuration.Save();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        var regex = Regex.Match(args, "^(\\w+) ?(.*)");
        var subcommand = regex.Success && regex.Groups.Count > 1 ? regex.Groups[1].Value : string.Empty;

        switch (subcommand.ToLower())
        {
            case "br":
            case "broadcast":
            {
                if (regex.Groups.Count < 2 || string.IsNullOrEmpty(regex.Groups[2].Value))
                {
                    Api.ChatGui.Print("[Broadcast] missing parameter");
                    return;
                }
                var arg = regex.Groups[2].Value;
                if (Api.ClientState.LocalPlayer == null)
                    return;

                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.Chat, new List<string>() { arg });
            }
            break;
        }
    }

    private void OnLogin()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        if (!Api.ClientState.LocalPlayer.IsValid())
            return;
                
        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.BCAdd, new System.Collections.Generic.List<string>()
        {
            Api.ClientState.LocalPlayer.Name.TextValue,
            Api.ClientState.LocalPlayer.HomeWorld.Id.ToString()
        });

        GoId = Api.ClientState.LocalPlayer.GameObjectId;
        PlayerName = Api.ClientState.LocalPlayer.Name.TextValue;
    }

    private void OnLogout()
    {
        if (Api.ClientState.LocalPlayer == null)
        {
            if (GoId == 0)
                return;
            Broadcaster.SendMessage(GoId, MessageType.BCRemove, new System.Collections.Generic.List<string>()
            {
                PlayerName
            });
            GoId = 0;
            PlayerName = "";
            return;
        }
        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.BCRemove, new System.Collections.Generic.List<string>()
        {
            Api.ClientState.LocalPlayer.Name.TextValue,
            Api.ClientState.LocalPlayer.HomeWorld.Id.ToString()
        });
    }

    private void DrawUI()
    {
        this.WindowSystem.Draw();
    }

    public void ToggleDrawConfigUI()
    {
        ConfigWindow.IsOpen = !ConfigWindow.IsOpen;
    }
}
