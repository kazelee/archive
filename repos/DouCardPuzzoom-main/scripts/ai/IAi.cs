using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;

namespace DouCardPuzzoom.scripts.ai; 

// DONE
// 补充：AI做决策需要知道自己的手牌，而这无法通过访问全局变量实现
// IAi 接口不知道自己是哪一个人的手牌，所以还是需要传递参数
// 考虑到部分AI可以看到全局，不如只传递name，让AI自行搜索
// 理论上，如果一个接口只对应一个AI名称，可以不表，但严谨起见还是加上

/// <summary>
/// AI类接口：定义游戏中NPC的出牌AI逻辑
/// </summary>
public interface IAi {
    // 直接读取GameLogic全局变量了还要专门的函数干什么？
    
    // /// <summary>
    // /// 每次AI出牌时，都调用此函数，更新一下当前局面的状态，根据此改变出牌逻辑
    // /// </summary>
    // public void LoadRound();

    /// <summary>
    /// 第一个出牌，根据加载的牌局信息，在所有可能牌中选择
    /// </summary>
    /// <param name="possibleCombs">所有可能的牌组</param>
    /// <param name="name">该Ai在局中的名称</param>
    /// <returns>必须返回一种可能牌组，一定是花色sensitive的！！！</returns>
    public CombData ChooseToLead(List<CombData> possibleCombs, string name);

    /// <summary>
    /// 跟牌，根据加载的牌局信息，在所有可能牌中选择
    /// </summary>
    /// <param name="possibleCombs">所有可能跟牌</param>
    /// <param name="name">该Ai在局中的名称</param>
    /// <returns>返回一个可能牌组或空值（不出），一定是花色sensitive的！！！</returns>
    public CombData ChooseToFollow(List<CombData> possibleCombs, string name);
}