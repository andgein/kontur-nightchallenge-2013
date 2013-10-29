;redcode
;name replicator
	ORG code
size	EQU 72
to	spl 0, (code+size)
from	spl 0, code
code	mov to, (to+size)
	mov from, (from+size)
loop	mov @from, @to
	add #1, from
	add #1, to
	djn loop, cnt
	jmp last
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
	dat 0, 0
last	jmp (code+size), 0


