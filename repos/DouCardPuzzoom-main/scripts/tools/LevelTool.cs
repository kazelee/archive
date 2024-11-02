using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SCG = System.Collections.Generic;

namespace DouCardPuzzoom.scripts.tools; 

public static class LevelTool {
    // id 其实可以直接设置成 int 类型的，当时是为了玩家自定义时可以输入字符
    // name 也一样，其实改成"x-x-abc"的格式更方便……历史遗留问题，也不改了……

    // 参考："Landlord", "Housekeeper", "ShuShu", "Madeline", "WangXiang", "Albert", "Student", "Geek", "Thief", "Girl"

    public static SCG.Dictionary<string, int> NPCNameLevelMap = new SCG.Dictionary<string, int>() {
        { "Landlord", 1 }, { "Housekeeper", 2 }, { "ShuShu", 3 }, { "WangXiang", 4 },
        { "Albert", 5 }, { "Student", 6 }, { "Thief", 7 }, { "Geek", 8 }
    };
    
    /// <summary>
    /// e.g. "Landlord" => "L_1" （实际需要“L_1_X”的形式）
    /// </summary>
    /// <returns></returns>
    public static string GetLevelKeyFirst(string cname) {
        return $"L_{NPCNameLevelMap[cname]}";
    }
    
    /// <summary>
    /// 获取当前关卡的上一个关卡
    /// </summary>
    /// <param name="name">关卡名称，就是KEY，如 L_1_1 ”</param>
    /// <param name="id">关卡序号，数字的字符串形式</param>
    /// <returns>关卡错误返回""，开始/结尾返回"BEG"/"END"，其余返回path</returns>
    public static string GetLastLevelPath(string name, string id) {
        if (id.ToInt() % 1000 == 1) return "BEG";

        return $"res://levels/{name}/{id.ToInt() - 1}.json";
    }

    public static string GetLevelPath(string name, string id) {
        return $"res://levels/{name}/{id}.json";
    }

    /// <summary>
    /// 获取当前关卡的下一个关卡
    /// </summary>
    /// <param name="name">关卡名称，没有序号，如“Newcomer”</param>
    /// <param name="id">关卡序号，数字的字符串形式</param>
    /// <returns>关卡错误返回""，开始/结尾返回"BEG"/"END"，其余返回path</returns>
    public static string GetNextLevelPath(string name, string id) {
        // 形如“L_1_1”
        var nameList = name.Split("_");
        var level = nameList[2];
        switch (level) {
            case "1":
            case "2":
                if (id.ToInt() % 1000 == 50) return "END";
                break;
            case "3":
                if (id.ToInt() % 1000 == 100) return "END";
                break;
            case "4":
                if (id.ToInt() % 1000 == 20) return "END";
                break;
        }
        
        return $"res://levels/{name}/{id.ToInt() + 1}.json";
    }

    /// <summary>
    /// 得到关卡的总数量
    /// </summary>
    /// <param name="name">确保name正确，不检查了</param>
    /// <returns></returns>
    public static int GetLevelNum(string name) {
        var nameList = name.Split("_");
        switch (nameList[2]) {
            case "1":
            case "2":
                return 50;
            case "3": return 100;
            case "4": return 20;
        }

        return 1;
        // return LevelNum[name];
    }

    public static int GetLevelBeg(string name) {
        // return LevelBeg[name];
        var nameList = name.Split("_");
        var begStr = $"{nameList[1]}{nameList[2]}" + "001";
        return begStr.ToInt();
    }

    public static int GetLevelEnd(string name) {
        // return LevelEnd[name];
        return GetLevelBeg(name) + GetLevelNum(name) - 1;
    }
}