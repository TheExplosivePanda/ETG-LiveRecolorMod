using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LiveRecolor
{
    class ColorPicker : UITextMesh
    {
        public ColorPicker(tk2dTextMesh textMesh, Color selectedColor, Color defaultColor) : base (textMesh, selectedColor,defaultColor)
        {
            this.textMesh = textMesh;
            this.selectedColor = selectedColor;
            this.defaultColor = defaultColor;
            textMesh.color = defaultColor;
            this._transform = textMesh.transform;
            baseText = "pew pew someone forgot to initialize eh?";
            this.num = 0;
        }
        byte num;
       

        public void SetValue(byte value)
        {
            num = value;
            UpdateText();
        }
        public byte GetValue()
        {
            return num;
        }

        private string baseText = null;
       

        override public void SetText(string text)
        {
            baseText = text;
            textMesh.text = baseText + " <" + num + ">";
        }


        override public string GetText()
        {
            return baseText;
        }
        void UpdateText()
        {
            SetText(baseText);
        }
        public override void PressLeft()
        {
            num--;
            SetValue(num);
            base.PressLeft();
        }
        public override void PressRight()
        {
            num++;
            SetValue(num);
            base.PressRight();
        }

    }
}
