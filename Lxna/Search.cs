namespace Lxna; 

internal class Search {
    private static int IterationMove;
    public static Board Board = new(Engine.START_POS);
    private static Timer _timer = new();
    private static int _nodes;
    private static readonly int[] PieceWeights = {0, 100, 310, 330, 500, 1000, 10000 };
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
    private enum MoveFlag { Alpha, Exact, Beta }
    private record struct TranspositionTableEntry(ulong Key, int Score, int Depth, MoveFlag Flag, int Move);
    private static readonly TranspositionTableEntry[] TranspositionTable = new TranspositionTableEntry[0x7FFFFF];

    public static int Think(Board board, int depth = 100) {
        _timer = new Timer();
        Board = board;

        int currentDepth, bestMove = 0;
        int alpha = -100000;
        int beta = 100000;

        for (currentDepth = 1; currentDepth <= depth; currentDepth++) {
            _nodes = 0;
            
            int iterationScore = Negamax(alpha, beta, currentDepth, 0, true);

            if (UniversalChessInterface.TimeControl && _timer.GetDiff() > UniversalChessInterface.AllowedTime) break;
            
            bestMove = IterationMove;
            
            if (iterationScore > 50000) break;
        
            Move.Print(bestMove);
            Console.WriteLine(" depth {0,1} nodes {1,9:n0} time {2,4:n0}ms", currentDepth, _nodes, _timer.GetDiff());
        }

        return bestMove;
    }

    public static int Negamax(int alpha, int beta, int depth, int ply, bool nullCheck) {
        bool isRoot = ply == 0;
        
        if (depth == 0) {
            _nodes++;
            return Quiescence(alpha, beta, 2);
        }
        
        ulong positionKey = Board.GetZobrist();
        
        TranspositionTableEntry entry = TranspositionTable[positionKey % 0x7FFFFF];
        
        if (entry.Key == positionKey && !isRoot && entry.Depth >= depth && 
            (entry.Flag == MoveFlag.Exact ||
             (entry.Flag == MoveFlag.Alpha && entry.Score <= alpha) ||
             (entry.Flag == MoveFlag.Beta && entry.Score >= beta))) return entry.Score;

        bool isInCheck = Board.IsInCheck();

        if (!isInCheck && depth >= 3 && ply > 0 && nullCheck)
        {
            Board.Copy();
            Board.SideToMove = (SideToMove)((int)Board.SideToMove ^ 1);
            Board.EnPassant = Square.NoSquare;
            
            int value = -Negamax(-beta, -beta + 1, depth - 3, ply, false);
            
            Board.TakeBack();

            if (value >= beta)
                return beta;
        }

        List<int> moves = Board.GetPseudoLegalMoves();
        int bestMove = 0;
        int bestScore = -100000;
        int startAlpha = alpha;
        int[] moveScores = new int[moves.Count];

        for (int i = 0; i < moves.Count; i++) {
            bool isCapture = Move.GetMoveCapture(moves[i]) > 0;
            int promotion = Move.GetMovePromotion(moves[i]);
            bool isPromotion = promotion > 0;
            
            moveScores[i] = moves[i] == entry.Move ? 1000 :
                isCapture ? 100 : isPromotion ? promotion : 0;
        }

        for (int i = 0; i < moves.Count; i++) {
            if (UniversalChessInterface.TimeControl && _timer.GetDiff() > UniversalChessInterface.AllowedTime) break;
            
            for (int j = i + 1; j < moves.Count; j++) {
                if (moveScores[i] < moveScores[j])
                    (moves[i], moves[j], moveScores[i], moveScores[j]) =
                        (moves[j], moves[i], moveScores[j], moveScores[i]);
            }
            
            if (!Board.MakeMove(moves[i])) continue;
            int score = -Negamax(-beta, -alpha, depth - 1, ply + 1, true);
            Board.TakeBack();
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = moves[i];
                
                alpha = Math.Max(alpha, bestScore);
                
                if (isRoot) IterationMove = moves[i];
                if (alpha >= beta) break;
            }
        }

        if (moves.Count == 0) return isInCheck ? -100000 + ply : 0;
        
        MoveFlag flag = bestScore >= beta ? MoveFlag.Beta :
            bestScore > startAlpha ? MoveFlag.Exact : MoveFlag.Alpha;

        TranspositionTable[positionKey % 0x7FFFFF] =
            new TranspositionTableEntry(positionKey, bestScore, depth, flag, bestMove);

        return alpha;
    }

    public static int Quiescence(int alpha, int beta, int limit) {
        _nodes++;
        int standPat = Evaluate();
        if (limit == 0) return standPat;
        if (standPat >= beta) return beta;
        if (alpha < standPat) alpha = standPat;

        List<int> moves = Board.GetPseudoLegalCaptures();

        for (int i = 0; i < moves.Count; i++) {
            if (!Board.MakeMove(moves[i])) continue;
            int score = -Quiescence(-beta, -alpha, limit - 1);
            Board.TakeBack();

            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }

        return alpha;
    }

    public static int Evaluate() {
        int total = 0;

        for (int isWhite = 0; isWhite < 1; isWhite++) {
            for (int piece = 1; piece <= 6; piece++) {
                ulong pieceBitBoard = Board.Bitboards[piece + (isWhite == 0 ? 0 : 6) - 1];
                
                while (pieceBitBoard != 0)
                {
                    int bitIndex = BitboardHelper.GetLSFBIndex(pieceBitBoard);
                    
                    BitboardHelper.PopBitAtIndex(bitIndex, ref pieceBitBoard);

                    total += (PieceWeights[piece] + PieceSquareTables[piece + (isWhite == 0 ? 0 : 6) - 1, bitIndex]) * (isWhite == 0 ? 1 : -1);
                }
            }
        }

        return total * (Board.SideToMove == SideToMove.White ? 1 : -1);
    }
}