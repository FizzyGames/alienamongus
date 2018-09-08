using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorHelperScript
{
    [UnityEditor.MenuItem("Tools/Debug/Log Json List Message")]
    public static void ShowSerializedMessage()
    {
        List<gm.sendIDMessageTP> sendIDMessageTPs = new List<gm.sendIDMessageTP>();
        Texture2D texture = Resources.Load<Texture2D>("Placeholders/test");
        Debug.Log(texture.GetPixel(0, 0));
        for (int i = 0; i < 10; i++)
        {
            sendIDMessageTPs.Add(new gm.sendIDMessageTP(i.ToString(), texture));
        }
        gm.listMessageTP listMessageTP = new gm.listMessageTP(sendIDMessageTPs);
        Debug.Log(DeJson.Serializer.Serialize(sendIDMessageTPs[0]));
        string output = DeJson.Serializer.Serialize(listMessageTP);
        Debug.Log(output);
        DeJson.Deserializer deserializer = new DeJson.Deserializer();
        listMessageTP = deserializer.Deserialize<gm.listMessageTP>(output);
        Debug.Log(((Texture2D)listMessageTP._messages[0].playerPhoto).GetPixel(0, 0));
    }
}
