using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;

public partial class PlayerPlace : Node2D {
    /// <summary>
    /// bool: isPass
    /// </summary>
    public event Action<bool> UpdateLimitNum; 
    
    public PlayerCardPlace PlayerCardPlace;
    public LeadCardBar LeadCardBar;
    public LeadPlace LeadPlace;

    public Label HintText;

    // 记录提示列表的内容
    public List<CombData> HintList = new();
    public int HintIndex = 0;

    public Tween HintTextTween;
    
    public override void _Ready() {
        PlayerCardPlace = GetNode<PlayerCardPlace>("PlayerCardPlace");
        LeadCardBar = GetNode<LeadCardBar>("LeadCardBar");
        LeadPlace = GetNode<LeadPlace>("PlayerLeadPlace");

        HintText = GetNode<Label>("HintText");
        HintText.Hide();

        // 初始化时位置就在右侧
        // LeadCardBar.Position = new Vector2(LeadCardBar.Position.X + 20, LeadCardBar.Position.Y);
        // LeadCardBar.Position = new Vector2(-29, 27);
        // LeadCardBar.Modulate = new Color(1, 1, 1, 0);
        
        // LeadCardBar.InitButtons();
        LeadCardBar.HasLedCards += UploadCards;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } button) {
            if (button.IsPressed()) {
                MouseManager.Dragging = true;
            }
            else if (button.IsReleased()) {
                MouseManager.Dragging = false;
                PlayerCardPlace.AfterSelectCards();
            }
        }
    }

    public void InitCards(List<CardData> cardDatas) {
        // foreach (var cd in cardDatas) {
        //     GD.Print(cd);
        // }
        PlayerCardPlace.CardsInHand = cardDatas;
        PlayerCardPlace.InitCards(cardDatas);
    }

    // 严格意义上，应该在没有牌可以跟的时候提示“您没有牌打过上家”
    // 同时，如果选择不出的话，也应该清空当前选牌
    // 但还是不再更改，保留这些设计，毕竟玩家其实也不需要（尤其是在Undo的时候）
    
    public async void ReadyForLead(List<CombData> possCombs) {
        if (possCombs != null && possCombs.Count != 0) {
            HintList = possCombs;
        }
        await DelayFunc();
        LeadPlace.Clear();
        // 先清除卡牌，再显示出牌栏 ×
        // 直接等待一段时间，然后同时清除卡牌并显示出牌栏 √
        LeadCardBar.InitButtons();
        LeadCardBar.MyShow();
        // ShowBar();
    }

    // public void ShowBar() {
    //     // 参见 LeadCardBar.MyShow();
    //     var tween = CreateTween();
    //     tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
    //     tween.TweenProperty(LeadCardBar, "position", new Vector2(-49, 27), 0.3);
    //     tween.Parallel().TweenProperty(LeadCardBar, "modulate:a", 1.0, 0.2);
    //     tween.TweenCallback(Callable.From(LeadCardBar.Show));
    // }
    
    public void UploadCards(LeadStates leadState) {
        HintTextTween?.Kill(); // if not null
        HintText.Hide(); // 太快的话就不能及时hide了…… 
        
        switch (leadState) {
            case LeadStates.Play:
                var leadCards = PlayerCardPlace.WillBeLeadCardDatas;
                // // [Test] 输出选择的牌列表
                // var tmpCardStr = "";
                // foreach (var card in leadCards) {
                //     tmpCardStr += $"{card} ";
                // }
                // GD.Print(tmpCardStr);
                
                // 出牌不允许不选牌
                if (leadCards.Count == 0) {
                    HintText.Text = TranslationServer.Translate("ERR_NOCARD");
                    HintText.Modulate = new Color(1, 1, 1, 0);
                    HintText.Show();
                    
                    var soundManager = GetNode<SoundManager>("/root/SoundManager");
                    soundManager.PlaySoundEffects("bell");
                    
                    HintTextTween = GetTree().CreateTween();
                    HintTextTween.TweenProperty(HintText, "modulate:a", 1.0, 0.3);
                    HintTextTween.TweenProperty(HintText, "modulate:a", 0.0, 1.0).SetDelay(1);
                    HintTextTween.TweenCallback(Callable.From(HintText.Hide));
                    return;
                }
                
                // 出牌必须合法
                if (!GameLogic.CurrentRule.IsCombValid(leadCards, out var toLeadComb)) {
                    HintText.Text = TranslationServer.Translate("ERR_INVALID");
                    HintText.Modulate = new Color(1, 1, 1, 0);
                    
                    var soundManager = GetNode<SoundManager>("/root/SoundManager");
                    soundManager.PlaySoundEffects("bell");
                    
                    HintText.Show();
                    HintTextTween = GetTree().CreateTween();
                    HintTextTween.TweenProperty(HintText, "modulate:a", 1.0, 0.3);
                    // await DelayFunc();
                    // tween.TweenCallback(Callable.From(async () => { await DelayFunc(); }));
                    // tween.TweenProperty(HintText, "modulate:a", 1.0, 1.0); // 不变但占用时长
                    HintTextTween.TweenProperty(HintText, "modulate:a", 0.0, 1.0).SetDelay(1);
                    HintTextTween.TweenCallback(Callable.From(HintText.Hide));
                    // 不用清除willbelead，玩家有可能是不小心选错了，清零会影响体验
                    return;
                }
                
                // 如果第一个出牌（所以这里不需要State？）
                if (GameLogic.CurrentBiggest == null) {
                    // GD.Print("1");
                    GameLogic.CurrentBiggest = toLeadComb;
                    GameLogic.NextTurnFirst = "YOU";
                }
                // 如果跟牌且能打过
                else if (GameLogic.CurrentRule.CanBeat(toLeadComb, GameLogic.CurrentBiggest)) {
                    // GD.Print("2");
                    GameLogic.CurrentBiggest = toLeadComb;
                    GameLogic.NextTurnFirst = "YOU";
                }
                // 打不过，重新选牌
                else {
                    HintText.Text = TranslationServer.Translate("ERR_UNBEAT");
                    HintText.Modulate = new Color(1, 1, 1, 0);
                    HintText.Show();
                    
                    var soundManager = GetNode<SoundManager>("/root/SoundManager");
                    soundManager.PlaySoundEffects("bell");
                    
                    HintTextTween = GetTree().CreateTween();
                    HintTextTween.TweenProperty(HintText, "modulate:a", 1.0, 0.3);
                    HintTextTween.TweenProperty(HintText, "modulate:a", 0.0, 1.0).SetDelay(1);
                    HintTextTween.TweenCallback(Callable.From(HintText.Hide));
                    
                    // GD.Print("3");
                    // CardTool.PrintCardDataList(GameLogic.CurrentBiggest.Cards, "YOU");
                    // foreach (var kv in toLeadComb.Types) {
                    //     GD.Print($"{kv.Key}: {kv.Value}");
                    // }
                    // foreach (var kv in GameLogic.CurrentBiggest.Types) {
                    //     GD.Print($"{kv.Key}: {kv.Value}");
                    // }
                    return;
                }
                
                // 为出牌排序cards list
                if (GameLogic.CurrentLevel.Rule == "RuleMath") {
                    // 只有Math需要额外设置排序方案
                    GameLogic.CurrentRule.SortToLead(leadCards, toLeadComb.Types);
                }
                else {
                    GameLogic.CurrentRule.SortToLead(leadCards);
                }
                
                // [Debug] 打印牌和类型
                CardTool.PrintCardDataList(leadCards, "YOU");
                var tmpT = "[Types] ";
                foreach (var kv in toLeadComb.Types) {
                    // GD.Print($"{kv.Key}: {kv.Value}");
                    tmpT += $"{kv.Key}: {kv.Value}; ";
                }
                GD.Print(tmpT);
                
                // GD.PrintS(leadCards);
                LeadPlace.UpdateCards(leadCards);
                PlayerCardPlace.AfterUploadCards();
                
                GameLogic.PassTimes = 0; // 实际上没必要移动位置，AddOneTurn不加passT
                // 添加记录
                GameLogic.AddOneTurnInfo("YOU", toLeadComb);
                GameLogic.SetWhoseCardsByLead(leadCards, "YOU");
                LeadCardBar.MyHide();
                HintIndex = 0;

                GameLogic.LeadLimitNum -= 1;
                GameLogic.PlayRequestNum -= 1;
                // 检查是否超了，输了就return
                if (GameLogic.CheckForCountIfLose()) {
                    return;
                }
                // 回旋镖这下来了，不是说不需要判断的吗？（乐）
                UpdateLimitNum?.Invoke(true); // isPass = true（实际上为了更新play req）
                
                // 在Player出牌时就检测，防止直接进入NextTurn（这样GameLogic的逻辑就不用写了）
                if (GameLogic.CheckForWinner("YOU")) {
                    return;
                }
                
                // 更新 GS UI 数值（放在输赢判断后面，这样最终出牌赢了不会减1，过牌赢了无所谓）
                UpdateLimitNum?.Invoke(false);
                
                GameLogic.NextTurn();
                
                break;
            
            case LeadStates.Hint:
                if (HintList == null || HintList.Count == 0) {
                    GD.PrintErr("HintList null or size 0!");
                    return;
                }
                // Test: cards不行，可能是Str……
                // CardTool.PrintCardDataList(HintList[0].Cards);
                // var tmp = "";
                // foreach (var s in HintList[0].RuleList) {
                //     tmp += $"{s} ";
                // }
                // GD.Print(tmp);
                
                if (HintList[HintIndex].IsSuitSensitive) {
                    PlayerCardPlace.HintSelect(HintList[HintIndex].Cards);
                }
                else {
                    PlayerCardPlace.HintSelect(HintList[HintIndex].RuleList);
                    // var tmp = "";
                    // foreach (var s in HintList[HintIndex].RuleList) {
                    //     tmp += $"{s} ";
                    // }
                    // GD.Print(tmp);
                    // CardTool.PrintCardDataList(PlayerCardPlace.WillBeLeadCardDatas, "YOU");
                }

                HintIndex = (HintIndex + 1) % HintList.Count;
                break;
            
            case LeadStates.Pass:
                LeadPlace.Pass();
                LeadCardBar.MyHide();
                HintIndex = 0;
                GD.Print("YOU: PASS");
                
                GameLogic.PassTimes += 1;
                // 添加记录
                GameLogic.AddOneTurnInfo("YOU", null);

                GameLogic.PassLimitNum -= 1;
                GameLogic.PassRequestNum -= 1;
                if (GameLogic.CheckForCountIfLose()) {
                    return;
                }
                UpdateLimitNum?.Invoke(true);
                
                GameLogic.NextTurn();
                break;
        }
    }

    public async Task DelayFunc() {
        await Task.Delay(ConstManager.DelayMs);
    }
    
}
