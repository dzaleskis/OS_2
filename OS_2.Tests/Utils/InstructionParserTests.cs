using System;
using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Utils;

namespace OS_2.Tests.Utils
{
    public class InstructionParserTests
    {

        [Test]
        public void ThrowsOnInvalidOpcodeLength()
        {
            var invalidOpcodeLength = new byte[] {0, 0, 0};
            Assert.Throws<ArgumentException>(() => InstructionParser.ParseOpcode(invalidOpcodeLength));
        }
        
        [Test]
        public void ThrowsOnInvalidOpcode()
        {
            var invalidOpcode = new byte[] {byte.MaxValue, 0};
            Assert.Throws<ArgumentException>(() => InstructionParser.ParseOpcode(invalidOpcode));
        }
        
        [Test]
        public void ParsesOpcodeCorrectly()
        {
            var validOpcodeBytes = BitConverter.GetBytes((short) Opcode.ADD);
            Assert.That(InstructionParser.ParseOpcode(validOpcodeBytes) == Opcode.ADD);
        }
        
        
        [Test]
        public void ThrowsOnInvalidOperandLength()
        {
            var invalidOperandLength = new byte[] {0, 0, 0};
            Assert.Throws<ArgumentException>(() => InstructionParser.ParseSignedOperand(invalidOperandLength));
            Assert.Throws<ArgumentException>(() => InstructionParser.ParseUnsignedOperand(invalidOperandLength));
        }

        [Test]
        public void ParsesSignedOperandCorrectly1()
        {
            var validOperandBytes = BitConverter.GetBytes(short.MaxValue);
            Assert.That(InstructionParser.ParseSignedOperand(validOperandBytes).Value == short.MaxValue);
        }
        
        [Test]
        public void ParsesSignedOperandCorrectly2()
        {
            var validOperandBytes = BitConverter.GetBytes(short.MinValue);
            Assert.That(InstructionParser.ParseSignedOperand(validOperandBytes).Value == short.MinValue);
        }
        
        [Test]
        public void ParsesUnsignedOperandCorrectly1()
        {
            var validOperandBytes = BitConverter.GetBytes(ushort.MaxValue);
            Assert.That(InstructionParser.ParseUnsignedOperand(validOperandBytes).Value == ushort.MaxValue);
        }
        
        [Test]
        public void ParsesUnsignedOperandCorrectly2()
        {
            var validOperandBytes = BitConverter.GetBytes(ushort.MinValue);
            Assert.That(InstructionParser.ParseUnsignedOperand(validOperandBytes).Value == ushort.MinValue);
        }
    }
}