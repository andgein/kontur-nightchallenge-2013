;redcode
;name ImpSpawnerV2
;author Skybuck Flying
;assert 1
;strategy Spawns an imp on each line
;history Created 18 december 2007
jmp begin
imp mov 0, 1
begin	mov imp, @counter
	spl @counter
	add #256, counter
djn begin, #32
mov 0, 1
counter dat #0
	
