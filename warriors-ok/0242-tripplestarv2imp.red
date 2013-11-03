;redcode
;name TrippleStarV2imp
;author Skybuck Flying
;strategy Tripple Deathstar with imp
;date 15 june 2008
;assert 1
	SPL    $23    ,   $0       
       	MOV    #14    ,   $8
       	MOV    #13    ,   $2
	MOV    <6     ,   <5
       DJN    $-1    ,   #13
      SPL    @3     ,   $0
	ADD    #4000  ,   $2
       JMP    $-6    ,   $0
       DAT    #0     , #4000
    DAT    #0     ,   #14
      SPL    $0     ,   $0
       SPL    $1,   $0
       SPL    $1     ,   $0
       MOV    $7     ,   <9
       MOV    $6,   <8
       MOV    $5     ,   <7
       MOV    $5     ,   <6
       MOV    $4,   <5
       MOV    $3     ,   <4
       JMP    $-6    ,   $0
       SPL    $1,   #-12
     DAT    #0     ,   #-12
     DAT    #0     ,   #-12
     MOV    $0,   $1
