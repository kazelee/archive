extends Node2D

@onready var dirt_cliff_tile_map: TileMap = $DirtCliffTileMap
@onready var camera_2d: Camera2D = $Camera2D


func _ready() -> void:
	var used := dirt_cliff_tile_map.get_used_rect()
	# tile_size 是图块的大小（实际像素=图块数*图块像素数）
	var tile_size := dirt_cliff_tile_map.tile_set.tile_size 
	
	# used 获取的是 position（左上角）和 end（右下角）的坐标
	camera_2d.limit_left = used.position.x * tile_size.x
	camera_2d.limit_top = used.position.y * tile_size.y
	camera_2d.limit_right = used.end.x * tile_size.x
	camera_2d.limit_bottom = used.end.y * tile_size.y
