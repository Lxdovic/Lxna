namespace Lxna; 

public class Search {
    public static int IterationMove;
    public static int IterationScore;
    private static Board _board = new(Engine.StartPos);
    private static Timer _timer = new();
    private static int _nodes; 
    private static readonly int[] PieceWeights = {100, 310, 330, 500, 1000, 10000 };
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
    private static readonly ulong TranspositionTableSize = 0x7FFFFF * 2;
    private static readonly int[] GetRank = {
        7, 7, 7, 7, 7, 7, 7, 7,
        6, 6, 6, 6, 6, 6, 6, 6,
        5, 5, 5, 5, 5, 5, 5, 5,
        4, 4, 4, 4, 4, 4, 4, 4,
        3, 3, 3, 3, 3, 3, 3, 3,
        2, 2, 2, 2, 2, 2, 2, 2,
        1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0
    };
    private static readonly int DoubledPawnPenalty = -10;
    private static readonly int IsolatedPawnPenalty = -10;
    private static readonly int[] PassedPawnBonus = { 0, 5, 10, 20, 35, 60, 100, 200 };
    private static readonly int SemiOpenFileScore = 10;
    private static readonly int OpenFileScore = 20;
    private static ulong[] FileMasks = new ulong[64];
    private static ulong[] RankMasks = new ulong[64];
    private static ulong[] IsolatedPawnMasks = new ulong[64];
    private static ulong[] WhitePassedMasks = new ulong[64];
    private static ulong[] BlackPassedMasks = new ulong[64];
    public enum MoveFlag { Alpha, Exact, Beta }
    public record struct TranspositionTableEntry(ulong Key, int Score, int Depth, MoveFlag Flag, int Move);
    public static readonly TranspositionTableEntry[] TranspositionTable = new TranspositionTableEntry[TranspositionTableSize];
    private static bool _timeControl = true;
    private static bool _stopSearch;
    private static int _time;

    public static ulong SetFileRankMasks(int file, int rank) {
        ulong mask = 0x0UL;

        for (int currentRank = 0; currentRank < 8; currentRank++) {
            for (int currentFile = 0; currentFile < 8; currentFile++) {
                int square = currentRank * 8 + currentFile;

                if (file >= 0 && file == currentFile) BitboardHelper.SetBitAtIndex(square, ref mask);
                
                else if (rank >= 0 && rank == currentRank) {
                    BitboardHelper.SetBitAtIndex(square, ref mask);
                }
            }
        }

        return mask;
    }

    public static void InitializeEvalMasks() {
        for (int currentRank = 0; currentRank < 8; currentRank++) {
            for (int currentFile = 0; currentFile < 8; currentFile++) {
                int square = currentRank * 8 + currentFile;

                FileMasks[square] |= SetFileRankMasks(currentFile, -1);
                RankMasks[square] |= SetFileRankMasks(-1, currentRank);
                IsolatedPawnMasks[square] |= SetFileRankMasks(currentFile - 1, -1);
                IsolatedPawnMasks[square] |= SetFileRankMasks(currentFile + 1, -1);
            }
        }

        for (int currentRank = 0; currentRank < 8; currentRank++) {
            for (int currentFile = 0; currentFile < 8; currentFile++) {
                int square = currentRank * 8 + currentFile;
                
                WhitePassedMasks[square] |= SetFileRankMasks(currentFile - 1, -1);
                WhitePassedMasks[square] |= SetFileRankMasks(currentFile, -1);
                WhitePassedMasks[square] |= SetFileRankMasks(currentFile + 1, -1);
                BlackPassedMasks[square] |= SetFileRankMasks(currentFile - 1, -1);
                BlackPassedMasks[square] |= SetFileRankMasks(currentFile, -1);
                BlackPassedMasks[square] |= SetFileRankMasks(currentFile + 1, -1);

                for (int i = 0; i < 8 - currentRank; i++) 
                    WhitePassedMasks[square] &= ~RankMasks[(7 - i) * 8 + currentFile];

                for (int i = 0; i < currentRank + 1; i++) 
                    BlackPassedMasks[square] &= ~RankMasks[i * 8 + currentFile];
            }
        }
    }

    public static int Think(Board board, bool timeControl, int time = 1000, int depth = 100, bool shouldPrint = true) {
        _timer = new Timer();
        _stopSearch = false;
        _board = board;
        _time = time;
        _timeControl = timeControl;

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
            
            moveScores[i] = moveSpan[i] == entry.Move ? 100000 :
                isCapture ? 100 : promotion > 0 ? promotion * 100 : 0;
            
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
        
        TranspositionTable[positionKey % TranspositionTableSize] =
            new TranspositionTableEntry(positionKey, bestScore, depth, flag, bestMove);

        return bestScore;
    }

    public static int Quiescence(int alpha, int beta, int limit) {
        _nodes++;
        int standPat = Evaluate();
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
    
    public static int Evaluate() {
        int total = 0;
        
        for (Piece piece = Piece.WhitePawn; piece <= Piece.BlackKing; piece++) {
            ulong bitboard = _board.Bitboards[(int)piece];
            bool isWhite = (int)piece < 6;
            int side = isWhite ? 1 : -1;

            while (bitboard > 0) {
                int index = BitboardHelper.GetLSFBIndex(bitboard);

                int pieceValue = PieceWeights[(int)piece % 6] + PieceSquareTables[(int)piece, index];

                if (piece == Piece.WhitePawn || piece == Piece.BlackPawn) {
                    int doubledPawns = BitboardHelper.CountBits(_board.Bitboards[(int)piece] & FileMasks[index]);
                    if (doubledPawns > 1) total += (doubledPawns * DoubledPawnPenalty) * side;
                
                    if ((_board.Bitboards[(int)piece] & IsolatedPawnMasks[index]) == 0) {
                        total += IsolatedPawnPenalty * side;
                    }
                
                    if (isWhite && (WhitePassedMasks[index] & _board.Bitboards[(int)Piece.BlackPawn]) == 0) {
                        total += PassedPawnBonus[GetRank[index]];
                    }
                
                    if (!isWhite && (BlackPassedMasks[index] & _board.Bitboards[(int)Piece.WhitePawn]) == 0) {
                        total -= PassedPawnBonus[GetRank[64 - index]];
                    }
                }

                if (piece == Piece.WhiteRook || piece == Piece.BlackRook) {
                    if ((_board.Bitboards[isWhite ? (int)Piece.WhitePawn : (int)Piece.BlackPawn] & FileMasks[index]) == 0) {
                        total += SemiOpenFileScore * side;
                    }
                    
                    if (((_board.Bitboards[(int)Piece.WhitePawn] | _board.Bitboards[(int)Piece.BlackPawn]) & FileMasks[index]) == 0) {
                        total += OpenFileScore * side;
                    }
                }

                total += pieceValue * side;
                
                BitboardHelper.PopBitAtIndex(index, ref bitboard);
            }
        }

        return total * (_board.SideToMove == SideToMove.White ? 1 : -1);
    }
    
    public static void Stop() {
        _stopSearch = true;
    }
}