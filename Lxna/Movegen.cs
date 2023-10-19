
namespace Lxna {
    internal class Movegen {
        // 2d array because pawns move differently based on the side (White/Black)
        // first dimension is for side to move and the second one is the actual tables
        public static ulong[,] PawnAttacks = new ulong[2, 64];
        public static ulong[] KnightAttacks = new ulong[64];
        public static ulong[] KingAttacks = new ulong[64];
        public static ulong[] BishopMask = new ulong[64];
        public static ulong[] RookMask = new ulong[64];
        public static ulong[,] BishopAttacks = new ulong[64, 512];
        public static ulong[,] RookAttacks = new ulong[64, 4096];

        public static readonly int[] BishopRelevantBits = {
            6, 5, 5, 5, 5, 5, 5, 6,
            5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 9, 9, 7, 5, 5,
            5, 5, 7, 7, 7, 7, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5,
            6, 5, 5, 5, 5, 5, 5, 6,
        };
        public static readonly int[] RookRelevantBits = {
            12, 11, 11, 11, 11, 11, 11, 12, 
            11, 10, 10, 10, 10, 10, 10, 11, 
            11, 10, 10, 10, 10, 10, 10, 11, 
            11, 10, 10, 10, 10, 10, 10, 11, 
            11, 10, 10, 10, 10, 10, 10, 11, 
            11, 10, 10, 10, 10, 10, 10, 11, 
            11, 10, 10, 10, 10, 10, 10, 11, 
            12, 11, 11, 11, 11, 11, 11, 12, 
        };
        
        public const ulong NotFileA = 18374403900871474942;
        public const ulong NotFileH = 9187201950435737471;
        public const ulong NotFileHG = 4557430888798830399;
        public const ulong NotFileAB = 18229723555195321596;
        public static void Init() {
            InitLeaperAttacks();
            InitSliderAttacks(Magics.BishopRookFlag.Bishop);
            InitSliderAttacks(Magics.BishopRookFlag.Rook);
        }
        
        public static void InitSliderAttacks(Magics.BishopRookFlag pieceType) {
            for (int square = 0; square < 64; square++) {
                BishopMask[square] = MaskBishopAttacks((Square)square);
                RookMask[square] = MaskRookAttacks((Square)square);

                ulong attackMask = pieceType == Magics.BishopRookFlag.Bishop
                    ? BishopMask[square]
                    : RookMask[square];

                int relevantBitsCount = BitboardHelper.CountBits(attackMask);
                int blockerIndicies = 1 << relevantBitsCount;

                for (int index = 0; index < blockerIndicies; index++) {
                    if (pieceType == Magics.BishopRookFlag.Bishop) {
                        ulong blockers = SetBlockers(index, relevantBitsCount, attackMask);

                        int magicIndex = (int)((blockers * Magics.BishopMagicNumbers[square]) >> (64 - BishopRelevantBits[square]));

                        BishopAttacks[square, magicIndex] = GenerateBishopAttacksOnTheFly((Square)square, blockers);
                    }

                    if (pieceType == Magics.BishopRookFlag.Rook) {
                        ulong blockers = SetBlockers(index, relevantBitsCount, attackMask);

                        int magicIndex = (int)((blockers * Magics.RookMagicNumbers[square]) >> (64 - RookRelevantBits[square]));

                        RookAttacks[square, magicIndex] = GenerateRookAttacksOnTheFly((Square)square, blockers);
                    }
                }
            }
        }
        
        public static void InitLeaperAttacks() {
            for (int square = 0; square < 64; square++) {
                PawnAttacks[(int)SideToMove.White, square] = MaskPawnAttacks((Square)square, SideToMove.White);
                PawnAttacks[(int)SideToMove.Black, square] = MaskPawnAttacks((Square)square, SideToMove.Black);
                KnightAttacks[square] = MaskKnightAttacks((Square)square);
                KingAttacks[square] = MaskKingAttacks((Square)square);
            }
        }

        public static ulong GetBishopAttacks(Square square, ulong blockers) {
            blockers &= BishopMask[(int)square];
            blockers *= Magics.BishopMagicNumbers[(int)square];
            blockers >>= 64 - BishopRelevantBits[(int)square];

            return BishopAttacks[(int)square, blockers];
        }

        public static ulong GetRookAttacks(Square square, ulong blockers) {
            blockers &= RookMask[(int)square];
            blockers *= Magics.RookMagicNumbers[(int)square];
            blockers >>= 64 - RookRelevantBits[(int)square];
            
            return RookAttacks[(int)square, blockers];
        }

        public static ulong GetQueenAttacks(Square square, ulong blockers) {
            return GetRookAttacks(square, blockers) | GetBishopAttacks(square, blockers);
        }

        public static ulong MaskKingAttacks(Square square) {
            ulong attacks = 0x0;
            ulong bitboard = 0x0;

            BitboardHelper.SetBitAtSquare(square, ref bitboard);

            if (bitboard >> 8 > 0) attacks |= bitboard >> 8;
            if (((bitboard >> 9) & NotFileH) > 0) attacks |= bitboard >> 9;
            if (((bitboard >> 7) & NotFileA) > 0) attacks |= bitboard >> 7;
            if (((bitboard >> 1) & NotFileH) > 0) attacks |= bitboard >> 1;
            if (bitboard << 8 > 0) attacks |= bitboard << 8;
            if (((bitboard << 9) & NotFileA) > 0) attacks |= bitboard << 9;
            if (((bitboard << 7) & NotFileH) > 0) attacks |= bitboard << 7;
            if (((bitboard << 1) & NotFileA) > 0) attacks |= bitboard << 1;

            return attacks;
        }

        public static ulong MaskKnightAttacks(Square square) {
            ulong attacks = 0x0;
            ulong bitboard = 0x0;

            BitboardHelper.SetBitAtSquare(square, ref bitboard);

            if (((bitboard >> 17) & NotFileH) > 0) attacks |= bitboard >> 17;
            if (((bitboard >> 15) & NotFileA) > 0) attacks |= bitboard >> 15;
            if (((bitboard >> 10) & NotFileHG) > 0) attacks |= bitboard >> 10;
            if (((bitboard >> 6) & NotFileAB) > 0) attacks |= bitboard >> 6;
            if (((bitboard << 17) & NotFileA) > 0) attacks |= bitboard << 17;
            if (((bitboard << 15) & NotFileH) > 0) attacks |= bitboard << 15;
            if (((bitboard << 10) & NotFileAB) > 0) attacks |= bitboard << 10;
            if (((bitboard << 6) & NotFileHG) > 0) attacks |= bitboard << 6;

            return attacks;
        }

        public static ulong MaskPawnAttacks(Square square, SideToMove side) {
            ulong attacks = 0x0;
            ulong bitboard = 0x0;

            BitboardHelper.SetBitAtSquare(square, ref bitboard);

            if (side == SideToMove.White) {
                if (((bitboard >> 7) & NotFileA) > 0) attacks |= bitboard >> 7;
                if (((bitboard >> 9) & NotFileH) > 0) attacks |= bitboard >> 9;
            }

            else {
                if (((bitboard << 7) & NotFileH) > 0) attacks |= bitboard << 7;
                if (((bitboard << 9) & NotFileA) > 0) attacks |= bitboard << 9;
            }

            return attacks;
        }

        public static ulong MaskBishopAttacks(Square square) {
            ulong attacks = 0x0;

            int targetRank = (int)square / 8, targetFile = (int)square % 8, rank, file;

            for (rank = targetRank + 1, file = targetFile + 1; rank <= 6 && file <= 6; rank++, file++)
                attacks |= (ulong)0x1 << (rank * 8 + file);

            for (rank = targetRank - 1, file = targetFile + 1; rank >= 1 && file <= 6; rank--, file++)
                attacks |= (ulong)0x1 << (rank * 8 + file);

            for (rank = targetRank - 1, file = targetFile - 1; rank >= 1 && file >= 1; rank--, file--)
                attacks |= (ulong)0x1 << (rank * 8 + file);

            for (rank = targetRank + 1, file = targetFile - 1; rank <= 6 && file >= 1; rank++, file--)
                attacks |= (ulong)0x1 << (rank * 8 + file);

            return attacks;
        }

        public static ulong MaskRookAttacks(Square square) {
            ulong attacks = 0x0;

            int targetRank = (int)square / 8, targetFile = (int)square % 8, rank, file;

            for (rank = targetRank + 1; rank <= 6; rank++)
                attacks |= (ulong)0x1 << (rank * 8 + targetFile);

            for (rank = targetRank - 1; rank >= 1; rank--)
                attacks |= (ulong)0x1 << (rank * 8 + targetFile);

            for (file = targetFile + 1; file <= 6; file++)
                attacks |= (ulong)0x1 << (targetRank * 8 + file);

            for (file = targetFile - 1; file >= 1; file--)
                attacks |= (ulong)0x1 << (targetRank * 8 + file);

            return attacks;
        }

        public static ulong GenerateBishopAttacksOnTheFly(Square square, ulong blockers) {
            ulong attacks = 0x0;

            int targetRank = (int)square / 8, targetFile = (int)square % 8, rank, file;

            for (rank = targetRank + 1, file = targetFile + 1; rank <= 7 && file <= 7; rank++, file++) {
                var currentSquare = (ulong)0x1 << (rank * 8 + file);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            for (rank = targetRank - 1, file = targetFile + 1; rank >= 0 && file <= 7; rank--, file++) {
                var currentSquare = (ulong)0x1 << (rank * 8 + file);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            for (rank = targetRank - 1, file = targetFile - 1; rank >= 0 && file >= 0; rank--, file--) {
                var currentSquare = (ulong)0x1 << (rank * 8 + file);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            for (rank = targetRank + 1, file = targetFile - 1; rank <= 7 && file >= 0; rank++, file--) {
                var currentSquare = (ulong)0x1 << (rank * 8 + file);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            return attacks;
        }

        public static ulong GenerateRookAttacksOnTheFly(Square square, ulong blockers) {
            ulong attacks = 0x0;

            int targetRank = (int)square / 8, targetFile = (int)square % 8, rank, file;

            for (rank = targetRank + 1; rank <= 7; rank++) {
                var currentSquare = (ulong)0x1 << (rank * 8 + targetFile);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            for (rank = targetRank - 1; rank >= 0; rank--) {
                var currentSquare = (ulong)0x1 << (rank * 8 + targetFile);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            for (file = targetFile + 1; file <= 7; file++) {
                var currentSquare = (ulong)0x1 << (targetRank * 8 + file);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            for (file = targetFile - 1; file >= 0; file--) {
                var currentSquare = (ulong)0x1 << (targetRank * 8 + file);
                attacks |= currentSquare;
                if ((currentSquare & blockers) > 0) break;
            }

            return attacks;
        }

        public static ulong SetBlockers(int index, int numBitsInMask, ulong attackMaskValue) {
            ulong blockers = 0x0;
            ulong attackMask = attackMaskValue;

            for (int count = 0; count < numBitsInMask; count++) {
                int squareIndex = BitboardHelper.GetLSFBIndex(attackMask);

                BitboardHelper.PopBitAtIndex(squareIndex, ref attackMask);

                if ((index & (1 << count)) > 0) {
                    BitboardHelper.SetBitAtIndex(squareIndex, ref blockers);
                    // blockers |= ((ulong)0x1 << squareIndex);
                }
            }
            
            return blockers;
        }
        public static List<int> GenerateMoves(Board board) {
            List<int> moves = new List<int>();
            int source, target;
            ulong attacks;
                
            // This whole sequence is heavily repetitive and could easily be merged into separate functions
            for (int piece = (int)Piece.WhitePawn; piece <= (int)Piece.BlackKing; piece++) {
                ulong bitboard = board.Bitboards[piece];

                if (board.SideToMove == SideToMove.White) {
                    if ((Piece)piece == Piece.WhitePawn) {
                        while (bitboard > 0) {
                            source = BitboardHelper.GetLSFBIndex(bitboard);
                            target = source - 8;

                            if (!(target < (int)Square.A8) && BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.Both]) <= 0) {
                                if (source >= (int)Square.A7 && source <= (int)Square.H7) {
                                    // Console.WriteLine("White Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "Q");
                                    // Console.WriteLine("White Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "R");
                                    // Console.WriteLine("White Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "B");
                                    // Console.WriteLine("White Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "N");
                                    
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteQueen, 0, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteRook, 0, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteBishop, 0, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteKnight, 0, 0, 0, 0));
                                }

                                else {
                                    // Console.WriteLine("White Pawn Push: {0,2}{1,2}", (Square)source, (Square)target);
                                    moves.Add(Board.EncodeMove(source, target, piece, 0, 0, 0, 0, 0));
                                    
                                    if (source >= (int)Square.A2 && source <= (int)Square.H2 &&
                                        BitboardHelper.GetBitAtIndex(target - 8, board.Blockers[(int)SideToMove.Both]) <= 0) {
                                        // Console.WriteLine("White Pawn Double Push: {0,2}{1,2}", (Square)source, (Square)target - 8);
                                        moves.Add(Board.EncodeMove(source, target - 8, piece, 0, 0, 1, 0, 0));
                                    }
                                }
                            }

                            attacks = PawnAttacks[(int)board.SideToMove, source] &
                                      board.Blockers[(int)SideToMove.Black];
                            
                            while (attacks > 0) {
                                target = BitboardHelper.GetLSFBIndex(attacks);
                                
                                if (source >= (int)Square.A7 && source <= (int)Square.H7) {
                                    // Console.WriteLine("White Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "Q");
                                    // Console.WriteLine("White Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "R");
                                    // Console.WriteLine("White Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "B");
                                    // Console.WriteLine("White Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "N");
                                    
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteQueen, 1, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteRook, 1, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteBishop, 1, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.WhiteKnight, 1, 0, 0, 0));
                                }
                            
                                else {
                                    // Console.WriteLine("White Pawn Capture: {0,2}{1,2}", (Square)source, (Square)target);
                                    
                                    moves.Add(Board.EncodeMove(source, target, piece, 0, 1, 0, 0, 0));
                                }
                                
                                BitboardHelper.PopBitAtIndex(target, ref attacks);
                            }

                            if (board.EnPassant != Square.NoSquare) {
                                ulong enPassantAttacks = PawnAttacks[(int)board.SideToMove, source] & ((ulong)0x1 << (int)board.EnPassant);

                                if (enPassantAttacks > 0) {
                                    int targetEnPassant = BitboardHelper.GetLSFBIndex(enPassantAttacks);
                                    
                                    // Console.WriteLine("White Pawn EnPassant Capture: {0,2}{1,2}", (Square)source, (Square)targetEnPassant);
                                    
                                    moves.Add(Board.EncodeMove(source, targetEnPassant, piece, 0, 1, 0, 1, 0));
                                }
                            }

                            BitboardHelper.PopBitAtIndex(source, ref bitboard);
                        }
                    }

                    if ((Piece)piece == Piece.WhiteKing) {
                        if ((board.Castling & (int)Castle.WhiteKingside) > 0) {
                            if (BitboardHelper.GetBitAtSquare(Square.F1, board.Blockers[(int)SideToMove.Both]) <= 0 &&
                                BitboardHelper.GetBitAtSquare(Square.G1, board.Blockers[(int)SideToMove.Both]) <= 0) {
                                if (!board.IsSquareAttacked(Square.E1, SideToMove.Black) &&
                                    !board.IsSquareAttacked(Square.F1, SideToMove.Black)) {
                                    // Console.WriteLine("White Castle Kingside: E1G1");
                                    
                                    moves.Add(Board.EncodeMove((int)Square.E1, (int)Square.G1, piece, 0, 0, 0, 0, 1));
                                }
                            }
                        }
                        
                        if ((board.Castling & (int)Castle.WhiteQueenside) > 0) {
                            if (BitboardHelper.GetBitAtSquare(Square.D1, board.Blockers[(int)SideToMove.Both]) <= 0 &&
                                BitboardHelper.GetBitAtSquare(Square.C1, board.Blockers[(int)SideToMove.Both]) <= 0 && 
                                BitboardHelper.GetBitAtSquare(Square.B1, board.Blockers[(int)SideToMove.Both]) <= 0) {
                                if (!board.IsSquareAttacked(Square.E1, SideToMove.Black) &&
                                    !board.IsSquareAttacked(Square.D1, SideToMove.Black)) {
                                    // Console.WriteLine("White Castle Queenside: E1C1");
                                    
                                    moves.Add(Board.EncodeMove((int)Square.E1, (int)Square.C1, piece, 0, 0, 0, 0, 1));
                                }
                            }
                        }
                    }
                }

                else {
                    if ((Piece)piece == Piece.BlackPawn) {
                        while (bitboard > 0) {
                            source = BitboardHelper.GetLSFBIndex(bitboard);
                            target = source + 8;

                            if (!(target > (int)Square.H1) && BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.Both]) <= 0) {
                                if (source >= (int)Square.A2 && source <= (int)Square.H2) {
                                    // Console.WriteLine("Black Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "Q");
                                    // Console.WriteLine("Black Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "R");
                                    // Console.WriteLine("Black Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "B");
                                    // Console.WriteLine("Black Pawn Promotion: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "N");
                                    
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackQueen, 0, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackRook, 0, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackBishop, 0, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackKnight, 0, 0, 0, 0));
                                }

                                else {
                                    // Console.WriteLine("Black Pawn Push: {0,2}{1,2}", (Square)source, (Square)target);
                                    
                                    moves.Add(Board.EncodeMove(source, target, piece, 0, 0, 0, 0, 0));

                                    if (source >= (int)Square.A7 && source <= (int)Square.H7 &&
                                        BitboardHelper.GetBitAtIndex(target + 8, board.Blockers[(int)SideToMove.Both]) <= 0) {
                                        // Console.WriteLine("Black Pawn Double Push: {0,2}{1,2}", (Square)source, (Square)target + 8);
                                        
                                        moves.Add(Board.EncodeMove(source, target + 8, piece, 0, 0, 1, 0, 0));
                                    }
                                }
                            }
                            
                            attacks = PawnAttacks[(int)board.SideToMove, source] &
                                      board.Blockers[(int)SideToMove.White];
                            
                            while (attacks > 0) {
                                target = BitboardHelper.GetLSFBIndex(attacks);
                                
                                if (source >= (int)Square.A2 && source <= (int)Square.H2) {
                                    // Console.WriteLine("Black Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "Q");
                                    // Console.WriteLine("Black Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "R");
                                    // Console.WriteLine("Black Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "B");
                                    // Console.WriteLine("Black Pawn Promotion Capture: {0,2}{1,2} -> {2,0}", (Square)source, (Square)target, "N");
                                    
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackQueen, 1, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackRook, 1, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackBishop, 1, 0, 0, 0));
                                    moves.Add(Board.EncodeMove(source, target, piece, (int)Piece.BlackKnight, 1, 0, 0, 0));
                                }
                            
                                else {
                                    // Console.WriteLine("Black Pawn Capture: {0,2}{1,2}", (Square)source, (Square)target);
                                    
                                    moves.Add(Board.EncodeMove(source, target, piece, 0, 1, 0, 0, 0));
                                }


                                BitboardHelper.PopBitAtIndex(target, ref attacks);
                            }

                            if (board.EnPassant != Square.NoSquare) {
                                ulong enPassantAttacks = 
                                    PawnAttacks[(int)board.SideToMove, source] & ((ulong)0x1 << (int)board.EnPassant);

                                if (enPassantAttacks > 0) {
                                    int targetEnPassant = BitboardHelper.GetLSFBIndex(enPassantAttacks);
                                    
                                    // Console.WriteLine("Black Pawn EnPassant Capture: {0,2}{1,2}", (Square)source, (Square)targetEnPassant);
                                    
                                    moves.Add(Board.EncodeMove(source, targetEnPassant, piece, 0, 1, 0, 1, 0));
                                }
                            }

                            BitboardHelper.PopBitAtIndex(source, ref bitboard);
                        }
                    }
                    
                    if ((Piece)piece == Piece.BlackKing) {
                        if ((board.Castling & (int)Castle.BlackKingside) > 0) {
                            if (BitboardHelper.GetBitAtSquare(Square.F8, board.Blockers[(int)SideToMove.Both]) <= 0 &&
                                BitboardHelper.GetBitAtSquare(Square.G8, board.Blockers[(int)SideToMove.Both]) <= 0) {
                                if (!board.IsSquareAttacked(Square.E8, SideToMove.White) &&
                                    !board.IsSquareAttacked(Square.F8, SideToMove.White)) {
                                    // Console.WriteLine("Black Castle Kingside: E8G8");
                                    
                                    moves.Add(Board.EncodeMove((int)Square.E8, (int)Square.G8, piece, 0, 0, 0, 0, 1));
                                }
                            }
                        }
                        
                        if ((board.Castling & (int)Castle.BlackQueenside) > 0) {
                            if (BitboardHelper.GetBitAtSquare(Square.D8, board.Blockers[(int)SideToMove.Both]) <= 0 &&
                                BitboardHelper.GetBitAtSquare(Square.C8, board.Blockers[(int)SideToMove.Both]) <= 0 &&
                                BitboardHelper.GetBitAtSquare(Square.B8, board.Blockers[(int)SideToMove.Both]) <= 0 ){
                                if (!board.IsSquareAttacked(Square.E8, SideToMove.White) &&
                                    !board.IsSquareAttacked(Square.D8, SideToMove.White)) {
                                    // Console.WriteLine("Black Castle Queenside: E8C8");
                                    
                                    moves.Add(Board.EncodeMove((int)Square.E8, (int)Square.C8, piece, 0, 0, 0, 0, 1));
                                }
                            }
                        }
                    }
                }
                
                if (board.SideToMove == SideToMove.White ? piece == (int)Piece.WhiteKnight : piece == (int)Piece.BlackKnight) {
                    while (bitboard > 0) {
                        source = BitboardHelper.GetLSFBIndex(bitboard);

                        attacks = KnightAttacks[source] & (board.SideToMove == SideToMove.White
                            ? ~board.Blockers[(int)SideToMove.White]
                            : ~board.Blockers[(int)SideToMove.Black]);

                        while (attacks > 0) {
                            target = BitboardHelper.GetLSFBIndex(attacks);

                            if (board.SideToMove == SideToMove.White
                                    ? BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.Black]) <= 0
                                    : BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.White]) <= 0) {
                                // Console.WriteLine("Knight Quiet Move: {0,2}{1,2}", (Square)source, (Square)target);
                                    
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 0, 0, 0, 0));
                            }
                            
                            else {
                                // Console.WriteLine("Knight Capture: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 1, 0, 0, 0));
                            }
                            
                            BitboardHelper.PopBitAtIndex(target, ref attacks);
                        }

                        BitboardHelper.PopBitAtIndex(source, ref bitboard);
                    }
                }
                
                if (board.SideToMove == SideToMove.White ? piece == (int)Piece.WhiteBishop : piece == (int)Piece.BlackBishop) {
                    while (bitboard > 0) {
                        source = BitboardHelper.GetLSFBIndex(bitboard);

                        attacks = GetBishopAttacks((Square)source, board.Blockers[(int)SideToMove.Both]) & (board.SideToMove == SideToMove.White
                            ? ~board.Blockers[(int)SideToMove.White]
                            : ~board.Blockers[(int)SideToMove.Black]);

                        while (attacks > 0) {
                            target = BitboardHelper.GetLSFBIndex(attacks);

                            if (board.SideToMove == SideToMove.White
                                    ? BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.Black]) <= 0
                                    : BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.White]) <= 0) {
                                // Console.WriteLine("Bishop Quiet Move: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 0, 0, 0, 0));
                            }
                            
                            else {
                                // Console.WriteLine("Bishop Capture: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 1, 0, 0, 0));
                            }
                            
                            BitboardHelper.PopBitAtIndex(target, ref attacks);
                        }

                        BitboardHelper.PopBitAtIndex(source, ref bitboard);
                    }
                }
                
                if (board.SideToMove == SideToMove.White ? piece == (int)Piece.WhiteRook : piece == (int)Piece.BlackRook) {
                    while (bitboard > 0) {
                        source = BitboardHelper.GetLSFBIndex(bitboard);

                        attacks = GetRookAttacks((Square)source, board.Blockers[(int)SideToMove.Both]) & (board.SideToMove == SideToMove.White
                            ? ~board.Blockers[(int)SideToMove.White]
                            : ~board.Blockers[(int)SideToMove.Black]);

                        while (attacks > 0) {
                            target = BitboardHelper.GetLSFBIndex(attacks);

                            if (board.SideToMove == SideToMove.White
                                    ? BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.Black]) <= 0
                                    : BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.White]) <= 0) {
                                // Console.WriteLine("Rook Quiet Move: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 0, 0, 0, 0));
                            }
                            
                            else {
                                // Console.WriteLine("Rook Capture: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 1, 0, 0, 0));
                            }
                            
                            BitboardHelper.PopBitAtIndex(target, ref attacks);
                        }

                        BitboardHelper.PopBitAtIndex(source, ref bitboard);
                    }
                }
                
                if (board.SideToMove == SideToMove.White ? piece == (int)Piece.WhiteQueen : piece == (int)Piece.BlackQueen) {
                    while (bitboard > 0) {
                        source = BitboardHelper.GetLSFBIndex(bitboard);

                        attacks = GetQueenAttacks((Square)source, board.Blockers[(int)SideToMove.Both]) & (board.SideToMove == SideToMove.White
                            ? ~board.Blockers[(int)SideToMove.White]
                            : ~board.Blockers[(int)SideToMove.Black]);

                        while (attacks > 0) {
                            target = BitboardHelper.GetLSFBIndex(attacks);

                            if (board.SideToMove == SideToMove.White 
                                    ? BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.Black]) <= 0
                                    : BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.White]) <= 0) {
                                // Console.WriteLine("Queen Quiet Move: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 0, 0, 0, 0));
                            }
                            
                            else {
                                // Console.WriteLine("Queen Capture: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 1, 0, 0, 0));
                            }
                            
                            BitboardHelper.PopBitAtIndex(target, ref attacks);
                        }

                        BitboardHelper.PopBitAtIndex(source, ref bitboard);
                    }
                }
                
                if (board.SideToMove == SideToMove.White ? piece == (int)Piece.WhiteKing : piece == (int)Piece.BlackKing) {
                    while (bitboard > 0) {
                        source = BitboardHelper.GetLSFBIndex(bitboard);

                        attacks = KingAttacks[source] & (board.SideToMove == SideToMove.White
                            ? ~board.Blockers[(int)SideToMove.White]
                            : ~board.Blockers[(int)SideToMove.Black]);

                        while (attacks > 0) {
                            target = BitboardHelper.GetLSFBIndex(attacks);

                            if (board.SideToMove == SideToMove.White
                                    ? BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.Black]) <= 0
                                    : BitboardHelper.GetBitAtIndex(target, board.Blockers[(int)SideToMove.White]) <= 0) {
                                // Console.WriteLine("King Quiet Move: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 0, 0, 0, 0));
                            }
                            
                            else {
                                // Console.WriteLine("King Capture: {0,2}{1,2}", (Square)source, (Square)target);
                                
                                moves.Add(Board.EncodeMove(source, target, piece, 0, 1, 0, 0, 0));
                            }
                            
                            BitboardHelper.PopBitAtIndex(target, ref attacks);
                        }

                        BitboardHelper.PopBitAtIndex(source, ref bitboard);
                    }
                }
            }

            return moves;
        }
    }
}