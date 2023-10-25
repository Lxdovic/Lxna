
namespace Lxna {
    internal class BitboardHelper {
        public ulong PopBitAtSquare(Square square, ref ulong value) {
            return GetBitAtSquare(square, value) > 0 ? value ^= (ulong)0x1 << (int)square : 0;
        }

        public static ulong PopBitAtIndex(int index, ref ulong value) {
            return GetBitAtIndex(index, value) > 0 ? value ^= (ulong)0x1 << index : 0;
        }
   
        public static ulong GetBitAtSquare(Square square, ulong value) {
            return value & ((ulong)0x1 << (int)square);
        }

        public static ulong GetBitAtIndex(int index, ulong value) {
            return value & ((ulong)0x1 << index);
        }

        public static void SetBitAtSquare(Square square, ref ulong value) {
            value |= (ulong)0x1 << (int)square;
        }

        public static void SetBitAtIndex(int index, ref ulong value) {
            value |= (ulong)0x1 << index;
        }
        
        public static int CountBits(ulong value) {
            // return System.Runtime.Intrinsics.X86.Popcnt.PopCount(Value);
            return System.Numerics.BitOperations.PopCount(value);
            
        }

        // Least Significant First Bit
        public static int GetLSFBIndex(ulong value) {
            // return System.Runtime.Intrinsics.X86.Bmi1.X64.TrailingZeroCount(Value);
            return System.Numerics.BitOperations.TrailingZeroCount(value);
        }

        public static void Print(ulong value) {
            for (var square = 0; square < 64; square++) {
                var rank = square / 8;
                var file = square % 8;

                var bit = GetBitAtIndex(square, value);

                if (file == 0) {
                    Console.ResetColor();
                    Console.Write("\n{0,2} ", 8 - rank);
                }

                Console.BackgroundColor = bit > 0 ? ConsoleColor.Cyan : ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.Write("{0,2} ", bit > 0 ? 1 : 0);
            }

            Console.ResetColor();
            Console.WriteLine("\n    a  b  c  d  e  f  g  h");
            Console.WriteLine("\nbitboard: {0, 17}", value);
        }
    }
}