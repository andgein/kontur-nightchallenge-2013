;redcode
;name rewired
;author John Metcalf
;strategy bomber
;assert 1

; 27 May 2008

    for CORESIZE==8000
    step equ 2376
    time equ 498
    imp equ 2667
    rof

    for CORESIZE==8192
    step equ 2456
    time equ 510
    imp equ 2731
    rof

    first equ step
    gate equ 8

m    mov -1,        <step
j    jmp -step-1,   -step+7

loop add #2*step+1, ptr
    mov m,         @ptr
ptr  mov j,         @first
    mov s,         <ptr
    djn loop,      #time

s    spl 0,         <-gate
    mov 2,         <-2
    djn -1,        <s-gate-2*imp
    dat <-gate-1,  <-gate-imp

    end loop

