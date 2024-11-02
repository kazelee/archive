using System.Collections.Generic;

namespace DouCardPuzzoom.scripts.classes; 

/// <summary>
/// 待打出的牌组类（手牌不适用此类）
/// </summary>
public class CombData {
    public int[] temp;
    
    /// <summary>
    /// 封装原本的牌组列表
    /// </summary>
    public List<CardData> Cards;

    /// <summary>
    /// 只有点数（为部分rule设计）
    /// </summary>
    public List<string> RuleList;
    
    /// <summary>
    /// 所有的类型可能（每种类型 string 只记录最高的权重 int）
    /// </summary>
    public Dictionary<string, int> Types;
    
    /// <summary>
    /// 牌组是否花色敏感（给AI的选牌列表需要强调这一点）<br/>
    /// 如：若AI只出含红心的牌，而提示只根据点数顺序选择一种花色可能，则需要再做处理
    /// </summary>
    public bool IsSuitSensitive;

    // 确保构造时就是非空合法结构
    public CombData(List<CardData> cards, Dictionary<string, int> types) {
        Cards = cards;
        Types = types;
        // 无需通过构造函数赋值，直接根据加载数据类型判断
        IsSuitSensitive = true;
    }

    public CombData(List<string> cards, Dictionary<string, int> types) {
        RuleList = cards;
        Types = types;
        IsSuitSensitive = false;
    }

    // 拷贝构造函数
    public CombData(CombData combData) {
        // 如果一开始就初始化new就不会有null的问题，暂时不打算重构代码
        Cards = combData.Cards != null ? new List<CardData>(combData.Cards) : null;
        RuleList = combData.RuleList != null ? new List<string>(combData.RuleList) : null;
        Types = new Dictionary<string, int>(combData.Types);
        IsSuitSensitive = combData.IsSuitSensitive;
    }
}