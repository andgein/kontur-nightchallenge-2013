;redcode
;name Galling
;author Christian Schmidt
;strategy "Satellite Defending Network"
;strategy rewritten in '88 code
;assert 1

;optimax 1234
;optimax work 88galling3
;optimax rounds 1 100 200 200
;optimax suite fsh88t

;optimax phase2 fsh88t/scn/scanthecan.red
;optimax phase2 120

;optimax phase3 top20
;optimax phase3 scn:cds:pap
;optimax phase3 140

;optimax phase4 top20
;optimax phase4 100%


pSt1  equ 6956
pSt2  equ 6188
rSt1  equ 2120
rSt2  equ 6851

sSt1  equ 3995
sSt2  equ 7846
sSt3  equ 5507
sSt4  equ 3655

pLen  equ 4

boot2 dat #0,             #rGo+pLen+rSt1
      dat #0,             #rGo+pLen
sBoo  dat #0,             #pGo+pLen+pSt2
      dat #0,             #sGo+pLen
iBoo  dat #0,             #rGo+pLen+rSt2
      dat #0,             #iGo+pLen

boot  spl 1,              pGo+pLen+pSt1
      spl 1,              pGo+pLen
      mov <boot+1,        <boot
      mov <sBoo+1,        <sBoo
      mov <boot2+1,       <boot2
      spl pGo
      mov <iBoo+1,        <iBoo

;paper for the imp satellite

rGo   spl rSt1,           pLen+(rSt1*2)
      mov <rGo+pLen+rSt1, <rGo
      mov <rGo+rSt2+4,    <1
      jmp @0,             rGo+rSt2+4+rSt1

;imp satellite

iGo   spl 0,              0
      add #2731,          1
      jmp @0,             imp-2732
imp   mov 0,              2731

   for 6
      dat #0,             #0
   rof

;paper for the stone satellite

pGo   spl pSt1,           pLen+(pSt1*2)
      mov <pGo+pLen+pSt1, <pGo
      mov <pGo+pSt2+4,    <1
      jmp @0,             pGo+pSt2+4+pSt1

;stone satellite

sGo   mov <sSt1,          sSt2
      spl -1,             <sSt4
      add sGo,            sGo
      jmp -2,             <sSt3

   for 14
      dat #0,             #0
   rof

qs    equ 322
qd    equ 161

qscan cmp 2*qs+qd       , 2*qs
qt1  jmp qa0           , <3*qs
     cmp qscan+5*qs+qd , qscan+5*qs
qt2  jmp qa1           , <4*qs
     cmp qscan+4*qs+qd , qscan+4*qs
qs1  djn qa1           , #qt1
     cmp qscan+10*qs-2 , qscan+10*qs+qd-2
qs2  djn qa2           , #qt2
     cmp qscan+9*qs+qd , qscan+9*qs
qt3  jmp qa2           , <6*qs
     cmp qscan+6*qs+qd , qscan+6*qs
     jmp qa2           , <qa1
     cmp qscan+8*qs+qd , qscan+8*qs
     jmp qa2           , <qs1
     cmp qscan+11*qs   , qscan+11*qs+qd
     jmp qa3           , <qa2
     cmp qscan+18*qs-8 , qscan+18*qs+qd-8
qs3  djn qa3           , #qt3
     cmp qscan+16*qs-2 , qscan+16*qs+qd-2
     jmp qa3           , <qs2
     cmp qscan+12*qs   , qscan+12*qs+qd
     jmp qa3           , <qa1
     cmp qscan+14*qs   , qscan+14*qs+qd
     jmp qa3           , <qs1
     jmz boot          , qscan+15*qs

qa3  add @qs3          , qp
qa2  add @qs2          , @qa3
qa1  add @qs1          , @qa3
qa0  cmp @qp           , <1234
     cmp @0            , 0
     add #qd           , qp
ql   mov qb            , @qp
qp   mov <2345         , <qscan+2*qs
     sub #9            , @ql
     djn ql            , #6
qb   jmp boot          , <43

     end qscan

