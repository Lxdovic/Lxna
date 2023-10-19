
using System.Diagnostics;

namespace Lxna {
    internal class Move {
        public static void PrintMoveList(List<int> moves, bool verbose) {
            foreach (var move in moves) {
                if (!verbose) { Print(move); continue; }
                
                PrintVerbose(move);
            }
        }
        
        public static void Print(int move) {
            Square source = Board.GetMoveSource(move);
            Square target = Board.GetMoveTarget(move);
            
            Console.WriteLine("Move: {0}{1}", source, target);
        }
        
        public static void PrintVerbose(int move) {
            Square source = Board.GetMoveSource(move);
            Square target = Board.GetMoveTarget(move);
            Piece piece = Board.GetMovePiece(move);
            int promotion = Board.GetMovePromotion(move);
            int capture = Board.GetMoveCapture(move);
            int doublePush = Board.GetMoveDoublePush(move);
            int enPassant = Board.GetMoveEnPassant(move);
            int castling = Board.GetMoveCastling(move);
            
            Console.WriteLine("------------Move----------\nSource:    {0,15} \nTarget:    {1,15} \nPiece:     {2,15} \nPromotion: {3,15} \nCapture:   {4,15} \nDoublePush:{5,15} \nEnPassant: {6,15} \nCastling:  {7,15}\n--------------------------", 
                source, target, piece, promotion == 0 ? promotion : (Piece)promotion, capture, doublePush, enPassant, castling);
        }
    }
}