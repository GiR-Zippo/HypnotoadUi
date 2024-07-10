using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyIpc.Messaging;

namespace HypnotoadUi.IPC
{
    public class BroadcastMessage
    {
        public MessageType msgType { get; init; } = MessageType.None;
        public ulong senderGoId { get; init; } = 0;
        public List<string> message { get; init; } = new List<string>();
    }

    public static class Broadcaster
    {
        //Broadcaster
        private static readonly TinyMessageBus MessagebusSend = new("DalamudBroadcaster.HypnoToadUi");
        private static readonly TinyMessageBus MessagebusReceive = new("DalamudBroadcaster.HypnoToadUi");

        public static IClientState clientState;

        public static void Initialize()
        {
            //Init the messagebus
            MessagebusReceive.MessageReceived += (sender, e) => MessageReceived((byte[])e.Message);
        }

        //Messagebus Actions
        private static unsafe void MessageReceived(byte[] message)
        {
            Api.Framework.RunOnTick(delegate
            {
                var localPlayer = Api.ClientState?.LocalPlayer;

                BroadcastMessage msg = JsonConvert.DeserializeObject<BroadcastMessage>(Encoding.UTF8.GetString((byte[])message));
                if (msg is null) { return; }
                if (localPlayer is null) { return; }

                switch (msg.msgType)
                {
                    case MessageType.BCAdd:
                        LocalPlayerCollector.Add(msg.senderGoId, msg.message[0], Convert.ToUInt32(msg.message[1]));
                        break;
                    case MessageType.BCRemove:
                        LocalPlayerCollector.Remove(msg.senderGoId, msg.message[0]);
                        break;
                    case MessageType.SetGfx:
                        if (localPlayer.GameObjectId == msg.senderGoId)
                            break;
                        IPCProvider.SetGfxLowAction(Convert.ToBoolean(msg.message[0]));
                        break;
                    case MessageType.Chat:
                        IPCProvider.SendChatAction(msg.message[0]);
                        break;
                    case MessageType.PartyInviteAccept:
                        if (localPlayer.GameObjectId == msg.senderGoId)
                            break;
                        IPCProvider.PartyInviteAcceptAction();
                        break;
                    case MessageType.PartyPromote:
                        if (!GroupManager.Instance()->GetGroup()->IsEntityIdPartyLeader(localPlayer.EntityId))
                            break;
                        IPCProvider.PartySetLeadAction(msg.message[0]);
                        break;
                    case MessageType.PartyEnterHouse:
                        IPCProvider.PartyEnterHouseAction();
                        break;
                    case MessageType.PartyTeleport:
                        if (localPlayer.GameObjectId == msg.senderGoId)
                            break;
                        IPCProvider.PartyTeleportAction(false);
                        break;
                    case MessageType.PartyFollow:
                        if (localPlayer.GameObjectId == msg.senderGoId)
                            break;
                        if (Convert.ToBoolean(msg.message[0]))
                            IPCProvider.PartyFollowAction(msg.message[1] + ";" + Convert.ToUInt16(msg.message[2]).ToString());
                        else
                            IPCProvider.PartyUnFollowAction();
                        break;
                }
            }, default(TimeSpan), 0, default(CancellationToken));


        }

        public static void SendMessage(ulong selfgoId, MessageType type, List<string> msg)
        {
            MessagebusSend.PublishAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new BroadcastMessage{
                senderGoId = selfgoId,
                msgType = type,
                message = msg
            })));
        }

        public static void Dispose()
        {
            MessagebusReceive.MessageReceived -= (sender, e) => MessageReceived((byte[])e.Message);
            MessagebusReceive.Dispose();
            MessagebusSend.Dispose();
        }
    }
}
