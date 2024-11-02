using Godot;
using System;
using DouCardPuzzoom.scripts.manager;

public partial class Room_C_HK : Sprite2D
{
    public InterArea Outdoor;
    public override void _Ready() {
        base._Ready();
        Outdoor = GetNode<InterArea>("OutDoor");
        Outdoor.OnInteracted += () => {
            SaveManager.PlayerPositionScene1 = new Vector2(-48, 50); // 绿门的位置下方
            var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
            sceneChanger.ChangeScene("res://scenes/MainHouseScene1.tscn");
        };
    }
}
