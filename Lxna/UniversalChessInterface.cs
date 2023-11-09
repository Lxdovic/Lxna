namespace Lxna {
    public static class UniversalChessInterface {
        private static Thread? _searchThread;
        private static bool _shouldQuit;

        public static void StartLoop(bool fastInit) {
            // let JIT run for a bit
            // https://stackoverflow.com/a/28950600
            if (!fastInit) Search.Think(Engine.Board, false, 0, 8, false);
            
            Console.WriteLine("_________________________________________________________________\n");
            Console.WriteLine("       Lxna engine by Lxdovic (https://github.com/Lxdovic)       ");
            Console.WriteLine("_________________________________________________________________\n");
            Console.WriteLine("> help for command list");

            while (!_shouldQuit) {
                var command = Console.ReadLine();

                if (command == null) continue;

                HandleCommand(command);
            }
        }

        public static void HandleCommand(String command) {
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
                case "move":
                    ParseMakeMove(instructions.Skip(1).ToArray());
                    break;
                case "fen":
                    PrintFen();
                    break;
                case "quit":
                    _shouldQuit = true;
                    return;
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
            Console.WriteLine("    * wtime <x>");
            Console.WriteLine("                          search as white with x milliseconds left on the timer");
            Console.WriteLine("    * btime <x>");
            Console.WriteLine("                          search as black with x milliseconds left on the timer");
            Console.WriteLine("    * infinite");
            Console.WriteLine("                          search until the 'stop' command is executed.");
            Console.WriteLine("    * perft <x>");
            Console.WriteLine("                          executes a performance test x plies");
            Console.WriteLine("* move <move>             make a move using he from and to square (eg. e2e4)");
            Console.WriteLine("* stop                    stop calculating as soon as possible");
            Console.WriteLine("* fen                     print the current fen");
            Console.WriteLine("* quit                    exit the program");
        }

        public static void StopSearch() {
            Search.Stop();
            if (_searchThread != null) _searchThread.Interrupt();
        }
        
        public static void PrintFen() {
            String fen = Engine.Board.GetFen();
            
            Console.WriteLine(fen);
        }

        public static void ParseMakeMove(String[] instructions) {
            int move = ParseMove(Engine.Board, instructions[0]);
            
            if (!Engine.Board.MakeMove(move)) Console.WriteLine("illegal move.");
            else Engine.Board.Print();
        }

        public static void PrintUci() {
            Console.WriteLine("id name Lxna");
            Console.WriteLine("id author Lxdovic");
            Console.WriteLine("uciok");
        }

        public static void IsReady() {
            Console.WriteLine("readyok");
        }

        public static void ParseGo(String[] instructions) {
            switch (instructions[0]) {
                case "depth": {
                    // time set to 0 because timeControl = false
                    _searchThread = new Thread(() => Search.Think(Engine.Board, false, 0, int.Parse(instructions[1])));
                    _searchThread.Start();
                    break;
                }

                case "btime": case "wtime": {
                    _searchThread = new Thread(() => Search.Think(Engine.Board, true,int.Parse(instructions[1])));
                    _searchThread.Start();
                    break;
                }

                case "infinite": {
                    _searchThread = new Thread(() => Search.Think(Engine.Board, false));
                    _searchThread.Start();
                    break;
                }

                case "perft": {
                    _searchThread = new Thread(() => Search.Think(Engine.Board, false));
                    _searchThread = new Thread(() => Perft.PerfTest(Engine.Board, int.Parse(instructions[1])));
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

        public static int ParseMove(Board board, int source, int target) {
            List<int> moves = board.GetPseudoLegalMoves();

            foreach (var currentMove in moves) {
                if (source == Move.GetMoveSource(currentMove) && target == Move.GetMoveTarget(currentMove)) {
                    return currentMove;
                }
            }

            return 0;
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
