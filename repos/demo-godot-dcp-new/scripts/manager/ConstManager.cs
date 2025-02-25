namespace DouCardPuzzoom.scripts.manager; 

public static class ConstManager {
    /// <summary>
    /// 卡牌之间的距离
    /// </summary>
    public const int CardSideDistance = 12; // 60/5=12
    /// <summary>
    /// 卡牌被选中上移的距离
    /// </summary>
    public const int CardSelectShift = 10; // 50/5=10

    // /// <summary>
    // /// 所有像素素材导入的scale均为扩大5倍
    // /// </summary>
    // public const float PixelAssetScale = 5f;
    
    /// <summary>
    /// LeadCard直接相对卡牌素材的大小缩放
    /// </summary>
    public const float LeadCardScale = 0.6f;
    /// <summary>
    /// AICardPlace直接相对卡牌素材的大小缩放
    /// </summary>
    public const float AiCardScale = 0.8f;

    // public const float WaitTime = 0.5f;
    public const int DelayMs = 500;
}