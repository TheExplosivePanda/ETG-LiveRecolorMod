using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using Ionic.Zip;

namespace LiveRecolor
{
    static class FileReader
    {
        public static string CharacterDirectory = Path.Combine(ETGMod.ResourcesDirectory, "../CustomCharacterData/");
        public static string CcDataFile = "characterdata.txt";
        public static string ColorDataFile = "recolordata.txt";


        public static void Init()
        {
            LoadPlayersDataDirectories();
            LoadZips();
        }
        private static void LoadZips()
        {
            var zipFiles = Directory.GetFiles(CharacterDirectory, "*.zip", SearchOption.TopDirectoryOnly);
            
            foreach (string zipFilePath in zipFiles)
            {
                string[] lines;
                string name = "";
                PlayerColorData colorData = null;
                string fileName = Path.GetFileName(zipFilePath);
                try
                {
                    using (var zip = ZipFile.Read(zipFilePath))
                    {
                        foreach (var entry in zip)
                        {
                            if (string.Equals(Path.GetFileName(entry.FileName), CcDataFile, StringComparison.OrdinalIgnoreCase))
                            {
                                lines = entry.ReadAllLines();
                                name = GetPlayerName(lines);
                            }
                        }
                        foreach (var entry in zip)
                        {
                            if (string.Equals(Path.GetFileName(entry.FileName), ColorDataFile, StringComparison.OrdinalIgnoreCase))
                            {
                                lines = entry.ReadAllLines();
                                colorData = GetPlayerColorData(lines,name);
                            }
                        }
                        if(!string.IsNullOrEmpty(name) && colorData!= null)
                        {
                            Module.PlayerColorDataDictionary.Add(name, colorData);
                            colorData = null;
                        }
                    }
                }
                catch (Exception e){}
                finally{}
            }
        }
        public static void LoadPlayersDataDirectories()
        {
            string[] directories = Directory.GetDirectories(CharacterDirectory);
            for (int i = 0; i < directories.Length; i++)
            {
                string customCharacterDir = Path.Combine(CharacterDirectory, directories[i]);
                string ccDataFilePath = Path.Combine(customCharacterDir, CcDataFile);
                if (!File.Exists(ccDataFilePath))
                {
                    continue;
                }

                string ColorDataFilePath = Path.Combine(customCharacterDir, ColorDataFile);
                if (!File.Exists(ColorDataFilePath))
                {
                    continue;
                }

                string[] lines = File.ReadAllLines(ccDataFilePath);
                string name = GetPlayerName(lines);
                string[] colorDataLines = File.ReadAllLines(ColorDataFilePath);
                
                PlayerColorData colorData = GetPlayerColorData(colorDataLines,name);
                Module.PlayerColorDataDictionary.Add(name, colorData);
            }
        }
        public static PlayerColorData GetPlayerColorData(string[] lines, string PlayerName)
        {
            List<ColorGroup> bodyParts = new List<ColorGroup>();
            string emissiveBodyPart = "";
            float emissiveSensitivity = 0.1f;
            float emissiveColorPower = 0;
            float emissivePower = 0;
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].ToLower().Trim();
                string lineOrig = lines[i];
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;

                int dividerIndex = lineOrig.IndexOf(':');
                if (dividerIndex < 0) continue;
                
                
                if (lineOrig.StartsWith("emissivepart"))
                {
                    emissiveBodyPart = lineOrig.Substring(dividerIndex + 1).Trim();
                }
                else if (lineOrig.StartsWith("emissivesensitivity"))
                {
                    float.TryParse(lineOrig.Substring(dividerIndex + 1), out emissiveSensitivity);
                }
                else if (lineOrig.StartsWith("emissivepower"))
                {
                    float.TryParse(lineOrig.Substring(dividerIndex + 1), out emissivePower);
                }
                else if (lineOrig.StartsWith("emissivecolorpower"))
                {
                    float.TryParse(lineOrig.Substring(dividerIndex + 1), out emissiveColorPower);
                }
                else
                {

                    string BodyPartName = lineOrig.Substring(0, dividerIndex).Trim();
                    string colorStringRaw = lineOrig.Substring(dividerIndex + 1).Trim();
                    Color32 color = ParseColor(colorStringRaw);
                    if (color.a == 255)
                    {
                        bodyParts.Add(new ColorGroup(color, color, BodyPartName));
                    }
                }
            }
            return new PlayerColorData(bodyParts,emissiveColorPower,emissivePower,emissiveSensitivity, emissiveBodyPart,PlayerName);
        }
        public static Color32 ParseColor(string colordata)
        {
            string data = colordata.Trim();
            if(!(data.Contains('(') && data.Contains(')')))
            {
                return new Color32(0, 0, 0, 0);
            }
            char[] trimChars = { '(', ')' };
            data = data.Trim(trimChars);
            string[] colorValues = data.Split(',');
            if(colorValues.Length != 3)
            {
                return new Color32(0, 0, 0, 0);
            }
            bool validValues = false;
            byte r =0;
            byte g =0;
            byte b =0;
            validValues =                byte.TryParse(colorValues[0], out r);
            validValues = validValues && byte.TryParse(colorValues[1], out g);
            validValues = validValues && byte.TryParse(colorValues[2], out b);
            if(validValues)
            {
                return new Color32(r, g, b, 255);
            }
            return new Color32(0, 0, 0, 0);
        }
        //cc code but with 65% less cc code
        public static string GetPlayerName(string[] lines)
        {
            string name = null;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].ToLower().Trim();
                string lineCaseSensitive = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;
                int dividerIndex = line.IndexOf(':');
                if (dividerIndex < 0) continue;
                string value = lineCaseSensitive.Substring(dividerIndex + 1).Trim();
                if (line.StartsWith("name short:"))
                {
                    name = "Player" + value.Replace(" ", "_");
                    continue;
                }           
            }
            return name;
        }
    }
}
