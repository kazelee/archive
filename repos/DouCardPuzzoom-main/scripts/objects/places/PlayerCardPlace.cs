using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;

public partial class PlayerCardPlace : Node2D {
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
	}

	public async void InitCards(List<CardData> cardDatas) {
		WillBeLeadCardDatas = new();
		
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
			// cardP 相对 P 的 y 位置是 80，deck 相对 P 的 y 位置是 27，即初始化在 -53 处
			// 中心的 deck 初始缩放是 0.6f
			cardVisual.Position = new Vector2(0, -53);
			cardVisual.Scale = new Vector2(0.6f, 0.6f);
			
			// 卡牌相对 place 的位置
			var newPosition = Vector2.Right * (i - (cardDatas.Count - 1) / 2f) * ConstManager.CardSideDistance;
			// cardVisual.Position = newPosition; // 如使用动画需去除该语句
			
			// 参考：https://www.reddit.com/r/godot/comments/167sytd/whats_the_appropriate_way_to_remove_a_tween/

			var tween = GetTree().CreateTween(); // 可以避免清除警告？
			tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
			tween.TweenProperty(cardVisual, "position", newPosition, 0.3);
			tween.TweenProperty(cardVisual, "scale", Vector2.One, 0.1);
			// tween.Parallel().TweenProperty 的效果才是原来预期的，但感觉没有分开的效果好
			
			await DelayFunc(10);
			
			cardVisual.CardSelectedEvent += SelectCards;
		}
	}
	
	public void UpdateCards(List<CardData> cardDatas) {
		WillBeLeadCardDatas = new();
		// // [Test]
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
			var newPosition = Vector2.Right * (i - (cardDatas.Count - 1) / 2f) * ConstManager.CardSideDistance;
			cardVisual.Position = newPosition; // 如使用动画需去除该语句
			
			cardVisual.CardSelectedEvent += SelectCards;
		}
	}

	public async Task DelayFunc(int delayMs) {
		await Task.Delay(delayMs);
	}
	
	public void SelectCards(CardVisual cardVisual) {
		if (!SelectedCards.Contains(cardVisual)) {
			SelectedCards.Add(cardVisual);
		}
	}

	public void AfterSelectCards() {
		// [Test] 测试打印每次选取的图像
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

		// // [Test]
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

	public void HintSelect(List<CardData> hintList) {
		// 清除已有的选择
		foreach (var node in GetChildren()) {
			if (node is not CardVisual visual) continue;
			visual.IsSelected = false;
			visual.AfterSelected();
		}

		WillBeLeadCardDatas = new();
		
		// 匹配现有牌
		foreach (var cd in hintList) {
			foreach (var node in GetChildren()) {
				if (node is not CardVisual visual) continue;
				if (visual.Data != cd || visual.IsSelected) continue;
				visual.IsSelected = true;
				visual.AfterSelected();
				break;
			}
		}
		WillBeLeadCardDatas = hintList;
	}

	public void HintSelect(List<string> hintList) {
		// 清除已有的选择
		foreach (var node in GetChildren()) {
			if (node is not CardVisual) continue;
			((CardVisual)node).IsSelected = false;
			((CardVisual)node).AfterSelected();
		}

		WillBeLeadCardDatas = new();
		
		// 匹配现有牌
		foreach (var cd in hintList) {
			foreach (var node in GetChildren()) {
				if (node is not CardVisual visual) continue;
				if (visual.Data.PointNum != CardTool.GetPointNumUnsafe(cd) ||
					visual.IsSelected) continue;
				visual.IsSelected = true;
				visual.AfterSelected();
				WillBeLeadCardDatas.Add(visual.Data);
				break;
			}
		}
	}
}
