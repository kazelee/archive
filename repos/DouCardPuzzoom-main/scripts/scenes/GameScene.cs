using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class GameScene : Sprite2D {
	public Button RuleInfo;
	public Button ModeInfo;
	public Button PassNumInfo;
	public Button PlayNumInfo;
	public Button PassNumInfoR;
	public Button PlayNumInfoR;
	public Button LevelNumButton;
	public Button CompleteNumButton;
	
	public AiImage AiImage;
	public AiImage AiImage2;
	public Sprite2D TableLayer;
	public AnimatedSprite2D AiHat;
	
	public PlayerPlace PlayerPlace;
	public AiPlace AiPlace;
	public AiPlace AiPlace2;

	public Sprite2D DeckLayer;
	public bool IsUndoable = false; // 是否可以回退，仅在玩家回合可行
	// public bool IsResetable = false;

	public TextureRect RoundOverScreen;
	public Label WinnerText;
	/// <summary>
	/// 实际上是对输赢情况的提示，历史遗留问题，不改了
	/// </summary>
	public Label CompleteProgress;
	
	public TextureButton GoBackButton;
	public TextureButton RestartButton;
	public TextureButton HomeButton;
	public TextureButton LevelButton;
	public TextureButton LastButton;
	public TextureButton NextButton;

	public LevelChooseScreen LevelChooseScreen;
	public Panel LeavingScreen;
	public TextureButton LeavingScreenEscape;

	public Button ToRoom;
	public Button ToTitle;

	public InfoScreen RuleInfoScreen;
	public ColorRect ColorRect; // 出框禁止其他操作
	public InfoScreen ModeInfoScreen;
	public InfoScreen PassInfoScreen;
	public InfoScreen PlayInfoScreen;
	public InfoScreen PassInfoScreen2;
	public InfoScreen PlayInfoScreen2;
	public InfoScreen LevelInfoScreen;
	public InfoScreen ProgressInfoScreen;

	public AiInfoScreen AiInfoScreen;
	
	public override void _Ready() {
		// var soundManager = GetNode<SoundManager>("/root/SoundManager");
		// soundManager.MusicPlayer.Stop();
		
		// 时刻清除鼠标的 click 效果
		Input.SetCustomMouseCursor(MouseManager.Arrow);
		MouseManager.IsInterAreaAble = true;

		ColorRect = GetNode<ColorRect>("ColorRect");
		
		RuleInfo = GetNode<Button>("LevelInfoBar/RuleInfo");
		ModeInfo = GetNode<Button>("LevelInfoBar/ModeInfo");
		PassNumInfo = GetNode<Button>("LevelInfoBar/PassNumInfo");
		PlayNumInfo = GetNode<Button>("LevelInfoBar/LeadNumInfo");
		PassNumInfoR = GetNode<Button>("LevelInfoBar/PassNumInfoR");
		PlayNumInfoR = GetNode<Button>("LevelInfoBar/LeadNumInfoR");
		LevelNumButton = GetNode<Button>("LevelNumBar/LevelNum");
		CompleteNumButton = GetNode<Button>("LevelNumBar/CompleteNum");
		
		AiImage = GetNode<AiImage>("AiImage1");
		AiImage2 = GetNode<AiImage>("AiImage2");
		TableLayer = GetNode<Sprite2D>("TableLayer");
		AiHat = GetNode<AnimatedSprite2D>("AIHat");
		
		RoundOverScreen = GetNode<TextureRect>("RoundOverScreen");
		WinnerText = GetNode<Label>("RoundOverScreen/WinnerText");
		CompleteProgress = GetNode<Label>("RoundOverScreen/CompleteProgress");

		AiImage.OnAiImageClicked += ShowAiInfo;
		AiImage2.OnAiImageClicked += ShowAiInfo;
		
		PlayerPlace = GetNode<PlayerPlace>("PlayerPlace");
		AiPlace = GetNode<AiPlace>("AiPlace");
		AiPlace2 = GetNode<AiPlace>("AiPlace2");
		DeckLayer = GetNode<Sprite2D>("DeckLayer");

		GoBackButton = GetNode<TextureButton>("Buttons/GoBackButton");
		RestartButton = GetNode<TextureButton>("Buttons/RestartButton");
		HomeButton = GetNode<TextureButton>("Buttons/HomeButton");
		LevelButton = GetNode<TextureButton>("Buttons/LevelButton");
		LastButton = GetNode<TextureButton>("Buttons/LastButton");
		NextButton = GetNode<TextureButton>("Buttons/NextButton");
		
		Ready += GameLogic.InitLevel;
		GameLogic.LevelLoadedEvent += LoadLevelVisual;
		GameLogic.PlayerLeadEvent += PlayerLead;
		GameLogic.PlayerLeadOver += CloseUndo;
		GameLogic.UndoFailed += AllowUndo;
		
		GameLogic.AiLeadEvent += AiLead;
		GameLogic.Ai2LeadEvent += Ai2Lead;
		GameLogic.TurnOverEvent += ClearLead;
		GameLogic.UndoEvent += UndoVisual;

		GameLogic.RoundOver += ShowResult;

		GoBackButton.Pressed += GoBack;
		RestartButton.Pressed += Restart;

		LastButton.Pressed += EnterLastLevel;
		NextButton.Pressed += EnterNextLevel;

		LevelButton.Pressed += ShowLevelChooseScreen;
		HomeButton.Pressed += LeavingGame;

		PlayerPlace.UpdateLimitNum += CountMinusOne;
		PlayerPlace.LeadCardBar.UndoFinished += AllowUndo;

		LevelChooseScreen = GetNode<LevelChooseScreen>("LevelChooseScreen");
		LevelChooseScreen.LevelChosen += ChangeToLevel;
		LevelChooseScreen.Hide();

		LeavingScreen = GetNode<Panel>("LeavingScreen");
		LevelChooseScreen.Hide();

		LeavingScreenEscape = GetNode<TextureButton>("LeavingScreen/Escape");
		LeavingScreenEscape.Pressed += () => {
			LeavingScreen.Hide();
			ColorRect.Hide();
		};

		ToRoom = GetNode<Button>("LeavingScreen/LeaveButtons/ToRoom");
		ToTitle = GetNode<Button>("LeavingScreen/LeaveButtons/ToTitle");

		ToRoom.Pressed += () => {
			var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
			sceneChanger.ChangeScene("res://scenes/MainHouseScene1.tscn");
			ReleaseAllEventToGameLogic();
			// 回到房间时保存数据
			DataLoader.StoreCurrentSave();
		};
		
		ToTitle.Pressed += () => {
			var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
			sceneChanger.ChangeScene("res://scenes/TitleScreen.tscn");
			ReleaseAllEventToGameLogic();
			// 回到标题界面时保存数据
			DataLoader.StoreCurrentSave();
		};

		RuleInfoScreen = GetNode<InfoScreen>("UpUIScreens/RuleInfoScreen");
		RuleInfo.Pressed += () => {
			RuleInfoScreen.InitContentWithKey(RuleTool.GetRuleKey(GameLogic.CurrentLevel.Rule), RuleTool.GetRuleKey(GameLogic.CurrentLevel.Rule) + "_C");
			RuleInfoScreen.Show();
			ColorRect.Show();
		};
		RuleInfoScreen.HideInfo += () => {
			RuleInfoScreen.Hide();
			ColorRect.Hide();
		};

		ModeInfoScreen = GetNode<InfoScreen>("UpUIScreens/ModeInfoScreen");
		ModeInfo.Pressed += () => {
			ModeInfoScreen.InitContentWithKey(RuleTool.GetModeKey(GameLogic.CurrentLevel.Mode), RuleTool.GetModeKey(GameLogic.CurrentLevel.Mode) + "_C");
			ModeInfoScreen.Show();
			ColorRect.Show();
		};
		ModeInfoScreen.HideInfo += () => {
			ModeInfoScreen.Hide();
			ColorRect.Hide();
		};

		PassInfoScreen = GetNode<InfoScreen>("UpUIScreens/PassInfoScreen");
		
		PassNumInfo.Pressed += () => {
			// 直接传入button上面的值，因为这个值是画面值，不会出现-1的情况
			PassInfoScreen.InitContent(PassNumInfo.Text,
				TranslationServer.Translate("UI_PASSLIMIT_C"));
			PassInfoScreen.Show();
			ColorRect.Show();
		};
		PassInfoScreen.HideInfo += () => {
			PassInfoScreen.Hide();
			ColorRect.Hide();
		};
		
		PlayInfoScreen = GetNode<InfoScreen>("UpUIScreens/PlayInfoScreen");
		PlayNumInfo.Pressed += () => {
			// 直接传button，由于赢后再计算，所以最后出牌赢会保持1，不会为0（还是因为结算画面不算不可交互UI导致的）
			// 必须正好是0的时候才有节目效果，如果已经输了（-1）就不会触发（不过已经是0的话，输了还是会触发）
			PlayInfoScreen.InitContent(PlayNumInfo.Text,
				TranslationServer.Translate(GameLogic.LeadLimitNum == 0 && GameLogic.CurrentLevel.Mode != GameModes.Rest
					? "UI_PLAYLIMIT_CC"
					: "UI_PLAYLIMIT_C"),
				GameLogic.LeadLimitNum == 0 && GameLogic.CurrentLevel.Mode != GameModes.Rest);
			// ↑：保险起见，加入rest判断，这样rest模式下lead为0不算死
			PlayInfoScreen.Show();
			ColorRect.Show();
		};
		PlayInfoScreen.HideInfo += () => {
			PlayInfoScreen.Hide();
			ColorRect.Hide();
		};
		
		PassInfoScreen2 = GetNode<InfoScreen>("UpUIScreens/PassInfoScreen2");
		
		PassNumInfoR.Pressed += () => {
			// 直接传入button上面的值，因为这个值是画面值，不会出现-1的情况
			PassInfoScreen2.InitContent(PassNumInfoR.Text,
				TranslationServer.Translate("UI_PASSREQ_C"));
			PassInfoScreen2.Show();
			ColorRect.Show();
		};
		PassInfoScreen2.HideInfo += () => {
			PassInfoScreen2.Hide();
			ColorRect.Hide();
		};
		
		PlayInfoScreen2 = GetNode<InfoScreen>("UpUIScreens/PlayInfoScreen2");
		PlayNumInfoR.Pressed += () => {
			PlayInfoScreen2.InitContent(PlayNumInfoR.Text,
				TranslationServer.Translate("UI_PLAYREQ_C"));
			PlayInfoScreen2.Show();
			ColorRect.Show();
		};
		PlayInfoScreen2.HideInfo += () => {
			PlayInfoScreen2.Hide();
			ColorRect.Hide();
		};
		
		LevelInfoScreen = GetNode<InfoScreen>("UpUIScreens/LevelInfoScreen");
		LevelNumButton.Pressed += () => {
			LevelInfoScreen.InitContent(LevelNumButton.Text.StripEdges(), TranslationServer.Translate(
				((Array)((Dictionary)DataLoader.CurrentSave["pass_level"])[GameLogic.CurrentLevel.Name]).Contains(
					GameLogic.CurrentLevel.Id) ? "UI_LEVELSOLVED" : "UI_LEVELSTALLED"));
			LevelInfoScreen.Show();
			ColorRect.Show();
		};
		LevelInfoScreen.HideInfo += () => {
			LevelInfoScreen.Hide();
			ColorRect.Hide();
		};
		
		ProgressInfoScreen = GetNode<InfoScreen>("UpUIScreens/ProgressInfoScreen");

		AiInfoScreen = GetNode<AiInfoScreen>("AiInfoScreen");
		AiInfoScreen.HideInfo += () => {
			MouseManager.IsInterAreaAble = true;
			AiInfoScreen.Hide();
			ColorRect.Hide();
		};
	}

	/// <summary>
	/// 解绑所有与GameLogic的事件，在离开GS的时候调用<br/>
	/// - 由于GameLogic是Static的全局静态类，其中的事件逻辑尤为不安全，需要格外注意<br/>
	/// - 其他所有用到static的类基本都是工具类，不会涉及到游戏的核心算法<br/>
	/// - Godot内置节点的事件，都是在节点树中调用的，所以不会出现清空后保留事件的情况<br/>
	/// - 使用了这个函数后，的确解决了 ObjectDisposedException 问题
	/// </summary>
	public void ReleaseAllEventToGameLogic() {
		GameLogic.LevelLoadedEvent -= LoadLevelVisual;
		GameLogic.PlayerLeadEvent -= PlayerLead;
		GameLogic.PlayerLeadOver -= CloseUndo;
		GameLogic.UndoFailed -= AllowUndo;
		
		GameLogic.AiLeadEvent -= AiLead;
		GameLogic.Ai2LeadEvent -= Ai2Lead;
		GameLogic.TurnOverEvent -= ClearLead;
		GameLogic.UndoEvent -= UndoVisual;

		GameLogic.RoundOver -= ShowResult;
	}
	
	public void ShowAiInfo(string name) {
		MouseManager.IsInterAreaAble = false; // 确保弹出屏幕时，鼠标是箭头不是click
		Input.SetCustomMouseCursor(MouseManager.Arrow);
		// GD.Print($"Show {name}");
		AiInfoScreen.InitContentWithName(name, 1);
		AiInfoScreen.Show();
		ColorRect.Show();
	}

	public void ShowLevelChooseScreen() {
		LevelChooseScreen.InitLevelChooseScreen(GameLogic.CurrentLevel.Name);
		LevelChooseScreen.Show();
		ColorRect.Show();
	}

	public void ChangeToLevel(string id) {
		if (id == "") { // Hide 专用
			LevelChooseScreen.Hide();
			ColorRect.Hide();
			return; 
		}
		if (IsUndoable) {
			var levelStr = LevelTool.GetLevelPath(GameLogic.CurrentLevel.Name, id);
			if (levelStr == "") return;
		
			LevelChooseScreen.Hide();
			ColorRect.Hide();
			RoundOverScreen.Hide();
			ClearLead();
		
			GameLogic.CurrentLevelPath = levelStr;
			GameLogic.InitLevel();
		}
	}

	public void LeavingGame() {
		LeavingScreen.Show();
		ColorRect.Show();
	}
	
	public async void ShowResult(bool isWin, string text) {
		if (isWin) {
			// 赢了，切换icon，并且把数据放到字典里面！
			LevelNumButton.Icon = GD.Load<Texture2D>("res://assets/ui/lvl-solved.png");
			var passLevelArray = (Array)((Dictionary)DataLoader.CurrentSave["pass_level"])[GameLogic.CurrentLevel.Name];
			if (!passLevelArray.Contains(GameLogic.CurrentLevel.Id)) {
				passLevelArray.Add(GameLogic.CurrentLevel.Id); // 不包含的时候加入，包含就算了
			}
		}
		
		RoundOverScreen.Show(); // 应该在一开始就显示（避免动画取消的问题）
		RoundOverScreen.Position = new Vector2(-96, -170);
		
		await DelayFunc(1000);

		if (isWin) {
			var soundManager = GetNode<SoundManager>("/root/SoundManager");
			soundManager.PlayerResultEffects("solved");
		}
		else {
			var soundManager = GetNode<SoundManager>("/root/SoundManager");
			soundManager.PlayerResultEffects("unsolved");
		}
		
		IsUndoable = true;
		WinnerText.Text = isWin
			? TranslationServer.Translate("CONGRATULATIONS")
			: TranslationServer.Translate("CHALLENGEFAILED");
		CompleteProgress.Text = text;

		var tween = GetTree().CreateTween();
		tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Bounce);
		tween.TweenProperty(RoundOverScreen, "position", new Vector2(-96, 0), 0.3);
	}
	
	public void GoBack() {
		if (IsUndoable) {
			IsUndoable = false; // 为了动画效果，只能牺牲玩家体验了……
			RoundOverScreen.Hide();
			GameLogic.Undo();
		}
	}

	public void Restart() {
		if (IsUndoable) {
			IsUndoable = false;
			RoundOverScreen.Hide();
			ClearLead();
			GameLogic.InitLevel();
		}
	}

	// public void ClearAllTween() {
	// 	foreach (var node in GetChildren()) {
	// 		if (node.Owner == null) {
	// 			// node is Tween tween 错误
	// 			// 如果要清除动画，必须静态声明，不能动态添加（所以放弃修复此warning）
	// 			// 参考：https://docs.godotengine.org/zh-cn/4.x/classes/class_tween.html
	// 		}
	// 	}
	// }
	
	// 其实直接更新就行了，何苦还要判断一下……
	public void CountMinusOne(bool isPass) {
		if (isPass) {
			PassNumInfo.Text = TranslationServer.Translate("UI_PASSLIMIT") + $"  {GameLogic.PassLimitNum}";
		}
		else {
			PlayNumInfo.Text = TranslationServer.Translate("UI_PLAYLIMIT") + $"  {GameLogic.LeadLimitNum}";
		}

		if (GameLogic.PassRequestNum >= 0) {
			PassNumInfoR.Text = TranslationServer.Translate("UI_PASSREQ") + $"  {GameLogic.PassRequestNum}";
		}
		if (GameLogic.PlayRequestNum >= 0) {
			PlayNumInfoR.Text = TranslationServer.Translate("UI_PLAYREQ") + $"  {GameLogic.PlayRequestNum}";
		}
	}

	// 切换关卡过快会出现 动画父节点先于动画结束前free 的警告，不修复了
	// 玩家需要快速地切换，不应该等动画（Undo就可以快速回退）
	
	public void EnterLastLevel() {
		if (IsUndoable) {
			IsUndoable = false;
			var path = LevelTool.GetLastLevelPath(GameLogic.CurrentLevel.Name, GameLogic.CurrentLevel.Id);
			if (path == "") return;
			// 上一关必定不会是END
			if (path == "BEG") {
				IsUndoable = true;
				return;
			}

			// restart的逻辑
			RoundOverScreen.Hide();
			ClearLead();
			GameLogic.CurrentLevelPath = path;
			GameLogic.InitLevel();
		}
		
	}

	public void EnterNextLevel() {
		if (IsUndoable) {
			IsUndoable = false;
			var path = LevelTool.GetNextLevelPath(GameLogic.CurrentLevel.Name, GameLogic.CurrentLevel.Id);
			if (path == "") return;
			// 下一关必定不会是BEG
			if (path == "END") {
				IsUndoable = true;
				return;
			}

			// restart的逻辑
			RoundOverScreen.Hide();
			ClearLead();
			GameLogic.CurrentLevelPath = path;
			GameLogic.InitLevel();
		}
	}
	
	public override void _UnhandledInput(InputEvent @event) {
		if (@event.IsActionPressed("undo") && IsUndoable) {
			IsUndoable = false;
			RoundOverScreen.Hide();
			GameLogic.Undo();
		}

		if (@event.IsActionPressed("restart") && IsUndoable) {
			IsUndoable = false;
			RoundOverScreen.Hide();
			ClearLead();
			GameLogic.InitLevel();
		}
	}

	public void UndoVisual(OneTurnInfo tmpTurnInfo) {
		ClearLead();
		PlayerPlace.LeadCardBar.MyHide();
		// IsUndoable = true;
		
		// 恢复UI的数值（GameLogic.Undo已经重置了数值，直接调用即可）
		PassNumInfo.Text = TranslationServer.Translate("UI_PASSLIMIT") + $"  {GameLogic.PassLimitNum}";
		PlayNumInfo.Text = TranslationServer.Translate("UI_PLAYLIMIT") + $"  {GameLogic.LeadLimitNum}";
		// Undo的视觉部分也要重置！！！
		PassNumInfoR.Text = TranslationServer.Translate("UI_PASSREQ") + $"  {GameLogic.PassRequestNum}";
		PlayNumInfoR.Text = TranslationServer.Translate("UI_PLAYREQ") + $"  {GameLogic.PlayRequestNum}";
		
		// 能出牌就出，不能出就null
		// 因为是上上一轮，所以有可能为null，Rule报错
		if (GameLogic.CurrentBiggest == null) {
			PlayerPlace.ReadyForLead(null);
		}
		else {
			PlayerPlace.ReadyForLead(GameLogic.CurrentRule.CanFollow(GameLogic.CurrentBiggest,
				GameLogic.GetWhoseCards("YOU"), out var pcs)
				? pcs
				: null);
		}

		// 回退更新牌的时候，忘了更新cardsInHand了……（不是这个问题）
		PlayerPlace.PlayerCardPlace.CardsInHand = new List<CardData>(GameLogic.GetWhoseCards("YOU"));
		PlayerPlace.PlayerCardPlace.UpdateCards(GameLogic.GetWhoseCards("YOU"));
		AiPlace.AiCardPlace.UpdateCards(GameLogic.GetWhoseCards("YOU", 1), GameLogic.GameMode == GameModes.Blind);
		if (GameLogic.CurrentLevel.Number == 3) {
			AiPlace2.AiCardPlace.UpdateCards(GameLogic.GetWhoseCards("YOU", 2), GameLogic.GameMode == GameModes.Blind);
		}

		// 强调：pass pass 是上一局的，我们要恢复上上一局的场面！！！
		
		// // 第一个出牌的，无需恢复上一次情况
		// if (GameLogic.CurrentState == TurnStates.First) {
		//     return;
		// }
		
		// 强调：这里是tmpTurnInfo是上上一次的记录！！！

		// 上上一条记录，肯定不会是最后一条（废话！），除非是第一条才会不完整
		// 如果是第一条记录，特殊对待（AI可能先出）
		// if (tmpTurnInfo.Turns.Count == 0 || !tmpTurnInfo.Turns.Contains("YOU")) return;
		if (tmpTurnInfo.Turns.Count == 0) return;
		if (tmpTurnInfo.Turns.Count > 0 && !tmpTurnInfo.Turns.Contains("YOU")) {
			// 至少有一条记录，最多有两条记录
			
			// 只可能只有一个，如果有两条就必定有YOU（而且一定不可能PASS！）
			if (GameLogic.CurrentLevel.Number == 2) {
				AiPlace.AiLeadPlace.UpdateCards(tmpTurnInfo.Combs[0].Cards, GameLogic.GameMode == GameModes.Blind);
				return;
			}
			
			// 3人的情况，最多两条；一条的时候肯定不是空的！
			if (tmpTurnInfo.Turns.Count == 1) {
				AiPlace2.AiLeadPlace.UpdateCards(tmpTurnInfo.Combs[0].Cards, GameLogic.GameMode == GameModes.Blind);
			}
			else {
				AiPlace.AiLeadPlace.UpdateCards(tmpTurnInfo.Combs[0].Cards, GameLogic.GameMode == GameModes.Blind);
				if (tmpTurnInfo.Combs[1] == null) {
					AiPlace2.AiLeadPlace.Pass();
				}
				else {
					AiPlace2.AiLeadPlace.UpdateCards(tmpTurnInfo.Combs[1].Cards, GameLogic.GameMode == GameModes.Blind);
				}
			}

			return;
		}

		// 玩家的Lead显示部分（包括：如果对手都PASS，回退时不会再现的逻辑）
		if (GameLogic.CurrentLevel.Number == 2) {
			if (tmpTurnInfo.Combs[1] != null) {
				AiPlace.AiLeadPlace.UpdateCards(tmpTurnInfo.Combs[1].Cards, GameLogic.GameMode == GameModes.Blind);
			}
		}
		else {
			if (tmpTurnInfo.Combs[1] == null && tmpTurnInfo.Combs[2] == null) return;
			if (tmpTurnInfo.Combs[1] != null) {
				AiPlace.AiLeadPlace.UpdateCards(tmpTurnInfo.Combs[1].Cards, GameLogic.GameMode == GameModes.Blind);
			}
			else {
				AiPlace.AiLeadPlace.Pass();
			}
			
			if (tmpTurnInfo.Combs[2] != null) {
				AiPlace2.AiLeadPlace.UpdateCards(tmpTurnInfo.Combs[2].Cards, GameLogic.GameMode == GameModes.Blind);
			}
			else {
				AiPlace2.AiLeadPlace.Pass();
			}
		}
	}

	public async void LoadLevelVisual() {
		var soundManager = GetNode<SoundManager>("/root/SoundManager");
		soundManager.PlaySoundEffects("shuffle");
		
		// BGM 反而影响思考，而且和音效的配合不佳，不如不做
		// 把备选的BGM作为各个房间的背景音乐好了（对话和关卡都不做BGM，tmpDia除外，如果有轻音乐除外）
		// soundManager.PlayMusic(GameLogic.CurrentLevel.Name);
		var levelList = GameLogic.CurrentLevel.Name.Split("_");
		soundManager.PlayMusic($"{levelList[0]}_{levelList[1]}");
		
		// 新加载关卡时，把当前关卡输入到last level中
		var save = DataLoader.CurrentSave;
		var lastLevels = (Dictionary)save["last_level"];
		lastLevels[GameLogic.CurrentLevel.Name] = GameLogic.CurrentLevel.Id;
		
		// 还是决定隐藏上一关/下一关按钮，如果没有的话
		if (GameLogic.CurrentLevel.Id.ToInt() == LevelTool.GetLevelBeg(GameLogic.CurrentLevel.Name)) LastButton.Hide();
		else LastButton.Show();
		if (GameLogic.CurrentLevel.Id.ToInt() == LevelTool.GetLevelEnd(GameLogic.CurrentLevel.Name)) NextButton.Hide();
		else NextButton.Show();
		
		RuleInfo.Text = TranslationServer.Translate(RuleTool.GetRuleKey(GameLogic.CurrentLevel.Rule));
		ModeInfo.Text = TranslationServer.Translate(RuleTool.GetModeKey(GameLogic.CurrentLevel.Mode));
		// if (GameLogic.GameMode == GameModes.Normal) { ModeInfo.Hide(); }
		switch (GameLogic.GameMode) {
			case GameModes.Normal:
				ModeInfo.Text = TranslationServer.Translate("M_NORMAL");
				break;
			case GameModes.Rest:
				ModeInfo.Text = TranslationServer.Translate("M_REST");
				break;
			case GameModes.Blind:
				ModeInfo.Text = TranslationServer.Translate("M_BLIND");
				break;
		}
		
		// 999 作为不显示 Limit 的 魔数
		if (GameLogic.PassLimitNum != 999) {
			PassNumInfo.Text = TranslationServer.Translate("UI_PASSLIMIT") + $"  {GameLogic.PassLimitNum}";
			PassNumInfo.Show();
		}
		else {
			PassNumInfo.Hide();
		}

		if (GameLogic.LeadLimitNum != 999) {
			PlayNumInfo.Text = TranslationServer.Translate("UI_PLAYLIMIT") + $"  {GameLogic.LeadLimitNum}";
			PlayNumInfo.Show();
		}
		else {
			PlayNumInfo.Hide();
		}
		
		if (GameLogic.PassRequestNum != 0) {
			PassNumInfoR.Text = TranslationServer.Translate("UI_PASSREQ") + $"  {GameLogic.PassRequestNum}";
			PassNumInfoR.Show();
		}
		else {
			PassNumInfoR.Hide();
		}

		if (GameLogic.PlayRequestNum != 0) {
			PlayNumInfoR.Text = TranslationServer.Translate("UI_PLAYREQ") + $"  {GameLogic.PlayRequestNum}";
			PlayNumInfoR.Show();
		}
		else {
			PlayNumInfoR.Hide();
		}
		
		// Level Id 后三位就是关卡数，所以 Id.ToInt % 1000 就是在这关的实际位置
		LevelNumButton.Text = TranslationServer.Translate("UI_LEVEL") +
		                      $"  {GameLogic.CurrentLevel.Id.ToInt() % 1000}";
		
		if (((Array)((Dictionary)DataLoader.CurrentSave["pass_level"])[GameLogic.CurrentLevel.Name]).Contains(
			    GameLogic.CurrentLevel.Id)) { // 有，过了
			LevelNumButton.Icon = GD.Load<Texture2D>("res://assets/ui/lvl-solved.png");
		}
		else {
			LevelNumButton.Icon = GD.Load<Texture2D>("res://assets/ui/lvl-stalled.png");
		}

		CompleteNumButton.Text = TranslationServer.Translate("UI_SOLVED") +
		                         $"  {((Array)((Dictionary)DataLoader.CurrentSave["pass_level"])[GameLogic.CurrentLevel.Name]).Count}" +
		                         $"/{LevelTool.GetLevelNum(GameLogic.CurrentLevel.Name)}";
		
		DeckLayer.Show();
		
		// Test：加载自己的牌（画面部分）
		PlayerPlace.InitCards(GameLogic.GetWhoseCards("YOU"));
		// PlayerPlace.InitCards(GameLogic.CurrentLevel.Cards[GameLogic.PlayerIndexMap["YOU"]]);
		
		if (GameLogic.CurrentLevel.Number == 2) {
			AiImage.InitAiImage(GameLogic.GetWhoseName("YOU", 1), PlacePositions.Up);
			AiImage2.Hide();
			
			AiPlace.InitPosition(PlacePositions.Up);
			AiPlace.InitCards(GameLogic.GetWhoseCards("YOU", 1));
			AiPlace2.Hide();

			if (GameLogic.CurrentLevel.Landlord == GameLogic.GetWhoseName("YOU", 1)) {
				AiHat.Position = new Vector2(20, -35); // Up: (0, -20)
				AiHat.Show();
			}
			else {
				AiHat.Hide();
			}
		}
		else { // must be 3
			AiImage.InitAiImage(GameLogic.GetWhoseName("YOU", 1), PlacePositions.Right);
			AiImage2.InitAiImage(GameLogic.GetWhoseName("YOU", 2), PlacePositions.Left);
			
			AiPlace.InitPosition(PlacePositions.Right);
			AiPlace.InitCards(GameLogic.GetWhoseCards("YOU", 1));
			AiPlace2.InitPosition(PlacePositions.Left);
			AiPlace2.InitCards(GameLogic.GetWhoseCards("YOU", 2));
			AiPlace2.Show();
			
			if (GameLogic.CurrentLevel.Landlord == GameLogic.GetWhoseName("YOU", 1)) {
				AiHat.Position = new Vector2(80, -25); // Left/Right: (±100, -10)
				AiHat.Show();
			}
			else if (GameLogic.CurrentLevel.Landlord == GameLogic.GetWhoseName("YOU", 2)) {
				AiHat.Position = new Vector2(-80, -25);
			}
			else {
				AiHat.Hide();
			}
		}

		// 根据最大牌张数，确定等待时间
		var cardMax = Math.Max(GameLogic.CurrentLevel.Cards[0].Count, GameLogic.CurrentLevel.Cards[1].Count);
		if (GameLogic.CurrentLevel.Number == 3) {
			cardMax = Math.Max(cardMax, GameLogic.CurrentLevel.Cards[2].Count);
		}
		
		// + 100 ms 视觉效果更好？
		await DelayFunc(cardMax * 10 + 100); //  50 -> 10 ms
		DeckLayer.Hide();
		
		GameLogic.FirstTurn();
	}

	public async void PlayerLead(List<CombData> possCombs) {
		// 出牌前需要等待，出牌后直接刷新
		// await DelayFunc();
		
		// 但如果是第一个出牌，就不考虑加时间了
		if (GameLogic.CurrentState != TurnStates.First) {
			await DelayFunc();
		}

		// IsUndoable = true;
		
		PlayerPlace.ReadyForLead(possCombs);
	}
	
	// 放弃使用 async，因为警告：由于此调用不会等待，因此在此调用完成之前将会继续执行当前方法。请考虑将 "await" 运算符应用于调用结果。 
	public async void AiLead(CombData leadComb) {
		await DelayFunc();
		if (leadComb == null) {
			AiPlace.Pass();
			
			// [Debug]
			GD.Print($"Ai1: PASS");
		}
		else {
			AiPlace.LeadCard(leadComb.Cards);
			
			// [Debug]
			CardTool.PrintCardDataList(leadComb.Cards, "Ai1");
			var tmpT = "[Types] ";
			foreach (var kv in leadComb.Types) {
				// GD.Print($"{kv.Key}: {kv.Value}");
				tmpT += $"{kv.Key}: {kv.Value}; ";
			}
			GD.Print(tmpT);
		}
		
		// await DelayFunc();
	}
	
	public async void Ai2Lead(CombData leadComb) {
		await DelayFunc();
		if (leadComb == null) {
			AiPlace2.Pass();
			
			// [Debug]
			GD.Print($"Ai2: PASS");
		}
		else {
			AiPlace2.LeadCard(leadComb.Cards);
			
			// [Debug]
			CardTool.PrintCardDataList(leadComb.Cards, "Ai2");
			var tmpT = "[Types] ";
			foreach (var kv in leadComb.Types) {
				// GD.Print($"{kv.Key}: {kv.Value}");
				tmpT += $"{kv.Key}: {kv.Value}; ";
			}
			GD.Print(tmpT);
		}

		
		// await DelayFunc();
	}

	// 每turn（牌权更新）清除场上的出牌情况
	public void ClearLead() {
		// PlayerPlace.LeadCardBar.MyHide();
		PlayerPlace.LeadCardBar.Hide();
		PlayerPlace.LeadPlace.Clear();
		AiPlace.AiLeadPlace.Clear();
		if (GameLogic.CurrentLevel.Number == 3) {
			AiPlace2.AiLeadPlace.Clear();
		}
	}

	// 间接使用 async，曲线救国
	public async Task DelayFunc(int delayMs = 500) {
		await Task.Delay(delayMs);
	}

	public void CloseUndo() {
		IsUndoable = false;
	}

	public void AllowUndo() {
		IsUndoable = true;
	}
	
}
