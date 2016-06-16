using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BLPage;
using BLEntity;
using BLTexture;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

    public static bool SavePage(string path, Page page)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(Page));
            var encoding = Encoding.GetEncoding("UTF-8");
            using (StreamWriter stream = new StreamWriter(path, false, encoding))
            {
                serializer.Serialize(stream, page);
                stream.Close();
            }
            //var stream = new FileStream(path, FileMode.Create);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return false;
    }
}

public static class GenericCopier<T>    //deep copy a list
{
    public static T DeepCopy(object objectToCopy)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, objectToCopy);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }
}