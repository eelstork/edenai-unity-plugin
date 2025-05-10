using System.IO;
using UnityEngine;

namespace EdenAI{
public static class Stopper{

    static string path = ".stop_token";

    public static void Reset(){
        UnityEngine.Debug.Log("CLEAR STOP TOKEN");
        File.Delete(path);
    }

    public static void Stop() => File.WriteAllText(path, ":)");

    public static bool should_stop => File.Exists(path);

}}
