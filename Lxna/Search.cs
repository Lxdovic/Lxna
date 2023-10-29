namespace Lxna; 

public class Search {
    public static int IterationMove;
    public static int IterationScore;
    private static Board _board = new(Engine.StartPos);
    private static Timer _timer = new();
    private static int _nodes; 
    private static readonly int[] PieceWeights = {100, 310, 330, 500, 1000, 10000 };
    public enum MoveFlag { Alpha, Exact, Beta }
    public record struct TranspositionTableEntry(ulong Key, int Score, int Depth, MoveFlag Flag, int Move);
    public static readonly TranspositionTableEntry[] TranspositionTable = new TranspositionTableEntry[0x7FFFFF];
    private static bool _timeControl = true;
    private static bool _stopSearch;
    private static int _time;
    
    public static void Stop() {
        _stopSearch = true;
    }

    public static int Think(Board board, bool timeControl, int time = 1000, int depth = 100) {
        _timer = new Timer();
        _stopSearch = false;
        _board = board;
        _time = time;
        _timeControl = timeControl;
        
        Array.Clear(TranspositionTable, 0, TranspositionTable.Length);
        
        int currentDepth, bestMove = 0;

        for (currentDepth = 1; currentDepth <= depth; currentDepth++) {
            _nodes = 0;
            
            int iterationScore = Negamax(-100000, 100000, currentDepth, 0, true);

            if ((_timeControl && _timer.GetDiff() > _time / 30) || _stopSearch) break;
            
            bestMove = IterationMove;
            
            if (iterationScore > 50000) break;
        
            Move.Print(bestMove);
            Console.WriteLine(" depth {0,2} score {1,4}, nodes {2,9:n0} time {3,4:n0}ms", currentDepth, IterationScore, _nodes, _timer.GetDiff());
        }

        Console.WriteLine("bestmove {0}{1}{2}", 
            ((Square)Move.GetMoveSource(bestMove)).ToString().ToLower(), 
            ((Square)Move.GetMoveTarget(bestMove)).ToString().ToLower(), 
            Move.GetMovePromotion(bestMove) > 0 ? Board.PiecesChar[(Piece)Move.GetMovePromotion(bestMove)].ToLower() : "");

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

        // if (moveSpan.Length == 0) return isInCheck ? -100000 + ply : 0;
        
        MoveFlag flag = bestScore >= beta ? MoveFlag.Beta :
            bestScore > startAlpha ? MoveFlag.Exact : MoveFlag.Alpha;
        
        TranspositionTable[positionKey % 0x7FFFFF] =
            new TranspositionTableEntry(positionKey, bestScore, depth, flag, bestMove);

        return bestScore;
    }

    // public static int Quiescence(int alpha, int beta, int limit) {
    //     _nodes++;
    //     int standPat = EvaluatePeStO();
    //     if (limit == 0) return standPat;
    //     if (standPat >= beta) return beta;
    //     if (alpha < standPat) alpha = standPat;
    //
    //     List<int> moves = _board.GetPseudoLegalCaptures();
    //
    //     for (int i = 0; i < moves.Count; i++) {
    //         if (!_board.MakeMove(moves[i])) continue;
    //         int score = -Quiescence(-beta, -alpha, limit - 1);
    //         _board.TakeBack();
    //
    //         if (score >= beta) return beta;
    //         if (score > alpha) alpha = score;
    //     }
    //
    //     return alpha;
    // }
    
    // private static int GetPstValue(int psq) => (int)(((_pestoTables[psq / 10] >> (6 * (psq % 10))) & 63) - 20) * 8;
    //
    //
    // private static int EvaluatePeStO()
    // {
    //     // if (_board.IsInCheckmate())
    //     //     return _board.IsWhiteToMove ? 10000 - (_board.PlyCount) : -10000 + _board.PlyCount;
    //     
    //     int phase = 0, mg = 0, eg = 0;
    //     
    //     foreach (bool isWhite in new[] {true, false}) {
    //         for (var pieceType = 0; pieceType <= 5; pieceType++) {
    //             ulong pieceBitBoard = _board.Bitboards[pieceType + (isWhite ? 0 : 6)];
    //
    //             while (pieceBitBoard != 0)
    //             {
    //                 phase += _gamePhase[pieceType];
    //
    //                 int lsbIndex = BitboardHelper.GetLSFBIndex(pieceBitBoard);
    //                 BitboardHelper.PopBitAtIndex(lsbIndex, ref pieceBitBoard);
    //                 
    //                 int index = 128 * pieceType + lsbIndex ^ (isWhite ? 56 : 0);
    //                 mg += GetPstValue(index) + _pieceWeights[pieceType];
    //                 eg += GetPstValue(index + 64) + _pieceWeights[pieceType];
    //             }
    //         }
    //
    //         mg = -mg;
    //         eg = -eg;
    //     }
    //
    //     return (mg * phase + eg * (24 - phase)) / 24 * (_board.SideToMove == SideToMove.White ? 1 : -1);
    // }

    public static int Evaluate() {
        int total = 0;
    
        for (int isWhite = 0; isWhite <= 1; isWhite++) {
            for (int piece = 0; piece <= 5; piece++) {
                ulong pieceBitBoard = _board.Bitboards[piece + (isWhite == 0 ? 0 : 6)];
                
                while (pieceBitBoard != 0)
                {
                    int bitIndex = BitboardHelper.GetLSFBIndex(pieceBitBoard);
                    
                    BitboardHelper.PopBitAtIndex(bitIndex, ref pieceBitBoard);
    
                    total += PieceWeights[piece] * (isWhite == 0 ? 1 : -1);
                }
            }
        }
    
        return total * (_board.SideToMove == SideToMove.White ? 1 : -1);
    }
}