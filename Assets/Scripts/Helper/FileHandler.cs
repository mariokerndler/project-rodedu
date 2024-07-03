using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileHandler
{
    public static void SaveToJson<T> (List<T> toSave, string filename) {
        var content = JsonHelper.ToJson<T> (toSave.ToArray ());
        WriteFile (GetPath (filename), content);
    }

    public static void SaveToJson<T> (T toSave, string filename) {
        var content = JsonUtility.ToJson (toSave);
        WriteFile (GetPath (filename), content);
    }

    public static List<T> ReadListFromJson<T> (string filename) {
        var content = ReadFile (GetPath (filename));

        if (string.IsNullOrEmpty (content) || content == "{}") {
            return new List<T> ();
        }

        var res = JsonHelper.FromJson<T> (content).ToList ();

        return res;

    }

    public static T ReadFromJson<T> (string filename) {
        var content = ReadFile (GetPath (filename));

        if (string.IsNullOrEmpty (content) || content == "{}") {
            return default (T);
        }

        var res = JsonUtility.FromJson<T> (content);

        return res;
    }

    private static string GetPath (string filename) {
        return Application.persistentDataPath + "/" + filename;
    }

    private static void WriteFile (string path, string content) {
        var fileStream = new FileStream (path, FileMode.Create);

        using (var writer = new StreamWriter (fileStream)) {
            writer.Write (content);
        }
    }

    private static string ReadFile (string path) {
        if (!File.Exists(path)) return "";
        
        using (var reader = new StreamReader (path)) {
            var content = reader.ReadToEnd ();
            return content;
        }
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T> (string json) {
        var wrapper = JsonUtility.FromJson<Wrapper<T>> (json);
        return wrapper.Items;
    }

    public static string ToJson<T> (T[] array) {
        var wrapper = new Wrapper<T>
        {
            Items = array
        };
        return JsonUtility.ToJson (wrapper);
    }

    public static string ToJson<T> (T[] array, bool prettyPrint) {
        var wrapper = new Wrapper<T>
        {
            Items = array
        };
        return JsonUtility.ToJson (wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T> {
        public T[] Items { get; set; }
    } 
}