width equ 80
        org    koch

        stack equ count+30

koch:   mov    #2-ptr, >stack
        jmp    count,  }move

        sub.a  #2,     move
        mov    #2-ptr, >stack
        jmp    count

        jmp    return, }move

count:  djn    koch,   #11
        mod.a  #8,     move
        div.a  #2,     move
        add.b  *move,  pos
        mul.a  #2,     move
pos:    add    #1,     koch+71*width/2+16
return: mov.ba <stack, ptr
ptr:    jmp    0,      >count

move:   dat    -1
        dat    -width
        dat    1
        dat    width