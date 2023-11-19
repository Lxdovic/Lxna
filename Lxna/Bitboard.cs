
using System.Runtime.CompilerServices;

namespace Lxna {
    public class BitboardHelper {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PopBitAtSquare(Square square, ref ulong value) {
            return GetBitAtSquare(square, value) > 0 ? value ^= 0x1UL << (int)square : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PopBitAtIndex(int index, ref ulong value) {
            return GetBitAtIndex(index, value) > 0 ? value ^= 0x1UL << index : 0;
        }
   
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetBitAtSquare(Square square, ulong value) {
            return value & (0x1UL << (int)square);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetBitAtIndex(int index, ulong value) {
            return value & (0x1UL << index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBitAtSquare(Square square, ref ulong value) {
            value |= 0x1UL << (int)square;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBitAtIndex(int index, ref ulong value) {
            value |= 0x1UL << index;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountBits(ulong value) {
            return System.Numerics.BitOperations.PopCount(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLSFBIndex(ulong value) {
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