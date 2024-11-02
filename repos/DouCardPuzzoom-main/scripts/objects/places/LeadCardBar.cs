using Godot;
using System;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.enums;

public partial class LeadCardBar : HBoxContainer {
    public event Action<LeadStates> HasLedCards;

    public event Action UndoFinished;
    
    public TextureButton PassButton;
    public TextureButton HintButton;
    public TextureButton PlayButton;

    public Tween tween;

    public override void _Ready() {
        // tween = CreateTween();
        PassButton = GetNode<TextureButton>("PassButton");
        HintButton = GetNode<TextureButton>("HintButton");
        PlayButton = GetNode<TextureButton>("PlayButton");

        InitVisual();
        
        PassButton.Pressed += OnPassButtonPressed;
        HintButton.Pressed += OnHintButtonPressed;
        PlayButton.Pressed += OnPlayButtonPressed;
    }

    /// <summary>
    /// 直接读取GameLogic数据，不需要传递参数
    /// </summary>
    public void InitButtons() {
        switch (GameLogic.CurrentState) {
            case TurnStates.First:
                PassButton.Hide();
                HintButton.Hide();
                PlayButton.Show();
                break;
            case TurnStates.None:
                PassButton.Show();
                PlayButton.Hide();
                HintButton.Hide();
                break;
            case TurnStates.Follow:
                PassButton.Show();
                PlayButton.Show();
                HintButton.Show();
                if (GameLogic.GameMode == GameModes.Rest) {
                    PassButton.Hide();
                }
                if (GameLogic.GameMode == GameModes.Blind) {
                    HintButton.Hide();
                }
                break;
        }
    }

    public void InitVisual() {  
        if (TranslationServer.GetLocale() is "zh_CN" or "zh_TW") {  
            var passNormalPressed = ResourceLoader.Load<Texture2D>("res://assets/buttons/button-pass-cn.png");  
            PassButton.TextureNormal = passNormalPressed;  
            PassButton.TexturePressed = passNormalPressed;  
            PassButton.TextureHover = ResourceLoader.Load<Texture2D>("res://assets/buttons/hover-pass-cn.png");
            
            var hintNormalPressed = ResourceLoader.Load<Texture2D>("res://assets/buttons/button-hint-cn.png");  
            HintButton.TextureNormal = hintNormalPressed;  
            HintButton.TexturePressed = hintNormalPressed;  
            HintButton.TextureHover = ResourceLoader.Load<Texture2D>("res://assets/buttons/hover-hint-cn.png"); 
            
            var playNormalPressed = ResourceLoader.Load<Texture2D>("res://assets/buttons/button-play-cn.png");  
            PlayButton.TextureNormal = playNormalPressed;  
            PlayButton.TexturePressed = playNormalPressed;  
            PlayButton.TextureHover = ResourceLoader.Load<Texture2D>("res://assets/buttons/hover-play-cn.png");  
        }
        else { // if tr.locate is en or ja  
            var passNormalPressed = ResourceLoader.Load<Texture2D>("res://assets/buttons/button-pass-en.png");  
            PassButton.TextureNormal = passNormalPressed;  
            PassButton.TexturePressed = passNormalPressed;  
            PassButton.TextureHover = ResourceLoader.Load<Texture2D>("res://assets/buttons/hover-pass-en.png"); 
            
            var hintNormalPressed = ResourceLoader.Load<Texture2D>("res://assets/buttons/button-hint-en.png");  
            HintButton.TextureNormal = hintNormalPressed;  
            HintButton.TexturePressed = hintNormalPressed;  
            HintButton.TextureHover = ResourceLoader.Load<Texture2D>("res://assets/buttons/hover-hint-en.png");  
            var playNormalPressed = ResourceLoader.Load<Texture2D>("res://assets/buttons/button-play-en.png"); 
            
            PlayButton.TextureNormal = playNormalPressed;  
            PlayButton.TexturePressed = playNormalPressed;  
            PlayButton.TextureHover = ResourceLoader.Load<Texture2D>("res://assets/buttons/hover-play-en.png");  
        }
    }
    
    public void OnPassButtonPressed() {
        HasLedCards(LeadStates.Pass);
    }
    public void OnHintButtonPressed() {
        HasLedCards(LeadStates.Hint);
    }
    public void OnPlayButtonPressed() {
        HasLedCards(LeadStates.Play);
    }
    
    // Postion.X ± 20: 取绝对值，-49 to -29; y === 27
    
    public void MyHide() {
        tween?.Kill();
        
        tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
        tween.TweenProperty(this, "position", new Vector2(-29, 27), 0.3);
        tween.Parallel().TweenProperty(this, "modulate:a", 0.0, 0.2);
        tween.TweenCallback(Callable.From(Hide));
    }

    public void MyShow() {
        tween?.Kill();

        Position = new Vector2(-29, 27);
        Modulate = new Color(1, 1, 1, 0);
        Show(); 
        
        tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
        tween.TweenProperty(this, "position", new Vector2(-49, 27), 0.2);
        tween.Parallel().TweenProperty(this, "modulate:a", 1.0, 0.3);
        tween.TweenCallback(Callable.From(() => { UndoFinished?.Invoke(); }));
    }
    
}
