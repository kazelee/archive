using Godot;
using System;
using DouCardPuzzoom.scripts.manager;

public partial class QuestionScreen : CanvasLayer {
    public Label Question;
    public Button LeftButton;
    public Button RightButton;

    public string LeftButtonKey = "";
    public string RightButtonKey = "";
    
    public TextureButton EscapeButton;
    public override void _Ready() {
        base._Ready();
        Question = GetNode<Label>("Panel/Question");
        LeftButton = GetNode<Button>("Panel/Decisions/LeftButton");
        RightButton = GetNode<Button>("Panel/Decisions/RightButton");
        EscapeButton = GetNode<TextureButton>("Panel/Escape");

        LeftButton.Pressed += () => {
            ChangeToScene(LeftButtonKey);
        };

        RightButton.Pressed += () => {
            ChangeToScene(RightButtonKey);
        };
        
        EscapeButton.Pressed += () => {
            Hide();
            MouseManager.IsInterAreaAble = true;
        };
    }

    // 最好不要在ready函数外绑定事件信号……不然委托重复有够受的……
    public void InitContent(string name) {
        // InitContent说明有东西交互了
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        // soundManager.PlaySoundEffects("pick");
        // soundManager.AreaSFXPlayer.Play(0.5f);
        
        var nameList = name.Split("-");
        if (nameList[0] == "Stairs") {
            // 上下楼
            soundManager.PlayAreaEffects("updown");
            
            Question.Text = TranslationServer.Translate("UI_WHEREYOUGO");
            var leftButtonKey = "";
            var rightButtonKey = "";
            switch (nameList[1]) {
                case "1":
                    leftButtonKey = "BTN_FL_2";
                    rightButtonKey = "BTN_FL_3";
                    break;
                case "2":
                    leftButtonKey = "BTN_FL_1";
                    rightButtonKey = "BTN_FL_3";
                    break;
                case "3":
                    leftButtonKey = "BTN_FL_1";
                    rightButtonKey = "BTN_FL_2";
                    break;
                default:
                    GD.PrintErr("警告：非法的楼梯号！");
                    break;
            }

            LeftButton.Text = TranslationServer.Translate(leftButtonKey);
            RightButton.Text = TranslationServer.Translate(rightButtonKey);

            LeftButtonKey = leftButtonKey;
            RightButtonKey = rightButtonKey;

            // LeftButton.Pressed += () => { ChangeToScene(leftButtonKey); };
            // RightButton.Pressed += () => { ChangeToScene(rightButtonKey); };
        }
        else if (nameList[0] == "Door") {
            // 门
            soundManager.PlayAreaEffects("knock");
            
            // 字符串都应该是key值，如 C_LL (Door-C_LL)
            Question.Text = String.Format(TranslationServer.Translate("UI_TRYENTER"),
                TranslationServer.Translate(nameList[1]));

            LeftButton.Text = TranslationServer.Translate("BTN_YES");
            RightButton.Text = TranslationServer.Translate("BTN_NO");

            LeftButtonKey = $"Room_{nameList[1]}";
            RightButtonKey = "";

            // RightButton.Pressed += () => {
            //     Hide();
            //     MouseManager.IsInterAreaAble = true;
            // };
        }
        else { // Room 包括 Outdoor
            if (nameList[1] == "MyRoom") {
                Question.Text = TranslationServer.Translate("UI_WHEREYOUGO");
                LeftButton.Text = TranslationServer.Translate("UI_MYROOM");
                RightButton.Text = TranslationServer.Translate("UI_TITLESCR");

                LeftButtonKey = "";
                RightButtonKey = "TITLE";

                // RightButton.Pressed += () => { ChangeToScene("TITLE"); };
            }
        }
    }

    public void ChangeToScene(string where) {
        if (where == "") {
            Hide();
            MouseManager.IsInterAreaAble = true;
            return;
        }
        
        var canChange = true;
        var targetPath = "";
        
        switch (where) { 
            case "BTN_FL_1":
                // canChange = true;
                targetPath = "res://scenes/MainHouseScene1.tscn";
                break;
            case "BTN_FL_2":
                // canChange = true;
                targetPath = "res://scenes/MainHouseScene2.tscn";
                break;
            case "BTN_FL_3":
                // canChange = true;
                targetPath = "res://scenes/MainHouseScene3.tscn";
                break;
            case "TITLE":
                // canChange = true;
                targetPath = "res://scenes/TitleScreen.tscn";
                break;
            default:
                targetPath = $"res://scenes/rooms/{where}.tscn";
                break;
        }

        if (canChange) {
            Hide();
            var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
            sceneChanger.ChangeScene(targetPath);
            MouseManager.IsInterAreaAble = true;
        }
    }
}
