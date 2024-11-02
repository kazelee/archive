extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "email"
    type = "add_any_item"
    display_name = "Add Any Item"
    header_text = "Add Any Item"
    text = "Feel free to add an item!"
    replies = ["<icon_confirm>"]
    reply_results = [{"emails_to_add": [{"type": "add_item_prompt", "extra_values": {"after_spin": true, "forced_rarity": ["very_rare"], }}], "value_to_change": "items_to_choose_from", "diff": 100, }]