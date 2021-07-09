using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace LiveRecolor
{
    //note , naming for this project has been inconsistent so you might see odd method names, or see "colors" swapped with "Bodypars" and vice versa
    public class Module : ETGModule
    {

        
        public static readonly string MOD_NAME = "Live Recolor Mod";
        public static readonly string VERSION = "1.0.0";
        public static readonly string TEXT_COLOR = "#3366ff";
        
        // a dictionary of player names and their color data. allows fast access to color data based on player name
        // names should look like "Player + ShortName". when the mod loads cc's it adds the "Player" bit to the dictionary key on its own.
        public static Dictionary<string, PlayerColorData> PlayerColorDataDictionary = new Dictionary<string, PlayerColorData>();
        //um, lets not ask questions please. i never learned how to count
        public static int CutOffLength = "(Clone)".Length;
        public static int PlayerLength = "Player".Length;
        public override void Start()
        {
            FakePrefabHooks.Init();

            ItemBuilder.Init();

            Tools.Init();
            //adds the base game chars to the charater dictionary
            StaticCharacterDefinitions.DefineAndAddBaseCharacters();
            //reads recolordata.txt and characterdata.txt from all cc's and adds them to the dictionary if exists
            FileReader.Init();
            //hooks that makes mod possible
            Hook hook = new Hook(
               typeof(PlayerController).GetProperty("LocalShaderName", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
               typeof(Module).GetMethod("LocalShaderNameGetHook")
             );

            // text go print
            Log($"{MOD_NAME} v{VERSION} Initialized.", TEXT_COLOR);

            Log("Characters that should be avialable for recolors:", "#3366ff");
            foreach (string key in PlayerColorDataDictionary.Keys)
            {
                Log("LR:"+"    " + key.Substring(PlayerLength), "#3399ff");
            }
        }
        
        public static string LocalShaderNameGetHook(Func<PlayerController, string> orig, PlayerController self)
        {
            if (!GameOptions.SupportsStencil)
            {
                return "Brave/PlayerShaderNoStencil";
            }
            //cuts off the "(Clone)" bit 
            string cutName = self.name.Substring(0, self.name.Length - CutOffLength);
            //attempts to find our char, if exists applies the gui handler and material manager
            if (PlayerColorDataDictionary.ContainsKey(cutName))
            {
                SetUpPlayerRecolor(self, cutName);
                return MaterialManager.ShaderName;
            }
            return orig(self);
        }
        static void SetUpPlayerRecolor(PlayerController player, string key)
        {
            //a tad jank, but basically applies out components if necessary as well as the color data
            GUIhandler handler = player.gameObject.GetOrAddComponent<GUIhandler>();
            MaterialManager manager = player.gameObject.GetOrAddComponent<MaterialManager>();
            manager.colorData = PlayerColorDataDictionary[key];
            handler.enabled = true;
            manager.enabled = true;
        }
        public static void Log(string text, string color = "FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
        public override void Exit(){}
        public override void Init(){ }

    }
}
