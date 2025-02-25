using Godot;
using System;
using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;


public partial class LeadPlace : Node2D {
    [Export] public PlacePositions Place = PlacePositions.Down;
    public TextureRect PassRect;

    public override void _Ready() {
        base._Ready();
        PassRect = GetNode<TextureRect>("PassRect");

        if (TranslationServer.GetLocale() is "zh_CN" or "zh_TW") {
            PassRect.Texture = ResourceLoader.Load<Texture2D>("res://assets/buttons/pass-cn.png");
        }
        else {
            PassRect.Texture = ResourceLoader.Load<Texture2D>("res://assets/buttons/pass-en.png");
        }
        
        // PassRect.Hide();
        // PassRect.Modulate = new Color(1, 1, 1, 0);
    }

    public void Clear() {
        PassRect.Hide();
        foreach (var node in GetChildren()) {
            if (node.Owner == null) {
                node.QueueFree();
            }
        }
    }

    // 【补充】不需要用一个函数处理两种情况
    // public void ShowLeadCards(List<CardData> cards) {
    //     Clear();
    //     if (cards.Count == 0) {
    //         Label.Show();
    //     }
    //     else {
    //         UpdateCards(cards);
    //     }
    // }

    public void Pass() {
        Clear();
        PassShow();
    }

    // public void PassHide() {
    //     var tween = CreateTween();
    //     tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
    //     // tween.TweenProperty(PassRect, "position", Vector2.Zero, 0.3);
    //     tween.TweenProperty(PassRect, "modulate:a", 0.0, 0.2);
    //     tween.TweenCallback(Callable.From(PassRect.Hide));
    // }

    
    // 注：Rect 是 Control 控件，偏移 -12.5，-5.5
    public void PassShow() {
        var targetPos = Vector2.Zero;
        switch (Place) {
            case PlacePositions.Up:
                targetPos = new Vector2(-12.5f, -29.5f); // 4 + 20 + 5.5
                break;
            case PlacePositions.Left:
                targetPos = new Vector2(-42.5f, -19.5f); // 100 - 70 + 12.5; 4 + 10 + 5.5
                break;
            case PlacePositions.Down:
                targetPos = new Vector2(-12.5f, 39.5f);
                break;
            case PlacePositions.Right:
                targetPos = new Vector2(17.5f, -19.5f); // 100 -70 - 12.5
                break;
        }

        PassRect.Position = targetPos;
        PassRect.Modulate = new Color(1, 1, 1, 0);
        
        PassRect.Show();
        var tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
        tween.TweenProperty(PassRect, "position", new Vector2(-12.5f, -5.5f), 0.2);
        tween.Parallel().TweenProperty(PassRect, "modulate:a", 1.0, 0.2);
    }
    
    /// <summary>
    /// 生成卡牌的方法，后续根据不同位置需要重写
    /// </summary>
    /// <param name="cards">卡牌数据 List（非图像）</param>
    public virtual void UpdateCards(List<CardData> cards, bool isBlind = false) {
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlaySoundEffects("flipcard"); // 玩家和AI公用updatecards，直接播放
        
        Clear(); // ShowLeadCards 函数遗留
        var s = cards.Count;
        for(int i = 0; i < s; i++) {
            var suitName = CardTool.GetSuitName(cards[i].SuitNum);
            var pointName = CardTool.GetPointName(cards[i].PointNum);
            var card = new Sprite2D();
            if (isBlind) {
                card.Texture = GD.Load<Texture2D>("res://assets/pokers/back-orange.png");
            }
            else {
                card.Texture = GD.Load<Texture2D>($"res://assets/pokers/{suitName}-{pointName}.png");    
            }
            card.Scale = Vector2.One * ConstManager.LeadCardScale;
            AddChild(card);
            card.Modulate = new Color(1, 1, 1, 0);
            
            // DONE：动画改成从人物出发
            
            // Lead 相对 Place ±70，4；0，4；需要位置 0,-20; -115,0
            
            // 两排卡牌的上下距离差也取10f（为了看清花色）
            // y: -(i/10) => if y <= 10 then 0 else -1
            var newPosition = new Vector2();
            var upDownDis = 10f;
            switch (Place) {
                case PlacePositions.Down:
                    card.Position = new Vector2(0, 45);
                    newPosition = new Vector2(
                        (i - (s - 1) / 2f) * ConstManager.CardSideDistance * ConstManager.LeadCardScale, 0);
                    break;
                case PlacePositions.Up:
                    card.Position = new Vector2(0, -24);
                    newPosition = new Vector2(
                        (i - (s - 1) / 2f) * ConstManager.CardSideDistance * ConstManager.LeadCardScale, 0);
                    break;
                case PlacePositions.Left:
                    card.Position = new Vector2(-30, -14);
                    newPosition = new Vector2(
                            (i % 10) * ConstManager.CardSideDistance * ConstManager.LeadCardScale,
                            -(i / 10) * upDownDis);
                    break;
                case PlacePositions.Right:
                    card.Position = new Vector2(30, -14);
                    if (s > 10) {
                        // 第二牌靠右侧
                        if (i < 10) {
                            newPosition = new Vector2(
                                (i - 10 + 1) * ConstManager.CardSideDistance * ConstManager.LeadCardScale, -upDownDis);
                        }
                        else {
                            newPosition = new Vector2(
                                (i % 10 - s + 1) * ConstManager.CardSideDistance * ConstManager.LeadCardScale, 0);
                        }
                    }
                    else {
                        newPosition = new Vector2(
                            (i - s + 1) * ConstManager.CardSideDistance * ConstManager.LeadCardScale, 0);
                    }
                    break;
            }
            
            // 直接一把全出即可，没必要做欢乐斗地主那种连对/飞机特有的「平铺」动画
            // 真实的出牌位置初始化应该在「人物」身上，而不是cardPlace……
            
            var tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
            tween.TweenProperty(card, "position", newPosition, 0.2);
            tween.Parallel().TweenProperty(card, "modulate:a", 1.0, 0.2);
        }
    }
    
}
