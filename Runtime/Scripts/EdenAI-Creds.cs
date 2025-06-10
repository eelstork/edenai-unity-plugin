using System; using System.IO; using UnityEditor;
using Newtonsoft.Json; using EdenAI;

public static class EdenAICreds{

    const string EditorPrefsKey = "EdenAI_API_Key";

    public static string FindCreds(){

        var key = EditorPrefs.GetString(EditorPrefsKey, null);
        if(key != null) return key;
        var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var authPath = $"{userPath}/.edenai/auth.json";
        if (File.Exists(authPath)){
            var json = File.ReadAllText(authPath);
            Key auth = JsonConvert.DeserializeObject<Key>(json);
            return auth.api_key;
        }else{
            return null;
        }
    }

}
