extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "email"
    type = "terminal"
    display_name = "Terminal"
    header_text = "[Terminal]"
    text = "(Ignore this email.)"
    replies = ["<icon_confirm>"]
    reply_results = [ ]
    
    ready()

func ready():
    var terminal_layer = load("res://TerminalLayer.tscn").instance()
    $"/root".add_child(terminal_layer)