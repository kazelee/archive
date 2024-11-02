using Godot;
using System;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class LevelScreen : CanvasLayer {
    public TextureButton EscapeButton;

    public Deck Deck1;
    public Deck Deck2;
    public Deck Deck3;
    public Deck Deck4;

    public override void _Ready() {
        base._Ready();
        EscapeButton = GetNode<TextureButton>("LevelPanel/Escape");
        EscapeButton.Pressed += () => {
            Hide();
            MouseManager.IsInterAreaAble = true; // 点叉肯定可以转换为true
        };
        Deck1 = GetNode<Deck>("LevelPanel/Decks/Deck1");
        Deck2 = GetNode<Deck>("LevelPanel/Decks/Deck2");
        Deck3 = GetNode<Deck>("LevelPanel/Decks/Deck3");
        Deck4 = GetNode<Deck>("LevelPanel/Decks/Deck4");
    }

    public void InitDecks(string cname) {
        var levelKeyFirst = LevelTool.GetLevelKeyFirst(cname);
        SetForDeck(Deck1, levelKeyFirst+"_1");
        SetForDeck(Deck2, levelKeyFirst+"_2");
    }
    
    public void SetForDeck(Deck deck, string levelKey) {
        var levelKeyList = levelKey.Split("_");
        deck.LevelName.Text = $"{levelKeyList[1]}-{levelKeyList[2]}\n{TranslationServer.Translate(levelKey)}";
        deck.Progress.Text = $"{TranslationServer.Translate("UI_SOLVED")}" +
		                         $"  {((Array)((Dictionary)DataLoader.CurrentSave["pass_level"])[levelKey]).Count}" +
		                         $"/{LevelTool.GetLevelNum(levelKey)}";
        deck.OnInteracted += () => {
            // 在转换场景之前，记录下当前玩家的坐标？
            
            var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
            sceneChanger.ChangeScene("res://scenes/GameScene.tscn");
            GameLogic.CurrentLevelPath = $"res://levels/{levelKey}/" +
                                         ((Dictionary)DataLoader.CurrentSave["last_level"])[levelKey] +
                                         ".json";
            this.Hide();
        };
    }
    
    
}
