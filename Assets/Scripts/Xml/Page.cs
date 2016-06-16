using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BLPage 
{
    [Serializable]  
    [XmlRoot("page")]
    public class Page
    {
        [XmlAttribute("id")]
        public string id;
        [XmlAttribute("alpha")]
        public string alpha;
        [XmlElement("component")]
        public List<Component> Components = new List<Component>();

    }

    [Serializable]  
    public class Component
    {
        [XmlAttribute("id")] 
        public string id;
        [XmlAttribute("event")]
        public string eventStr;
        [XmlAttribute("visible")]
        public string visible;
        [XmlElement("entity")] 
        public Entity entity;
        [XmlElement("parent")] 
        public Parent parent;
        [XmlElement("param")]
        public Param param;
        [XmlElement("touchpad")]
        public Touchpad touchpad;

        public Component Copy()
        {
            return GenericCopier<Component>.DeepCopy(this);
        }
    }

    [Serializable]  
    public class Entity
    {
        [XmlAttribute("package")] 
        public string package;
        [XmlAttribute("id")] 
        public string id;
    }

    [Serializable]  
    public class Parent
    {
        [XmlAttribute("id")] 
        public string id;
    }

    [Serializable]  
    public class Param
    {
        [XmlAttribute("width")]
        public string width;
        [XmlAttribute("height")]
        public string height;
        [XmlAttribute("x")]
        public string x;
        [XmlAttribute("y")]
        public string y;
        [XmlAttribute("margin")] 
        public string margin;
        [XmlElement("rules")]
        public Rules rules;
        //[XmlArray("rules")]
        //[XmlArrayItem("rule")]
        //public List<Rule> rules = new List<Rule>(); 
    }


   [Serializable]  
    public class Rules
    {
        [XmlElement("rule")]
        public Rule rule;
    }

    [Serializable]  
    public class Rule
    {
        [XmlAttribute("anchor")] 
        public string anchor;
        [XmlAttribute("align")] 
        public string align;
    }

    [Serializable]  
    public class Touchpad
    {
        [XmlAttribute("value")]
        public string value;
    }
}
