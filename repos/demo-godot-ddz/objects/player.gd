extends Node2D

signal landlord_out

enum Position {
	DOWN, RIGHT, LEFT
}

enum Lead {
	PASS, HINT, SHOT
}

enum State {
	FIRST, NONE, FOLLOW
}

@export var player_position := Position.DOWN

@onready var face: Sprite2D = $Face
@onready var id_label: Label = $ID
@onready var score_label: Label = $Score

@onready var card_place: Node2D = $CardPlace
@onready var lead_place: Node2D = $LeadPlace

@onready var call_landlord_bar: HBoxContainer = $CallLandlordBar
@onready var lead_card_bar: HBoxContainer = $LeadCardBar

@onready var call_landlord_timer: Timer = $CallLandlordTimer
@onready var lead_card_timer: Timer = $LeadCardTimer

var is_ai := false
var my_id := 0

var hint_list := []
var hint_index := 0

func _ready() -> void:
	Game.new_game.connect(reset)
	Game.to_show_cards.connect(show_cards)

	Game.to_call.connect(call_landlord)
	Game.call_taken.connect(_show_call_result)
	call_landlord_bar.has_called.connect(upload_called)
	Game.call_over.connect(check_landlord)

	Game.to_lead.connect(lead_cards)
	Game.lead_taken.connect(_show_lead_result)
	lead_card_bar.has_led.connect(upload_led)
	Game.lead_over.connect(Callable(lead_place, "clear"))

	Game.to_show_final.connect(update_scores)

	_init_player_id()
	lead_place.init_lead_position(player_position)


func reset() -> void:
	face.texture = preload("res://assets/icons/icon_farmer.png")
	card_place.reset()
	lead_place.clear()
	call_landlord_bar.reset()


func show_cards() -> void:
	if player_position == Position.DOWN:
		print("[%10s] my cards: %s" % [my_id, Game.get_cards(my_id)])
		card_place.update_cards(Game.get_cards(my_id))

		await get_tree().create_timer(0.5).timeout
		Game.call_landlord()

	else:
		card_place.init_others_cards()


func call_landlord(id: int) -> void:
	if id != my_id:
		return

	lead_place.show_count_down(15)

	if is_ai:
		# 随机叫分的逻辑仍然保留（随机出牌就算了）
		print("now turn: %s" % Game.get_now_turn())
		print("[%10s] try to call landlord" % my_id)
		randomize()
		await get_tree().create_timer(3).timeout
		var call_score = [1, 2, 3, 0][randi_range(Game.highest_call, 3)]
		print("[%10s] called: %s" % [my_id, call_score])
		Game.take_my_call(my_id, call_score)
		return

	if player_position == Position.DOWN:
		print("now turn: %s" % Game.get_now_turn())
		print("[%10s] try to call landlord" % my_id)
		call_landlord_bar.init_buttons(Game.highest_call)
		call_landlord_bar.show()
		call_landlord_timer.start(15)


func upload_called(call_score: int) -> void:
	call_landlord_bar.hide()
	call_landlord_timer.stop()

	print("[%10s] called: %s" % [my_id, call_score])
	Game.rpc("take_my_call", my_id, call_score)


func check_landlord() -> void:
	lead_place.clear()
	if my_id == Game.highest_caller:
		await get_tree().create_timer(0.5).timeout

		if player_position == Position.DOWN:
			print("[%10s] cards -> %s" % [my_id, Game.get_cards(my_id)])
			card_place.update_cards(Game.get_cards(my_id))
		else:
			card_place.update_others_cards(20)
		face.texture = preload("res://assets/icons/icon_landlord.png")


func lead_cards(id: int) -> void:
	if id != my_id:
		return

#	if is_ai:
#		lead_place.show_count_down(25)
#		print("now turn: %s" % Game.get_now_turn())
#		print("[%10s] try to lead cards" % my_id)
##		var timer := Timer.new()
##		add_child(timer)
##		timer.start(10)
#		var s = Game.biggest_lead.size()
#		randomize()
#		var selected := []
#		var tmp_cards := Game.get_cards(my_id)
#		var times := 1000
#		while times > 0:
#			times -= 1
#			var num := 0
#
#			if s == 0:
#				var ran := randfn(4.0, 2.0)
#				num = int(ran) % 20 + 1
#			else:
#				var choice := []
#				choice.resize(8)
#				choice.fill(s)
#				choice.append_array([4, 2])
#				num = choice[randi_range(0, 9)]
#
#			print(num)
#			while selected.size() < num:
#				var tmp :int = tmp_cards.pick_random()
#				if tmp not in selected:
#					selected.append(tmp)
#			print(selected)
#			if Rule.is_valid(selected):
#				break
#			else:
#				selected = []
#				continue
#		print(selected)
#		if not Rule.is_valid(selected):
#			selected = []
#			if Rule.get_lead_state(my_id) == State.FIRST:
#				selected.append(tmp_cards.back())
#
##		selected = selected if Rule.is_valid(selected) else \
##		[tmp_cards.back()] if Rule.get_lead_state(my_id) == State.FIRST else []
#		print("[%10s] led: %s" % [my_id, selected])
#		Game.take_my_lead(my_id, selected)
##		timer.queue_free()
#		return

	if is_ai:
		# 默认出右起第一张，提示有牌出提示牌（默认第一个提示）
		if Game.get_lead_state(my_id) == State.FIRST:
			await get_tree().create_timer(3).timeout
			var back = Game.get_cards(my_id).back()
			var lead = []
			lead.append(back)
			Game.take_my_lead(my_id, lead)
			return
#			_show_lead_result(my_id, lead)

		print("now turn: %s" % Game.get_now_turn())
		print("[%10s] try to lead cards" % my_id)
		if Game.biggest_lead != []:
			var last_str := GlobalEngine.idarr2str(Game.biggest_lead)
			var my_cards := GlobalEngine.idarr2str(Game.get_cards(my_id))
			var cards_dic: Dictionary = GlobalEngine.list_greater_cards(last_str, my_cards)
			if cards_dic == {}:
				await get_tree().create_timer(1).timeout
				Game.take_my_lead(my_id, [])
				return
			else:
				var hint_one = cards_dic.values()[0][0]
				var try := GlobalEngine.str2cards(hint_one)
				var cards_reverse = Game.get_cards(my_id)
				cards_reverse.reverse()
				var visited := []
				for point in try:
					for item in cards_reverse:
						if point == GlobalEngine.point2str(item) and item not in visited:
							visited.append(item)
							break

				await get_tree().create_timer(3).timeout
				Game.take_my_lead(my_id, visited)
				return


	if player_position == Position.DOWN:
		print("now turn: %s" % Game.get_now_turn())
		print("[%10s] try to lead cards" % my_id)
		if Game.biggest_lead != []:
			var last_str := GlobalEngine.idarr2str(Game.biggest_lead)
			var my_cards := GlobalEngine.idarr2str(Game.get_cards(my_id))
			var cards_dic: Dictionary = GlobalEngine.list_greater_cards(last_str, my_cards)
			if cards_dic == {}:
				lead_card_bar.init_buttons(State.NONE)
				lead_card_bar.show()
				lead_card_timer.start(5)
				lead_place.show_count_down(5)
				return
			for arr in cards_dic.values():
				hint_list.append_array(arr)
			print(hint_list)

		lead_card_bar.init_buttons(Game.get_lead_state(my_id))
		lead_card_bar.show()
		lead_card_timer.start(25)

	lead_place.show_count_down(25)


func upload_led(lead: Lead) -> void:
	match lead:
		Lead.SHOT:
			var selected: Array = card_place.selected_cards
			if selected == []:
				return
#			if not Rule.is_valid(selected):
#				return
			var last_str := GlobalEngine.idarr2str(Game.biggest_lead)
			print(last_str)
			var cards_str := GlobalEngine.idarr2str(selected)
			print(cards_str)
			var last_cards := GlobalEngine.str2cards(last_str)
			print(last_cards)
			var my_cards := GlobalEngine.str2cards(cards_str)
			print(my_cards)
			# means last_cards > my_cards?
#			if GlobalEngine.cards_greater(last_cards, my_cards):
#				print("not?")
#				return
			if not GlobalEngine.cards_greater(my_cards, last_cards):
				print("cant")
				return
			lead_card_bar.hide()
			lead_card_timer.stop()
			print("[%10s] led: %s" % [my_id, selected])
			Game.rpc("take_my_lead", my_id, Game.sorted(selected))
			card_place.selected_cards = []
			hint_list = []
			hint_index = 0

			await get_tree().create_timer(0.5).timeout
			card_place.update_cards(Game.get_cards(my_id))

		Lead.HINT:
			print(hint_index)
			print(hint_list)
			var try_str: String = hint_list[hint_index]
			print(try_str)
			var try := GlobalEngine.str2cards(try_str)
			var cards_reverse = Game.get_cards(my_id)
			cards_reverse.reverse()
			var visited := []
			for point in try:
				for item in cards_reverse:
					if point == GlobalEngine.point2str(item) and item not in visited:
						visited.append(item)
						break
			card_place.select_cards(visited)
			hint_index = (hint_index + 1) % len(hint_list)


		Lead.PASS:
			lead_card_bar.hide()
			lead_card_timer.stop()
			Game.rpc("take_my_lead", my_id, [])
			hint_list = []
			hint_index = 0


func update_scores(_id, _new_scores, _final_scores) -> void:
	score_label.text = "Score: " + str(Game.get_scores(my_id))


func _init_player_id() -> void:
	match player_position:
		Position.DOWN:
			my_id = Game.my_id

		Position.RIGHT:
			my_id = Game.get_id_after(Game.my_id, 1)

		Position.LEFT:
			my_id = Game.get_id_after(Game.my_id, 2)

	id_label.text = "ID: " + str(my_id)
	is_ai = Game.is_ai_mode and my_id != 1


func _show_call_result(id: int, score: int) -> void:
	if my_id == id:
		lead_place.show_call_result(score)
		var s = "score_" + str(score)
		print(s)
		SoundManager.play_sound(s)


func _on_call_landlord_timer_timeout() -> void:
	upload_called(0)


func _show_lead_result(id: int, cards: Array) -> void:
	if my_id == id:
		lead_place.show_lead_result(cards)
		SoundManager.play_sound("deal")
		if player_position != Position.DOWN:
			card_place.minus_others_cards(cards.size())


func _on_lead_card_timer_timeout() -> void:
	if player_position != Position.DOWN:
		return
	if my_id != Game.get_now_turn():
		return
	lead_card_bar.hide()
	if lead_card_bar.lead_state == State.FIRST:
		var selected := []
		selected.append(Game.get_cards(my_id).back())
		print("[%10s] led: %s" % [my_id, selected])
		Game.rpc("take_my_lead", my_id, selected)
		hint_list = []
		hint_index = 0
		await get_tree().create_timer(0.5).timeout
		card_place.update_cards(Game.get_cards(my_id))
	else:
		print("[%10s] led: []" % my_id)
		Game.rpc("take_my_lead", my_id, [])

