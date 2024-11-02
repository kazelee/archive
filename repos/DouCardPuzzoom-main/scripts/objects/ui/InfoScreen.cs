using Godot;
using System;

public partial class InfoScreen : Panel {
    public event Action HideInfo;
    public Label TitleLabel;
    
    // Godot 的 ScrollContainer 似乎只支持 Label，不支持 RichTextLabel？
    // 而且会提示 Label 警告，需要设置最小尺寸（然而Container里面不让设置），但实际上不会出问题
    public Label Content;
    public TextureButton EscapeButton;
    
    public override void _Ready() {
        TitleLabel = GetNode<Label>("TitleLabel");
        Content = GetNode<Label>("ScrollContainer/VBoxContainer/Content");
        EscapeButton = GetNode<TextureButton>("Escape");
        EscapeButton.Pressed += () => { HideInfo?.Invoke(); };
    }

    public void InitContentWithKey(string titleKey, string contentKey) {
        TitleLabel.Text = TranslationServer.Translate(titleKey);
        Content.Text = TranslationServer.Translate(contentKey);
    }

    public void InitContent(string title, string content, bool setContentRed = false) {
        TitleLabel.Text = title;
        Content.Text = content;
        // https://blog.csdn.net/qianhang120/article/details/135239696
        // not "theme_override_colors/font_color" ?
        // Content.Set("custom_colors/font_color",
        //     setContentRed ? new Color(186, 30, 25) : new Color(178, 111, 80)); 
        // #ba1e19 // #b26f50
        
        // 参考：https://tieba.baidu.com/p/7890473542 （还得是timo大神）
        // Content.RemoveThemeFontOverride("font_color");
        // Content.AddThemeColorOverride("font_color", setContentRed ? new Color(186, 30, 25) : new Color(178, 111, 80));
        
        // 无Theme的情况下怎么overwrite都不行，直接改theme了
        Content.Theme =
            GD.Load<Theme>(setContentRed ? "res://scenes/ui/LabelRed.tres" : "res://scenes/ui/LabelBrown.tres");
    }
}
