namespace Lxna; 

public class Search {
    public static int IterationMove;
    public static int IterationScore;
    private static Board _board = new(Engine.StartPos);
    private static Timer _timer = new();
    private static int _nodes; 
    // private static readonly int[] PieceWeights = {100, 310, 330, 500, 1000, 10000 };
    private static readonly int[,] PieceWeightsMiddleGame = {
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            98, 134, 61, 95, 68, 126, 34, -11,
            -6, 7, 26, 31, 65, 56, 25, -20,
            -14, 13, 6, 21, 23, 12, 17, -23,
            -27, -2, -5, 12, 17, 6, 10, -25,
            -26, -4, -4, -10, 3, 3, 33, -12,
            -35, -1, -20, -23, -15, 24, 38, -22,
            0, 0, 0, 0, 0, 0, 0, 0,
        }, {
            -167, -89, -34, -49, 61, -97, -15, -107,
            -73, -41, 72, 36, 23, 62, 7, -17,
            -47, 60, 37, 65, 84, 129, 73, 44,
            -9, 17, 19, 53, 37, 69, 18, 22,
            -13, 4, 16, 13, 28, 19, 21, -8,
            -23, -9, 12, 10, 19, 17, 25, -16,
            -29, -53, -12, -3, -1, 18, -14, -19,
            -105, -21, -58, -33, -17, -28, -19, -23,
        }, {
            -29, 4, -82, -37, -25, -42, 7, -8,
            -26, 16, -18, -13, 30, 59, 18, -47,
            -16, 37, 43, 40, 35, 50, 37, -2,
            -4, 5, 19, 50, 37, 37, 7, -2,
            -6, 13, 13, 26, 34, 12, 10, 4,
            0, 15, 15, 15, 14, 27, 18, 10,
            4, 15, 16, 0, 7, 21, 33, 1,
            -33, -3, -14, -21, -13, -12, -39, -21,
        }, {
            32, 42, 32, 51, 63, 9, 31, 43,
            27, 32, 58, 62, 80, 67, 26, 44,
            -5, 19, 26, 36, 17, 45, 61, 16,
            -24, -11, 7, 26, 24, 35, -8, -20,
            -36, -26, -12, -1, 9, -7, 6, -23,
            -45, -25, -16, -17, 3, 0, -5, -33,
            -44, -16, -20, -9, -1, 11, -6, -71,
            -19, -13, 1, 17, 16, 7, -37, -26,
        }, {
            -28, 0, 29, 12, 59, 44, 43, 45,
            -24, -39, -5, 1, -16, 57, 28, 54,
            -13, -17, 7, 8, 29, 56, 47, 57,
            -27, -27, -16, -16, -1, 17, -2, 1,
            -9, -26, -9, -10, -2, -4, 3, -3,
            -14, 2, -11, -2, -5, 2, 14, 5,
            -35, -8, 11, 2, 8, 15, -3, 1,
            -1, -18, -9, 10, -15, -25, -31, -50,
        }, {
            -65, 23, 16, -15, -56, -34, 2, 13,
            29, -1, -20, -7, -8, -4, -38, -29,
            -9, 24, 2, -16, -20, 6, 22, -22,
            -17, -20, -12, -27, -30, -25, -14, -36,
            -49, -1, -27, -39, -46, -44, -33, -51,
            -14, -14, -22, -46, -44, -30, -15, -27,
            1, 7, -8, -64, -43, -16, 9, 8,
            -15, 36, 12, -54, 8, -28, 24, 14,
        }

    };
    private static readonly int[,] PieceWeightsEndGame = {
        {
            
            0,   0,   0,   0,   0,   0,   0,   0,
            178, 173, 158, 134, 147, 132, 165, 187,
            94, 100,  85,  67,  56,  53,  82,  84,
            32,  24,  13,   5,  -2,   4,  17,  17,
            13,   9,  -3,  -7,  -7,  -8,   3,  -1,
            4,   7,  -6,   1,   0,  -5,  -1,  -8,
            13,   8,   8,  10,  13,   0,   2,  -7,
            0,   0,   0,   0,   0,   0,   0,   0,
        },
        {
            
            -58, -38, -13, -28, -31, -27, -63, -99,
            -25,  -8, -25,  -2,  -9, -25, -24, -52,
            -24, -20,  10,   9,  -1,  -9, -19, -41,
            -17,   3,  22,  22,  22,  11,   8, -18,
            -18,  -6,  16,  25,  16,  17,   4, -18,
            -23,  -3,  -1,  15,  10,  -3, -20, -22,
            -42, -20, -10,  -5,  -2, -20, -23, -44,
            -29, -51, -23, -15, -22, -18, -50, -64,
        },
        {
            
            -14, -21, -11,  -8, -7,  -9, -17, -24,
            -8,  -4,   7, -12, -3, -13,  -4, -14,
            2,  -8,   0,  -1, -2,   6,   0,   4,
            -3,   9,  12,   9, 14,  10,   3,   2,
            -6,   3,  13,  19,  7,  10,  -3,  -9,
            -12,  -3,   8,  10, 13,   3,  -7, -15,
            -14, -18,  -7,  -1,  4,  -9, -15, -27,
            -23,  -9, -23,  -5, -9, -16,  -5, -17,
        },
        {
            
            13, 10, 18, 15, 12,  12,   8,   5,
            11, 13, 13, 11, -3,   3,   8,   3,
            7,  7,  7,  5,  4,  -3,  -5,  -3,
            4,  3, 13,  1,  2,   1,  -1,   2,
            3,  5,  8,  4, -5,  -6,  -8, -11,
            -4,  0, -5, -1, -7, -12,  -8, -16,
            -6, -6,  0,  2, -9,  -9, -11,  -3,
            -9,  2,  3, -1, -5, -13,   4, -20,
        },
        {
            
            -9,  22,  22,  27,  27,  19,  10,  20,
            -17,  20,  32,  41,  58,  25,  30,   0,
            -20,   6,   9,  49,  47,  35,  19,   9,
            3,  22,  24,  45,  57,  40,  57,  36,
            -18,  28,  19,  47,  31,  34,  39,  23,
            -16, -27,  15,   6,   9,  17,  10,   5,
            -22, -23, -30, -16, -16, -23, -36, -32,
            -33, -28, -22, -43,  -5, -32, -20, -41,
        },
        {
            
            -74, -35, -18, -18, -11,  15,   4, -17,
            -12,  17,  14,  17,  17,  38,  23,  11,
            10,  17,  23,  15,  20,  45,  44,  13,
            -8,  22,  24,  27,  26,  33,  26,   3,
            -18,  -4,  21,  24,  27,  23,   9, -11,
            -19,  -3,  11,  21,  23,  16,   7,  -9,
            -27, -11,   4,  13,  14,   4,  -5, -17,
            -53, -34, -21, -11, -28, -14, -24, -43
        }
    };
    private readonly static int[] _pieceWeights = {0, 100, 310, 330, 500, 1000, 10000 };
    private readonly static int[] _gamePhase = {0, 0, 1, 1, 2, 4, 0}; 
    public enum MoveFlag { Alpha, Exact, Beta }
    public record struct TranspositionTableEntry(ulong Key, int Score, int Depth, MoveFlag Flag, int Move);
    public static readonly TranspositionTableEntry[] TranspositionTable = new TranspositionTableEntry[0x7FFFFF];
    private static bool _timeControl = true;
    private static bool _stopSearch;
    private static int _time;
    
    public static void Stop() {
        _stopSearch = true;
    }

    public static int Think(Board board, bool timeControl, int time = 1000, int depth = 100, bool shouldPrint = true) {
        _timer = new Timer();
        _stopSearch = false;
        _board = board;
        _time = time;
        _timeControl = timeControl;
        
        Console.WriteLine("EVAAAAAAl {0}", EvaluatePeStO());

        int currentDepth, bestMove = _board.GetPseudoLegalMoves()[0];
        
        for (currentDepth = 1; currentDepth <= depth; currentDepth++) {
            _nodes = 0;
            
            int iterationScore = Negamax(-100000, 100000, currentDepth, 0, true);

            if ((_timeControl && _timer.GetDiff() > _time / 30) || _stopSearch) break;
            
            bestMove = IterationMove;
            
            if (iterationScore > 50000) break;
        
            if (shouldPrint) {
                Move.Print(bestMove);
                Console.WriteLine(" depth {0,2} score {1,4}, nodes {2,9:n0} time {3,4:n0}ms", currentDepth, IterationScore, _nodes, _timer.GetDiff());
            }
        }

        if (shouldPrint) Console.WriteLine("bestmove {0}{1}{2}",
            ((Square)Move.GetMoveSource(bestMove)).ToString().ToLower(), 
            ((Square)Move.GetMoveTarget(bestMove)).ToString().ToLower(), 
            Move.GetMovePromotion(bestMove) > 0 ? Board.PiecesChar[(Piece)Move.GetMovePromotion(bestMove)].ToLower() : "");
        
        Array.Clear(TranspositionTable, 0, TranspositionTable.Length);
        
        return bestMove;
    }

    public static int Negamax(int alpha, int beta, int depth, int ply, bool nullCheck) {
        bool isRoot = ply == 0;
        
        if (depth == 0) {
            _nodes++;
            return Quiescence(alpha, beta, 2);
        }
        
        ulong positionKey = _board.GetZobrist();
        
        TranspositionTableEntry entry = TranspositionTable[positionKey % 0x7FFFFF];
        
        if (entry.Key == positionKey && !isRoot && entry.Depth >= depth && 
            (entry.Flag == MoveFlag.Exact ||
             (entry.Flag == MoveFlag.Alpha && entry.Score <= alpha) ||
             (entry.Flag == MoveFlag.Beta && entry.Score >= beta))) return entry.Score;

        bool isInCheck = _board.IsInCheck();

        if (!isInCheck && depth >= 3 && ply > 0 && nullCheck)
        {
            _board.MakeNullMove();
            int value = -Negamax(-beta, -beta + 1, depth - 3, ply, false);
            _board.TakeBack();
        
            if (value >= beta)
                return beta;
        }
        
        Span<int> moveSpan = stackalloc int[218];
        _board.GetPseudoLegalMovesNonAlloc(ref moveSpan);
        int bestMove = 0;
        int bestScore = -100000;
        int startAlpha = alpha;
        int[] moveScores = new int[moveSpan.Length];

        for (int i = 0; i < moveSpan.Length; i++) {
            bool isCapture = Move.GetMoveCapture(moveSpan[i]) > 0;
            int promotion = Move.GetMovePromotion(moveSpan[i]);
            bool isPromotion = promotion > 0;
            
            moveScores[i] = moveSpan[i] == entry.Move ? 100000 :
                isCapture ? 100 : isPromotion ? promotion : 0;
            
            // moveScores[i] = isCapture ? 100 : isPromotion ? promotion : 0;
        }

        for (int i = 0; i < moveSpan.Length; i++) {
            if ((_timeControl && _timer.GetDiff() > _time / 30) || _stopSearch) break;
            
            for (int j = i + 1; j < moveSpan.Length; j++) {
                if (moveScores[i] < moveScores[j])
                    (moveSpan[i], moveSpan[j], moveScores[i], moveScores[j]) =
                        (moveSpan[j], moveSpan[i], moveScores[j], moveScores[i]);
            }

            int move = moveSpan[i];
            
            if (!_board.MakeMove(move)) continue;
            int score = -Negamax(-beta, -alpha, depth - 1, ply + 1, true);
            _board.TakeBack();
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
                
                alpha = Math.Max(alpha, bestScore);

                if (isRoot) {
                    IterationScore = alpha;
                    IterationMove = move;
                }
                if (alpha >= beta) break;
            }
        }

        if (moveSpan.Length == 0) return isInCheck ? -100000 + ply : 0;
        
        MoveFlag flag = bestScore >= beta ? MoveFlag.Beta :
            bestScore > startAlpha ? MoveFlag.Exact : MoveFlag.Alpha;
        
        TranspositionTable[positionKey % 0x7FFFFF] =
            new TranspositionTableEntry(positionKey, bestScore, depth, flag, bestMove);

        return bestScore;
    }

    public static int Quiescence(int alpha, int beta, int limit) {
        _nodes++;
        int standPat = EvaluatePeStO();
        if (limit == 0) return standPat;
        if (standPat >= beta) return beta;
        if (alpha < standPat) alpha = standPat;
    
        List<int> moves = _board.GetPseudoLegalCaptures();
    
        for (int i = 0; i < moves.Count; i++) {
            if (!_board.MakeMove(moves[i])) continue;
            int score = -Quiescence(-beta, -alpha, limit - 1);
            _board.TakeBack();
    
            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }
    
        return alpha;
    }
    
    private static int EvaluatePeStO()
    {
        int phase = 0, mg = 0, eg = 0;
        
        foreach (bool isWhite in new[] {true, false}) {
            for (var pieceType = 0; pieceType <= 5; pieceType++) {
                ulong pieceBitBoard = _board.Bitboards[pieceType + (isWhite ? 0 : 6)];
    
                while (pieceBitBoard != 0)
                {
                    phase += _gamePhase[pieceType];
    
                    int lsbIndex = BitboardHelper.GetLSFBIndex(pieceBitBoard);
                    BitboardHelper.PopBitAtIndex(lsbIndex, ref pieceBitBoard);

                    int index = lsbIndex ^ (isWhite ? 56 : 0);
                    
                    mg += PieceWeightsMiddleGame[pieceType, index] + _pieceWeights[pieceType];
                    eg += PieceWeightsEndGame[pieceType, index] + _pieceWeights[pieceType];
                }
            }
    
            mg = -mg;
            eg = -eg;
        }
    
        return (mg * phase + eg * (24 - phase)) / 24 * (_board.SideToMove == SideToMove.White ? 1 : -1);
    }
}