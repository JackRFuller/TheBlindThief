using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Prisoner
{
    [XmlAttribute("Name")]
    public string name;

    [XmlElement("LineOne")]
    public string lineOne;

    [XmlElement("LineTwo")]
    public string lineTwo;

    [XmlElement("LineThree")]
    public string lineThree;

}
