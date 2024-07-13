﻿using Dalamud.Game.ClientState.Objects.SubKinds;
using HypnotoadUi.IPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace HypnotoadUi.Formations
{
    public class FormationsData
    {
        public string Name;
        public Dictionary<long, FormationEntry> formationEntry = new Dictionary<long, FormationEntry>();
    }

    public class FormationEntry
    {
        public int Index { get; set; } = 0;

        public long CID { get; set; } = 0;

        public Vector3 RelativePosition { get; set; } = new Vector3();
        public float RelativeRotation { get; set; } = 0.0f;
    }

    public static class FormationFactory
    {
        public static void LoadFormation(FormationsData formation)
        {
            if (Api.ClientState.LocalPlayer == null)
                return;

            foreach (FormationEntry entry in formation.formationEntry.Values)
            {
                Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.FormationData, new List<string>()
                {
                    entry.CID.ToString(),
                    FormationCalculation.RelativeToAbsolute(entry, Api.ClientState.LocalPlayer).Key.ToString(),
                    (FormationCalculation.RelativeToAbsolute(entry, Api.ClientState.LocalPlayer).Value + 3.1415927f).ToString()
                });
            }
        }

        public static void StopFormation()
        {
            IPCProvider.MoveStopAction();
            Broadcaster.SendMessage(Api.ClientState.LocalPlayer.GameObjectId, MessageType.FormationStop, new List<string>());
        }

        public static List<string> ReadBtBFormationNames(string filename)
        {
            List<string> outdata = new List<string>();
            dynamic jData = JsonConvert.DeserializeObject(File.ReadAllText(filename));
            var f = jData["SavedFormationList"];
            foreach (var name in f)
                outdata.Add(Convert.ToString(name["11"]));

            return outdata;
        }

        public static FormationsData ConvertBtBFormation(string filename, string item)
        {
            string data = File.ReadAllText(filename);
            dynamic jData = JsonConvert.DeserializeObject(data);
            var f = jData["SavedFormationList"];
            foreach (var name in f)
            {
                if (Convert.ToString(name["11"]) == item)
                {
                    string da = JsonConvert.SerializeObject(name);
                    da = da.Replace("\"11\":", "\"Name\":");
                    da = da.Replace("\"22\":", "\"FormationEntry\":");
                    da = da.Replace("\"i\":", "\"Index\":");
                    da = da.Replace("\"Pepsi1\":", "\"CID\":");
                    da = da.Replace("\"Pepsi2\":", "\"RelativePosition\":");
                    da = da.Replace("\"Pepsi3\":", "\"RelativeRotation\":");
                    return JsonConvert.DeserializeObject<FormationsData>(da);
                }
            }
            return null;
        }

        public static void CheckMissingCIDs(string filename, string item, Configuration config)
        {
            dynamic jData = JsonConvert.DeserializeObject(File.ReadAllText(filename));
            string txt = JsonConvert.SerializeObject(jData["CidToNameWorld"]);
            txt = txt.Replace("\"Item1\":", "\"Key\":");
            txt = txt.Replace("\"Item2\":", "\"Value\":");
            var output = JsonConvert.DeserializeObject<Dictionary<long, KeyValuePair<string, string>>>(txt);

            foreach (var cid in output)
                config.ContentIDLookup.TryAdd(cid.Key, cid.Value);
        }
    }
    public static class FormationCalculation
    {
        internal static KeyValuePair<Vector3, float> RelativeToAbsolute(FormationEntry relativePosition, IPlayerCharacter absolutTarget)
        {
            KeyValuePair<Vector3, float> absolutePosition = FormationCalculation.RelativeToAbsolute(new KeyValuePair<Vector3, float>(relativePosition.RelativePosition, relativePosition.RelativeRotation), 
                                                                                                    new KeyValuePair<Vector3, float>(absolutTarget.Position,            absolutTarget.Rotation));
            return new KeyValuePair<Vector3, float>(absolutePosition.Key, absolutePosition.Value);
        }

        internal static KeyValuePair<Vector3, float> RelativeToAbsolute(KeyValuePair<Vector3, float> relativePosition, KeyValuePair<Vector3, float> absolutPosition)
        {
            //Create the roatation matrix
            Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationY(absolutPosition.Value + MathF.PI);
            //Transform relative pos to our absolute and add the absolute position
            //add the the rel rotation to our absolute
            //Will turned later on by +pi to correct the viewing direction
            return new KeyValuePair<Vector3, float>(Vector3.Transform(relativePosition.Key, rotationMatrix) + absolutPosition.Key, relativePosition.Value + absolutPosition.Value);
        }
    }
}