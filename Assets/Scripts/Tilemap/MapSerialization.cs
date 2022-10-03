using System;
using UnityEngine;

[Serializable]
public class TmxFile
{
    public Layer[] layers;
}

[Serializable]
public class Layer
{
    public int[] data;
    public string name;
}

public static class Serializer
{
    public static TmxFile Deserialize(string text)
    {
        return JsonUtility.FromJson<TmxFile>(text);
    }
}