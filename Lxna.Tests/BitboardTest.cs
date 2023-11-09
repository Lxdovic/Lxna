using Xunit.Abstractions;

namespace Lxna.Tests;

public class BitboardTest
{        
    [Fact]
    public void SetBit() {
        for (int i = 0; i < 64; i++) {
            ulong bitboard = 0x1UL << i;
            ulong testValue = 0x0;
            
            BitboardHelper.SetBitAtIndex(i, ref testValue);
            
            Assert.Equal(bitboard, testValue);
        }
    }    
    
    [Fact]
    public void GetBit() {
        for (int i = 0; i < 64; i++) {
            ulong bitboard = 0x1UL << i;
            bool isBit1Set = BitboardHelper.GetBitAtIndex(i, bitboard) > 0;
            bool isBit2Set = BitboardHelper.GetBitAtIndex(i + 1, bitboard) > 0;
            bool isBit3Set = BitboardHelper.GetBitAtIndex(i - 1, bitboard) > 0;
        
            Assert.True(isBit1Set);
            Assert.False(isBit2Set);
            Assert.False(isBit3Set);
        }
    }
    
    [Fact]
    public void PopBit() {
        ulong bitboard = ulong.MaxValue;
        
        for (int i = 0; i < 64; i++) {
            BitboardHelper.PopBitAtIndex(i, ref bitboard);
            
            int bitCount = BitboardHelper.CountBits(bitboard);
            
            Assert.Equal(bitCount, 63 - i);
        }
    }
    
    [Fact]
    public void GetLsfbIndex() {
        for (int i = 0; i < 64; i++) {
            ulong bitboard = 0x1UL << i;
            int leastSignificantBitIndex = BitboardHelper.GetLSFBIndex(bitboard);
            
            Assert.Equal(leastSignificantBitIndex, i);
        }
    }
    
    [Fact]
    public void CountBits() {
        ulong bitboard = 0x0;
        
        for (int i = 0; i < 64; i++) {
            bitboard |= 0x1UL << i;
            
            int bitCount = BitboardHelper.CountBits(bitboard);
            
            Assert.Equal(bitCount, i + 1);
        }
    }
}