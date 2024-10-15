extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "item"
    type = "item_box"
    inherit_effects = false
    inherit_art = false
    inherit_groups = false
    display_name = "Item Box"
    localized_names = {}
    description = ""
    localized_descriptions = {}
    values = [10]
    manually_destroyable = true
    rarity = "common"
    groups = []
    sfx = []
    effects = [
        {
            "emails_to_add": [{"type": "add_any_item"}],
            "items_to_add": [{"type": "golden_carrot"}]
        },
    ]