using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LiveRecolor
{
    class FloatPicker : UITextMesh
    {
        public FloatPicker(tk2dTextMesh textMesh, Color selectedColor, Color defaultColor) : base(textMesh, selectedColor, defaultColor)
        {
            this.textMesh = textMesh;
            this.selectedColor = selectedColor;
            this.defaultColor = defaultColor;
            textMesh.color = defaultColor;
            this._transform = textMesh.transform;
            baseText = "pew pew someone forgot to initialize eh?";
            this.num = 1f;
        }

        float num;
        public float scrollAmount = 0.1f;
        public event Action<float> OnValueChanged;


        public void SetValue(float value)
        {
            num = value;
            UpdateText();
        }
        public float GetValue()
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
            num-= scrollAmount;
            SetValue(num);
            if (OnValueChanged != null)
            {
                OnValueChanged(num);
            }
            base.PressLeft();
        }
        public override void PressRight()
        {
            num+= scrollAmount;
            SetValue(num);
            if(OnValueChanged != null)
            {
                OnValueChanged(num);
            }
            base.PressRight();
        }
    }
}
