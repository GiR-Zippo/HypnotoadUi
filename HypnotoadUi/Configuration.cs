using Dalamud.Configuration;
using Dalamud.Plugin;
using HypnotoadUi.Misc;
using System;

namespace HypnotoadUi
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool MultiboxingEnabled { get; set; } = false;


        public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private IDalamudPluginInterface pluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            //if mb is enabled
            if (MultiboxingEnabled)
                Multiboxing.RemoveHandle();
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
