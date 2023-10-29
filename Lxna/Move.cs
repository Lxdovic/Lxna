using System.Runtime.CompilerServices;

namespace Lxna {
    public class Move {
        public static void PrintMoveList(List<int> moves, bool verbose) {
            foreach (var move in moves) {
                if (!verbose) { Print(move); continue; }
                
                PrintVerbose(move);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EncodeMove(int source, int target, int piece, int promoted, int capture,
            int doublePush, int enPassant,
            int castling) {
            return source | (target << 6) | (piece << 12) | (promoted << 16) | (capture << 20) | (doublePush << 21) | (enPassant << 22) | (castling << 23);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveSource(int move) { return move & 0x3f; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveTarget(int move) { return (move & 0xfc0) >> 6; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMovePiece(int move) { return (move & 0xf000) >> 12; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMovePromotion(int move) { return (move & 0xf0000) >> 16; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveCapture(int move) { return move & 0x100000; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveDoublePush(int move) { return move & 0x200000; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveEnPassant(int move) { return move & 0x400000; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMoveCastling(int move) { return move & 0x800000; }
        
        public static void Print(int move) {
            Square source = (Square)GetMoveSource(move);
            Square target = (Square)GetMoveTarget(move);
            
            Console.Write("move {0}{1}", source, target);
        }
        
        public static void PrintVerbose(int move) {
            Square source = (Square)GetMoveSource(move);
            Square target = (Square)GetMoveTarget(move);
            Piece piece = (Piece)GetMovePiece(move);
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