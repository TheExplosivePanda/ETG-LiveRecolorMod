using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LiveRecolor
{
    class UIBodyPartPicker : UITextMesh
    {
        public UIBodyPartPicker(tk2dTextMesh textMesh, Color selectedColor, Color defaultColor) : base(textMesh, selectedColor, defaultColor)
        {
            this.textMesh = textMesh;
            this.selectedColor = selectedColor;
            this.defaultColor = defaultColor;
            textMesh.color = defaultColor;
            this._transform = textMesh.transform;
            baseText = "pew pew someone forgot to initialize eh?";
            index = 0;
        }
        string baseText = null;
        List<ColorGroup> bodyParts;
        int index;
        public event Action<int> OnIndexChanged;

        public int GetIndex()
        {
            return index;
        }
        public void SetIndex(int idx)
        {
            if(bodyParts == null) { return; }
            if(idx >= bodyParts.Count)
            {
                index = bodyParts.Count-1; 
                UpdateText();
                return;
            }
            if(idx <= 0) 
            {
                index = 0; 
                UpdateText();
                return;
            }
            index = idx;
            UpdateText();

        }
        public void SetBodyParts(List<ColorGroup> bodyParts)
        {
            this.bodyParts = bodyParts;
            UpdateText();
        }
      
        override public void SetText(string text)
        {
            baseText = text;
            if (bodyParts == null || bodyParts.Count <= 0)
            {
                textMesh.text = baseText;
                return;
            }
            textMesh.text = baseText + " <" + bodyParts[index].name + ">";
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
            index--;
            SetIndex(index);
            if (OnIndexChanged!= null)
            {
                OnIndexChanged(index);
            }
            UpdateText();
            base.PressLeft();
        }
        public override void PressRight()
        {
            index++ ;
            SetIndex(index);
            if (OnIndexChanged != null)
            {
                OnIndexChanged(index);
            }
            UpdateText();
            base.PressRight();
        }
    }
}
