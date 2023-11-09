namespace Lxna; 

public class Perft {
    private static ulong _nodes;
    private static Board _board;
    
    public static ulong PerfTest(Board board, int depth) {
        _board = board;
        _nodes = 0;
            
        Span<int> moveSpan = stackalloc int[218];
        _board.GetPseudoLegalMovesNonAlloc(ref moveSpan);
            
        Timer timer = new Timer();
            
        foreach (int move in moveSpan) {
            if (!_board.MakeMove(move)) continue;
            
            ulong oldNodes = _nodes;
                
            PerfDriver(depth - 1);
                
            _board.TakeBack();

            ulong cumulativeNodes = _nodes - oldNodes;
                
            double timeSeconds = timer.GetDiff() / 1000.0;
            double nps = _nodes / timeSeconds;
                
            Move.Print(move);
            Console.WriteLine("  nodes {0,6:n0}  KN/s {1,5:n0} time: {2,5:n0}", cumulativeNodes, nps / 1000, timer.GetDiff());
        }
            
        Console.WriteLine("\nperft depth {0,1} nodes {1,9:n0} time {2,4}\n", depth, _nodes, timer.GetDiff());

        return _nodes;
    }

    private static void PerfDriver(int depth) {
        if (depth == 0) {
            _nodes++;

            return;
        }
            
        Span<int> moveSpan = stackalloc int[218];
        _board.GetPseudoLegalMovesNonAlloc(ref moveSpan);
            
        for (int i = 0; i < moveSpan.Length; i++) {
            if (!_board.MakeMove(moveSpan[i])) continue;
                
            PerfDriver(depth - 1);
                
            _board.TakeBack();
        }
    }
}