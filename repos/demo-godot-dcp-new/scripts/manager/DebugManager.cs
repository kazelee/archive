using DouCardPuzzoom.scripts.enums;

namespace DouCardPuzzoom.scripts.manager; 

public static class DebugManager {
    // public const bool DebugScreenOpen = true; // 写起来太复杂，算了
    
    /// <summary>
    /// 仅在测试的时候使用
    /// </summary>
    public static bool IsDebugMode = false; // 进入 GameScene 的时候切换，运行两个示例不就行了

    public const bool AllowFourWithTwo = true;
    public const bool AllowPlane = true;

    // public const bool AllowFourWithTwoAnyTimes = false;
    // public const bool AllowPlaneAnyTimes = false;

    // public static int FirstChoiceIndex = 0; // 至少第一个选择是固定的？
}