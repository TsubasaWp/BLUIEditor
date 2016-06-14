using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BLPage;
using BLEntity;
using BLTexture;

public class XmlLoader
{
    public static Page LoadPage(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Page));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as Page;
        }
    }

    public static BLEntity.Entity LoadEntity(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(BLEntity.Entity));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as BLEntity.Entity;
        }
    }

 
}
