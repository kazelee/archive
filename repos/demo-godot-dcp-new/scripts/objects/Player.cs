using Godot;
using System;
using DouCardPuzzoom.scripts.manager;

public partial class Player : CharacterBody2D {
    public AnimatedSprite2D AnimatedSprite2D;
    public CollisionShape2D CollisionShape2D;

    public float RunSpeed = 160f;
    public float Acceleration = 800f;

    // public bool IsLeft = false; // 默认方向向右
    public float ScaleNum = 2; // 2-2.8, rect 1-1.4, y 34-50
    
    public override void _Ready() {
        base._Ready();
        AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if (Velocity.IsZeroApprox()) {
            AnimatedSprite2D.Play("idle");
            AnimatedSprite2D.Scale = Vector2.One * ScaleNum;
            // AnimatedSprite2D.Scale = new Vector2(IsLeft ? -ScaleNum : ScaleNum, ScaleNum);
            
            // 停下就立刻断当前声音，如果是walking的话
            var soundManager = GetNode<SoundManager>("/root/SoundManager");
            if (soundManager.CurrentPlayingSFX == "walking" && soundManager.SFXPlayer.Playing) {
                soundManager.SFXPlayer.Stop();
            }
        }
        else {
            AnimatedSprite2D.Play("walk");
            AnimatedSprite2D.Scale = Vector2.One * ScaleNum;

            var soundManager = GetNode<SoundManager>("/root/SoundManager");
            // 当前播放别的音效（没播放完）或者walking播完了
            if (soundManager.CurrentPlayingSFX != "walking" || !soundManager.SFXPlayer.Playing) {
                soundManager.PlaySoundEffects("walking");
            }
            
            // AnimatedSprite2D.Scale = new Vector2(IsLeft ? -ScaleNum : ScaleNum, ScaleNum);
            
            // 不动不必调用（用了大量魔数，参考对应的场景）
            // ScaleNum = (2.8f - 2) / (216 - 100) * (Position.Y - 100 + 50); // 线性插值（还得是脚的位置？）
            // ScaleNum = 3f / 300 * Position.Y; // 至少可以确保scale一定为正数
            // CollisionShape2D.Position = new Vector2(0, (50f - 34) / (216 - 100) * (Position.Y - 100 + 50));
            // var rect = new RectangleShape2D();
            // rect.Size = new Vector2(27, 9) * ScaleNum;
            // CollisionShape2D.Shape = rect;

            ScaleNum = 2 + (2.8f - 2) / (216 - 100) * (Position.Y - 100 + 50);
            
            // 标准的结果反而没有之前乱试出来的效果好，阴影一下就对上了……
            // ScaleNum = 2 + (2.8f - 2) / (216 - 118) * (Position.Y + 108 - 118);
            
            // 测毛测，y=50的时候s肯定是2，傻了，初始化Y设成50就行……
            // GD.Print($"[Debug] Scale: {ScaleNum}; Position.Y: {Position.Y}");
        }
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        if (MouseManager.IsInterAreaAble) {
            var inputVector = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
            var direction = Input.GetAxis("ui_left", "ui_right");

            // 按住快跑键跑得更快
            Velocity = Velocity.MoveToward(
                Input.IsActionPressed("run") ? inputVector * RunSpeed * 2 : inputVector * RunSpeed,
                Acceleration * (float)delta);

            if (!Mathf.IsZeroApprox(direction)) {
                // Y 始终是不变的，所以用 Y 给 X 赋值
                // 能改flip-H就没必要改scale的……
                AnimatedSprite2D.FlipH = direction < 0;
                // AnimatedSprite2D.Scale = new Vector2(direction < 0 ? -ScaleNum : ScaleNum, ScaleNum);
                // IsLeft = direction < 0;
            }
        }
        else {
            // 不能接触时保持禁止
            Velocity = Velocity.MoveToward(Vector2.Zero, Acceleration * (float)delta);
        }
        
        MoveAndSlide();
    }
}
