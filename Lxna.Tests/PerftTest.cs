using Xunit.Abstractions;

namespace Lxna.Tests;

public class PerftTest
{        
    private static readonly String _startPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 ";
    private static readonly String _kiwipete = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -";
    private static readonly String _pos3 = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ";
    private static readonly String _pos4 = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
    private static readonly String _pos5 = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8 ";
    private static readonly String _pos6 = "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10";
    
    [Fact]
    // https://www.chessprogramming.org/Perft_Results#Initial_Position
    public void InitialPosition()
    {
        Movegen.Init();
        Board board = new Board(_startPos);
        board.Print();
        
        ulong totalNodes = Perft.PerfTest(board, 5);
        
        Assert.Equal((ulong)4865609, totalNodes);
    }

    [Fact]
    // https://www.chessprogramming.org/Perft_Results#Position_2
    public void KiwipetePosition() {
        Movegen.Init();
        Board board = new Board(_kiwipete);
        board.Print();

        ulong totalNodes = Perft.PerfTest(board, 4);
        
        Assert.Equal((ulong)4085603, totalNodes);
    }
    
    [Fact]
    // https://www.chessprogramming.org/Perft_Results#Position_3
    public void Position3() {
        Movegen.Init();
        Board board = new Board(_pos3);
        board.Print();

        ulong totalNodes = Perft.PerfTest(board, 6);
        
        Assert.Equal((ulong)11030083, totalNodes);
    }
    
    [Fact]
    // https://www.chessprogramming.org/Perft_Results#Position_4
    public void Position4() {
        Movegen.Init();
        Board board = new Board(_pos4);
        board.Print();

        ulong totalNodes = Perft.PerfTest(board, 4);
        
        Assert.Equal((ulong)422333, totalNodes);
    }
    
    [Fact]
    // https://www.chessprogramming.org/Perft_Results#Position_5
    public void Position5() {
        Movegen.Init();
        Board board = new Board(_pos5);
        board.Print();

        ulong totalNodes = Perft.PerfTest(board, 4);
        
        Assert.Equal((ulong)2103487, totalNodes);
    }
    
    [Fact]
    // https://www.chessprogramming.org/Perft_Results#Position_6
    public void Position6() {
        Movegen.Init();
        Board board = new Board(_pos6);
        board.Print();
    
        ulong totalNodes = Perft.PerfTest(board, 4);
        
        Assert.Equal((ulong)3894594, totalNodes);
    }
}