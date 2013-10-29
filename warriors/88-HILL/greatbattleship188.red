;redcode
;name Great Battleship V1 for '88
;author Skybuck Flying
;strategy Fires bombs like a great naval battleship ! =D
;inspiration Empire Total War Great Naval Sea Battles ! =D
;date 1 march 2009
;version 1
;mostly fun to watch ! =D
;slightly modified to make it more usefull...
;linesize offset changed so it bombs more...
;not bad for a first version ! :)
;it actually does work a bit this bombing pattern funnnnny ! =D
;modified to play with '88 rules only :)
;assert 1

LineSize equ 128+2

spl TopCannon1Begin
spl BottomCannon1Begin

spl TopCannon2Begin
spl BottomCannon2Begin

spl TopCannon3Begin
spl BottomCannon3Begin

spl TopCannon4Begin
spl BottomCannon4Begin

spl TopCannon5Begin
spl BottomCannon5Begin

spl TopCannon6Begin
spl BottomCannon6Begin
dat #0, #0

TopCannon1Begin
spl $0, $0
TopCannon1Target mov $TopCannon1Bomb, $-LineSize
sub #LineSize, TopCannon1Target
TopCannon1Bomb
dat #0, #0
TopCannon1End

BottomCannon1Begin
spl $0, $0
BottomCannon1Target mov $BottomCannon1Bomb, $LineSize
add #LineSize, BottomCannon1Target
BottomCannon1Bomb
dat #0, #0
BottomCannon1End

TopCannon2Begin
spl $0, $0
TopCannon2Target mov $TopCannon2Bomb, $-LineSize
sub #LineSize, TopCannon2Target
TopCannon2Bomb
dat #0, #0
TopCannon2End

BottomCannon2Begin
spl $0, $0
BottomCannon2Target mov $BottomCannon2Bomb, $LineSize
add #LineSize, BottomCannon2Target
BottomCannon2Bomb
dat #0, #0
BottomCannon2End

TopCannon3Begin
spl $0, $0
TopCannon3Target mov $TopCannon3Bomb, $-LineSize
sub #LineSize, TopCannon3Target
TopCannon3Bomb
dat #0, #0
TopCannon3End

BottomCannon3Begin
spl $0, $0
BottomCannon3Target mov $BottomCannon3Bomb, $LineSize
add #LineSize, BottomCannon3Target
BottomCannon3Bomb
dat #0, #0
BottomCannon3End

TopCannon4Begin
spl $0, $0
TopCannon4Target mov $TopCannon4Bomb, $-LineSize
sub #LineSize, TopCannon4Target
TopCannon4Bomb
dat #0, #0
TopCannon4End

BottomCannon4Begin
spl $0, $0
BottomCannon4Target mov $BottomCannon4Bomb, $LineSize
add #LineSize, BottomCannon4Target
BottomCannon4Bomb
dat #0, #0
BottomCannon4End

TopCannon5Begin
spl $0, $0
TopCannon5Target mov $TopCannon5Bomb, $-LineSize
sub #LineSize, TopCannon5Target
TopCannon5Bomb
dat #0, #0
TopCannon5End

BottomCannon5Begin
spl $0, $0
BottomCannon5Target mov $BottomCannon5Bomb, $LineSize
add #LineSize, BottomCannon5Target
BottomCannon5Bomb
dat #0, #0
BottomCannon5End

TopCannon6Begin
spl $0, $0
TopCannon6Target mov $TopCannon6Bomb, $-LineSize
sub #LineSize, TopCannon6Target
TopCannon6Bomb
dat #0, #0
TopCannon6End

BottomCannon6Begin
spl $0, $0
BottomCannon6Target mov $BottomCannon6Bomb, $LineSize
add #LineSize, BottomCannon6Target
BottomCannon6Bomb
dat #0, #0
;BottomCannon6End

