using System.Collections.Generic;
using Godot;
using Godot.Collections;

using SCG = System.Collections.Generic;

namespace DouCardPuzzoom.scripts.tools; 

public static class DialogueTool {
    public static readonly string[] AllNPCName = new[] {
        "Landlord", "Housekeeper", "ShuShu", "Madeline", "WangXiang", "Albert", "Student",
        "Geek", "Thief", "Girl"
    };
    
    // public static Array LoadOneDialogue(string name, string id) {
    //     var readFile = FileAccess.Open($"res://dialogues/{name}.json", FileAccess.ModeFlags.Read);
    //     var dataStr = readFile.GetAsText();
    //     var dataDir = Json.ParseString(dataStr).AsGodotDictionary();
    //     return (Array)dataDir[id];
    // }
    public static readonly SCG.Dictionary<string, string> CNameMap = new SCG.Dictionary<string, string>() {
        { "Landlord", "C_LL" }, { "Housekeeper", "C_HK" }, {"Robot", "C_ROBOT"},
        { "ShuShu", "C_BL" }, { "Madeline", "C_CL" }, { "WangXiang", "C_SM" }, { "Albert", "C_EX" },
        { "Student", "C_ST" }, { "Geek", "C_GK" }, { "Thief", "C_TH" }, { "Girl", "C_LG" }
    };

    public static string GetNPCFirstIconName(string cname) {
        // if (cname == "Robot") return ""; // 保险起见
        var nameKeyList = CNameMap[cname].Split("_");
        return $"I_{nameKeyList[1]}_1";
    }

    public static string GetAiScreenIconName(string name) {
        return $"icon-{name}";
    }
    
    public static Dictionary LoadDialogue(string name) {
        using var readFile = FileAccess.Open($"res://dialogues/{name}.json", FileAccess.ModeFlags.Read);
        var dataStr = readFile.GetAsText();
        var dataDir = Json.ParseString(dataStr).AsGodotDictionary();
        return dataDir;
    }

    public static Dictionary LoadTempDialogue() {
        using var readFile = FileAccess.Open("res://dialogues/Temp.json", FileAccess.ModeFlags.Read);
        var dataStr = readFile.GetAsText();
        var dataDir = Json.ParseString(dataStr).AsGodotDictionary();
        return dataDir;
    }

    public static string GetDialogueNameTranslated(string name) {
        return TranslationServer.Translate(CNameMap[name] + "_N");
    }

    public static string GetDialogueJobTranslated(string name) {
        return TranslationServer.Translate(CNameMap[name]);
    }

    public static string GetAiContentWithState(string name, int state) {
        // state 0 都是一样的，留白，没必要专门搞一个key了……
        // 不对，还是要讲一下怎么解锁……
        // if (state == 0) return "???";
        // if (state == 0) return TranslationServer.Translate("C_0");
        return TranslationServer.Translate(CNameMap[name] + $"_{state}");
    }
}