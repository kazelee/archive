using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;

namespace DouCardPuzzoom.scripts.rule; 

/// <summary>
/// 规则类接口：提供不同规则下所需要的最基本工具函数
/// </summary>
public interface IRule {
    /// 【AI】第一个出牌，返回 Comb 列表（不会为空列表，先出者永远有牌出）
    public List<CombData> GetAllComb(List<CardData> myCardsInHand);
    
    /// 【玩家】判断 CardData List 是否合法，合法则封装成 CombData（不合法为 null）
    public bool IsCombValid(List<CardData> tryLeadCards, out CombData toLeadComb);
    
    /// 【玩家】跟牌，判断是否能够打过（Comb 必然合法）
    public bool CanBeat(CombData comb1, CombData comb2);
    
    /// 【玩家/AI】跟牌，所有可能的牌组（打不过返回初始化空列表）
    public bool CanFollow(CombData target, List<CardData> myCardsInHand, out List<CombData> possibleCombs);
    
    /// <summary>
    /// 【原地排序】对手牌进行排序：优先点数，其次花色
    /// </summary>
    /// <param name="cardsInHand"></param>
    public void SortInHand(List<CardData> cardsInHand);

    /// <summary>
    /// 【仅考虑出牌】对待出牌进行排序，根据牌型（如多张放在前面，带牌放在后面）
    /// </summary>
    /// <param name="combToLead">仍然使用List CardData</param>
    /// <param name="type">为特殊排序规则设计，主要是为了player</param>
    public void SortToLead(List<CardData> combToLead, Dictionary<string, int> type = null);
    
    
}