namespace Lxna; 

internal class Search {
    private static int _iterationMove;
    private static Board _board = new(Engine.StartPos);
    private static Timer _timer = new();
    private static int _nodes;
    private static readonly int[] PieceWeights = {0, 100, 310, 320, 500, 900, 20000 };
    // private static readonly int[] PieceWeights = {0, 100, 310, 330, 500, 1000, 10000 };
    private static readonly int[,] PieceSquareTables = {
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
            5, 5, 10, 25, 25, 10, 5, 5,
            0, 0, 0, 20, 20, 0, 0, 0,
            5, -5, -10, 0, 0, -10, -5, 5,
            5, 10, 10, -20, -20, 10, 10, 5,
            0, 0, 0, 0, 0, 0, 0, 0
        }, 
        {
            -50, -40, -30, -30, -30, -30, -40, -50,
            -40, -20, 0, 0, 0, 0, -20, -40,
            -30, 0, 10, 15, 15, 10, 0, -30,
            -30, 5, 15, 20, 20, 15, 5, -30,
            -30, 0, 15, 20, 20, 15, 0, -30,
            -30, 5, 10, 15, 15, 10, 5, -30,
            -40, -20, 0, 5, 5, 0, -20, -40,
            -50, -40, -30, -30, -30, -30, -40, -50,
        }, 
        {
            -20, -10, -10, -10, -10, -10, -10, -20,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -10, 0, 5, 10, 10, 5, 0, -10,
            -10, 5, 5, 10, 10, 5, 5, -10,
            -10, 0, 10, 10, 10, 10, 0, -10,
            -10, 10, 10, 10, 10, 10, 10, -10,
            -10, 5, 0, 0, 0, 0, 5, -10,
            -20, -10, -10, -10, -10, -10, -10, -20,
        }, 
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            5, 10, 10, 10, 10, 10, 10, 5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            0, 0, 0, 5, 5, 0, 0, 0
        },
        {
            -20, -10, -10, -5, -5, -10, -10, -20,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -10, 0, 5, 5, 5, 5, 0, -10,
            -5, 0, 5, 5, 5, 5, 0, -5,
            0, 0, 5, 5, 5, 5, 0, -5,
            -10, 5, 5, 5, 5, 5, 0, -10,
            -10, 0, 5, 0, 0, 0, 0, -10,
            -20, -10, -10, -5, -5, -10, -10, -20
        }, 
        {
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -20, -30, -30, -40, -40, -30, -30, -20,
            -10, -20, -20, -20, -20, -20, -20, -10,
            20, 20, 0, 0, 0, 0, 20, 20,
            20, 30, 10, 0, 0, 10, 30, 20
        }, 
        {
            0, 0, 0, 0, 0, 0, 0, 0, 5, 10, 10, -20, -20, 10, 10, 5, 5, -5, -10, 0, 0, -10, -5, 5, 0, 0, 0, 20, 20, 0, 0,
            0, 5, 5, 10, 25, 25, 10, 5, 5, 10, 10, 20, 30, 30, 20, 10, 10, 50, 50, 50, 50, 50, 50, 50, 50, 0, 0, 0, 0,
            0, 0, 0, 0
        }, 
        {
            -50,
            -40, -30, -30, -30, -30, -40, -50, -40, -20, 0, 5, 5, 0, -20, -40, -30, 5, 10, 15, 15, 10, 5, -30, -30, 0,
            15, 20, 20, 15, 0, -30, -30, 5, 15, 20, 20, 15, 5, -30, -30, 0, 10, 15, 15, 10, 0, -30, -40, -20, 0, 0, 0,
            0, -20, -40, -50, -40, -30, -30, -30, -30, -40, -50
        }, 
        {
            -20, -10, -10, -10, -10, -10, -10, -20, -10, 5, 0, 0, 0, 0, 5, -10, -10, 10, 10, 10, 10, 10, 10, -10, -10,
            0, 10, 10, 10, 10, 0, -10, -10, 5, 5, 10, 10, 5, 5, -10, -10, 0, 5, 10, 10, 5, 0, -10, -10, 0, 0, 0, 0, 0,
            0, -10, -20, -10, -10, -10, -10, -10, -10, -20
        }, 
        {
            0, 0, 0, 5, 5, 0, 0, 0, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, -5, 0,
            0, 0, 0, 0, 0, -5, -5, 0, 0, 0, 0, 0, 0, -5, 5, 10, 10, 10, 10, 10, 10, 5, 0, 0, 0, 0, 0, 0, 0, 0
        },
        {
            -20, -10, -10, -5, -5, -10, -10, -20, -10, 0, 0, 0, 0, 5, 0, -10, -10, 0, 5, 5, 5, 5, 5, -10, -5, 0, 5, 5,
            5, 5, 0, 0, -5, 0, 5, 5, 5, 5, 0, -5, -10, 0, 5, 5, 5, 5, 0, -10, -10, 0, 0, 0, 0, 0, 0, -10, -20, -10, -10,
            -5, -5, -10, -10, -20
        }, 
        {
            20, 30, 10, 0, 0, 10, 30, 20, 20, 20, 0, 0, 0, 0, 20, 20, -10, -20, -20, -20, -20, -20, -20, -10, -20, -30, -30, -40, -40, -30, -30, -20, -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30, -30, -40, -40, -50, -50, -40, -40, -30
        }
    };
    public enum MoveFlag { Alpha, Exact, Beta }
    public record struct TranspositionTableEntry(ulong Key, int Score, int Depth, MoveFlag Flag, int Move);
    public static readonly TranspositionTableEntry[] TranspositionTable = new TranspositionTableEntry[0x7FFFFF];
    public static bool TimeControl = true;
    private static bool _stopSearch;
    public static int WhiteTime = 60000;
    public static int BlackTime = 60000;
    public static int SideTime = 0;
    public static void Stop() {
        _stopSearch = true;
    }

    public static int Think(Board board, int depth = Int32.MaxValue) {
        _timer = new Timer();
        _stopSearch = false;
        _board = board;
        SideTime = _board.SideToMove == SideToMove.White ? WhiteTime : BlackTime;
        Array.Clear(TranspositionTable, 0, TranspositionTable.Length);

        int currentDepth, bestMove = 0;
        int alpha = -100000;
        int beta = 100000;

        for (currentDepth = 1; currentDepth <= depth; currentDepth++) {
            _nodes = 0;
            
            int iterationScore = Negamax(alpha, beta, currentDepth, 0, true);

            if ((TimeControl && _timer.GetDiff() > SideTime / 30) || _stopSearch) break;
            
            bestMove = _iterationMove;
            
            if (iterationScore > 50000) break;
        
            Move.Print(bestMove);
            Console.WriteLine(" depth {0,2} nodes {1,9:n0} time {2,4:n0}ms", currentDepth, _nodes, _timer.GetDiff());
        }

        return bestMove;
    }

    public static int Negamax(int alpha, int beta, int depth, int ply, bool nullCheck) {
        bool isRoot = ply == 0;
        
        if (depth == 0) {
            _nodes++;
            // return Quiescence(alpha, beta, 2);
            return Evaluate();
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
        // List<int> moves = _board.GetPseudoLegalMoves();
        
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
            
            moveScores[i] = moveSpan[i] == entry.Move ? 1000 :
                isCapture ? 100 : isPromotion ? promotion : 0;
        }

        for (int i = 0; i < moveSpan.Length; i++) {
            if ((TimeControl && _timer.GetDiff() > SideTime / 30) || _stopSearch) break;
            
            for (int j = i + 1; j < moveSpan.Length; j++) {
                if (moveScores[i] < moveScores[j])
                    (moveSpan[i], moveSpan[j], moveScores[i], moveScores[j]) =
                        (moveSpan[j], moveSpan[i], moveScores[j], moveScores[i]);
            }
            
            if (!_board.MakeMove(moveSpan[i])) continue;
            int score = -Negamax(-beta, -alpha, depth - 1, ply + 1, true);
            _board.TakeBack();
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = moveSpan[i];
                
                alpha = Math.Max(alpha, bestScore);
                
                if (isRoot) _iterationMove = moveSpan[i];
                if (alpha >= beta) break;
            }
        }

        if (moveSpan.Length == 0) return isInCheck ? -100000 + ply : 0;
        
        MoveFlag flag = bestScore >= beta ? MoveFlag.Beta :
            bestScore > startAlpha ? MoveFlag.Exact : MoveFlag.Alpha;

        TranspositionTable[positionKey % 0x7FFFFF] =
            new TranspositionTableEntry(positionKey, bestScore, depth, flag, bestMove);

        return alpha;
    }

    // public static int Quiescence(int alpha, int beta, int limit) {
    //     _nodes++;
    //     int standPat = Evaluate();
    //     if (limit == 0) return standPat;
    //     if (standPat >= beta) return beta;
    //     if (alpha < standPat) alpha = standPat;
    //
    //     List<int> moves = Board.GetPseudoLegalCaptures();
    //
    //     for (int i = 0; i < moves.Count; i++) {
    //         if (!Board.MakeMove(moves[i])) continue;
    //         int score = -Quiescence(-beta, -alpha, limit - 1);
    //         Board.TakeBack();
    //
    //         if (score >= beta) return beta;
    //         if (score > alpha) alpha = score;
    //     }
    //
    //     return alpha;
    // }

    public static int Evaluate() {
        int total = 0;

        for (int isWhite = 0; isWhite < 1; isWhite++) {
            for (int piece = 1; piece <= 6; piece++) {
                ulong pieceBitBoard = _board.Bitboards[piece + (isWhite == 0 ? 0 : 6) - 1];
                
                while (pieceBitBoard != 0)
                {
                    int bitIndex = BitboardHelper.GetLSFBIndex(pieceBitBoard);
                    
                    BitboardHelper.PopBitAtIndex(bitIndex, ref pieceBitBoard);

                    total += (PieceWeights[piece] + PieceSquareTables[piece + (isWhite == 0 ? 0 : 6) - 1, bitIndex]) * (isWhite == 0 ? 1 : -1);
                }
            }
        }

        return total * (_board.SideToMove == SideToMove.White ? 1 : -1);
    }
}