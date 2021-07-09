using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gungeon;
using ItemAPI;
using System.Collections;

namespace LiveRecolor
{
    //thx kyle
    class GUIhandler : MonoBehaviour
    {
        static tk2dTextMesh textPrefab = Instantiate<tk2dTextMesh>(BraveResources.Load<GameObject>("textbox").GetComponentInChildren<tk2dTextMesh>());
        GameObject panel;
        UIBodyPartPicker bodyPartPicker;
        ColorPicker redPicker;
        ColorPicker greenPicker;
        ColorPicker bluePicker;
        BooleanPicker tintToggler;
        BooleanPicker glowToggler;
        UIBodyPartPicker emissiveColorSelector;
        FloatPicker emissiveSensitivity;
        FloatPicker emissivePower;
        FloatPicker emissiveColorPower;
        UITextMesh restDefaults;
        UITextMesh restoreShader;
        int activePickerIndex =0;
        UITextMesh[] pickers = new UITextMesh[12];
        MaterialManager matManager;
        

       
        
        void Start()
        {
            //the gui handler intefaces heavily with the material manager
            matManager = GetComponent<MaterialManager>();
            //makes it so out text meshes dont start a new line after a set amount of characters
            textPrefab.wordWrapWidth = int.MaxValue;
            m_player = gameObject.GetComponent<PlayerController>();
            
            if(textPrefab == null)
            {
                textPrefab = Instantiate<tk2dTextMesh>(BraveResources.Load<GameObject>("textbox").GetComponentInChildren<tk2dTextMesh>());  
            }
            //wobbly text tend to break with quick starting and such, so bugs can arise
            textPrefab.supportsWooblyText = false;
            if(!FakePrefab.IsFakePrefab(textPrefab.gameObject))
            {
                FakePrefab.MarkAsFakePrefab(textPrefab.gameObject);
            }
            //basically our gui game object
            panel = new GameObject("panel");
            DontDestroyOnLoad(panel);
            panel.transform.SetParent(m_player.transform,false);
            panel.transform.localPosition = Vector3.zero;

            //a lot of label setup. most of these inteface directly with the material manger except the colors and the first body picker one because i was too lazy to figure it out 
            bodyPartPicker = new UIBodyPartPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, 5, 0), Quaternion.identity), Color.magenta, Color.white);
            bodyPartPicker.GetTransform().SetParent(panel.transform, false);
            bodyPartPicker.SetText("body part:");
            bodyPartPicker.SetBodyParts(matManager.colorData.bodyParts);
            bodyPartPicker.onPressedDirection += OnBodyPickerIndexChanged;
            pickers[0] = bodyPartPicker;
            
            redPicker = new ColorPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, 4, 0), Quaternion.identity), Color.red, Color.white);
            redPicker.GetTransform().SetParent(panel.transform,false);
            redPicker.SetText("red:");
            redPicker.onPressedDirection += OnColorPressed;
            pickers[1] = redPicker;

            greenPicker = new ColorPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, 3, 0), Quaternion.identity), Color.green, Color.white);
            greenPicker.GetTransform().SetParent(panel.transform, false);
            greenPicker.SetText("green:");
            greenPicker.onPressedDirection += OnColorPressed;
            pickers[2] = greenPicker;

            bluePicker = new ColorPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, 2, 0), Quaternion.identity), Color.blue, Color.white);
            bluePicker.GetTransform().SetParent(panel.transform, false);
            bluePicker.SetText("blue:");
            bluePicker.onPressedDirection += OnColorPressed;
            pickers[3] = bluePicker;

            tintToggler = new BooleanPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, 1, 0), Quaternion.identity), Color.yellow, Color.white);
            tintToggler.GetTransform().SetParent(panel.transform, false);
            tintToggler.SetText("color tinting:");
            tintToggler.SetState(true);
            tintToggler.OnValueChanged += matManager.ToggleTint;
            pickers[4] = tintToggler;

            glowToggler = new BooleanPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, 0, 0), Quaternion.identity), Color.yellow, Color.white);
            glowToggler.GetTransform().SetParent(panel.transform, false);
            glowToggler.SetText("emssive mode:");
            glowToggler.SetState(matManager.colorData.isDefaultEmissive);
            glowToggler.OnValueChanged += matManager.ToggleEmission;
            pickers[5] = glowToggler;


            emissiveColorSelector = new UIBodyPartPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, -1, 0), Quaternion.identity), Color.yellow, Color.white);
            emissiveColorSelector.GetTransform().SetParent(panel.transform, false);
            emissiveColorSelector.SetText("emissive body part: ");
            emissiveColorSelector.SetBodyParts(matManager.colorData.bodyParts);
            emissiveColorSelector.SetIndex(matManager.colorData.defaultEmissiveIndex);
            emissiveColorSelector.OnIndexChanged += matManager.SetEmissiveColorIndex;
            pickers[6] = emissiveColorSelector;


            emissiveSensitivity = new FloatPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, -2, 0), Quaternion.identity), Color.cyan, Color.white);
            emissiveSensitivity.GetTransform().SetParent(panel.transform, false);
            emissiveSensitivity.SetText("emissive sensitivity:");
            emissiveSensitivity.SetValue(matManager.colorData.defaultEmissiveSensetivity);
            emissiveSensitivity.OnValueChanged += matManager.SetEmissiveSensitivity;
            emissiveSensitivity.scrollAmount = 0.05f;
            pickers[7] = emissiveSensitivity;

            emissiveColorPower = new FloatPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, -3, 0), Quaternion.identity), Color.cyan, Color.white);
            emissiveColorPower.GetTransform().SetParent(panel.transform, false);
            emissiveColorPower.SetText("emissive color power:");
            emissiveColorPower.SetValue(matManager.colorData.defaultEmissiveColorPower);
            emissiveColorPower.OnValueChanged += matManager.SetEmissiveColorPower;
            emissiveColorPower.scrollAmount = 1f;
            pickers[8] = emissiveColorPower;

            emissivePower = new FloatPicker(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, -4, 0), Quaternion.identity), Color.cyan, Color.white);
            emissivePower.GetTransform().SetParent(panel.transform, false);
            emissivePower.SetText("emissive power:");
            emissivePower.SetValue(matManager.colorData.defaultEmissivePower);
            emissivePower.OnValueChanged += matManager.SetEmissivePower;
            emissivePower.scrollAmount = 1f;
            pickers[9] = emissivePower;

            restDefaults = new UITextMesh(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, -5, 0), Quaternion.identity), Color.red, Color.white);
            restDefaults.GetTransform().SetParent(panel.transform, false);
            restDefaults.SetText("reset default color settings");
            restDefaults.onPressedDirection += matManager.RestoreDefaults;
            restDefaults.onPressedDirection += ResetLabels;
            pickers[10] = restDefaults;

            restoreShader = new UITextMesh(Instantiate<tk2dTextMesh>(textPrefab, new Vector3(2, -6, 0), Quaternion.identity), Color.red, Color.white);
            restoreShader.GetTransform().SetParent(panel.transform, false);
            restoreShader.SetText("restore shader");
            restoreShader.onPressedDirection += matManager.RestoreShader;
            pickers[11] = restoreShader;

            MoveCursor(0);
            OnBodyPickerIndexChanged();
            panel.SetActive(false);           
        }
        PlayerController m_player;
        
      
        void OnDestroy()
        {
            Destroy(panel);
            for(int i =0; i < pickers.Length;i++)
            {
                pickers[i].Destroy();
            }
        }

        bool locked = true;
        void Update()
        {
            //toggles the ui. taken directly from kyle. if player presses interact button for more than 5 seconds gui will pop up
            if (!BraveInput.HasInstanceForPlayer(m_player.PlayerIDX)) { return; }
            if (Key(GungeonActions.GungeonActionType.Interact) && KeyTime(GungeonActions.GungeonActionType.Interact) > .5f && !locked)
            {
                Toggle();
                locked = true;
            }

            if (!Key(GungeonActions.GungeonActionType.Interact))
                locked = false;

            if (shown)
            {
                //allows scrolling thorugh the labels and actiavting their press left and right methods.
                //if player holds down the left or right button it will auto scroll as fast as your frames go
                GungeonActions.GungeonActionType type = GungeonActions.GungeonActionType.SelectLeft;
                if (KeyDown(type) || (Key(type) && KeyTime(type) > .5f)) 
                { 
                    pickers[activePickerIndex].PressLeft();
                }
                type = GungeonActions.GungeonActionType.SelectRight;
                if (KeyDown(type) || (Key(type) && KeyTime(type) > .5f)) 
                { 
                    pickers[activePickerIndex].PressRight();
                }
                type = GungeonActions.GungeonActionType.SelectUp;
                if (KeyDown(type) ) 
                {
                    MoveCursor(-1);
                }
                type = GungeonActions.GungeonActionType.SelectDown;
                if (KeyDown(type)) 
                {
                    MoveCursor(1);
                }
            }
        }
        void MoveCursor(int dir)
        {
            dir = activePickerIndex + dir;
            dir = Mathf.Clamp(dir, 0, pickers.Length - 1);
            pickers[activePickerIndex].ToggleColor(false);
            pickers[dir].ToggleColor(true);
            activePickerIndex = dir;
        }
        bool shown;
        void Toggle()
        {
            shown = !shown;
            if (shown)
            {
                m_player.SetInputOverride("color picker");
                panel.SetActive(true);
            }
            else
            {
                m_player.ClearInputOverride("color picker");
                panel.SetActive(false);
            }
        }
        public float KeyTime(GungeonActions.GungeonActionType action)
        {
            return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
        }

        public bool KeyDown(GungeonActions.GungeonActionType action)
        {
            return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
        }

        public bool Key(GungeonActions.GungeonActionType action)
        {
            return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
        }

        //pushes the colors inot mat manager
        void OnColorPressed()
        {
            Color32 color = new Color32(redPicker.GetValue(), greenPicker.GetValue(), bluePicker.GetValue(), 255);
            matManager.colorData.bodyParts[bodyPartPicker.GetIndex()].color = color;
            matManager.UpdateColors();
        }
        //basically updates our color labels to the body part thats picked
        void OnBodyPickerIndexChanged()
        {
            Color32 color = matManager.colorData.bodyParts[bodyPartPicker.GetIndex()].color;
            redPicker.SetValue(color.r);
            greenPicker.SetValue(color.g);
            bluePicker.SetValue(color.b);
        }
        //gotta reset them labels
        void ResetLabels()
        {
            tintToggler.SetState(true);
            bodyPartPicker.SetIndex(0);
            emissiveColorSelector.SetIndex(matManager.colorData.defaultEmissiveIndex);
            emissiveColorPower.SetValue(matManager.colorData.defaultEmissiveColorPower);
            glowToggler.SetState(matManager.colorData.isDefaultEmissive);
            emissivePower.SetValue(matManager.colorData.defaultEmissivePower);
            OnBodyPickerIndexChanged();
            matManager.UpdateColors();
        }
    }
}
