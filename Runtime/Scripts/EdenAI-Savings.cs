using System; using System.IO;

namespace EdenAI{
public static class Stopper{

    static string path = ".stop_token";

    public static void Reset() => File.Delete(path);

    public static void Stop() => File.WriteAllText(path, ":)");

    public static bool should_stop => File.Exists(path);

}

// MaxCost is set via MaxCost.cs
public class Savings : Exception{

    public static double cumul;
    public static double max_cost;

    public static void CheckCost(){
        if(cumul < max_cost - 0.01){
            return;
        }else{
            throw new Savings(cumul);
        }
    }

    public Savings(string reason)
    : base(reason){
    }

    public Savings(double val)
    : base($"Keeping costs under ${val}"){
    }

}

}  // EdenAI
