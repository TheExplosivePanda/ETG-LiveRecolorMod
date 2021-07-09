using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LiveRecolor
{
    class BooleanPicker : UITextMesh
    {
        public BooleanPicker(tk2dTextMesh textMesh, Color selectedColor, Color defaultColor) : base(textMesh, selectedColor, defaultColor)
        {
            this.textMesh = textMesh;
            this.selectedColor = selectedColor;
            this.defaultColor = defaultColor;
            textMesh.color = defaultColor;
            this._transform = textMesh.transform;
        }

        bool _toggleState;

        public event Action<bool> OnValueChanged;

        public void SetState(bool state)
        {
            _toggleState = state;
            UpdateText();
        }
        public bool GetState()
        {
            return _toggleState;
        }
        private string baseText = null;
      

        override public void SetText(string text)
        {
            baseText = text;
            textMesh.text = baseText + " <" + (_toggleState ? "on" : "off") + ">";
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
            SetState(!GetState());
            if (OnValueChanged != null)
            {
                OnValueChanged(_toggleState);
            }
            base.PressLeft();
        }
        public override void PressRight()
        {
            SetState(!GetState());
            if (OnValueChanged != null)
            {
                OnValueChanged(_toggleState);
            }
            base.PressRight();
        }
    }
}
