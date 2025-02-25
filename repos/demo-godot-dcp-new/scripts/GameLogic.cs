using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DouCardPuzzoom.scripts.ai;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.rule;
using DouCardPuzzoom.scripts.tools;
using Godot;
using Godot.Collections;

namespace DouCardPuzzoom.scripts; 


/// <summary>
/// 游戏 核心逻辑部分
/// </summary>
public static class GameLogic {
    public static event Action LevelLoadedEvent;

    public static event Action<List<CombData>> PlayerLeadEvent;
    public static event Action PlayerLeadOver; // 用于取消IsUndoable
    public static event Action UndoFailed; // 用于 Undo 第一条 return 时恢复 Undo
    public static event Action<CombData> AiLeadEvent;
    public static event Action<CombData> Ai2LeadEvent;

    public static event Action TurnOverEvent;
    public static event Action<OneTurnInfo> UndoEvent;

    public static event Action<bool, string> RoundOver;
    
    // 临时记录当前的path等待GameScene Ready后加载
    public static string CurrentLevelPath;
    
    /// <summary>
    /// name: str, id: str, rule: str, number: int, players: [str], landlord: str, cards: [[str]]
    /// </summary>
    public static LevelInfo CurrentLevel;
    
    /// <summary>
    /// e.g. YOU: 0, Landlord: 1, Julia: 2
    /// </summary>
    public static System.Collections.Generic.Dictionary<string, int> PlayerIndexMap = new();
    
    public static IRule CurrentRule;
    public static IAi Ai;
    public static IAi Ai2;
    
    public static int PassLimitNum = 999;
    public static int LeadLimitNum = 999; // Lead 就是 Play，懒得改了
    // 【新增】过牌/出牌要求
    public static int PassRequestNum = 0;
    public static int PlayRequestNum = 0;
    
    /// <summary>
    /// 记录每一轮次的出牌记录
    /// </summary>
    public static List<OneTurnInfo> RoundRecord = new();
    // /// <summary>
    // /// 每次记录的临时存储
    // /// </summary>
    // public static OneTurnInfo TmpTurnInfo = new();
    public static List<string> TmpTurns = new();
    public static List<CombData> TmpCombs = new();
    
    public static TurnStates CurrentState;
    public static CombData CurrentBiggest;
    public static string NextTurnFirst; // 好像没啥用啊？
    public static int PassTimes;

    public static GameModes GameMode;
    
    // 还是直接倒数PassLimitNum来的方便……
    // public static int PlayerPassCount;
    // public static int PlayerLeadCount;
    
    /// <summary>
    /// GameScene加载完后才会调用
    /// </summary>
    public static void InitLevel() {
        // 清空旧记录
        CurrentBiggest = null;
        TmpTurns = new();
        TmpCombs = new();
        RoundRecord = new();
        PlayerIndexMap = new();
        // PassLimitNum = CurrentLevel.PassLimit;
        // LeadLimitNum = CurrentLevel.LeadLimit; // 下面reset过了
        
        // 加载关卡（如果失败则退出）
        if (CurrentLevelPath == "") return;
        if (!LoadTool.TryLoadLevel(CurrentLevelPath, ref CurrentLevel)) {
            return;
        }

        // GameMode = CurrentLevel.Mode;
        // GameLimit = CurrentLevel.Limit;
        
        // 下一轮出牌的人默认设置为Landlord
        NextTurnFirst = CurrentLevel.Landlord;
        PassTimes = 0;
        PassLimitNum = CurrentLevel.PassLimit;
        LeadLimitNum = CurrentLevel.LeadLimit;
        PassRequestNum = CurrentLevel.PassRequest;
        PlayRequestNum = CurrentLevel.LeadRequest;
        
        // 将玩家名称字符串与列表中的index存储成字典
        for (var i = 0; i < CurrentLevel.Players.Count; i++) {
            PlayerIndexMap.Add(CurrentLevel.Players[i], i);
        }
        
        // // [Test] 测试PlayerIndexMap是否加载
        // foreach (var kv in PlayerIndexMap) {
        //     GD.Print($"{kv.Key}: {kv.Value}");
        // }
        
        // 预先赋值CurrentState
        CurrentState = CurrentLevel.Landlord == "YOU" ? TurnStates.First : TurnStates.Follow;

        // 获取当前游戏的模式
        GameMode = CurrentLevel.Mode;
        
        // 获取Rule接口类
        CurrentRule = RuleTool.GetRule(CurrentLevel.Rule);
        
        // 获取Ai接口类
        Ai = AiTool.GetAi(GetWhoseName("YOU", 1));
        if (CurrentLevel.Number == 3) {
            Ai2 = AiTool.GetAi(GetWhoseName("YOU", 2));
        }
        
        // // [Test] 检测CurrentRule是否存在
        // GD.Print(CurrentRule);
        // GD.Print(CurrentRule != null);
        
        LevelLoadedEvent?.Invoke();
    }
    
    public static async void FirstTurn() {
        // 玩家先出牌（直接跳过）
        if (CurrentLevel.Landlord == "YOU") {
            CurrentState = TurnStates.First;
            
            // 记录的第一条状态，即开局的状态
            AddRoundRecord();
            
            // 还是没必要在出牌的时候也做一个提示，主要有些关卡提示了就没意思了……（如四带两对的关卡）
            // 而且也的确有概率出更多的bug，还是算了
            PlayerLeadEvent?.Invoke(null);
            return;
        }
        // Ai先出牌
        var landlord = CurrentLevel.Landlord;
        var possCombs = CurrentRule.GetAllComb(GetWhoseCards(landlord));
        var shift = GetShift(landlord);
        GD.Print(shift);
        
        // 只有两个人
        if (CurrentLevel.Number == 2) {
            var leadComb = Ai.ChooseToLead(possCombs, landlord);
            CurrentBiggest = leadComb;
            NextTurnFirst = landlord;
            PassTimes = 0;
            
            AiLeadEvent?.Invoke(leadComb);
            // 添加记录
            AddOneTurnInfo(landlord, leadComb);
            SetWhoseCardsByLead(leadComb.Cards, landlord);
            
            if (CheckForWinner(landlord)) {
                return;
            }

            CurrentState = CurrentRule.CanFollow(leadComb, GetWhoseCards("YOU"), out var pcs)
                ? TurnStates.Follow
                : TurnStates.None;
            
            AddRoundRecord();
            
            PlayerLeadEvent?.Invoke(pcs);
            return;
        }
        
        // 三个人
        if (shift == 1) {
            var leadComb = Ai.ChooseToLead(possCombs, landlord);
            CurrentBiggest = leadComb;
            NextTurnFirst = landlord;
            PassTimes = 0;
            
            AiLeadEvent?.Invoke(leadComb);
            // 添加记录
            AddOneTurnInfo(landlord, leadComb);
            SetWhoseCardsByLead(leadComb.Cards, landlord);
            if (CheckForWinner(landlord)) {
                return;
            }

            await DelayFunc(500);
            
            // AI2
            if (CurrentRule.CanFollow(CurrentBiggest, GetWhoseCards(landlord, 1), out var pcs1)) {
                var leadComb2 = Ai.ChooseToFollow(pcs1, GetWhoseName(landlord, 1));
                CurrentBiggest = leadComb2;
                NextTurnFirst = GetWhoseName(landlord, 1);
                PassTimes = 0;
                
                Ai2LeadEvent?.Invoke(leadComb2);
                // 添加记录
                AddOneTurnInfo(NextTurnFirst, leadComb2);
                SetWhoseCardsByLead(leadComb2.Cards, landlord, 1);
                if (CheckForWinner(GetWhoseName(landlord, 1))) {
                    return;
                }
            }
            else {
                PassTimes += 1;
                Ai2LeadEvent?.Invoke(null); // pass 肯定不会赢，不考虑
                // 添加记录
                AddOneTurnInfo(GetWhoseName(landlord, 1), null);
            }
        }
        else if (shift == 2) {
            var leadComb = Ai2.ChooseToLead(possCombs, landlord);
            CurrentBiggest = leadComb;
            NextTurnFirst = landlord;
            PassTimes = 0;
            Ai2LeadEvent?.Invoke(leadComb);
            // 添加记录
            AddOneTurnInfo(landlord, leadComb);
            SetWhoseCardsByLead(leadComb.Cards, landlord);
            if (CheckForWinner(landlord)) {
                return;
            }
        }
        
        CurrentState = CurrentRule.CanFollow(CurrentBiggest, GetWhoseCards("YOU"), out var pcs2)
            ? TurnStates.Follow
            : TurnStates.None;
        
        AddRoundRecord();
        
        PlayerLeadEvent?.Invoke(pcs2);
    }
    
    /// <summary>
    /// 永远是ai和ai2的出牌顺序
    /// </summary>
    public static async void NextTurn() {
        // 玩家牌出完了（不是LeadEvent！）
        PlayerLeadOver?.Invoke();
        
        // 永远是玩家最先出牌/pass
        // [2] 玩家 lead, ai 跟牌；玩家 pass，ai 出牌
        
        // 以下是AI的操作
        if (CurrentLevel.Number == 2) {
            // 2个人，注定是玩家pass，新出牌
            if (PassTimes == 1) {
                CurrentBiggest = null;
                
                await DelayFunc();
                TurnOverEvent?.Invoke();
                
                AiAction(1, TurnStates.First, null);
                // CheckForWinner 必须相对与 NextTurn 函数 return
                if (CheckForWinner(GetWhoseName("YOU", 1))) {
                    return;
                }
            }
            else {
                // 玩家出牌，ai1跟牌，玩家跟牌
                if (CurrentRule.CanFollow(CurrentBiggest, GetWhoseCards("YOU", 1), out var pcs)) {
                    AiAction(1, TurnStates.Follow, pcs);
                
                    if (CheckForWinner(GetWhoseName("YOU", 1))) {
                        return;
                    }
                
                    // PlayerAction(false);
                }
                else { // ai1 pass，玩家出牌
                    AiAction(1, TurnStates.None, null);

                    // PlayerAction(true);
                }
            }

            if (PassTimes == 1) {
                CurrentBiggest = null; // 已经确定下一局了，不算Ai的action
                
                await DelayFunc(1000); // Ai Pass 时间长一些？
                TurnOverEvent?.Invoke(); // 延时与是否清空也没有关系？
                
                PlayerAction(true);
            }
            else {
                PlayerAction(false);
            }

            return;
        }
        
        // DONE：三个人的情况
        // [3] 1. ai2牌权，玩家 pass, ai1 pass，ai2出牌: passT = 1, ai pass, ai2
        //     2. ai1牌权，ai2 pass， 玩家pass，ai1出牌: passT = 2, ai1
        //     3. 玩家出牌，ai1 pass， ai2pass，玩家出牌: passT = 0, 最后判断
        // 其余情况，只要有人出牌，其余人都得跟牌
        
        // 另一种理解方式：到我时，passT=2，说明我出牌，否则我跟牌
        // Turn模式下，如果到我了，才需要去思考是否轮到我
        
        // AI1
        if (PassTimes == 2) {
            // Ai1出牌
            CurrentBiggest = null;
            
            await DelayFunc(1000);
            TurnOverEvent?.Invoke();
            
            AiAction(1, TurnStates.First, null);
            // CheckForWinner 必须相对与 NextTurn
            if (CheckForWinner(GetWhoseName("YOU", 1))) {
                return;
            }
        }
        else {
            // ai1跟牌
            if (CurrentRule.CanFollow(CurrentBiggest, GetWhoseCards("YOU", 1), out var pcs)) {
                AiAction(1, TurnStates.Follow, pcs);
                
                if (CheckForWinner(GetWhoseName("YOU", 1))) {
                    return;
                }
            }
            else {
                AiAction(1, TurnStates.None, null);
            }
        }

        await DelayFunc(500);
        
        // Ai2
        if (PassTimes == 2) {
            // Ai2出牌
            CurrentBiggest = null;
            
            await DelayFunc(1000);
            TurnOverEvent?.Invoke();
            
            AiAction(2, TurnStates.First, null);
            // CheckForWinner 必须相对与 NextTurn
            if (CheckForWinner(GetWhoseName("YOU", 2))) {
                return;
            }
        }
        else {
            // ai2跟牌
            if (CurrentRule.CanFollow(CurrentBiggest, GetWhoseCards("YOU", 2), out var pcs)) {
                AiAction(2, TurnStates.Follow, pcs);
                
                if (CheckForWinner(GetWhoseName("YOU", 2))) {
                    return;
                }
            }
            else {
                AiAction(2, TurnStates.None, null);
            }
        }
        
        // Player
        if (PassTimes == 2) {
            CurrentBiggest = null;
            
            await DelayFunc(1000);
            TurnOverEvent?.Invoke();
            
            PlayerAction(true);
        }
        else {
            PlayerAction(false);
        }
    }

    /// <summary>
    /// 根据名称和偏移量获得下一个人的名称
    /// </summary>
    /// <param name="name"></param>
    /// <param name="shift"></param>
    /// <returns></returns>
    public static string GetWhoseName(string name, int shift) {
        return CurrentLevel.Players[(CurrentLevel.Players.IndexOf(name) + shift) % CurrentLevel.Number];
    }
    
    /// <summary>
    /// 通过player名称获取对应的手牌
    /// </summary>
    /// <param name="name">人物名称</param>
    /// <param name="shift">相对该人物往后的移位，默认是0</param>
    /// <returns></returns>
    public static List<CardData> GetWhoseCards(string name, int shift = 0) {
        // foreach (var cd in CurrentLevel.Cards[PlayerIndexMap[name]]) {
        //     GD.Print(cd);
        // }
        
        // 一个事实是：必须加载Level后再Sort，因为Sort的规则由Rule决定
        
        CurrentRule.SortInHand(CurrentLevel.Cards[(PlayerIndexMap[name] + shift) % CurrentLevel.Number]);
        return CurrentLevel.Cards[(PlayerIndexMap[name] + shift) % CurrentLevel.Number];
    }

    public static void SetWhoseCardsByLead(List<CardData> leadCards, string name, int shift = 0) {
        // CurrentRule.SortInHand(leadCards);
        foreach (var cd in leadCards) {
            CurrentLevel.Cards[(PlayerIndexMap[name] + shift) % CurrentLevel.Number].Remove(cd);
        }
    }

    // 撤回时需要用到的回退函数
    public static void SetWhoseCardsByUndo(List<CardData> leadBackCards, string name, int shift = 0) {
        var cardsInHand = CurrentLevel.Cards[(PlayerIndexMap[name] + shift) % CurrentLevel.Number];
        cardsInHand.AddRange(leadBackCards);
        CurrentRule.SortInHand(cardsInHand);
    }

    public static void Undo() {
        // 无论是不是地主，第一条记录肯定不考虑恢复（只有第一条记录可能有空Turns和Combs）
        if (RoundRecord.Count <= 1) {
            UndoFailed?.Invoke();
            return;
        }
        var lastTurnInfo = RoundRecord[^1];
        var tmpTurnInfo = RoundRecord[^2];
        // 拷贝构造函数必须非null（毕竟是初始化new)
        CurrentBiggest = tmpTurnInfo.CurrentBiggest != null ? new CombData(tmpTurnInfo.CurrentBiggest) : null;
        CurrentState = tmpTurnInfo.CurrentState;
        NextTurnFirst = tmpTurnInfo.NextTurnFirst;
        PassTimes = tmpTurnInfo.PassTimes;
        PassLimitNum = tmpTurnInfo.PassLimitNum;
        LeadLimitNum = tmpTurnInfo.PlayLimitNum;
        PassRequestNum = tmpTurnInfo.PassRequestNum;
        PlayRequestNum = tmpTurnInfo.PlayRequestNum;

        // 但是，GameLogic中的Cards回退用的是上一局的出牌！
        for (int i = 0; i < lastTurnInfo.Turns.Count; i++) {
            if (lastTurnInfo.Combs[i] == null) continue;
            SetWhoseCardsByUndo(lastTurnInfo.Combs[i].Cards, GetWhoseName(lastTurnInfo.Turns[i], 0));
        }
        
        // lead需要恢复的是上上一次的情形！且PlayerLead不需要考虑！
        UndoEvent?.Invoke(RoundRecord[^2]);
        
        // 删除倒数第一个
        RoundRecord.RemoveAt(RoundRecord.Count - 1);
        
        // [Debug]
        // foreach (var oneTurnInfo in RoundRecord) {
        //     GD.Print(oneTurnInfo);
        // }
        // foreach (var cardList in CurrentLevel.Cards) {
        //     CardTool.PrintCardDataList(cardList);
        // }
    }
    
    /// <summary>
    /// 获得AI相对玩家的偏移值
    /// </summary>
    /// <param name="name"></param>
    /// <param name="according"></param>
    /// <returns></returns>
    public static int GetShift(string name, string according = "YOU") {
        var num = CurrentLevel.Players.IndexOf(name) - CurrentLevel.Players.IndexOf(according);
        // [2] you, ai1; ai1, you
        // [3] you, ai1, ai2; ai2, you, ai1, ai1, ai2, you
        if (num < 0) {
            num += CurrentLevel.Number;
        }

        return num;
    }

    public static void AiAction(int i, TurnStates turn, List<CombData> pcs = null) {
        if (i != 1 && i != 2) return;
        if (turn == TurnStates.Follow && pcs == null) return;
        
        switch (turn) {
            case TurnStates.First: {
                var possCombs = CurrentRule.GetAllComb(GetWhoseCards("YOU", i));
                var leadComb = i == 1
                    ? Ai.ChooseToLead(possCombs, GetWhoseName("YOU", i))
                    : Ai2.ChooseToLead(possCombs, GetWhoseName("YOU", i));
                CurrentBiggest = leadComb;
                NextTurnFirst = GetWhoseName("YOU", i);
                PassTimes = 0;
    
                if (i == 1) AiLeadEvent?.Invoke(leadComb);
                else Ai2LeadEvent?.Invoke(leadComb);
                // 添加记录
                AddOneTurnInfo(GetWhoseName("YOU", i), leadComb);
                // if (leadComb != null) ;
                SetWhoseCardsByLead(leadComb.Cards, "YOU", i);
                break;
            }
            case TurnStates.Follow: {
                var leadComb = i == 1
                    ? Ai.ChooseToFollow(pcs, GetWhoseName("YOU", i))
                    : Ai2.ChooseToFollow(pcs, GetWhoseName("YOU", i));
                
                // 原来GameLogic也有一样的问题……
                if (leadComb == null) { 
                    PassTimes += 1;
                    if (i == 1) AiLeadEvent?.Invoke(null);
                    else Ai2LeadEvent?.Invoke(null);
                    AddOneTurnInfo(GetWhoseName("YOU", i), null);
                    return;
                }
                
                CurrentBiggest = leadComb;
                NextTurnFirst = GetWhoseName("YOU", i);
                PassTimes = 0;
                
                if (i == 1) AiLeadEvent?.Invoke(leadComb);
                else Ai2LeadEvent?.Invoke(leadComb);
                // 添加记录
                AddOneTurnInfo(GetWhoseName("YOU", i), leadComb);
                // CardTool.PrintCardDataList(leadComb.Cards);
                SetWhoseCardsByLead(leadComb.Cards, "YOU", i);
            }
                break;
            case TurnStates.None: {
                PassTimes += 1;
                // Pass 不考虑赢的可能
                if (i == 1) AiLeadEvent?.Invoke(null);
                else Ai2LeadEvent?.Invoke(null);
                // 添加记录
                AddOneTurnInfo(GetWhoseName("YOU", i), null);
            }
                break;
        }
    }

    public static void PlayerAction(bool isFirst) {
        if (isFirst) {
            CurrentState = TurnStates.First;
            AddRoundRecord();
            
            PlayerLeadEvent?.Invoke(null);
            return;
        }
        
        CurrentState = CurrentRule.CanFollow(CurrentBiggest, GetWhoseCards("YOU"), out var pcs)
            ? TurnStates.Follow
            : TurnStates.None;
        AddRoundRecord();
        
        PlayerLeadEvent?.Invoke(pcs);
    }

    /// <summary>
    /// 需要在玩家出牌、记录，且没有调用NextTurn的时候调用
    /// </summary>
    /// <returns></returns>
    public static bool CheckForCountIfLose() {
        // if (PassLimitNum == 999 && LeadLimitNum == 999) return false; // 反正也达不到
        if (PassLimitNum >= 0 && LeadLimitNum >= 0) return false;
        AddRoundRecord();
        RoundOver?.Invoke(false,
            PassLimitNum < 0
                ? TranslationServer.Translate("OVER_LOSE_PASSLMT")
                // PlayerLeadCount > LeadLimitNum
                : TranslationServer.Translate("OVER_LOSE_PLAYLMT"));

        return true;
    }
    
    // GameLogic中只需要对Ai进行判断，对Player的判断在PlayerPlace中
    public static bool CheckForWinner(string leader) {
        if (GetWhoseCards(leader).Count == 0) {
            // 胜利的时候肯定需要再记录一下，无论是谁赢了（ai2赢了会return出去）
            AddRoundRecord();
            
            // 玩家赢了：最先出完的；或者不是地主，且leader也不是（无所谓2/3人）
            if (leader == "YOU" || CurrentLevel.Landlord != "YOU" && CurrentLevel.Landlord != leader) {
                // GD.Print("win!");
                // 如果是Rest模式的话，玩家出完反而输了
                if (GameMode != GameModes.Rest) {
                    if (PlayRequestNum > 0) {
                        RoundOver?.Invoke(false, TranslationServer.Translate("OVER_LOSE_PLAYREQ"));
                    }
                    else if (PassRequestNum > 0) {
                        RoundOver?.Invoke(false, TranslationServer.Translate("OVER_LOSE_PASSREQ"));
                    }
                    else {
                        // DataLoader.CurrentSave[""]
                        RoundOver?.Invoke(true,
                            leader == "YOU"
                                ? TranslationServer.Translate("OVER_WIN_NORMAL")
                                : TranslationServer.Translate("OVER_WIN_NORMAL_P"));
                    }
                }
                else {
                    RoundOver?.Invoke(false,
                        leader == "YOU"
                            ? TranslationServer.Translate("OVER_LOSE_REST")
                            : TranslationServer.Translate("OVER_LOSE_REST_P"));
                }
                
            }
            // 否则，玩家输了
            else {
                // GD.Print("lose...");
                if (GameMode != GameModes.Rest) {
                    RoundOver?.Invoke(false, TranslationServer.Translate("OVER_LOSE_NORMAL"));
                }
                else {
                    if (PlayRequestNum > 0) {
                        RoundOver?.Invoke(false, TranslationServer.Translate("OVER_LOSE_PLAYREQ"));
                    }
                    else if (PassRequestNum > 0) {
                        RoundOver?.Invoke(false, TranslationServer.Translate("OVER_LOSE_PASSREQ"));
                    }
                    else {
                        RoundOver?.Invoke(true, TranslationServer.Translate("OVER_WIN_REST"));
                    }
                }
            }

            foreach (var oneTurnInfo in RoundRecord) {
                GD.Print(oneTurnInfo);
            }

            return true;
        }

        return false;
    }

    // public static void LoadToSaveDic() {
    //     // 在GS中完成吧……
    // }
    
    public static async Task DelayFunc(int delayMs = 1000) {
        await Task.Delay(delayMs); // 比500略长一些
    }

    /// <summary>
    /// 记录在PlayerLead之前
    /// </summary>
    public static void AddRoundRecord() {
        RoundRecord.Add(new OneTurnInfo() {
            Combs = TmpCombs,
            Turns = TmpTurns,
            CurrentBiggest = CurrentBiggest,
            NextTurnFirst = NextTurnFirst,
            CurrentState = CurrentState,
            PassTimes = PassTimes,
            PassLimitNum = PassLimitNum,
            PlayLimitNum = LeadLimitNum,
            PlayRequestNum = PlayRequestNum,
            PassRequestNum = PassRequestNum
        });
        TmpTurns = new();
        TmpCombs = new();
    }

    /// 记录在每次出牌之后
    public static void AddOneTurnInfo(string turn, CombData comb) {
        TmpTurns.Add(turn);
        TmpCombs.Add(comb);
    }
    
}