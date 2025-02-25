using Godot;
using System;
using DouCardPuzzoom.scripts.tools;

public partial class AiInfoScreen : Panel
{
    public event Action HideInfo;
    public TextureRect AiIcon;
    public Label AiNameLabel;
    public Label AiInfoLabel;
    public Label Content;
    public Label ContentThree;
    public Label Addition;
    public TextureButton EscapeButton;
    
    public override void _Ready() {
        AiIcon = GetNode<TextureRect>("AiPanel/AiIcon");
        AiNameLabel = GetNode<Label>("AiNameLabel");
        AiInfoLabel = GetNode<Label>("AiInfoLabel");
        Content = GetNode<Label>("ScrollContainer/VBoxContainer/Content");
        ContentThree = GetNode<Label>("ScrollContainer/VBoxContainer/ContentThree");
        Addition = GetNode<Label>("ScrollContainer/VBoxContainer/Addition");
        EscapeButton = GetNode<TextureButton>("Escape");
        EscapeButton.Pressed += () => { HideInfo?.Invoke(); };
    }

    /// <summary>
    /// state不影响robot，无需担心
    /// </summary>
    /// <param name="name">ai的英文文件格式名称，参考aiTool</param>
    /// <param name="state">0表示未知，1表示表规则，2表示三人规则，3表示里规则，默认为1</param>
    public void InitContentWithName(string name, int state = 1) {
        AiIcon.Texture = GD.Load<Texture2D>($"res://assets/icons/{DialogueTool.GetAiScreenIconName(name)}.png");
        if (name == "Robot") {
            // AiNameLabel.Text = DialogueTool.GetDialogueJobTranslated(name);
            AiNameLabel.Text = TranslationServer.Translate("C_ROBOT");
            AiInfoLabel.Text = "";
            // 即便是3人的情况下，robot还是该出就出，所以不必设计额外的文字提示了
            Content.Text = TranslationServer.Translate("C_ROBOT_1");
            Addition.Text = "";
            Addition.Hide();
            ContentThree.Hide();
            // AiInfoLabel.Hide(); // 其实不Hide也行，就留空就行了
            return;
        }

        AiNameLabel.Text = DialogueTool.GetDialogueNameTranslated(name);
        AiInfoLabel.Text = DialogueTool.GetDialogueJobTranslated(name);
        // 约定一般Ai有三个阶段：初始阶段（未知）中期阶段（打赢5关后解锁）末期阶段（介绍最终的弱点）
        // 如果有的弱点与规则有关，也记录在AI上（规则就不要再改了，心累）
        // 状态依次为（state_LL_0, state_LL_1, state_LL_2）
        // Robot没有上述状态，需要单独处理
        // 【补充】AI还有打队友的设计，所以还有一个state……
        switch (state) {
            case 0:
                Content.Text = DialogueTool.GetAiContentWithState(name, state);
                ContentThree.Hide();
                Addition.Text = "";
                Addition.Hide(); // 为了显示在前面的效果，还是要Hide
                break;
            case 1:
                Content.Text = DialogueTool.GetAiContentWithState(name, 1);
                ContentThree.Text = DialogueTool.GetAiContentWithState(name, 20);
                ContentThree.Show();
                Addition.Text = "";
                Addition.Hide(); // 为了显示在前面的效果，还是要Hide
                break;
            case 2:
                Content.Text = DialogueTool.GetAiContentWithState(name, 1);
                ContentThree.Text = DialogueTool.GetAiContentWithState(name, 2);
                ContentThree.Show();
                Addition.Text = "";
                Addition.Hide(); // 为了显示在前面的效果，还是要Hide
                break;
            case 3:
                Content.Text = DialogueTool.GetAiContentWithState(name, 1);
                ContentThree.Text = DialogueTool.GetAiContentWithState(name, 2);
                ContentThree.Show();
                Addition.Text = DialogueTool.GetAiContentWithState(name, 3);
                Addition.Show();
                break;
            default:
                GD.PrintErr($"Error: wrong state {state}!");
                break;
        }
    }
}
