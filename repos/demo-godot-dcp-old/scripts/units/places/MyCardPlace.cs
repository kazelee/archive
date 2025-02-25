using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.managers;
using DouCardPuzzoom.scripts.utils;

public partial class MyCardPlace : Node2D {
    /// <summary>
    /// 每次选中的卡牌列表，每次选完后清零
    /// </summary>
    public List<CardVisual> SelectedCards = new();
    
    /// <summary>
    /// 最终选出的卡牌数据列表，每次出牌后清零
    /// </summary>
    public List<CardData> WillBeLeadCardDatas = new();
    
    // 最终选出的卡牌列表，每次选完后清零（私有）
    // public List<CardVisual> WillBeLeadCards = new();
    // 【补充】直接重新刷新牌组即可，无需再搜索删除

    /// <summary>
    /// 实时记录当前手牌，后续出牌后会更新此值
    /// </summary>
    public List<CardData> CardsInHand = new();
    
    public override void _Ready() {
        base._Ready();
        var test = new List<CardData> {
            new(SuitNums.Heart, PointNums.N5),
            new(SuitNums.Diamond, PointNums.K),
            new(SuitNums.Joker, PointNums.CJ),
            new(SuitNums.Club, PointNums.N10),
            new(SuitNums.Spade, PointNums.N6),
            new(SuitNums.Club, PointNums.N7),
            new(SuitNums.Heart, PointNums.A),
            new(SuitNums.Heart, PointNums.Q),
            new(SuitNums.Diamond, PointNums.N10),
            new(SuitNums.Joker, PointNums.BJ),
            new(SuitNums.Club, PointNums.N2),
            new(SuitNums.Spade, PointNums.J),
            new(SuitNums.Club, PointNums.N9),
            new(SuitNums.Heart, PointNums.A),
            new(SuitNums.Heart, PointNums.Q),
            new(SuitNums.Diamond, PointNums.N10),
            new(SuitNums.Joker, PointNums.BJ),
            new(SuitNums.Club, PointNums.N2),
            new(SuitNums.Spade, PointNums.J),
            new(SuitNums.Club, PointNums.N9),
        };
        test = CardTool.Sorted(test);
        CardsInHand = test;
        UpdateCards(test);
    }

    public void UpdateCards(List<CardData> cardDatas) {
        // [Test]
        // foreach (var cardData in cardDatas) {
        //     GD.Print(cardData);
        // }

        foreach (var node in GetChildren()) {
            if (node.Owner == null) {
                node.QueueFree();
            }
        }

        if (cardDatas.Count == 0) {
            return;
        }

        for (int i = 0; i < cardDatas.Count; i++) {
            var cardVisual = new CardVisual();

            if (i == cardDatas.Count - 1) {
                cardVisual.IsLastOne = true;
            }
            
            cardVisual.SetCardData(cardDatas[i]);
            AddChild(cardVisual);
            // 卡牌相对 place 的位置
            var newPosition = Vector2.Right *(i - (cardDatas.Count - 1) / 2f) * ConstManager.CardSideDistance;
            cardVisual.Position = newPosition; // 如使用动画需去除该语句
            
            // 刷新牌会重新播放动画，原则上应该分两个函数处理
            // 考虑到「少就是多」的极简解谜游戏设计原则，决定直接去除喧宾夺主的动画
            // var tween = CreateTween();
            // tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
            // tween.TweenProperty(cardVisual, "position", newPosition, 0.3);
            
            cardVisual.CardSelectedEvent += SelectCards;
        }
    }

    public void SelectCards(CardVisual cardVisual) {
        if (!SelectedCards.Contains(cardVisual)) {
            SelectedCards.Add(cardVisual);
        }
    }

    public void AfterSelectCards() {
        foreach (var card in SelectedCards) {
            // 选中的卡牌改变状态（选择/不选择）
            card.IsSelected = !card.IsSelected;
            card.AfterSelected();

            if (!WillBeLeadCardDatas.Contains(card.Data)) {
                WillBeLeadCardDatas.Add(card.Data);
                // WillBeLeadCards.Add(card);
            }
            else {
                WillBeLeadCardDatas.Remove(card.Data);
                // WillBeLeadCards.Remove(card);
            }
        }

        // [Test]
        // // Linq 表达式，与注释的 foreach 语句相同
        // var cardDatas = WillBeLeadCardDatas.Aggregate("", (current, cardData) => current + $"{cardData} ");
        // // foreach (var cardData in WillBeLeadCards) {
        // //     cardDatas += $"{cardData} ";
        // // }
        // GD.Print(cardDatas);
        
        // 确定鼠标 release 后，清空暂时选择的牌列表
        SelectedCards = new List<CardVisual>();
    }

    public void AfterUploadCards() {
        foreach (var cardData in WillBeLeadCardDatas) {
            CardsInHand.Remove(cardData);
        }
        UpdateCards(CardsInHand);
        // 上传之后，清空预备出的牌
        WillBeLeadCardDatas = new List<CardData>();
    }
    
}
