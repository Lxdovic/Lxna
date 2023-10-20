namespace Lxna
{
    internal class BoardCopy {
        public Square EnPassant;
        public SideToMove SideToMove;
        public int Castling;
        public ulong[] Bitboards;
        public ulong[] Blockers;

        public BoardCopy(Square enPassant, SideToMove sideToMove, int castling, ulong[] bitboards, ulong[] blockers) {
            EnPassant = enPassant;
            SideToMove = sideToMove;
            Castling = castling;
            Bitboards = bitboards;
            Blockers = blockers;
        }
    }
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
        public static readonly String[] UNICODEPieces = {
            "\u265f",
            "\u265E",
            "\u265D",
            "\u265C",
            "\u265B",
            "\u265A",
        };
        private BoardCopy _boardCopy;

        public Board(String fen) {
            ParseFen(fen);
            _boardCopy = new BoardCopy(EnPassant, SideToMove, Castling, Bitboards, Blockers);
        }
        
        public void ParseFen(String fen) {
            Bitboards = new ulong[12] {
                0x0,
                0x0,
                0x0,
                0x0,
                0x0,
                0x0,
                0x0,
                0x0,
                0x0,
                0x0,
                0x0,
                0x0
            };
            Blockers = new ulong[3] {
                0x0,
                0x0,
                0x0,
            };
            SideToMove = SideToMove.White;
            EnPassant = Square.NoSquare;
            Castling = 0;

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
            _boardCopy.SideToMove = SideToMove;
            _boardCopy.EnPassant = EnPassant;
            _boardCopy.Castling = Castling;
            _boardCopy.Bitboards = Bitboards.ToArray();
            _boardCopy.Blockers = Blockers.ToArray();
            
            // Array.Copy(Bitboards, _boardCopy.Bitboards, 12);
            // Array.Copy(Blockers, _boardCopy.Blockers, 3);
        }
        
        public void TakeBack() {
            
            SideToMove = _boardCopy.SideToMove;
            EnPassant = _boardCopy.EnPassant;
            Castling = _boardCopy.Castling;
            Bitboards = _boardCopy.Bitboards.ToArray();
            Blockers = _boardCopy.Blockers.ToArray();
            // Array.Copy(_boardCopy.Bitboards, Bitboards, 12);
            // Array.Copy(_boardCopy.Blockers, Blockers, 3);
        }

        public void PrintAttacks(SideToMove side) {
            ulong attacks = 0x0;
            
            for (int square = 0; square < 64; square++) {
                if (IsSquareAttacked((Square)square, side)) BitboardHelper.SetBitAtIndex(square, ref attacks);
            }
            
            BitboardHelper.Print(attacks);
        }

        public List<int> GetLegalMoves(bool capturesOnly) {
             return capturesOnly ? Movegen.GenerateCaptureMoves(this) : Movegen.GenerateMoves(this);
        }

        public void MakeMove(int move) {
            Copy();

            Square source = Move.GetMoveSource(move);
            Square target = Move.GetMoveTarget(move);
            Piece piece = Move.GetMovePiece(move);
            Piece promoted = (Piece)Move.GetMovePromotion(move);
            int capture = Move.GetMoveCapture(move);
            int doublePush = Move.GetMoveDoublePush(move);
            int enPassant = Move.GetMoveEnPassant(move);
            int castling = Move.GetMoveCastling(move);

            BitboardHelper.PopBitAtSquare(source, ref Bitboards[(int)piece]);
            BitboardHelper.SetBitAtSquare(target, ref Bitboards[(int)piece]);
        }

        public bool IsSquareAttacked(Square square, SideToMove side) {
            if (side == SideToMove.White && 
                (Movegen.PawnAttacks[(int)SideToMove.Black, (int)square] & 
                 Bitboards[(int)Piece.WhitePawn]) > 0) return true;
            
            if (side == SideToMove.Black && 
                (Movegen.PawnAttacks[(int)SideToMove.White, (int)square] & 
                 Bitboards[(int)Piece.BlackPawn]) > 0) return true;

            if ((Movegen.KnightAttacks[(int)square] &
                 (side == SideToMove.White
                     ? Bitboards[(int)Piece.WhiteKnight]
                     : Bitboards[(int)Piece.BlackKnight]
                 )) > 0) return true;

            if ((Movegen.KingAttacks[(int)square] &
                 (side == SideToMove.White
                     ? Bitboards[(int)Piece.WhiteKing]
                     : Bitboards[(int)Piece.BlackKing]
                 )) > 0) return true;

            if ((Movegen.GetBishopAttacks(square, Blockers[(int)SideToMove.Both]) & 
                 (side == SideToMove.White
                     ? Bitboards[(int)Piece.WhiteBishop]
                     : Bitboards[(int)Piece.BlackBishop]
                 )) > 0) return true;

            if ((Movegen.GetRookAttacks(square, Blockers[(int)SideToMove.Both]) & 
                 (side == SideToMove.White
                     ? Bitboards[(int)Piece.WhiteRook]
                     : Bitboards[(int)Piece.BlackRook]
                 )) > 0) return true;

            if ((Movegen.GetQueenAttacks(square, Blockers[(int)SideToMove.Both]) & 
                 (side == SideToMove.White
                     ? Bitboards[(int)Piece.WhiteQueen]
                     : Bitboards[(int)Piece.BlackQueen]
                 )) > 0) return true;
            
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
                        pieceToPrint = UNICODEPieces[(int)piece % 6];
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