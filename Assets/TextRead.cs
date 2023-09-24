using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextRead : MonoBehaviour
{
    public static void ReadString()
    {
        string path = "Assets/Texts/test.txt";
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}
