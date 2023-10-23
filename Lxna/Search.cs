namespace Lxna; 

internal class Search {
    private static int _iterationMove;
    public static Board _board;
    private static Timer _timer;

    public static int Think(Board board) {
        _timer = new Timer();
        _board = board;

        int bestMove = 0;

        for (int currentDepth = 1; currentDepth <= 100; currentDepth++)
        {
            RootNegamax(currentDepth);

            if (_timer.GetDiff() > 1000) break;

            bestMove = _iterationMove;
            
            Move.Print(bestMove);
            Console.WriteLine(" depth {0,1}", currentDepth);
        }

        return bestMove;
    }

    public static void RootNegamax(int depth) {
        int bestScore = -100000;

        List<int> moves = _board.GetPseudoLegalMoves();

        foreach (int move in moves) {
            if (!_board.MakeMove(move)) continue;
            int score = -Negamax(-100000, 100000, depth - 1);
            _board.TakeBack();

            if (score > bestScore) {
                bestScore = score;
                _iterationMove = move;
            }
        }
    }

    public static int Negamax(int alpha, int beta, int depth) {

        if (depth == 0) return Eval.EvaluatePeStO();
        
        List<int> moves = _board.GetPseudoLegalMoves();

        foreach (int move in moves) {
            if (_timer.GetDiff() > 1000) break;
            if (!_board.MakeMove(move)) continue;
            int score = -Negamax(-beta, -alpha, depth - 1);
            _board.TakeBack();

            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }

        return alpha;
    }

}