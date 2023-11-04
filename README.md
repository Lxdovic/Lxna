# Lxna Engine

Bitboard chess engine written from scratch in C#

## Authors

- [@Lxdovic](https://www.github.com/Lxdovic)

## Features (V0.2.0)

- [PeStO's Evaluation Function](https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function)
- [Negamax](https://www.chessprogramming.org/Negamax)
- [Alpha-Beta Pruning](https://www.chessprogramming.org/Alpha-Beta)
- [Iterative Deepening](https://www.chessprogramming.org/Iterative_Deepening)
- [Transposition Tables](https://www.chessprogramming.org/Transposition_Table)
- [Move Ordering](https://www.chessprogramming.org/Move_Ordering)
- [Null Move Forward Pruning](https://web.archive.org/web/20071031095933/http://www.brucemo.com/compchess/programming/nullmove.htm)
- [Quiescence Search](https://www.chessprogramming.org/Quiescence_Search)
- [Principal Variation Search](https://www.chessprogramming.org/Principal_Variation_Search)
- [Killer Heuristic](https://www.chessprogramming.org/Killer_Heuristic)
- [History Heuristic](https://www.chessprogramming.org/History_Heuristic)

## Documentation (UCI Protocol)

### Show commands

`help`

### Display engine informations

`uci`

<p>output</p>

```
id name Lxna
id author Lxdovic
uciok
```

### Check if the engine is ready

`isready`

<p>output</p>

```
readyok
```

### Start a new game and setup start position

`ucinewgame`

<p>output</p>

```
 8  ♜  ♞  ♝  ♛  ♚  ♝  ♞  ♜
 7  ♟  ♟  ♟  ♟  ♟  ♟  ♟  ♟
 6
 5
 4
 3
 2  ♟  ♟  ♟  ♟  ♟  ♟  ♟  ♟
 1  ♜  ♞  ♝  ♛  ♚  ♝  ♞  ♜
    a  b  c  d  e  f  g  h

SideToMove:           White
EnPassant:         NoSquare
Castling:              KQkq
```

### Set a position

`position [fen <fenstring> | startpos] moves <move1> ... <movei>`

example input

```
position fen r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - moves e2a6 b4c3
```

example output

```
 8  ♜           ♚        ♜
 7  ♟     ♟  ♟  ♛  ♟  ♝
 6  ♝  ♞        ♟  ♞  ♟
 5           ♟  ♞
 4              ♟
 3        ♟        ♛     ♟
 2  ♟  ♟  ♟  ♝     ♟  ♟  ♟
 1  ♜           ♚        ♜
    a  b  c  d  e  f  g  h

SideToMove:           White
EnPassant:         NoSquare
Castling:              KQkq
```

### Start searching

example input

```
go
```

- Search x plies
  ```
  depth <x>
  ```
- Search with x time (ms) left on the clock for white
  ```
  wtime <x>
  ```
- Search with x time (ms) left on the clock for black
  ```
  btime <x>
  ```
- Search until the stop command is called
  ```
  infinite
  ```
- Executes a performance test x plies
  ```
  perft <x>
  ```
  example input

```
go depth 8
```

example output

```
move F3F6 depth  1 score  330, nodes        52 time   77ms
move B2C3 depth  2 score   20, nodes       120 time   77ms
move D2G5 depth  3 score  230, nodes     3,269 time   78ms
move B2C3 depth  4 score  -80, nodes     5,223 time   82ms
move B2C3 depth  5 score  120, nodes    77,864 time  114ms
move D2C3 depth  6 score -110, nodes    81,983 time  170ms
move D2C3 depth  7 score  190, nodes   751,752 time  342ms
move D2C3 depth  8 score -210, nodes   469,464 time  613ms
bestmove d2c3
```

example input

```
go perft 5
```

example output

```
move A2A3  nodes 181,046  KN/s 7,872 time:    23
move A2A4  nodes 217,832  KN/s 9,729 time:    41
move B2B3  nodes 215,255  KN/s 10,588 time:    58
move B2B4  nodes 216,145  KN/s 11,220 time:    74
move C2C3  nodes 222,861  KN/s 11,447 time:    92
move C2C4  nodes 240,082  KN/s 11,651 time:   111
move D2D3  nodes 328,511  KN/s 11,924 time:   136
move D2D4  nodes 361,790  KN/s 12,169 time:   163
move E2E3  nodes 402,988  KN/s 12,365 time:   193
move E2E4  nodes 405,385  KN/s 12,464 time:   224
move F2F3  nodes 178,889  KN/s 12,482 time:   239
move F2F4  nodes 198,473  KN/s 12,477 time:   254
move G2G3  nodes 217,210  KN/s 12,542 time:   270
move G2G4  nodes 214,048  KN/s 12,545 time:   287
move H2H3  nodes 181,044  KN/s 12,522 time:   302
move H2H4  nodes 218,829  KN/s 12,540 time:   319
move B1A3  nodes 198,572  KN/s 12,572 time:   334
move B1C3  nodes 234,656  KN/s 12,596 time:   352
move G1F3  nodes 233,491  KN/s 12,614 time:   370
move G1H3  nodes 198,502  KN/s 12,605 time:   386

perft depth 5 nodes 4,865,609 time  386
```

### Stop searching

`stop`

### Exit program

`quit`
