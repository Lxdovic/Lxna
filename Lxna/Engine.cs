﻿using System;

namespace Lxna {
    public enum Square {
        A8,
        B8,
        C8,
        D8,
        E8,
        F8,
        G8,
        H8,
        A7,
        B7,
        C7,
        D7,
        E7,
        F7,
        G7,
        H7,
        A6,
        B6,
        C6,
        D6,
        E6,
        F6,
        G6,
        H6,
        A5,
        B5,
        C5,
        D5,
        E5,
        F5,
        G5,
        H5,
        A4,
        B4,
        C4,
        D4,
        E4,
        F4,
        G4,
        H4,
        A3,
        B3,
        C3,
        D3,
        E3,
        F3,
        G3,
        H3,
        A2,
        B2,
        C2,
        D2,
        E2,
        F2,
        G2,
        H2,
        A1,
        B1,
        C1,
        D1,
        E1,
        F1,
        G1,
        H1,
        NoSquare
    }
    public enum SideToMove {
        White,
        Black,
        Both
    }
    public enum Castle {
        WhiteKingside = 1,
        WhiteQueenside = 2,
        BlackKingside = 4,
        BlackQueenside = 8
    }
    public enum Piece {
        WhitePawn,
        WhiteKnight,
        WhiteBishop,
        WhiteRook,
        WhiteQueen,
        WhiteKing,
        BlackPawn,
        BlackKnight,
        BlackBishop,
        BlackRook,
        BlackQueen,
        BlackKing
    }
    public enum PromotedPiece {
        WhiteKnight,
        WhiteBishop,
        WhiteRook,
        WhiteQueen,
        BlackKnight,
        BlackBishop,
        BlackRook,
        BlackQueen,
    }

    public struct MoveList {
        private int[] moves;
        private int count;
    }
    
    internal class Engine {
        public static readonly String EMPTY_BOARD = "8/8/8/8/8/8/8/8 w - - ";
        public static readonly String TRICKY_POS = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1 ";
        public static readonly String START_POS = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 ";
        public static readonly String KILLER_POS = "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1";
        public static readonly String CMK_POS = "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 ";

        public static void Main(string[] args) {
            Movegen.Init();
            //
            Board board = new Board(TRICKY_POS);
            
            board.Print();
            
            List<int> moves = board.GetLegalMoves();

            Move.PrintMoveList(moves, true);
            
            Console.WriteLine("MoveList Length: {0,2}", moves.Count);
        }
    }
}