
using System.Diagnostics;

namespace Lxna {
    internal class Move {
        public static void PrintMoveList(List<int> moves, bool verbose) {
            foreach (var move in moves) {
                if (!verbose) { Print(move); continue; }
                
                PrintVerbose(move);
            }
        }
        
        public static int EncodeMove(int source, int target, int piece, int promoted, int capture,
            int doublePush, int enPassant,
            int castling) {
            return source | (target << 6) | (piece << 12) | (promoted << 16) | (capture << 20) | (doublePush << 21) | (enPassant << 22) | (castling << 23);
        }

        public static Square GetMoveSource(int move) { return (Square)(move & 0x3f); }
        public static Square GetMoveTarget(int move) { return (Square)((move & 0xfc0) >> 6); }
        public static Piece GetMovePiece(int move) { return (Piece)((move & 0xf000) >> 12); }
        public static int GetMovePromotion(int move) { return (move & 0xf0000) >> 16; }
        public static int GetMoveCapture(int move) { return move & 0x100000; }
        public static int GetMoveDoublePush(int move) { return move & 0x200000; }
        public static int GetMoveEnPassant(int move) { return move & 0x400000; }
        public static int GetMoveCastling(int move) { return move & 0x800000; }
        
        public static void Print(int move) {
            Square source = GetMoveSource(move);
            Square target = GetMoveTarget(move);
            
            Console.Write("Move: {0}{1}", source, target);
        }
        
        public static void PrintVerbose(int move) {
            Square source = GetMoveSource(move);
            Square target = GetMoveTarget(move);
            Piece piece = GetMovePiece(move);
            int promotion = GetMovePromotion(move);
            int capture = GetMoveCapture(move);
            int doublePush = GetMoveDoublePush(move);
            int enPassant = GetMoveEnPassant(move);
            int castling = GetMoveCastling(move);
            
            Console.WriteLine("------------Move----------\nSource:    {0,15} \nTarget:    {1,15} \nPiece:     {2,15} \nPromotion: {3,15} \nCapture:   {4,15} \nDoublePush:{5,15} \nEnPassant: {6,15} \nCastling:  {7,15}\n--------------------------", 
                source, target, piece, promotion == 0 ? promotion : (Piece)promotion, capture, doublePush, enPassant, castling);
        }
    }
}