using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LiveRecolor
{
    public class ColorGroup
    {
        public ColorGroup(Color32 origColor, Color32 color, string name)
        {
            OriginalColor = origColor;
            this.color = color;
            this.name = name;
        }
        public Color32 OriginalColor;
        public Color32 color;
        public string name = "";
    }
}
