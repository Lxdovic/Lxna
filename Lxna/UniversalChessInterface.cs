namespace Lxna {
    internal static class UniversalChessInterface {
        private static Thread? _searchThread;

        public static void StartLoop() {
            Console.WriteLine("_________________________________________________________________\n");
            Console.WriteLine("       Lxna engine by Lxdovic (https://github.com/Lxdovic)       ");
            Console.WriteLine("_________________________________________________________________\n");
            Console.WriteLine("> help for command list");

            while (true) {
                var command = Console.ReadLine();

                if (command == null) continue;

                var instructions = command.Split(" ");

                switch (instructions[0]) {
                    case "uci":
                        PrintUci();
                        break;
                    case "isready":
                        IsReady();
                        break;
                    case "ucinewgame":
                        ParsePosition("position startpos");
                        break;
                    case "go":
                        ParseGo(instructions.Skip(1).ToArray());
                        break;
                    case "stop":
                        StopSearch();
                        break;
                    case "position":
                        ParsePosition(command);
                        break;
                    case "help":
                        PrintCommands();
                        break;
                    case "quit": return;
                }
            }
        }

        private static void PrintCommands() {
            Console.WriteLine("_________________________________________________________________\n");
            Console.WriteLine("                         Lxna UCI Commands                       ");
            Console.WriteLine("_________________________________________________________________\n");
            Console.WriteLine("* uci                     print engine information");
            Console.WriteLine("* isready                 is the engine ready");
            Console.WriteLine("* ucinewgame              start a new game");
            Console.WriteLine("* position [fen <fenstring> | startpos] moves <move1> ... <movei>");
            Console.WriteLine("                          setup the position described nad play the moves on");
            Console.WriteLine("                          the internal chess board.");
            Console.WriteLine("* go                      start searching on the current position set up with");
            Console.WriteLine("                          the 'position' command");
            Console.WriteLine("    * depth <x>");
            Console.WriteLine("                          search x plies");
            Console.WriteLine("    * infinite");
            Console.WriteLine("                          search until the 'stop' command is executed.");
            Console.WriteLine("    * perft <x>");
            Console.WriteLine("                          executes a performance test x plies");
            Console.WriteLine("* stop                    stop calculating as soon as possible");
            Console.WriteLine("* quit                    exit the program");
        }

        public static void StopSearch() {
            Search.Stop();
            if (_searchThread != null) _searchThread.Interrupt();
        }

        public static void PrintUci() {
            Console.WriteLine("id name Lxna");
            Console.WriteLine("id author Lxdovic");
            Console.WriteLine("uciok");
        }

        public static void IsReady() {
            Console.WriteLine("readyok");
        }

        public static void SearchBlackTime(int time) {
            Search.TimeControl = true;
            Search.BlackTime = time;
            int bestMove = Search.Think(Engine.Board);

            Console.WriteLine("bestmove {0}{1}",
                ((Square)Move.GetMoveSource(bestMove)).ToString().ToLower(),
                ((Square)Move.GetMoveTarget(bestMove)).ToString().ToLower());
        }
        
        public static void SearchWhiteTime(int time) {
            Search.TimeControl = true;
            Search.WhiteTime = time;
            int bestMove = Search.Think(Engine.Board);

            Console.WriteLine("bestmove {0}{1}",
                ((Square)Move.GetMoveSource(bestMove)).ToString().ToLower(),
                ((Square)Move.GetMoveTarget(bestMove)).ToString().ToLower());
        }
        
        public static void SearchInfinite() {
            Search.TimeControl = false;
            int bestMove = Search.Think(Engine.Board);

            Console.WriteLine("bestmove {0}{1}",
                ((Square)Move.GetMoveSource(bestMove)).ToString().ToLower(),
                ((Square)Move.GetMoveTarget(bestMove)).ToString().ToLower());
        }

        public static void SearchDepth(int depth) {
            Search.TimeControl = false;
            int bestMove = Search.Think(Engine.Board, depth);

            Console.WriteLine("bestmove {0}{1}",
                ((Square)Move.GetMoveSource(bestMove)).ToString().ToLower(),
                ((Square)Move.GetMoveTarget(bestMove)).ToString().ToLower());
        }

        public static void SearchPerft(int depth) {
            Engine.PerfTest(depth);
        }

        public static void ParseGo(String[] instructions) {

            switch (instructions[0]) {
                case "depth": {
                    _searchThread = new Thread(() => SearchDepth(int.Parse(instructions[1])));
                    _searchThread.Start();
                    break;
                }

                case "btime": {
                    _searchThread = new Thread(() => SearchBlackTime(int.Parse(instructions[1])));
                    _searchThread.Start();
                    break;
                }

                case "wtime": {
                    _searchThread = new Thread(() => SearchWhiteTime(int.Parse(instructions[1])));
                    _searchThread.Start();
                    break;
                }

                case "infinite": {
                    _searchThread = new Thread(SearchInfinite);
                    _searchThread.Start();
                    break;
                }

                case "perft": {
                    _searchThread = new Thread(() => SearchPerft(int.Parse(instructions[1])));
                    _searchThread.Start();
                    break;
                }
            }
        }

        public static void ParsePosition(String command) {
            var instructions = command.Split(" ");
            var movesString = command.Split("moves ");
            var fenString = movesString[0].Split("fen ");

            if (instructions[1].Equals("startpos")) {
                Engine.Board = new Board(Engine.StartPos);
            }

            if (fenString.Length > 1) {
                Engine.Board = new Board(fenString[1]);
            }

            if (movesString.Length > 1) {
                String[] moves = movesString[1].Split(" ");

                foreach (var moveString in moves) {
                    Console.WriteLine(moveString);

                    int move = ParseMove(Engine.Board, moveString);

                    if (move == 0) continue;

                    Engine.Board.MakeMove(move);
                }
            }

            Engine.Board.Print();
        }

        public static int ParseMove(Board board, String move) {
            List<int> moves = board.GetPseudoLegalMoves();

            int source = move[0] - 'a' + (8 - (move[1] - '0')) * 8;
            int target = move[2] - 'a' + (8 - (move[3] - '0')) * 8;

            foreach (var currentMove in moves) {
                if (source == Move.GetMoveSource(currentMove) && target == Move.GetMoveTarget(currentMove)) {
                    int promotedPiece = Move.GetMovePromotion(currentMove);

                    if (promotedPiece > 0) {
                        if (((Piece)promotedPiece == Piece.WhiteQueen || (Piece)promotedPiece == Piece.BlackQueen) &&
                            move[4].Equals('q')) {
                            return currentMove;
                        }

                        if (((Piece)promotedPiece == Piece.WhiteRook || (Piece)promotedPiece == Piece.BlackRook) &&
                            move[4].Equals('r')) {
                            return currentMove;
                        }

                        if (((Piece)promotedPiece == Piece.WhiteBishop || (Piece)promotedPiece == Piece.BlackBishop) &&
                            move[4].Equals('b')) {
                            return currentMove;
                        }

                        if (((Piece)promotedPiece == Piece.WhiteKnight || (Piece)promotedPiece == Piece.BlackKnight) &&
                            move[4].Equals('n')) {
                            return currentMove;
                        }

                        continue;
                    }

                    return currentMove;
                }
            }

            return 0;
        }
    };
}