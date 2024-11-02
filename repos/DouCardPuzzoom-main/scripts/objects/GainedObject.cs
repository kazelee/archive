using System.Collections.Generic;
using Godot;

namespace DouCardPuzzoom.scripts.objects; 

public partial class GainedObject : Area2D {
    public string ObjectName;
    private Texture2D _texture;

    public override void _Ready() {
        base._Ready();
    }
}