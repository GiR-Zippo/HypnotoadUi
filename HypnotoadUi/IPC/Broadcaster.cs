using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using HypnotoadUi.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TinyIpc.Messaging;

namespace HypnotoadUi.IPC
{
    [Serializable]
    public class BroadcastMessage
    {
        public ushort msgType { get; init; } = (ushort)MessageType.None;
        public ulong LocalContentId { get; init; } = 0;
        public List<string> message { get; init; } = new List<string>();
    }

    public static class Broadcaster
    {
        //Broadcaster
        private static readonly TinyMessageBus MessagebusSend = new("DalamudBroadcaster.HypnoToadUi");
        private static readonly TinyMessageBus MessagebusReceive = new("DalamudBroadcaster.HypnoToadUi");

        private static HypnotoadUi plugin { get; set; } = null;

        public static IClientState clientState;

        public static void Initialize(HypnotoadUi pluginmain)
        {
            plugin = pluginmain;
            //Init the messagebus
            MessagebusReceive.MessageReceived += (sender, e) => MessageReceived((byte[])e.Message);
        }

        //Messagebus Actions
        private static unsafe void MessageReceived(byte[] message)
        {
            Api.Framework.RunOnTick(delegate
            {
                var localPlayer = Api.ClientState;
                BroadcastMessage msg = JsonConvert.DeserializeObject<BroadcastMessage>(Encoding.UTF8.GetString((byte[])message));
                if (msg is null) { return; }
                if (localPlayer is null) { return; }

                switch ((MessageType)msg.msgType)
                {
                    case MessageType.BCAdd:
                        LocalPlayerCollector.Add(msg.LocalContentId, msg.message[0], Convert.ToUInt32(msg.message[1]), Convert.ToInt32(msg.message[2]));
                        break;
                    case MessageType.BCRemove:
                        LocalPlayerCollector.Remove(msg.LocalContentId, msg.message[0]);
                        break;
                    case MessageType.BCEnabled:
                        if (localPlayer.LocalContentId == Convert.ToUInt64(msg.message[0]))
                            LocalPlayerCollector.BroadCastEnabled(Convert.ToBoolean(msg.message[1]));
                        break;
                }

                //Check if we listen to the broadcast
                var p = LocalPlayerCollector.localPlayers.FirstOrDefault(n => n.LocalContentId == localPlayer.LocalContentId && n.BroadCastEnabled);
                if (p == default)
                    return;

                switch ((MessageType)msg.msgType)
                {
                    case MessageType.FormationData:
                        if (localPlayer.LocalContentId == msg.LocalContentId)
                            break;
                        if (Convert.ToUInt64(msg.message[0]) == Api.ClientState.LocalContentId)
                        {
                            string[] temp = msg.message[1].Substring(1, msg.message[1].Length - 2).Split(',');
                            float x = float.Parse(temp[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                            float y = float.Parse(temp[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                            float z = float.Parse(temp[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                            float w = float.Parse(msg.message[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                            IPCProvider.MoveToAction(x,y,z, w);
                        }
                        break;
                    case MessageType.ClientLogout:
                        if (Convert.ToUInt64(msg.message[0]) == Api.ClientState.LocalContentId)
                            IPCProvider.CharacterLogoutAction();
                        break;
                    case MessageType.GameShutdown:
                        if (Convert.ToUInt64(msg.message[0]) == Api.ClientState.LocalContentId)
                            IPCProvider.GameShutdownAction();
                        break;
                    case MessageType.FormationStop:
                        IPCProvider.MoveStopAction();
                        break;
                    case MessageType.SetGfx:
                        if (localPlayer.LocalContentId == msg.LocalContentId)
                            break;
                        IPCProvider.SetGfxLowAction(Convert.ToBoolean(msg.message[0]));
                        break;
                    case MessageType.Chat:
                        if (Convert.ToBoolean(msg.message[0]))
                            Chat.HandleBroadcastCommands(msg.message);
                        else
                        {
                            if (localPlayer.LocalContentId == msg.LocalContentId)
                                break;
                            Chat.HandleBroadcastCommands(msg.message);
                        }
                        break;
                    case MessageType.PartyInviteAccept:
                        if (localPlayer.LocalContentId == msg.LocalContentId)
                            break;
                        IPCProvider.PartyInviteAcceptAction();
                        break;
                    case MessageType.PartyPromote:
                        if (!GroupManager.Instance()->GetGroup()->IsEntityIdPartyLeader(localPlayer.LocalPlayer.EntityId))
                            break;
                        IPCProvider.PartySetLeadAction(msg.message[0]);
                        break;
                    case MessageType.PartyLeave:
                        IPCProvider.PartyLeaveAction();
                        break;
                    case MessageType.PartyEnterHouse:
                        IPCProvider.PartyEnterHouseAction();
                        break;
                    case MessageType.PartyTeleport:
                        if (localPlayer.LocalContentId == msg.LocalContentId)
                            break;
                        IPCProvider.PartyTeleportAction(false);
                        break;
                    case MessageType.PartyFollow:
                        if (localPlayer.LocalContentId == msg.LocalContentId)
                            break;
                        if (Convert.ToBoolean(msg.message[0]))
                            IPCProvider.PartyFollowAction(Convert.ToUInt64(msg.message[1]), msg.message[2], Convert.ToUInt16(msg.message[3]));
                        else
                            IPCProvider.PartyUnFollowAction();
                        break;
                    case MessageType.CamHack:
                        if (localPlayer.LocalContentId == msg.LocalContentId)
                            break;
                        if (Convert.ToBoolean(msg.message[0]))
                            CamHack.Instance.Enable();
                        else
                            CamHack.Instance.Disable();
                        break;
                }
            }, default(TimeSpan), 0, default(CancellationToken));


        }

        public static void SendMessage(ulong localContentId, MessageType type, List<string> msg)
        {
            var x = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new BroadcastMessage
            {
                LocalContentId = localContentId,
                msgType = (ushort)type,
                message = msg
            }));
            MessagebusSend.PublishAsync(x).Wait();
        }

        public static void Dispose()
        {
            MessagebusReceive.MessageReceived -= (sender, e) => MessageReceived((byte[])e.Message);
            MessagebusReceive.Dispose();
            MessagebusSend.Dispose();
        }
    }
}
