;redcode
;name Whirl
;author John Metcalf
;strategy qscan -> oneshot
;assert CORESIZE==8000

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

pgo     mov last,          boot+10
c       for 10
        mov last-c,        <pgo
        rof
        jmp boot+7


step    equ -12
first   equ (201*12-8)
boot    equ (pgo-first-100)

dbmb    dat <2667,         <-10

clear   spl 0,             <dbmb-sptr-8
cloop   mov @sloop,        <sptr
        mov @sloop,        <sptr
bomb    djn cloop,         <dbmb-4

steps   dat <step*3,       <step*3+1

sloop   add steps,         @cloop
        mov dbmb,          <sptr
sptr    cmp first+step*2,  @first+1
        jmp <sloop
last    jmp sloop

        end qgo

