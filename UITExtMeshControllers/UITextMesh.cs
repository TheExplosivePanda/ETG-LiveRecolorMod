using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;

namespace LiveRecolor
{
    class UITextMesh
    {
        //im not going to go over all of the inherited classes because theyre pretty self explanatory imo, but basically that is a thing that takes a tk2dTextMesh
        //and turns it into a ui piece with starndard inteface of "press right" and  "press left"
        public UITextMesh(tk2dTextMesh textMesh,Color selectedColor, Color defaultColor)
        {
            this.textMesh = textMesh;
            this.selectedColor = selectedColor;
            this.defaultColor = defaultColor;
            textMesh.color = defaultColor;
            textMesh.color2 = defaultColor;
            this._transform = textMesh.transform;
            
        }
        protected Transform _transform;
        protected tk2dTextMesh textMesh;
        protected Color selectedColor;
        protected Color defaultColor;
        public event Action onPressedLeft;
        public event Action onPressedRight;
        public event Action onPressedDirection;

        public void Destroy()
        {
            UnityEngine.Object.Destroy(textMesh);
        }
        
        public virtual void PressLeft()
        {
            if (onPressedLeft != null)
            {
                onPressedLeft();
            }
            if (onPressedDirection != null)
            {
                onPressedDirection();
            }
        }
        public virtual void PressRight()
        {
            if (onPressedRight != null)
            {
                onPressedRight();
            }
            if (onPressedDirection != null)
            {
                onPressedDirection();
            }

        }
        public void ToggleColor(bool on)
        {
            if (on)
            { 
                textMesh.color = selectedColor;
                textMesh.color2 = selectedColor;
            }
            else
            { 
                textMesh.color = defaultColor;
                textMesh.color2 = defaultColor;
            }
        }
        protected void setText(string text)
        {
            textMesh.text = text;
        }
        public virtual void SetText(string text)
        {
            textMesh.text = text;
        }

        public virtual string GetText()
        {
            return textMesh.text;
        }
        
        public Transform GetTransform()
        {
            return _transform; 
        }
    }
}
