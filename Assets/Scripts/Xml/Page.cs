using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BLPage 
{

    [XmlRoot("page")]
    public class Page
    {
        [XmlElement("component")]
        public List<Component> Components = new List<Component>(); 
    }

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

    }

    public class Entity
    {
        [XmlAttribute("package")] 
        public string package;
        [XmlAttribute("id")] 
        public string id;
    }

    public class Parent
    {
        [XmlAttribute("id")] 
        public string id;
    }

    public class Param
    {
        [XmlAttribute("width")] 
        public string width;
        [XmlAttribute("height")] 
        public string height;
        [XmlAttribute("margin")] 
        public string margin;

        [XmlElement("rules")]
        public Rules rules;
        //[XmlArray("rules")]
        //[XmlArrayItem("rule")]
        //public List<Rule> rules = new List<Rule>(); 
    }

    public class Rules
    {
        [XmlElement("rule")]
        public Rule rule;
    }

    public class Rule
    {
        [XmlAttribute("anchor")] 
        public string anchor;
        [XmlAttribute("align")] 
        public string align;
    }
}
