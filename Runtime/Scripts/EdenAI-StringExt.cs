using System;

namespace EdenAI{
public static class EdenAIStringExt{

    public static bool ContainsAllCI(
        this string self, params string[] args
    ){
        foreach (var k in args){
            if (self.IndexOf(k, StringComparison.OrdinalIgnoreCase) < 0)
                return false;
        }
        return true;
    }

}}
