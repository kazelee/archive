extends Node

signal new_game
signal to_show_cards

signal to_call
signal call_taken
signal call_over

signal to_lead
signal lead_taken
signal lead_over

signal to_show_final

signal score_doubled

var is_ai_mode := false


var _order := PackedInt32Array([47, 51, 3, 7, 11, 15, 19, 23, 27, 31, 35, 39, 43, 46, 50, 2, 6, 10, 14, 18, 22, 26, 30, 34, 38, 42, 45, 49, 1, 5, 9, 13, 17, 21, 25, 29, 33, 37, 41, 44, 48, 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 52, 53])


var _players := [1, 2, 3]
var _scores := {1: 1000, 2: 1000, 3: 1000}
var _new_scores := {1: 100, 2: 50, 3: 50}
var _all_cards := {}

var my_id := 1

var _round_first_id := 1
var _now_turn_id := 1: set = set_now_turn_id

var highest_call := 0
var highest_caller := 1
var _call_times := 0

var biggest_lead := []:
	set(v):
		biggest_lead = v
		var last_str := GlobalEngine.idarr2str(biggest_lead)
		var last_cards := GlobalEngine.str2cards(last_str)
		last_type = GlobalEngine.check_card_type(last_cards)

var biggest_leader := 1
var _pass_times := 0


#func _ready() -> void:
#	Rule.score_doubled.connect(_double_scores)


# -------- Rule.gd BEG --------

enum State {
	FIRST, NONE, FOLLOW
}

var last_type := []: set = set_type
#var last_cards := []

# type: [str, int]
func set_type(v: Array) -> void:
	last_type = v
	if v == []:
		return
	if last_type[0][0] == "bomb" or last_type[0][0] == "rocket":
		score_doubled.emit()
		_double_scores()


func get_lead_state(id: int) -> State:
	if biggest_lead == []:
		return State.FIRST
	return State.FOLLOW


@rpc("any_peer", "call_local")
func set_last_type(type: Array) -> void:
	last_type = type.duplicate(true)

# -------- Rule.gd END --------


func register(id: int, ids: Array) -> void:
	my_id = id
	_players = ids
	_scores = {}
	for i in ids:
		_scores[i] = 1000
	_new_scores = {}


func reset() -> void:
	_round_first_id = get_id_after(_round_first_id)
	_now_turn_id = _round_first_id

	highest_call = 0
	_call_times = 0
	biggest_lead = []


func new_round() -> void:
	new_game.emit()
	if my_id == 1:
		_shuffle_and_deal_cards()

	await get_tree().create_timer(0.5).timeout
	to_show_cards.emit()


func call_landlord() -> void:
	to_call.emit(_now_turn_id)


@rpc("any_peer", "call_local")
func take_my_call(id: int, called: int) -> void:
	call_taken.emit(id, called)
	if called >= highest_call:
		highest_call = called
		highest_caller = id

	_call_times += 1
	_turn_to_next()

	if _is_landlord_out():
		_now_turn_id = highest_caller

		_new_scores[highest_caller] = 100 * highest_call
		_new_scores[get_id_after(highest_caller)] = 50 * highest_call
		_new_scores[get_id_after(highest_caller, 2)] = 50 * highest_call

		_deal_extra_cards()
		await get_tree().create_timer(0.5).timeout
		call_over.emit()
		await get_tree().create_timer(0.5).timeout
		lead_cards()
		return

	if _no_one_calls():
		await get_tree().create_timer(0.5).timeout
		reset()
		new_round()
		return

	await get_tree().create_timer(0.5).timeout
	call_landlord()


func lead_cards() -> void:
	to_lead.emit(_now_turn_id)


@rpc("any_peer", "call_local")
func take_my_lead(id: int, cards: Array) -> void:

	if cards == []:
		_pass_times += 1
	else:
		biggest_lead = cards.duplicate(true)
		biggest_leader = id
		_pass_times = 0
		for card in cards:
#			get_cards(id).erase(card)
			_all_cards[id].erase(card)
		_all_cards[id] = sorted(_all_cards[id])
#	print(_pass_times)
	_turn_to_next()

	if get_cards(id) == []:
		lead_taken.emit(id, cards)
		await get_tree().create_timer(1).timeout
		round_end(id)
		return

	if _pass_times == 2:
		_now_turn_id = biggest_leader
		biggest_lead = []
		_pass_times = 0
#		Rule.last_type = []
#		Rule.rpc("set_last_type", [])
		self.rpc("set_last_type", [])
		await get_tree().create_timer(0.5).timeout
		lead_taken.emit(id, cards)
		await get_tree().create_timer(0.5).timeout
		lead_over.emit()
		lead_cards()
		return

#	lead_taken.emit(id, sorted(cards))
#	await get_tree().create_timer(0.5).timeout

	await get_tree().create_timer(0.5).timeout
	lead_taken.emit(id, cards)
	await get_tree().create_timer(0.5).timeout
	lead_cards()


func round_end(win_id: int) -> void:
	for id in _scores.keys():
		if id == win_id or highest_caller != id and highest_caller != win_id:
			pass
		else:
			_new_scores[id] *= -1
		_scores[id] += _new_scores[id]
	await get_tree().create_timer(0.5).timeout
	to_show_final.emit(my_id, _new_scores, _scores)


func _shuffle_and_deal_cards() -> void:
	var new_deck := range(54)
	randomize()
	new_deck.shuffle()
	for i in range(3):
		var cards_slice = new_deck.slice(i, 51, 3)
		self.rpc("_share_cards", _players[i], sorted(cards_slice))

	self.rpc("_share_cards", 0, new_deck.slice(51, 54))


@rpc("any_peer", "call_local")
func _share_cards(id: int, cards: Array) -> void:
	_all_cards[id] = cards.duplicate(true)


func sorted(cards: Array) -> Array:
	var new_cards = cards
	new_cards.sort_custom(func(a, b) -> bool: return _order[a] > _order[b])
	return new_cards


func _turn_to_next() -> void:
	_now_turn_id = get_id_after(_now_turn_id)


func _is_landlord_out() -> bool:
	return highest_call == 3 or highest_call != 0 and _call_times == 3


func _deal_extra_cards() -> void:
	var landlord_cards = get_cards(highest_caller)
	landlord_cards.append_array(get_cards(0))
	_all_cards[highest_caller] = landlord_cards.duplicate(true)
	_share_cards(highest_caller, sorted(landlord_cards))


func _no_one_calls() -> bool:
	return highest_call == 0 and _call_times == 3


func _double_scores() -> void:
	for id in _players:
		_new_scores[id] *= 2


func set_now_turn_id(v: int) -> void:
	_now_turn_id = v
#	print("[%10s] now turn: %s" % [my_id, _now_turn_id])


func get_id_after(id: int, shift: int = 1) -> int:
	var index = _players.find(id)
	return _players[(index + shift) % 3]


func get_cards(id: int) -> Array:
	return _all_cards[id]


func get_scores(id: int) -> int:
	return _scores[id]


func get_now_turn() -> int:
	return _now_turn_id

