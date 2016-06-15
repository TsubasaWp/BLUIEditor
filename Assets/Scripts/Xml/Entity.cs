using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace BLEntity
{

    [XmlRoot("package")]
    public class Entity
    {
        [XmlAttribute("id")] public string id;
        [XmlElement("G_View")] public List<G_View> views = new List<G_View>();
        [XmlElement("G_TextView")] public List<G_TextView> textViews = new List<G_TextView>();
        [XmlElement("G_Button")] public List<G_Button> buttons = new List<G_Button>();
        [XmlElement("G_GridView")] public List<G_GridView> gridViews = new List<G_GridView>();
        [XmlElement("G_ListView")]
        public List<G_ListView> listViews = new List<G_ListView>();
        [XmlElement("G_RelativeLayout")]
        public List<G_RelativeLayout> relativeLayouts = new List<G_RelativeLayout>(); 
    }

    public class G_View
    {

        [XmlAttribute("id")]
        public string id;
        [XmlElement("members")] public ViewMember member;

        public class ViewMember
        {
            [XmlElement("texture")]
            public Texture texture;
        }

    }

    public class G_TextView
    {
        [XmlAttribute("id")]
        public string id;
        [XmlElement("members")] public TextMembers member;
        [XmlElement("G_View")] public ColorView view;
        public class ColorView
        {
            [XmlElement("members")]
            public ColorMember member;
        }
        public class TextMembers
        {
            [XmlElement("text")]
            public Text text;
        }
        public class ColorMember
        {
            [XmlElement("color")]
            public Color color;
        }

        public class Text
        {
            [XmlAttribute("size")]
            public string size;
            [XmlAttribute("color")]
            public string color;
            [XmlAttribute("bordersize")]
            public string borderSize;
            [XmlAttribute("bordercolor")]
            public string borderColor;
            [XmlAttribute("style")]
            public string style;
            [XmlAttribute("space")]
            public string space;
            [XmlAttribute("align")]
            public string align;
            [XmlAttribute("string")]
            public string str;
            [XmlAttribute("stringID")]
            public string stringID;
        }
    }

    public class G_Button
    {
        [XmlAttribute("id")]
        public string id;
        [XmlElement("member")] public MouseDownMember member;
        [XmlElement("G_TextView")] public ButtonTextNode textview;

        public class MouseDownMember
        {
            [XmlElement("texture_mouse_down")]
            public Texture texture;
        }
        
        public class ButtonTextNode
        {
            [XmlElement("G_View")] public ButtonView view;
        }

        public class ButtonView
        {
            [XmlElement("members")] public ButtonViewMember members;
        }

        public class ButtonViewMember
        {
            [XmlElement("texture")] public Texture texture;
            [XmlElement("touchpad")] public ButtonTouchPad touchpad;
        }

        public class ButtonTouchPad
        {
            [XmlAttribute("touchpad")] public string touchpad;
        }
    } 

    public class G_GridView
    {

        [XmlAttribute("id")]
        public string id;
    }

    public class G_ListView
    {

        [XmlAttribute("id")]
        public string id;
    }

    public class G_RelativeLayout
    {
        [XmlAttribute("id")]
        public string id;
        [XmlElement("G_ViewGroup")]
        public G_ViewGroup viewGroup;

        public class G_ViewGroup
        {
            [XmlElement("G_View")]
            public G_View view;
        }

    }


    public class Texture
    {
        [XmlAttribute("package")]
        public string package;
        [XmlAttribute("id")]
        public string id;
    }

    public class Color
    {
        [XmlAttribute("default")]
        public string defaultColor;
    }


}
