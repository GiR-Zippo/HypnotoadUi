using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using HypnotoadUi.IPC;
using Hypostasis.Game.Structures;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HypnotoadUi.Functions;

public static unsafe class CamHack
{
    private static bool Initialized { get; set; } = false;
    private static void GetCameraPositionDetour(GameCamera* camera, GameObject* target, Vector3* position, Bool swapPerson)
    {
        position->Y += 3000f;
    }

    public static float? GetDefaultLookAtHeightOffset()
    {
        var worldCamera = Common.CameraManager->worldCamera;
        if (worldCamera == null || DalamudApi.ClientState.LocalPlayer is not { } p) return 0;

        var prev = worldCamera->lookAtHeightOffset;
        if (!GameCamera.updateLookAtHeightOffset.Original(worldCamera, (GameObject*)p.Address, false)) return null;

        var ret = worldCamera->lookAtHeightOffset;
        worldCamera->lookAtHeightOffset = prev;
        return ret;
    }

    public static void Enable(HypnotoadUi plugin, IDalamudPluginInterface pluginInterface)
    {
        Hypostasis.Hypostasis.Initialize(plugin, pluginInterface);

        if (Common.CameraManager == null || !Common.IsValid(Common.CameraManager->worldCamera) || !Common.IsValid(Common.InputData))
            throw new ApplicationException("Failed to validate core structures!");

        Initialized = true;

        var vtbl = Common.CameraManager->worldCamera->VTable;
        if (vtbl.getCameraPosition.Hook == null)
            vtbl.getCameraPosition.CreateHook(GetCameraPositionDetour);
        else
            vtbl.getCameraPosition.Hook.Enable();
    }

    public static void EnableOthers()
    {
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.CamHack, new List<string>() { (true).ToString() });
    }

    public static void Disable()
    {
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.CamHack, new List<string>() { (false).ToString() });
        if (Initialized)
        {
            var vtbl = Common.CameraManager->worldCamera->VTable;
            if (vtbl.getCameraPosition.Hook == null)
                return;
            vtbl.getCameraPosition.Hook.Disable();
        }
    }

    public static void Dispose()
    {
        if (!Initialized)
            return;
        var vtbl = Common.CameraManager->worldCamera->VTable;
        if (vtbl.getCameraPosition.Hook == null)
            return;
        vtbl.getCameraPosition.Hook.Dispose();       
    }
}