using Godot;
using System;
using System.Threading.Tasks;

public partial class SoundManager : Node {
    public AudioStreamPlayer SFXPlayer;
    public AudioStreamPlayer MusicPlayer;

    public AudioStreamPlayer ResultPlayer;
    public AudioStreamPlayer ButtonSFXPlayer;
    public AudioStreamPlayer AreaSFXPlayer;

    public Timer Timer;
    
    public string CurrentPlayingSFX = "";
    public string CurrentPlayingMusic = "";

    public override void _Ready() {
        base._Ready();
        Timer = new Timer();
        AddChild(Timer);
        Timer.OneShot = true;
        Timer.Timeout += OnTimeOut;
        
        SFXPlayer = GetNode<AudioStreamPlayer>("SFX/SFXPlayer");
        MusicPlayer = GetNode<AudioStreamPlayer>("Music/MusicPlayer");
        ResultPlayer = GetNode<AudioStreamPlayer>("SFX/ResultPlayer");
        ButtonSFXPlayer = GetNode<AudioStreamPlayer>("SFX/ButtonSFXPlayer");
        AreaSFXPlayer = GetNode<AudioStreamPlayer>("SFX/AreaSFXPlayer");

        MusicPlayer.Finished += () => {
            if (CurrentPlayingMusic == "") {
                return;
            }
            Timer.Start(10); 
            // await GetTree().CreateTimer(30);
        };
        
        // 把button音效的部分放在SoundManager里面了……
        // 参考：https://tieba.baidu.com/p/8187506969
        GetTree().NodeAdded += OnNodeAdded;
    }

    public void OnNodeAdded(Node node) {
        if (node is Button button) {
            button.Pressed += OnButtonPressed;
        }
        else if (node is TextureButton textureButton) {
            textureButton.Pressed += OnButtonPressed;
        }
    }

    public void OnButtonPressed() {
        ButtonSFXPlayer.Play();
    }
    
    public void PlaySoundEffects(string name, float pos = 0f) {
        SFXPlayer.Stop();
        CurrentPlayingSFX = name;
        SFXPlayer.Stream = GD.Load<AudioStreamMP3>($"res://assets/sound/sfx/{name}.mp3");
        switch (name) {
            case "walking":
                SFXPlayer.Play(0.5f); // 走路从1s开始（懒得改音频了）
                break;
            case "stair":
                SFXPlayer.Play(6.5f);
                break;
            case "pick":
                SFXPlayer.Play(0.5f);
                break;
            case "bell":
                SFXPlayer.Play(0.25f);
                break;
            default:
                SFXPlayer.Play(pos);
                break;
        }
    }
    
    public void PlayAreaEffects(string name, float pos = 0f) {
        AreaSFXPlayer.Stop();
        // CurrentPlayingSFX = name;
        AreaSFXPlayer.Stream = GD.Load<AudioStreamMP3>($"res://assets/sound/sfx/{name}.mp3");
        switch (name) {
            case "pick":
                AreaSFXPlayer.Play(0.5f);
                break;
            case "bell":
                AreaSFXPlayer.Play(0.25f);
                break;
            case "high-click":
                AreaSFXPlayer.Play(0.4f);
                break;
            default:
                AreaSFXPlayer.Play(pos);
                break;
        }
    }

    public void PlayerResultEffects(string name) {
        ResultPlayer.Stop();
        // CurrentPlayingSFX = name;
        ResultPlayer.Stream = GD.Load<AudioStreamMP3>($"res://assets/sound/sfx/{name}.mp3");
        if (name == "unsolved") ResultPlayer.Play(0.4f);
        else ResultPlayer.Play();
    }

    public void PlayMusic(string name) {
        if (CurrentPlayingMusic == name && MusicPlayer.Playing) return; // 必须正在播放才算数！
        MusicPlayer.Stop();
        Timer.Stop();
        
        MusicPlayer.Stream = GD.Load<AudioStreamMP3>($"res://assets/sound/music/{name}.mp3");
        CurrentPlayingMusic = name;
        MusicPlayer.Play();
    }

    public void PlayCurrent() {
        if (CurrentPlayingMusic == "") return;
        MusicPlayer.Stop();
        Timer.Stop();
        
        MusicPlayer.Stream = GD.Load<AudioStreamMP3>($"res://assets/sound/music/{CurrentPlayingMusic}.mp3");
        MusicPlayer.Play();
    }
    
    // public string TmpMusicName = "";
    public void PlayMusicTmp(string name) {
        MusicPlayer.Stop();
        Timer.Stop();
        
        MusicPlayer.Stream = GD.Load<AudioStreamMP3>($"res://assets/sound/music/{name}.mp3");
        MusicPlayer.Play();
    }
    
    public async Task DelayFunc(int delay = 30000) { // 默认30s
        await Task.Delay(delay);
    }

    public void OnTimeOut() {
        MusicPlayer.Play();
    }
    
}

