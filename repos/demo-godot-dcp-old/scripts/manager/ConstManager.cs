namespace DouCardPuzzoom.scripts.managers; 

public static class ConstManager {
    /// <summary>
    /// 卡牌之间的距离
    /// </summary>
    public const int CardSideDistance = 60;
    /// <summary>
    /// 卡牌被选中上移的距离
    /// </summary>
    public const int CardSelectShift = 50;
    
    /// <summary>
    /// LeadCard 相对 CardPlace 中的卡牌的大小缩放<br/>
    /// 【注意】CardPlace 的卡牌大小本身已经是 Assets 的 0.5 倍了，这里是相对这个大小的进一步缩小；
    /// 默认的 0.5 素材缩放比例不再独立成 const
    /// </summary>
    public const float LeadCardScale = 0.8f;
    /// <summary>
    /// AICardPlace 相对 MyCardPlace 的卡牌缩放大小<br/>
    /// （原则上比 MyCard 小，比 LeadCard 大，其他同 LeadCardScale）
    /// </summary>
    public const float AICardScale = 0.9f;
}