extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "email"
    type = "add_any_tile"
    display_name = "Add Any Tile"
    header_text = "Add Any Tile"
    text = "Feel free to add a tile!"
    replies = ["<icon_confirm>"]
    reply_results = [{"emails_to_add": [{"type": "add_tile_prompt", "extra_values": {"after_spin": true, "forced_rarity": ["very_rare"], "all_symbols_same": true}}], "value_to_change": "symbols_to_choose_from", "diff": 142, }]