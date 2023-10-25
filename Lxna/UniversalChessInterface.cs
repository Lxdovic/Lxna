
namespace Lxna {
    internal class UniversalChessInterface {
        public static bool TimeControl = true;
        public static int AllowedTime = 1000;
        public static void StartLoop() {
            Console.WriteLine("Lxna engine by Lxdovic (https://github.com/Lxdovic)");
            Console.WriteLine("UCI Protocol documentation available -> https://gist.github.com/Lxdovic/20f3d65d8c3459bb20ea3ce8d595ec4b\n");

            while (true) {
                var command = Console.ReadLine();

                if (command == null) continue;
                
                var instructions = command.Split(" ");

                switch (instructions[0]) {
                    case "isready": IsReady(); break;
                    case "ucinewgame": ParsePosition("position startpos"); break;
                    case "go": ParseGo(command); break;
                    case "position": ParsePosition(command); break;
                    case "uci": PrintUCI(); break;
                    case "quit": return;
                }
            };
        }

        public static void PrintUCI() {
            Console.WriteLine("id name Lxna");
            Console.WriteLine("id author Lxdovic");
            Console.WriteLine("uciok");
        }

        public static void IsReady() { Console.WriteLine("readyok"); }
        public static void ParseGo(String command) {
            var depthString = command.Split("depth ");
            var perftString = command.Split("perft ");

            if (depthString.Length > 1) {
                Engine.board.Print();
                TimeControl = false;
                Search.Think(Engine.board, int.Parse(depthString[1]));
            }
            else if (perftString.Length > 1) {
                Engine.PerfTest(int.Parse(perftString[1]));
            }
        }

        public static void ParsePosition(String command) {
            var instructions = command.Split(" ");
            var movesString = command.Split("moves ");
            var fenString = movesString[0].Split("fen ");
            
            if (instructions[1].Equals("startpos")) {
                Engine.board = new Board(Engine.START_POS);
            }
            
            if (fenString.Length > 1) {
                Engine.board = new Board(fenString[1]);
            }
            
            if (movesString.Length > 1) {
                String[] moves = movesString[1].Split(" ");

                foreach (var moveString in moves) {
                    Console.WriteLine(moveString);

                    int move = ParseMove(Engine.board, moveString);
                    
                    if (move == 0) continue;
                    
                    Engine.board.MakeMove(move);
                }
            }
            
            Engine.board.Print();
          
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
    }
}
