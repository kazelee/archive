extends TextureRect

@onready var network: Label = $NetworkInfo/Network
@onready var peer_id: Label = $NetworkInfo/PeerID

@onready var menu: VBoxContainer = $Menu


const ADDRESS := "127.0.0.1"
const PORT := 8910
const MAX_NUM := 3

var player_peer_ids := [1]
var my_peer_id := 1


func _ready() -> void:
	SoundManager.bgm_stop()
	multiplayer.multiplayer_peer.close()
	multiplayer.connected_to_server.connect(join_to_server)


func _on_host_pressed() -> void:
	menu.hide()

	var multiplayer_peer = ENetMultiplayerPeer.new()
	if multiplayer_peer.create_server(PORT, MAX_NUM - 1) == OK:
		print("Server created.")
	network.text = "Server"

	multiplayer.multiplayer_peer = multiplayer_peer
	my_peer_id = multiplayer.get_unique_id()
	peer_id.text = str(my_peer_id)


func _on_join_pressed() -> void:
	menu.hide()

	var multiplayer_peer = ENetMultiplayerPeer.new()
	if multiplayer_peer.create_client(ADDRESS, PORT) == OK:
		print("Client created.")
	network.text = "Client"

	multiplayer.multiplayer_peer = multiplayer_peer
	my_peer_id = multiplayer.get_unique_id()
	peer_id.text = str(my_peer_id)


func join_to_server() -> void:
	rpc_id(1, "join_player", my_peer_id)


@rpc("any_peer")
func join_player(id: int) -> void:
	player_peer_ids.append(id)
	if player_peer_ids.size() == MAX_NUM:
		await get_tree().create_timer(0.5).timeout
		self.rpc("game_start", player_peer_ids)


@rpc("any_peer", "call_local")
func game_start(ids: Array) -> void:
	player_peer_ids = ids
	print("[%10s] all ids: %s" % [my_peer_id, player_peer_ids])

	Game.register(my_peer_id, player_peer_ids)
	Game.new_round()
	get_tree().change_scene_to_file("res://scenes/game_scene.tscn")
