;redcode
;name BombSpawnerV4
;author Skybuck Flying
;assert 1
;strategy Spawns an bomb on each line (Improvement(?): Spawned bombs keep bombing their line)
;history Created 30 november 2007
here jmp begin
	; start of bomber
	bomber_line1 mov bomb, @bomb_counter
;	bomber_line1 mov @bomb_counter, bomb	; vice versa, scans for debugging purposes.
	bomber_line2 djn bomber_line1, bomb_counter
	bomber_line3 mov #126, bomb_counter
	bomber_line4 jmp bomber_line1
	bomber_line5 bomb dat #0
	bomber_line6 bomb_counter dat #126
	; end of bomber

counter dat #(here-1)+128
bombs   dat #61
begin
	mov bomber_line1, @counter
	add #1, counter
	mov bomber_line2, @counter
	add #1, counter
	mov bomber_line3, @counter
	add #1, counter
	mov bomber_line4, @counter
	add #1, counter
	mov bomber_line5, @counter
	add #1, counter
	mov bomber_line6, @counter
	sub #5, counter
	spl @counter
	add #128, counter
	djn begin, bombs
	mov #61, bombs
	mov #(here-1)+128, counter
jmp begin
	
