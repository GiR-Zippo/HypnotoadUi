namespace HypnotoadUi.IPC;

public enum MessageType
{
    None                = 0,
    BCAdd               = 1,
    BCRemove            = 2,
    FormationData       = 3,
    FormationStop       = 4,

    SetGfx              = 10,    //Set <bool> true=low false=normal

    Chat                = 40,

    PartyInviteAccept   = 61,
    PartyPromote        = 62,
    PartyEnterHouse     = 63,
    PartyTeleport       = 64,
    PartyFollow         = 65
}
