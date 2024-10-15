extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "email"
    type = "add_any_tile"
    display_name = "Add Any Tile"
    header_text = "Add Any Tile"
    localized_header_text = {"en": "Add Any Tile", "zh": "添加任意符号"}
    text = "Feel free to add a tile!"
    localized_text = {"en": "Feel free to add a tile!", "zh": "随意添加一个符号吧！"}
    replies = ["<icon_confirm>"]
    reply_results = [
        {
            "emails_to_add": [
                {
                    "type": "add_tile", 
                    "extra_values": {
                        "after_spin": false, 
                        "forced_rarity": ["very_rare"], 
                        "all_symbols_same": true
                    }
                }
            ], 
            "value_to_change": "symbols_to_choose_from", 
            "diff": 152, 
        }
    ]