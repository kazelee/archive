extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "item"
    type = "essence_box"
    inherit_effects = false
    inherit_art = false
    inherit_groups = false
    display_name = "Essence Box"
    localized_names = {"en": "Essence Box", "zh": "精华盒子"}
    description = "You may <color_E14A68>destroy<end> this item and <color_E14A68>add<end> all essences once."
    localized_descriptions = {"en": "You may <color_E14A68>destroy<end> this item and <color_E14A68>add<end> all essence once.", "zh": "你可以<color_E14A68>消除<end>此物品，然后<color_E14A68>添加<end>所有的精华 1 次"}
    values = [10]
    manually_destroyable = true
    rarity = "common"
    groups = []
    sfx = []
    effects = [
        {
            "comparisons": [
                {"a": "destroyed", "b": true}
            ],
            "items_to_add": [{"type": "adoption_papers_essence"}, {"type": "ancient_lizard_blade_essence"}, {"type": "anthropology_degree_essence"}, {"type": "bag_of_holding_essence"}, {"type": "barrel_o_dwarves_essence"}, {"type": "birdhouse_essence"}, {"type": "black_cat_essence"}, {"type": "black_pepper_essence"}, {"type": "blue_pepper_essence"}, {"type": "blue_suits_essence"}, {"type": "booster_pack_essence"}, {"type": "bowling_ball_essence"}, {"type": "brown_pepper_essence"}, {"type": "capsule_machine_essence"}, {"type": "cardboard_box_essence"}, {"type": "checkered_flag_essence"}, {"type": "chicken_coop_essence"}, {"type": "chili_powder_essence"}, {"type": "cleaning_rag_essence"}, {"type": "clear_sky_essence"}, {"type": "coffee_essence"}, {"type": "coin_on_a_string_essence"}, {"type": "comfy_pillow_essence"}, {"type": "compost_heap_essence"}, {"type": "conveyor_belt_essence"}, {"type": "copycat_essence"}, {"type": "credit_card_essence"}, {"type": "cursed_katana_essence"}, {"type": "cyan_pepper_essence"}, {"type": "dark_humor_essence"}, {"type": "devils_deal_essence"}, {"type": "dishwasher_essence"}, {"type": "dwarven_anvil_essence"}, {"type": "egg_carton_essence"}, {"type": "fertilizer_essence"}, {"type": "fifth_ace_essence"}, {"type": "fish_bowl_essence"}, {"type": "flush_essence"}, {"type": "four_leaf_clover_essence"}, {"type": "frozen_pizza_essence"}, {"type": "fruit_basket_essence"}, {"type": "frying_pan_essence"}, {"type": "golden_carrot_essence"}, {"type": "goldilocks_essence"}, {"type": "grave_robber_essence"}, {"type": "gray_pepper_essence"}, {"type": "green_pepper_essence"}, {"type": "guillotine_essence"}, {"type": "happy_hour_essence"}, {"type": "holy_water_essence"}, {"type": "horseshoe_essence"}, {"type": "jackolantern_essence"}, {"type": "kyle_the_kernite_essence"}, {"type": "lefty_the_rabbit_essence"}, {"type": "lemon_essence"}, {"type": "lime_pepper_essence"}, {"type": "lint_roller_essence"}, {"type": "lockpick_essence"}, {"type": "looting_glove_essence"}, {"type": "lucky_carrot_essence"}, {"type": "lucky_cat_essence"}, {"type": "lucky_dice_essence"}, {"type": "lucky_seven_essence"}, {"type": "lunchbox_essence"}, {"type": "maxwell_the_bear_essence"}, {"type": "mining_pick_essence"}, {"type": "mobius_strip_essence"}, {"type": "ninja_and_mouse_essence"}, {"type": "nori_the_rabbit_essence"}, {"type": "oil_can_essence"}, {"type": "oswald_the_monkey_essence"}, {"type": "piggy_bank_essence"}, {"type": "pink_pepper_essence"}, {"type": "pizza_the_cat_essence"}, {"type": "pool_ball_essence"}, {"type": "popsicle_essence"}, {"type": "protractor_essence"}, {"type": "purple_pepper_essence"}, {"type": "quigley_the_wolf_essence"}, {"type": "quiver_essence"}, {"type": "rain_cloud_essence"}, {"type": "recycling_essence"}, {"type": "red_pepper_essence"}, {"type": "red_suits_essence"}, {"type": "reroll_essence"}, {"type": "ricky_the_banana_essence"}, {"type": "ritual_candle_essence"}, {"type": "rusty_gear_essence"}, {"type": "shattered_mirror_essence"}, {"type": "shedding_season_essence"}, {"type": "shrine_essence"}, {"type": "sunglasses_essence"}, {"type": "swapping_device_essence"}, {"type": "swear_jar_essence"}, {"type": "symbol_bomb_big_essence"}, {"type": "symbol_bomb_quantum_essence"}, {"type": "symbol_bomb_small_essence"}, {"type": "symbol_bomb_very_big_essence"}, {"type": "tax_evasion_essence"}, {"type": "telescope_essence"}, {"type": "time_machine_essence"}, {"type": "treasure_map_essence"}, {"type": "triple_coins_essence"}, {"type": "turtle_and_rabbit_essence"}, {"type": "undertaker_essence"}, {"type": "void_party_essence"}, {"type": "void_portal_essence"}, {"type": "wanted_poster_essence"}, {"type": "watering_can_essence"}, {"type": "white_pepper_essence"}, {"type": "x_ray_machine_essence"}, {"type": "yellow_pepper_essence"}, {"type": "zaroffs_contract_essence"},]
        },
    ]