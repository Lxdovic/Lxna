namespace Lxna; 

public class Eval {
    
    private static readonly int[] _pieceWeights = {0, 100, 310, 330, 500, 1000, 10000 };

    public static int Evaluate() {
        int total = 0;

        foreach (bool isWhite in new[] { true, false }) {
            for (int piece = 1; piece <= 6; piece++) {
                ulong pieceBitBoard = Search._board.Bitboards[piece + (isWhite ? 0 : 6) - 1];
                
                while (pieceBitBoard != 0)
                {
                    int bitIndex = BitboardHelper.GetLSFBIndex(pieceBitBoard);
                    
                    BitboardHelper.PopBitAtIndex(bitIndex, ref pieceBitBoard);

                    total += _pieceWeights[piece] * (isWhite ? 1 : -1);
                }
            }
        }

        return total * (Search._board.SideToMove == SideToMove.White ? 1 : -1);
    }

}