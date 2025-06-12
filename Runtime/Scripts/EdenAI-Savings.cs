using System; using System.IO;

namespace EdenAI{

// MaxCost is set via MaxCost.cs
public class Savings : Exception{

    public static bool enabled;
    public static double cumul;
    public static double max_cost;

    public static void CheckCost(){
        if(!enabled){
            //nityEngine.Debug.Log("Bypass penny fuse");
            return;
        }
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
