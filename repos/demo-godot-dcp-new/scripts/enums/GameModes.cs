namespace DouCardPuzzoom.scripts.enums; 

public enum GameModes {
    // 正常的谁出完谁获胜模式
    Normal,
    
    // 谁出完谁输，该模式下能跟牌不允许Pass
    // 像Landlord和Robot的AI是不需要担心的，有牌一定会出
    // 该部分逻辑的限制应该放在IAi的实现中
    Rest,
    
    // 盲牌，该模式下没有Hint（出牌全部换成back就行）
    Blind
    
    // 就不搞 Rest 和 Blind 混合在一起的设计了……
    // 盲牌和传统斗地主还是不一样的，至少传统斗地主会告诉你出了什么牌
    // 毕竟，如果显示出牌，就等于明牌了
    // 最后还是要设计一个通关后明牌复盘的逻辑
}