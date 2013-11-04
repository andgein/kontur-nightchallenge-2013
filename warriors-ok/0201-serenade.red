;redcode
;name serenade
;author John Metcalf
;strategy new qscan -> bomber with separate clear
;assert CORESIZE==8000

; 27 May 2008

       qfirst equ (qp2+2*qstep)
       qdist  equ qfirst+111
       qstep  equ 222

       qi  equ 7        
       qr  equ 7

qbomb   dat <qi/2-qi*qr,   <qi*qr-qi/2

       qa  equ qstep*16
       qb  equ qstep*5+2
       qc  equ qstep*10
       qd  equ qstep*2
       qe  equ qstep*1

qgo     cmp qdist+qc,      qfirst+qc
       jmp qfast,         <qa
       cmp qdist+qe+qd,   qfirst+qe+qd
qp1     jmp <qfast,        <qc
qp2     cmp qdist,         qfirst
qp3     jmp qskip,         <qe

       cmp qdist+qb,      qfirst+qb
q1      djn qfast,         #qp1

       cmp qdist+qd+qc,   qfirst+qd+qc
       jmp qslow,         <qfirst+qd+qc+4
       cmp qdist+qd+qb,   qfirst+qd+qb
x1      jmp qslow,         <q1
       cmp qdist+qc+qc,   qfirst+qc+qc
q2      djn qslow,         #qp2
       cmp qdist+qd,      qfirst+qd
       jmp qslow,         <qfast
       cmp qdist+qa,      qfirst+qa
       jmp q1,            <q1

       cmp qdist+qa+qd,   qfirst+qa+qd
       jmp x1,            <q1
       cmp qdist+qc+qb,   qfirst+qc+qb
       jmp q2,            <q1
       cmp qdist+qe+qd+qc,qfirst+qe+qd+qc
       jmp qslower,       <qfirst+qe+qd+qc+4
       cmp qdist+qe+qd+qb,qfirst+qe+qd+qb
       jmp qslower,       <q1
       cmp qdist+qe+qc+qc,qfirst+qe+qc+qc
       jmp qslower,       <q2
       cmp qdist+qd+qd+qc,qfirst+qd+qd+qc
q3      djn qslower,       #qp3
       cmp qdist+qe+qc,   qfirst+qe+qc
       jmp <qfast,        <q2
       cmp qdist+qd+qd,   qfirst+qd+qd
       jmp <qfast,        <q3
       cmp qdist+qd+qd+qb,qfirst+qd+qd+qb
       slt <q3,           <q1

       jmz pgo,           qdist+qe+qd+qc+10

qslower add @q3,           @qslow
qslow   add @q2,           qkil
qfast   add @q1,           @qslow

qskip   cmp <qdist+qstep+50, @qkil
       jmp qloop,         <1234

       add #qdist-qfirst, qkil
qloop   mov qbomb,         @qkil
qkil    mov <qfirst+qstep+50, <qfirst
       sub #qi,           @qloop
       djn qloop,         #qr+2

    step equ 2376
    time equ 498
    first equ step

    dist equ 6500
    pgo equ boot

boot mov loop+5,    boot+dist+5
    mov loop+4,    <boot
    mov loop+3,    <boot
    mov loop+2,    <boot
    mov loop+1,    <boot
    mov loop,      <boot
    mov loop-1,    <boot

cboo mov wipe+3,    boot+dist+5-step+2
    mov wipe+2,    <cboo
    mov wipe+1,    <cboo
    mov wipe,      <cboo

    jmp boot+dist+1

m    mov -1,        <step

loop add #2*step+1, ptr
    mov m,         @ptr
ptr  mov j,         @first
    mov 1-step,    <ptr
    djn loop,      #time
j    jmp -step-1,   -step+7

wipe spl 0,         <-8
    mov 2,         <-5
    djn -1,        <-5344
    dat <-9,       <-2675

    end qgo

