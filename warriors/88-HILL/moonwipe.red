;name Moonwipe
;author Christian Schmidt
;strategy  bomber based on Mintardjo's Winter Werewolf
;assert CORESIZE==8000

sStp    EQU 2206
sAwa    EQU 4410
sPtr    EQU 18

sBoot EQU (pGo+6599)

pGo   mov sBmb+6  , sBoot+6
      mov sBmb+5  , <pGo
      mov sBmb+4  , <pGo
      mov sBmb+3  , <pGo
      mov sBmb+2  , <pGo
      mov sBmb+1  , <pGo
      mov sBmb+0  , <pGo
      mov dBmb    , sBoot-2-sPtr
      jmp sBoot+1 , <sBoot-1997

sBmb  spl 0       , <-3-sStp-sPtr
sGo   mov sBck    , @3
      mov sBmb    , <2
      add #sStp   , @-1
      jmp sGo     , sAwa
      mov @sGo    , <sPtr
sBck  jmp -1      , 1
dBmb  dat <-4-sPtr, #0


   for 41
      dat   #0    ,     #0
   rof

qs    equ 322
qd    equ 161

qscan cmp 2*qs+qd       , 2*qs
qt1   jmp qa0           , <3*qs
      cmp qscan+5*qs+qd , qscan+5*qs
qt2   jmp qa1           , <4*qs
      cmp qscan+4*qs+qd , qscan+4*qs
qs1   djn qa1           , #qt1
      cmp qscan+10*qs-2 , qscan+10*qs+qd-2
qs2   djn qa2           , #qt2
      cmp qscan+9*qs+qd , qscan+9*qs
qt3   jmp qa2           , <6*qs
      cmp qscan+6*qs+qd , qscan+6*qs
      jmp qa2           , <qa1
      cmp qscan+8*qs+qd , qscan+8*qs
      jmp qa2           , <qs1
      cmp qscan+11*qs   , qscan+11*qs+qd
      jmp qa3           , <qa2
      cmp qscan+18*qs-8 , qscan+18*qs+qd-8
qs3   djn qa3           , #qt3
      cmp qscan+16*qs-2 , qscan+16*qs+qd-2
      jmp qa3           , <qs2
      cmp qscan+12*qs   , qscan+12*qs+qd
      jmp qa3           , <qa1
      cmp qscan+14*qs   , qscan+14*qs+qd
      jmp qa3           , <qs1
      jmz pGo           , qscan+15*qs

qa3   add @qs3          , qp
qa2   add @qs2          , @qa3
qa1   add @qs1          , @qa3
qa0   cmp @qp           , <1234
      cmp @0            , 0
      add #qd           , qp
ql    mov qb            , @qp
qp    mov <2345         , <qscan+2*qs
      sub #9            , @ql
      djn ql            , #6
qb    jmp pGo           , <43

      end qscan

