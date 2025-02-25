using Godot;
using System;
using System.Threading.Tasks;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.scenes;

// [Global]

// [GlobalClass]
public partial class SceneChanger : CanvasLayer {
    public ColorRect ColorRect;
    // public Tween Tween;

    public override void _Ready() {
        base._Ready();
        ColorRect = GetNode<ColorRect>("ColorRect");
    }

    // 直接照搬Godot GDS代码，不考虑C#静态语言优化了
    public void ChangeScene(string path) {
        // var soundManager = GetNode<SoundManager>("/root/SoundManager");
        // soundManager.PlaySoundEffects("door-opening");
        
        var tween = GetTree().CreateTween();
        tween.TweenCallback(Callable.From(ColorRect.Show));
        tween.TweenProperty(ColorRect, "color:a", 1.0, 0.2);
        // C# doesn't support `bind()` but can use lambda expression instead
        tween.TweenCallback(Callable.From(() => InnerChangeScene(path)));
        tween.TweenProperty(ColorRect, "color:a", 0.0, 0.3);
        tween.TweenCallback(Callable.From(ColorRect.Hide));
    }

    public void InnerChangeScene(string path) {
        var oldScene = GetTree().CurrentScene;
        // path must be PackedScene (*.tscn)
        var newScene = GD.Load<PackedScene>(path).Instantiate();

        OnSceneChanged(oldScene, newScene);
        
        var root = GetTree().Root;
        root.RemoveChild(oldScene);
        root.AddChild(newScene);
        GetTree().CurrentScene = newScene;
        
        oldScene.QueueFree();
    }

    public void OnSceneChanged(Node oldScene, Node newScene) {
        if (oldScene is LogoScreen) return;
        if (oldScene is MainHouseScene && newScene is MainHouseScene) {
            // 播放上下楼声音
            var soundManager = GetNode<SoundManager>("/root/SoundManager");
            soundManager.PlaySoundEffects("stair");

            // await DelayFunc();
            // var tween = GetTree().CreateTween();
            // tween.TweenProperty(ColorRect, "color:a", 0.0, 1.5); // 上楼的时候慢一些
            // tween.TweenCallback(Callable.From(ColorRect.Hide));
            
        }
        else { // 1. 同层房间互转；2. 切换到GS（可能后续会另外设置）
            if (oldScene is MainHouseScene mainHouseScene && newScene is GameScene) {
                SaveManager.LastPlayerPosition = mainHouseScene.Player.Position;
            }
            // 播放开关门声音
            var soundManager = GetNode<SoundManager>("/root/SoundManager");
            soundManager.PlaySoundEffects("door-close");
            
            // var tween = GetTree().CreateTween();
            // tween.TweenProperty(ColorRect, "color:a", 0.0, 0.5);
            // tween.TweenCallback(Callable.From(ColorRect.Hide));
        }
        // else {
        // }
    }

    // public async Task DelayFunc(int delay = 1000) {
    //     await Task.Delay(delay);
    // }
}
