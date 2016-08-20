using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[System.Serializable]
[XmlRoot("PrisonerCollection")]
public class PrisonerContainer
{
    [XmlArray("Prisoners"), XmlArrayItem("Prisoner")]
    public List<Prisoner> prisoner = new List<Prisoner>();    

    public static PrisonerContainer Load(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        XmlSerializer serializer = new XmlSerializer(typeof(PrisonerContainer));
        StringReader reader = new StringReader(xml.text);
        PrisonerContainer prisoners = serializer.Deserialize(reader) as PrisonerContainer;

        reader.Close();

        return prisoners;
    }

}
