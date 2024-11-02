using DouCardPuzzoom.scripts.ai;
using Godot;

namespace DouCardPuzzoom.scripts.tools; 

// (tmp)DONE

/// <summary>
/// 与AI有关的工具类
/// </summary>
public static class AiTool {
    /// <summary>
    /// 根据字符串获取AI接口类
    /// </summary>
    /// <param name="aiName">AI名称，会检查是否合法</param>
    /// <returns>不合法返回null</returns>
    public static IAi GetAi(string aiName) {
        switch (aiName) {
            case "Landlord":
                return new AiLandlord();
            case "Robot":
                return new AiRobot();
            case "Housekeeper":
                return new AiHousekeeper();
            default:
                GD.PrintErr($"AI名称 {aiName} 不合法！");
                return null;
        }
    }
}