using System; using System.IO; using UnityEditor;
using Newtonsoft.Json; using EdenAI;


public static class EdenAIBaseCreds{

    const string EditorPrefsKey = "EdenAI_API_Key";
    static string _key;

    public static string FindCreds(){
        if(_key != null) return _key;
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

    public static void CacheValues(){
        _key = EditorPrefs.GetString(EditorPrefsKey, null);
    }

}
