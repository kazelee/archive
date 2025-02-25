using Godot;
using System;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.tools;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class LevelChooseScreen : Panel {
    public event Action<string> LevelChosen; 
    
    public Label LevelName;
    public GridContainer ButtonContainer;
    public TextureButton EscapeButton;

    public override void _Ready() {
        base._Ready();
        LevelName = GetNode<Label>("LevelName");
        ButtonContainer = GetNode<GridContainer>("ScrollContainer/ButtonContainer");
        EscapeButton = GetNode<TextureButton>("Escape");
        EscapeButton.Pressed += () => { LevelChosen?.Invoke(""); }; // 为空表示没选，退出
    }

    /// <summary>
    /// 根据给入的levelName，加载对应的关卡网格（包括完成情况）
    /// </summary>
    /// <param name="levelName">KEY</param>
    public void InitLevelChooseScreen(string levelName) {
        // LevelName.Text = LevelTool.GetLevelFullName(levelName, true);
        // LevelName.Text = TranslationServer.Translate(
        //     LevelTool.GetTranslateKey(levelName));
        // LevelName.Text = LevelTool.GetLevelNameTranslated(levelName);
        var levelNameList = levelName.Split("_");
        LevelName.Text = $"{levelNameList[1]}-{levelNameList[2]} {TranslationServer.Translate(levelName)}";
        foreach (var variableNode in ButtonContainer.GetChildren()) {
            variableNode.QueueFree();
        }

        var beg = LevelTool.GetLevelBeg(levelName);
        var end = LevelTool.GetLevelEnd(levelName);
        for (int i = beg; i <= end; i++) {
            var button = new Button();
            button.Text = i.ToString();
            if (((Array)((Dictionary)DataLoader.CurrentSave["pass_level"])[levelName]).Contains(i.ToString())) {
                button.Icon = GD.Load<Texture2D>("res://assets/ui/lvl-solved.png");
            }
            else {
                button.Icon = GD.Load<Texture2D>("res://assets/ui/lvl-stalled.png");
            }
            
            button.Pressed += () => { LevelChosen?.Invoke(button.Text); };
            ButtonContainer.AddChild(button);
        }
    }
}
