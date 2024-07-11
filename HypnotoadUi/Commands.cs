using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using HypnotoadUi.IPC;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HypnotoadUi;

public class Commands : IDisposable
{
    private const string CommandName = "/ht";
    private const string CommandBrName = "/hbr";
    private static ICommandManager CommandManager { get; set; }

    public Commands(ICommandManager manager)
    {
        CommandManager = manager;
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        CommandManager.AddHandler(CommandBrName, new CommandInfo(OnCommandBr)
        {
            HelpMessage = "Broadcast your message to all."
        });
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler(CommandBrName);
        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommandBr(string command, string args)
    {
        if (args.Length <= 0)
            return;

        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.Chat, new List<string>() { args });
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

}
