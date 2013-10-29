;redcode
;name imp-stop
	ORG start
def	dat 0, -1
	dat 0, 0
loop	djn loop, def
	djn loop, def
start	spl loop
	spl loop
	spl loop
imp	mov 0, 1