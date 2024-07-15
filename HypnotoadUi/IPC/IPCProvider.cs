using Dalamud.Plugin.Ipc;

namespace HypnotoadUi.IPC
{
    public static class IPCProvider
    { 
        /// <summary>
        /// Gfx Low Action
        /// </summary>

        public static ICallGateSubscriber<bool, object> SetGfxLow;
        public static void SetGfxLowAction(bool low) => SetGfxLow.InvokeAction(low);

        /// <summary>
        /// Send Chat
        /// </summary>

        public static ICallGateSubscriber<string, object> SendChat;
        public static void SendChatAction(string msg) => SendChat.InvokeAction(msg);

        /// <summary>
        /// PartyInvite (string)"Name;HomeWorldId"
        /// </summary>

        public static ICallGateSubscriber<string, object> PartyInvite;
        public static void PartyInviteAction(string message) => PartyInvite.InvokeAction(message);

        /// <summary>
        /// PartyInvite Accept
        /// </summary>

        public static ICallGateSubscriber<object> PartyInviteAccept;
        public static void PartyInviteAcceptAction() => PartyInviteAccept.InvokeAction();

        /// <summary>
        /// Set Party Lead
        /// </summary>

        public static ICallGateSubscriber<string, object> PartySetLead;
        public static void PartySetLeadAction(string charname) => PartySetLead.InvokeAction(charname);

        /// <summary>
        /// Party Enter House
        /// </summary>

        public static ICallGateSubscriber<object> PartyEnterHouse;
        public static void PartyEnterHouseAction() => PartyEnterHouse.InvokeAction();

        /// <summary>
        /// Party Enter House
        /// </summary>

        public static ICallGateSubscriber<bool, object> PartyTeleport;
        public static void PartyTeleportAction(bool showMenu) => PartyTeleport.InvokeAction(showMenu);

        /// <summary>
        /// Party Follow
        /// </summary>

        public static ICallGateSubscriber<string, object> PartyFollow;
        public static void PartyFollowAction(string msg) => PartyFollow.InvokeAction(msg);

        /// <summary>
        /// Party UnFollow
        /// </summary>

        public static ICallGateSubscriber<object> PartyUnFollow;
        public static void PartyUnFollowAction() => PartyUnFollow.InvokeAction();


        /// <summary>
        /// MoveTo
        /// </summary>

        public static ICallGateSubscriber<string, object> MoveTo;
        public static void MoveToAction(string msg) => MoveTo.InvokeAction(msg);

        /// <summary>
        /// MoveStop
        /// </summary>

        public static ICallGateSubscriber<object> MoveStop;
        public static void MoveStopAction() => MoveStop.InvokeAction();

        /// <summary>
        /// CharacterLogout
        /// </summary>

        public static ICallGateSubscriber<object> CharacterLogout;
        public static void CharacterLogoutAction() => CharacterLogout.InvokeAction();

        /// <summary>
        /// GameShutdown
        /// </summary>

        public static ICallGateSubscriber<object> GameShutdown;
        public static void GameShutdownAction() => GameShutdown.InvokeAction();

        public static void Initialize()
        {
            SetGfxLow = Api.PluginInterface.GetIpcSubscriber<bool, object>("HypnoToad.SetGfxLow");
            SetGfxLow.Subscribe(SetGfxLowAction);

            SendChat = Api.PluginInterface.GetIpcSubscriber<string, object>("HypnoToad.SendChat");
            SendChat.Subscribe(SendChatAction);

            PartyInvite = Api.PluginInterface.GetIpcSubscriber<string, object>("HypnoToad.PartyInvite");
            PartyInvite.Subscribe(PartyInviteAction);

            PartyInviteAccept = Api.PluginInterface.GetIpcSubscriber<object>("HypnoToad.PartyInviteAccept");
            PartyInviteAccept.Subscribe(PartyInviteAcceptAction);

            PartySetLead = Api.PluginInterface.GetIpcSubscriber<string, object>("HypnoToad.PartySetLead");
            PartySetLead.Subscribe(PartySetLeadAction);

            PartyEnterHouse = Api.PluginInterface.GetIpcSubscriber<object>("HypnoToad.PartyEnterHouse");
            PartyEnterHouse.Subscribe(PartyEnterHouseAction);

            PartyTeleport = Api.PluginInterface.GetIpcSubscriber<bool, object>("HypnoToad.PartyTeleport");
            PartyTeleport.Subscribe(PartyTeleportAction);

            PartyFollow = Api.PluginInterface.GetIpcSubscriber<string, object>("HypnoToad.PartyFollow");
            PartyFollow.Subscribe(PartyFollowAction);

            PartyUnFollow = Api.PluginInterface.GetIpcSubscriber<object>("HypnoToad.PartyUnFollow");
            PartyUnFollow.Subscribe(PartyUnFollowAction);

            MoveTo = Api.PluginInterface.GetIpcSubscriber<string, object>("HypnoToad.MoveTo");
            MoveTo.Subscribe(MoveToAction);

            MoveStop = Api.PluginInterface.GetIpcSubscriber<object>("HypnoToad.MoveStop");
            MoveStop.Subscribe(MoveStopAction);

            CharacterLogout = Api.PluginInterface.GetIpcSubscriber<object>("HypnoToad.CharacterLogout");
            CharacterLogout.Subscribe(CharacterLogoutAction);

            GameShutdown = Api.PluginInterface.GetIpcSubscriber<object>("HypnoToad.GameShutdown");
            GameShutdown.Subscribe(GameShutdownAction);
        }

        public static void Dispose()
        {
            SetGfxLow?.Unsubscribe(SetGfxLowAction);
            SendChat?.Unsubscribe(SendChatAction);
            PartyInvite?.Unsubscribe(PartyInviteAction);
            PartyInviteAccept?.Unsubscribe(PartyInviteAcceptAction);
            PartySetLead?.Unsubscribe(PartySetLeadAction);
            PartyEnterHouse?.Unsubscribe(PartyEnterHouseAction);
            PartyTeleport?.Unsubscribe(PartyTeleportAction);
            PartyFollow?.Unsubscribe(PartyFollowAction);
            PartyUnFollow?.Unsubscribe(PartyUnFollowAction);
            MoveTo?.Unsubscribe(MoveToAction);
            MoveStop?.Unsubscribe(MoveStopAction);
            CharacterLogout?.Unsubscribe(CharacterLogoutAction);
            GameShutdown?.Unsubscribe(GameShutdownAction);
        }
    }
}
