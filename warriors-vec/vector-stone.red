step EQU 123
v  dat  #0,  #step
  dat  #0,  #2*step
  dat  #0,  #3*step
  dat  #0,  #4*step
loop add.4  #(5*step), v
  mov.4 v-4, @v
  jmp  loop
  end loop
