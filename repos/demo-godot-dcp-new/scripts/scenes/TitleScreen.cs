using Godot;
using System;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.test;

public partial class TitleScreen : Sprite2D {
	public Control TitleEn;
	public Control TitleZhCn;
	public Control TitleZhTw;
	public Control TitleJa;

	// public Sprite2D DoorSprite;
	public DoorArea DoorArea;
	public HangTagArea HangTagArea;

	public TextureButton SettingButton;
	public TextureButton InfoButton;
	public TextureButton LocateButton;

	public ColorRect ColorRectBG;
	public SavePanel SavePanel;
	
	// 测试用
	public TextEdit TextEdit;
	public Button Button;
	
	public override void _Ready() {
		var soundManager = GetNode<SoundManager>("/root/SoundManager");
		soundManager.PlayMusic("Title");
		
		TitleEn = GetNode<Control>("TitlePanel/Title_en");
		TitleZhCn = GetNode<Control>("TitlePanel/Title_zh_CN");
		TitleZhTw = GetNode<Control>("TitlePanel/Title_zh_TW");
		TitleJa = GetNode<Control>("TitlePanel/Title_ja");

		// DoorSprite = GetNode<Sprite2D>("DoorArea/DoorSprite");
		DoorArea = GetNode<DoorArea>("DoorArea");
		DoorArea.OnDoorOpened += StartGame;
		HangTagArea = GetNode<HangTagArea>("HangTagArea");
		HangTagArea.OnHangTagTouched += ChangeSave;

		SettingButton = GetNode<TextureButton>("Buttons/SettingButton");
		InfoButton = GetNode<TextureButton>("Buttons/InfoButton");
		LocateButton = GetNode<TextureButton>("Buttons/LocateButton");

		LocateButton.Pressed += SetToNextLocate;

		TextEdit = GetNode<TextEdit>("LevelTest/TextEdit");
		Button = GetNode<Button>("LevelTest/Button");

		ColorRectBG = GetNode<ColorRect>("ColorRectBG");
		SavePanel = GetNode<SavePanel>("SavePanel");
		SavePanel.Hide();
		SavePanel.ChosenOver += RefreshSave; 

		if (DebugManager.IsDebugMode) {
			TextEdit.Show();
			Button.Show();
			Button.Pressed += TestLevel;
		}
		
		RefreshSave();
		RefreshTitle();
		
		// [Test]
		// GameTest.CurrentLevelPath = "res://levels/1-1-Newcomer/11001.json";
		// GameTest.InitLevel();
		
		// [Test]
		// GD.Print(DataLoader.CurrentSave);
	}

	public void TestLevel() {
		var id = TextEdit.Text;
		if (id == "00000") {
			// 遍历测试
			DoLoopTest();
			return;
		}
		
		var levelStr = "L_";
		if (id.Length == 6) {
			levelStr += $"10_{id[2]}";
		}
		else if (id.ToInt() / 1000 == 15) { // 15xxx -> 14xxx
			levelStr += "1_5";
		}
		else {
			levelStr += $"{id[0]}_{id[1]}";
		}

		GameTest.CurrentLevelPath =
			$"res://levels/{levelStr}/{(id.ToInt() / 1000 == 15 ? id.ToInt() - 1000 : id)}.json";
		GD.Print(GameTest.CurrentLevelPath);
		GameTest.InitLevel();
	}
	
	public void StartGame() {
		// DebugManager.IsDebugMode = false;

		// SaveManager.PlayerPositionScene1 = new Vector2(135, 50); // 白门的位置
		
		SaveManager.PlayerPositionScene1 = new Vector2(200, 50); // 后面一律位于门的右下侧
		var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
		sceneChanger.ChangeScene("res://scenes/MainHouseScene1.tscn");
		
		// [Test]
		// var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
		// sceneChanger.ChangeScene("res://scenes/GameScene.tscn");
		// GameLogic.CurrentLevelPath = "res://levels/L_8_1/81001.json";
	}

	public void ChangeSave() {
		// GD.Print("Save!");
		MouseManager.IsInterAreaAble = false;
		ColorRectBG.Show();
		SavePanel.Show();
		Input.SetCustomMouseCursor(MouseManager.Arrow);
	}

	public void RefreshSave() {
		SavePanel.CurrentChosenButtonId = "";
		
		DataLoader.StoreUserSettings(); // 每次修改settings都要写入磁盘
		HangTagArea.Label.Text = $"No. {DataLoader.UserSettings["current_save"]}";
		
		SavePanel.Hide();
		ColorRectBG.Hide();
		MouseManager.IsInterAreaAble = true;
	}
	
	public void RefreshTitle() {
		TitleEn.Hide();
		TitleZhCn.Hide();
		TitleZhTw.Hide();
		TitleJa.Hide();

		switch (SettingManager.Language) {
			case "en": TitleEn.Show();
				break;
			case "zh_CN": TitleZhCn.Show();
				break;
			case "zh_TW": TitleZhTw.Show();
				break;
			case "ja": TitleJa.Show();
				break;
		}
	}

	public void SetToNextLocate() {
		SettingManager.SetToNextLanguage();
		GD.Print($"Set locate to {SettingManager.Language}");
		RefreshTitle();
	}


	public int TmpIdForTest;
	public void DoLoopTest() {
		LoopTest.Start();
	}
}
