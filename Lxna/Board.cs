﻿namespace Lxna
{
    internal class Board {
        public ulong[] Bitboards;
        public ulong[] Blockers;
        public Square EnPassant;
        public SideToMove SideToMove;
        public int Castling;
        public static readonly Dictionary<String, Piece> CharPieces = new Dictionary<String, Piece> {
            { "P", Piece.WhitePawn },
            { "N", Piece.WhiteKnight },
            { "B", Piece.WhiteBishop },
            { "R", Piece.WhiteRook },
            { "Q", Piece.WhiteQueen },
            { "K", Piece.WhiteKing },
            { "p", Piece.BlackPawn },
            { "n", Piece.BlackKnight },
            { "b", Piece.BlackBishop },
            { "r", Piece.BlackRook },
            { "q", Piece.BlackQueen },
            { "k", Piece.BlackKing }
        };
        public static readonly String[] UnicodePieces = {
            "\u265f",
            "\u265E",
            "\u265D",
            "\u265C",
            "\u265B",
            "\u265A",
        };
        public static readonly int[] CastlingRights = new int[64] {
             7, 15, 15, 15,  3, 15, 15, 11,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            13, 15, 15, 15, 12, 15, 15, 14
        };

        // private List<BoardCopy> _boardHistory = new List<BoardCopy>();
        private List<Square> _enPassantHistory = new();
        private List<SideToMove> _sideToMoveHistory = new();
        private List<int> _castlingistory = new();
        private List<ulong[]> _bitboardsHistory = new();        
        private List<ulong[]> _blockersHistory = new();


        // private BoardCopy _boardCopy;

        public Board(String fen) {
            // this originally was in ParseFen() but it got moved here to fix warnings
            Bitboards = new ulong[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            Blockers = new ulong[] { 0x0, 0x0, 0x0 };
            SideToMove = SideToMove.White;
            EnPassant = Square.NoSquare;
            Castling = 0;
            
            ParseFen(fen);
            
            // _boardHistory.Add(new BoardCopy(EnPassant, SideToMove, Castling, Bitboards, Blockers));
            
            _enPassantHistory.Add(EnPassant);
            _sideToMoveHistory.Add(SideToMove);
            _castlingistory.Add(Castling);
            _bitboardsHistory.Add(Bitboards);
            _blockersHistory.Add(Blockers);
            
            // _boardCopy = new BoardCopy(EnPassant, SideToMove, Castling, Bitboards, Blockers);
        }
        
        public void ParseFen(String fen) {
            int index, square = 0;

            for (index = 0; index < fen.Length; index++) {
                if (square >= 64) break;
                
                char character = fen[index];

                if (character.ToString().Equals("/")) continue;

                if (Char.IsNumber(character)) square += character - '0';

                if (Char.IsLetter(character) && CharPieces.ContainsKey(character.ToString())) {
                    Piece piece = CharPieces[character.ToString()];
                    
                    BitboardHelper.SetBitAtIndex(square, ref Bitboards[(int)piece]);

                    square++;
                }
            }
            
            index++; // skip to side to move

            SideToMove = fen[index].ToString().Equals("w") ? SideToMove.White : SideToMove.Black;
            
            index += 2;  // skip to castling

            for (int castling = 0; castling < 4; castling++) {
                if (fen[index].ToString().Equals("K")) { index++; Castling |= (int)Castle.WhiteKingside; }
                if (fen[index].ToString().Equals("Q")) { index++; Castling |= (int)Castle.WhiteQueenside; }
                if (fen[index].ToString().Equals("k")) { index++; Castling |= (int)Castle.BlackKingside; }
                if (fen[index].ToString().Equals("q")) { index++; Castling |= (int)Castle.BlackQueenside; }
                if (fen[index].ToString().Equals("-")) { index++; }
            }

            index++; // skip to en passant

            if (!fen[index].ToString().Equals("-")) {
                int file = fen[index] - 'a';
                int rank = 8 - (fen[index + 1] - '0');

                EnPassant = (Square)(rank * 8 + file);
            }

            else EnPassant = Square.NoSquare;

            // blockers
            for (int piece = (int)Piece.WhitePawn; piece <= (int)Piece.WhiteKing; piece++) {
                Blockers[(int)SideToMove.White] |= Bitboards[piece];
                Blockers[(int)SideToMove.Both] |= Bitboards[piece];
            }

            for (int piece = (int)Piece.BlackPawn; piece <= (int)Piece.BlackKing; piece++) {
                Blockers[(int)SideToMove.Black] |= Bitboards[piece];
                Blockers[(int)SideToMove.Both] |= Bitboards[piece];
            }
        }

        public void Copy() {
            // BoardCopy copy = new BoardCopy(EnPassant, SideToMove, Castling, Bitboards.ToArray(), Blockers.ToArray());
            
            ulong[] bitboardsCopyArray = new ulong[12];
            ulong[] blockersCopyArray = new ulong[3];
            
            _enPassantHistory.Add(EnPassant);
            _sideToMoveHistory.Add(SideToMove);
            _castlingistory.Add(Castling);
            // _bitboardsHistory.Add(Bitboards.ToArray());
            // _blockersHistory.Add(Blockers.ToArray()); 
            Bitboards.CopyTo(bitboardsCopyArray, 0); 
            Blockers.CopyTo(blockersCopyArray, 0);
            // Array.Copy(Bitboards, _bitboardsCopyArray, Bitboards.Length);
            // Array.Copy(Blockers, _blockersCopyArray, Blockers.Length);
            
            _bitboardsHistory.Add(bitboardsCopyArray);
            _blockersHistory.Add(blockersCopyArray);

            // _boardHistory.Add(copy);
            
            // _boardCopy.SideToMove = SideToMove;
            // _boardCopy.EnPassant = EnPassant;
            // _boardCopy.Castling = Castling;
            // _boardCopy.Bitboards = Bitboards.ToArray();
            // _boardCopy.Blockers = Blockers.ToArray();

            // Array.Copy(Bitboards, _boardCopy.Bitboards, Bitboards.Length);
            // Array.Copy(Blockers, _boardCopy.Blockers, Blockers.Length);
        }
        
        public void TakeBack() {
            // if (BoardPly <= 0) return;
            
            // SideToMove = _boardCopy.SideToMove;
            // EnPassant = _boardCopy.EnPassant;
            // Castling = _boardCopy.Castling;
            // Bitboards = _boardCopy.Bitboards.ToArray();
            // Blockers = _boardCopy.Blockers.ToArray();
            
            // SideToMove = _boardHistory[_boardHistory.Count - 1].SideToMove;
            // EnPassant = _boardHistory[_boardHistory.Count - 1].EnPassant;
            // Castling = _boardHistory[_boardHistory.Count - 1].Castling;
            // Bitboards = _boardHistory[_boardHistory.Count - 1].Bitboards.ToArray();
            // Blockers = _boardHistory[_boardHistory.Count - 1].Blockers.ToArray();
            
            EnPassant = _enPassantHistory[_enPassantHistory.Count - 1];
            SideToMove = _sideToMoveHistory[_sideToMoveHistory.Count - 1];
            Castling = _castlingistory[_castlingistory.Count - 1];
            Bitboards = _bitboardsHistory[_bitboardsHistory.Count - 1];
            Blockers = _blockersHistory[_blockersHistory.Count - 1];
            
            // _boardHistory.RemoveAt(_boardHistory.Count - 1);
            
            _enPassantHistory.RemoveAt(_enPassantHistory.Count - 1);
            _sideToMoveHistory.RemoveAt(_sideToMoveHistory.Count - 1);
            _castlingistory.RemoveAt(_castlingistory.Count - 1);
            _bitboardsHistory.RemoveAt(_bitboardsHistory.Count - 1);
            _blockersHistory.RemoveAt(_blockersHistory.Count - 1);

            // Array.Copy(_boardCopy.Bitboards, Bitboards, _boardCopy.Bitboards.Length);
            // Array.Copy(_boardCopy.Blockers, Blockers, _boardCopy.Blockers.Length);
        }

        public void PrintAttacks(SideToMove side) {
            ulong attacks = 0x0;
            
            for (int square = 0; square < 64; square++) {
                if (IsSquareAttacked((Square)square, side)) BitboardHelper.SetBitAtIndex(square, ref attacks);
            }
            
            BitboardHelper.Print(attacks);
        }

        public List<int> GetPseudoLegalMoves() {
             return Movegen.GenerateMoves(this);
        }
        public List<int> GetPseudoLegalCaptures() {
            return Movegen.GenerateCaptureMoves(this);
        }

        public bool MakeMove(int move) {
            Copy();

            int source = Move.GetMoveSource(move);
            int target = Move.GetMoveTarget(move);
            int piece = Move.GetMovePiece(move);
            int promoted = Move.GetMovePromotion(move);
            int capture = Move.GetMoveCapture(move);
            int doublePush = Move.GetMoveDoublePush(move);
            int enPassant = Move.GetMoveEnPassant(move);
            int castling = Move.GetMoveCastling(move);

            BitboardHelper.PopBitAtIndex(source, ref Bitboards[(int)piece]);
            BitboardHelper.SetBitAtIndex(target, ref Bitboards[(int)piece]);
            BitboardHelper.PopBitAtIndex((int)source, ref Blockers[(int)SideToMove]);
            BitboardHelper.SetBitAtIndex((int)target, ref Blockers[(int)SideToMove]);
            BitboardHelper.PopBitAtIndex((int)source, ref Blockers[(int)SideToMove.Both]);
            BitboardHelper.SetBitAtIndex((int)target, ref Blockers[(int)SideToMove.Both]);

            if (capture > 0) {
                // this could be changed to ignore kings of both sides
                for (int currentPiece = 11 - (int)SideToMove * 6; currentPiece > 11 - ((int)SideToMove + 1) * 6; currentPiece--) {
                    if (BitboardHelper.GetBitAtIndex((int)target, Bitboards[currentPiece]) > 0) {
                        BitboardHelper.PopBitAtIndex((int)target, ref Bitboards[currentPiece]);
                        BitboardHelper.PopBitAtIndex((int)target, ref Blockers[(int)SideToMove ^ 1]);
                        break;
                    }   
                }
            }

            if (promoted > 0) {
                BitboardHelper.PopBitAtIndex(target, ref Bitboards[(int)SideToMove * 6]);
                BitboardHelper.SetBitAtIndex(target, ref Bitboards[(int)promoted]);
            }

            if (enPassant > 0) {
                if (SideToMove == SideToMove.White) {
                    BitboardHelper.PopBitAtIndex((int)target + 8, ref Bitboards[6]);
                    BitboardHelper.PopBitAtIndex((int)target + 8, ref Blockers[(int)SideToMove.Both]);
                    BitboardHelper.PopBitAtIndex((int)target + 8, ref Blockers[(int)SideToMove ^ 1]);
                }
                else {
                    BitboardHelper.PopBitAtIndex((int)target - 8, ref Bitboards[0]);
                    BitboardHelper.PopBitAtIndex((int)target - 8, ref Blockers[(int)SideToMove.Both]);
                    BitboardHelper.PopBitAtIndex((int)target - 8, ref Blockers[(int)SideToMove ^ 1]);
                }
            }
            
            EnPassant = Square.NoSquare;
            
            if (doublePush > 0) {
                _ = SideToMove == SideToMove.White
                    ? EnPassant = (Square)((int)target + 8)
                    : EnPassant = (Square)((int)target - 8);
            }

            if (castling > 0) {
                switch (target) {
                    case (int)Square.G1: {
                        BitboardHelper.PopBitAtSquare(Square.H1, ref Bitboards[(int)Piece.WhiteRook]);
                        BitboardHelper.SetBitAtSquare(Square.F1, ref Bitboards[(int)Piece.WhiteRook]);
                        BitboardHelper.PopBitAtSquare(Square.H1, ref Blockers[(int)SideToMove]);
                        BitboardHelper.SetBitAtSquare(Square.F1, ref Blockers[(int)SideToMove]);
                        BitboardHelper.PopBitAtSquare(Square.H1, ref Blockers[(int)SideToMove.Both]);
                        BitboardHelper.SetBitAtSquare(Square.F1, ref Blockers[(int)SideToMove.Both]);
                        break;
                    }
                    
                    case (int)Square.C1: {
                        BitboardHelper.PopBitAtSquare(Square.A1, ref Bitboards[(int)Piece.WhiteRook]);
                        BitboardHelper.SetBitAtSquare(Square.D1, ref Bitboards[(int)Piece.WhiteRook]);
                        BitboardHelper.PopBitAtSquare(Square.A1, ref Blockers[(int)SideToMove]);
                        BitboardHelper.SetBitAtSquare(Square.D1, ref Blockers[(int)SideToMove]);
                        BitboardHelper.PopBitAtSquare(Square.A1, ref Blockers[(int)SideToMove.Both]);
                        BitboardHelper.SetBitAtSquare(Square.D1, ref Blockers[(int)SideToMove.Both]);
                        break;
                    }
                    case (int)Square.G8: {
                        BitboardHelper.PopBitAtSquare(Square.H8, ref Bitboards[(int)Piece.BlackRook]);
                        BitboardHelper.SetBitAtSquare(Square.F8, ref Bitboards[(int)Piece.BlackRook]);
                        BitboardHelper.PopBitAtSquare(Square.H8, ref Blockers[(int)SideToMove]);
                        BitboardHelper.SetBitAtSquare(Square.F8, ref Blockers[(int)SideToMove]);
                        BitboardHelper.PopBitAtSquare(Square.H8, ref Blockers[(int)SideToMove.Both]);
                        BitboardHelper.SetBitAtSquare(Square.F8, ref Blockers[(int)SideToMove.Both]);
                        break;
                    }
                    
                    case (int)Square.C8: {
                        BitboardHelper.PopBitAtSquare(Square.A8, ref Bitboards[(int)Piece.BlackRook]);
                        BitboardHelper.SetBitAtSquare(Square.D8, ref Bitboards[(int)Piece.BlackRook]);
                        BitboardHelper.PopBitAtSquare(Square.A8, ref Blockers[(int)SideToMove]);
                        BitboardHelper.SetBitAtSquare(Square.D8, ref Blockers[(int)SideToMove]);
                        BitboardHelper.PopBitAtSquare(Square.A8, ref Blockers[(int)SideToMove.Both]);
                        BitboardHelper.SetBitAtSquare(Square.D8, ref Blockers[(int)SideToMove.Both]);
                        break;
                    }
                }
            }

            Castling &= CastlingRights[source];
            Castling &= CastlingRights[target];

            // Blockers[0] = 0x0;
            // Blockers[1] = 0x0;
            // Blockers[2] = 0x0;
            //
            // for (int bitboard = (int)Piece.WhitePawn; bitboard <= (int)Piece.WhiteKing; bitboard++) {
            //     Blockers[(int)SideToMove.White] |= Bitboards[bitboard];
            // }
            //
            // for (int bitboard = (int)Piece.BlackPawn; bitboard <= (int)Piece.BlackKing; bitboard++) {
            //     Blockers[(int)SideToMove.Black] |= Bitboards[bitboard];
            // }
            //
            // Blockers[(int)SideToMove.Both] |= Blockers[(int)SideToMove.White];
            // Blockers[(int)SideToMove.Both] |= Blockers[(int)SideToMove.Black];

            SideToMove = (SideToMove)((int)SideToMove ^ 1);

            if (IsSquareAttacked(
                (Square)BitboardHelper.GetLSFBIndex(Bitboards[11 - (int)SideToMove * 6]),
                SideToMove)) {
                TakeBack();

                return false;
            }
            
            return true;
        }

        public bool IsSquareAttacked(Square square, SideToMove side) {
            if (side == SideToMove.White) {
                if ((Movegen.PawnAttacks[(int)SideToMove.Black, (int)square] & Bitboards[(int)Piece.WhitePawn]) > 0) return true;
                if ((Movegen.KnightAttacks[(int)square] & Bitboards[(int)Piece.WhiteKnight]) > 0) return true;
                if ((Movegen.KingAttacks[(int)square] & Bitboards[(int)Piece.WhiteKing]) > 0) return true;
                
                if ((Movegen.GetBishopAttacks(square, Blockers[(int)SideToMove.Both]) &
                     (Bitboards[(int)Piece.WhiteBishop] | Bitboards[(int)Piece.WhiteQueen])) > 0) return true;
                if ((Movegen.GetRookAttacks(square, Blockers[(int)SideToMove.Both]) &
                     (Bitboards[(int)Piece.WhiteRook] | Bitboards[(int)Piece.WhiteQueen])) > 0) return true;
                
                // if ((Movegen.GetBishopAttacks(square, Blockers[(int)SideToMove.Both]) & Bitboards[(int)Piece.WhiteBishop]) > 0) return true;
                // if ((Movegen.GetRookAttacks(square, Blockers[(int)SideToMove.Both]) & Bitboards[(int)Piece.WhiteRook]) > 0) return true;
                // if ((Movegen.GetQueenAttacks(square, Blockers[(int)SideToMove.Both]) & Bitboards[(int)Piece.WhiteQueen]) > 0) return true;
            }

            if (side == SideToMove.Black) {
                if ((Movegen.PawnAttacks[(int)SideToMove.White, (int)square] & Bitboards[(int)Piece.BlackPawn]) > 0) return true;
                if ((Movegen.KnightAttacks[(int)square] & Bitboards[(int)Piece.BlackKnight]) > 0) return true;
                if ((Movegen.KingAttacks[(int)square] & Bitboards[(int)Piece.BlackKing]) > 0) return true;
                
                if ((Movegen.GetBishopAttacks(square, Blockers[(int)SideToMove.Both]) &
                     (Bitboards[(int)Piece.BlackBishop] | Bitboards[(int)Piece.BlackQueen])) > 0) return true;
                if ((Movegen.GetRookAttacks(square, Blockers[(int)SideToMove.Both]) &
                     (Bitboards[(int)Piece.BlackRook] | Bitboards[(int)Piece.BlackQueen])) > 0) return true;
                
                // if ((Movegen.GetBishopAttacks(square, Blockers[(int)SideToMove.Both]) & Bitboards[(int)Piece.BlackBishop]) > 0) return true;
                // if ((Movegen.GetRookAttacks(square, Blockers[(int)SideToMove.Both]) & Bitboards[(int)Piece.BlackRook]) > 0) return true;
                // if ((Movegen.GetQueenAttacks(square, Blockers[(int)SideToMove.Both]) & Bitboards[(int)Piece.BlackQueen]) > 0) return true;
            }
            // if (side == SideToMove.White && 
            //     (Movegen.PawnAttacks[(int)SideToMove.Black, (int)square] & 
            //      Bitboards[(int)Piece.WhitePawn]) > 0) return true;
            //
            // if (side == SideToMove.Black && 
            //     (Movegen.PawnAttacks[(int)SideToMove.White, (int)square] & 
            //      Bitboards[(int)Piece.BlackPawn]) > 0) return true;
            //
            // if ((Movegen.KnightAttacks[(int)square] &
            //      (side == SideToMove.White
            //          ? Bitboards[(int)Piece.WhiteKnight]
            //          : Bitboards[(int)Piece.BlackKnight]
            //      )) > 0) return true;
            //
            // if ((Movegen.KingAttacks[(int)square] &
            //      (side == SideToMove.White
            //          ? Bitboards[(int)Piece.WhiteKing]
            //          : Bitboards[(int)Piece.BlackKing]
            //      )) > 0) return true;
            //
            // if ((Movegen.GetBishopAttacks(square, Blockers[(int)SideToMove.Both]) & 
            //      (side == SideToMove.White
            //          ? Bitboards[(int)Piece.WhiteBishop]
            //          : Bitboards[(int)Piece.BlackBishop]
            //      )) > 0) return true;
            //
            // if ((Movegen.GetRookAttacks(square, Blockers[(int)SideToMove.Both]) & 
            //      (side == SideToMove.White
            //          ? Bitboards[(int)Piece.WhiteRook]
            //          : Bitboards[(int)Piece.BlackRook]
            //      )) > 0) return true;
            //
            // if ((Movegen.GetQueenAttacks(square, Blockers[(int)SideToMove.Both]) & 
            //      (side == SideToMove.White
            //          ? Bitboards[(int)Piece.WhiteQueen]
            //          : Bitboards[(int)Piece.BlackQueen]
            //      )) > 0) return true;
            
            return false;
        }

        public void Print()
        {
            for (int square = 0; square < 64; square++) {
                int rank = square / 8;
                int file = square % 8;
                
                if (file == 0) {
                    Console.ResetColor();
                    Console.Write("\n{0,2} ", 8 - rank);
                }

                Console.BackgroundColor = (rank + file) % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray;

                String pieceToPrint = " ";

                foreach (Piece piece in Enum.GetValues(typeof(Piece))) {
                    ulong bitboard = Bitboards[(int)piece];

                    if (BitboardHelper.GetBitAtIndex(square, bitboard) > 0) {
                        Console.ForegroundColor = (int)piece > 5 ? ConsoleColor.Black : ConsoleColor.White;
                        pieceToPrint = UnicodePieces[(int)piece % 6];
                    }
                }
                
                Console.Write("{0, 2} ", pieceToPrint);
            }
      
            Console.ResetColor();
            Console.WriteLine("\n    a  b  c  d  e  f  g  h\n");
            Console.WriteLine("SideToMove: {0, 15}", SideToMove);
            Console.WriteLine("EnPassant:  {0, 15}", EnPassant);
            Console.WriteLine("Castling:   {0, 12}{1, 1}{2, 1}{3, 1}",
                (Castling & (int)Castle.WhiteKingside) > 0 ?  "K" : "-",
                (Castling & (int)Castle.WhiteQueenside) > 0 ?  "Q" : "-",
                (Castling & (int)Castle.BlackKingside) > 0 ?  "k" : "-",
                (Castling & (int)Castle.BlackQueenside) > 0 ?  "q" : "-");
        }
    }
}