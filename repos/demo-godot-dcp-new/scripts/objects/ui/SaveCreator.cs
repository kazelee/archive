using Godot;
using System;
using DouCardPuzzoom.scripts;
using Array = Godot.Collections.Array;

public partial class SaveCreator : Panel {
    public event Action OnCreated; 
    
    public TextureButton EscapeButton;
    public TextureButton EnterButton;

    public NumberSetter NumberSetter1;
    public NumberSetter NumberSetter2;
    public NumberSetter NumberSetter3;

    public int CountForThreeZero = 0;

    public Label HintText;
    public Tween HintTextTween;

    public override void _Ready() {
        base._Ready();
        EnterButton = GetNode<TextureButton>("Enter");
        EscapeButton = GetNode<TextureButton>("Escape");
        NumberSetter1 = GetNode<NumberSetter>("HBoxContainer/NumberSetter");
        NumberSetter2 = GetNode<NumberSetter>("HBoxContainer/NumberSetter2");
        NumberSetter3 = GetNode<NumberSetter>("HBoxContainer/NumberSetter3");

        HintText = GetNode<Label>("HintText");
        HintText.Hide();

        ResetAllNumber();

        EscapeButton.Pressed += () => {
            Hide();
            ResetAllNumber();
            CountForThreeZero = 0;
        };

        EnterButton.Pressed += () => {
            EnterLevel();
        };
    }

    public void ResetAllNumber() {
        NumberSetter1.NumLabel.Text = "0";
        NumberSetter2.NumLabel.Text = "0";
        NumberSetter3.NumLabel.Text = "1";
    }

    public void ShowHintTextWithKey(string key) {
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlaySoundEffects("bell");
        
        HintTextTween?.Kill();
        HintText.Text = TranslationServer.Translate(key);
        HintText.Modulate = new Color(1, 1, 1, 0);
        HintText.Show();

        HintTextTween = GetTree().CreateTween();
        HintTextTween.TweenProperty(HintText, "modulate:a", 1.0, 0.3);
        HintTextTween.TweenProperty(HintText, "modulate:a", 0.0, 1.0).SetDelay(1);
        HintTextTween.TweenCallback(Callable.From(HintText.Hide));
    }

    public void EnterLevel() {
        var res = $"{NumberSetter1.NumLabel.Text}{NumberSetter2.NumLabel.Text}{NumberSetter3.NumLabel.Text}";
        // 先检查是否已经存在
        if (((Array)DataLoader.UserSettings["saves"]).Contains(res)) {
            ShowHintTextWithKey("ERR_ALREADYHAS");
            return;
        }
        // 如果不存在000，进行判断
        if (res == "000") {
            if (CountForThreeZero < 10) {
                ShowHintTextWithKey("ERR_INVALIDNUM");
                CountForThreeZero += 1;
            }
            else {
                // 获得成就
                
                // 创建debug档
            }
        }
        else {
            DataLoader.CreateSave(res);
            DataLoader.StoreUserSettings();
            
            OnCreated?.Invoke();
            
            ResetAllNumber();
            CountForThreeZero = 0;
            Hide();
        }
    }

}
