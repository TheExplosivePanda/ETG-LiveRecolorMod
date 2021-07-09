using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.IO;

namespace LiveRecolor
{
    //the actual magic
    class MaterialManager : MonoBehaviour
    {

        //short explanation of how the color swap work. basically, if its on, the shader takes the red value of a pixel, goes to a matching pixel on a saved texture,
        //and uses that pixel instead. here is an article that explains generally in more details https://gamedevelopment.tutsplus.com/tutorials/how-to-use-a-shader-to-dynamically-swap-a-sprites-colors--cms-25129'
        //unlike that article tho, insteaad of a 1x256 palette gungeon uses 16x16, and trasparent colors dont resault in no swap, but rather in black pixels which is sad
        //conversion from a 0-255 red value to texture coordinates can be seen in the update colors method

        public PlayerController Player;
        //the texture out shader uses to swap
        public Texture2D paletteTexture;
        public int EmissiveBodyIndex;
        public PlayerColorData colorData;
        public static string ShaderName = "Brave/LitCutoutUber_ColorEmissive";
        void Start()
        {
    
            Player = GetComponent<PlayerController>();
            SetupPlayerForTint(Player);
            UpdateColors();
            SetEmissiveColorIndex(colorData.defaultEmissiveIndex);        
        }
        public void RestoreDefaults()
        {
            for (int i = 0; i < colorData.bodyParts.Count; i++)
            {
                colorData.bodyParts[i].color = colorData.bodyParts[i].OriginalColor;
            }
            SetupPlayerForTint(Player);
            UpdateColors();
        }
        public void RestoreShader()
        {
            Player.sprite.renderer.material.shader = ShaderCache.Acquire(ShaderName);
        }
        public Texture2D SetupPlayerForTint(PlayerController player)
        {

            //sets up the shader and material from scratch
            Player = player;
            Material mat = new Material(ShaderCache.Acquire(ShaderName));
            mat.SetTexture("_MainTexture", player.sprite.renderer.material.GetTexture("_MainTex"));//not sure this is necessary

            //sets our texture as the swap texture, very important unless you want to get the texture from the material every time which is also an option
            Texture2D tex = ResourceExtractor.GetTextureFromResource("LiveRecolor/Resources/blankTex.png");
            mat.SetTexture("_PaletteTex", tex);

            mat.SetColor("_EmissiveColor", colorData.bodyParts[colorData.defaultEmissiveIndex].color);//the color thatll glow

            mat.SetFloat("_EmissiveColorPower", colorData.defaultEmissiveColorPower);

            mat.SetFloat("_EmissivePower", colorData.defaultEmissivePower);

            mat.SetFloat("_EmissiveThresholdSensitivity", colorData.defaultEmissiveSensetivity);

            

            
            
            
            //ignore
            mat.SetColor("_OverrideColor", new Color(1, 1, 1, 0));
            mat.SetColor("_OutlineColor", new Color(0, 0, 0, 1));
            mat.SetFloat("_Perpendicular", 1f);
            mat.SetFloat("_Cutoff", 0.5f);
            mat.SetFloat("_ValueMinimum", 0.7f);
            mat.SetFloat("_ValueMaximum", 0.97f);

            //enables/disables amissive based on default
            if (colorData.isDefaultEmissive)
            {
                mat.EnableKeyword("EMISSIVE_ON");
                mat.DisableKeyword("EMISSIVE_OFF");
            }
            else
            {
                mat.DisableKeyword("EMISSIVE_ON");
                mat.EnableKeyword("EMISSIVE_OFF");
            }

            mat.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.EnableKeyword("BRIGHTNESS_CLAMP_OFF");

            //enables recoloring
            mat.DisableKeyword("PALETTE_OFF");
            mat.EnableKeyword("PALETTE_ON");
            mat.DisableKeyword("TINTING_ON");
            mat.EnableKeyword("TINTING_OFF");

            mat.DisableKeyword("DIRECTIONAL");


            player.sprite.renderer.material = mat;
            paletteTexture = tex;
            return tex;
        }

        public void ToggleTint(bool on)
        {
            //same as in material setup, except here it depends on the player toggling it
            if (Player == null) { return; }
            if (on)
            {
                Player.sprite.renderer.material.DisableKeyword("PALETTE_OFF");
                Player.sprite.renderer.material.EnableKeyword("PALETTE_ON");
            }
            else
            {
                Player.sprite.renderer.material.EnableKeyword("PALETTE_OFF");
                Player.sprite.renderer.material.DisableKeyword("PALETTE_ON");
            }
        }

        //same as tint toggle but for emission
        public void ToggleEmission(bool on)
        {
            if (Player == null) { return; }
            if (on)
            {
                Player.sprite.renderer.material.DisableKeyword("EMISSIVE_OFF");
                Player.sprite.renderer.material.EnableKeyword("EMISSIVE_ON");
            }
            else
            {
                Player.sprite.renderer.material.EnableKeyword("EMISSIVE_OFF");
                Player.sprite.renderer.material.DisableKeyword("EMISSIVE_ON");
            }
        }

        public void SetEmissiveColor(Color color)
        {
            if (Player == null) { return; }
            Player.sprite.renderer.material.SetColor("_EmissiveColor", color);
        }
        public void SetEmissiveSensitivity(float sensitivity)
        {
            if (Player == null) { return; }
            Player.sprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", sensitivity);
        }
        public void SetEmissiveColorPower(float colorPower)
        {
            if (Player == null) { return; }
            Player.sprite.renderer.material.SetFloat("_EmissiveColorPower", colorPower);
        }
        public void SetEmissivePower(float power)
        {
            if (Player == null) { return; }
            Player.sprite.renderer.material.SetFloat("_EmissivePower", power);
        }
        //colors are usually set using their index in the player's color data
        public void SetEmissiveColorIndex(int index)
        {
            EmissiveBodyIndex = index;
            UpdateColors();
        }
        public bool IsEmissive()
        {
            Material mat = Player.sprite.renderer.material;
            return !mat.IsKeywordEnabled("EMISSIVE_OFF") && mat.IsKeywordEnabled("EMISSIVE_ON");
        }
        public bool IsTinted()
        {
            Material mat = Player.sprite.renderer.material;
            return !mat.IsKeywordEnabled("PALETTE_OFF") && mat.IsKeywordEnabled("PALETTE_ON");
        }
        public void UpdateColors()
        {
            //for each color in color data, sets the according pixel in the swap texture to the right value
            //techniaclly using GetColors32 is faster but its not significant increase and it makes stuff more confusing
            for (int i = 0; i < colorData.bodyParts.Count; i++)
            {
                byte idx = colorData.bodyParts[i].OriginalColor.r;
                byte x = (byte)(idx / 16);
                byte y = (byte)(idx % 16);
                paletteTexture.SetPixel(x, y, colorData.bodyParts[i].color);
            }

            //since the emission is determined by body part rather than a direct color, it updates the current body part color each time
            //also checks if the player has color swapping on, so it knows what color to make glow
            if (IsTinted())
            {
                SetEmissiveColor(colorData.bodyParts[EmissiveBodyIndex].color);
            }
            else
            {
               SetEmissiveColor(colorData.bodyParts[EmissiveBodyIndex].OriginalColor);
            }

            //apply uploads textures to the gpu meaning it actually takes effect
            paletteTexture.Apply();

        }
    }
}
