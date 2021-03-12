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
        [TestCase(short.MinValue)]
        [TestCase(short.MaxValue)]
        public void ParsesSignedOperandCorrectly1(short s)
        {
            var validOperandBytes = BitConverter.GetBytes(s);
            Assert.That(InstructionParser.ParseSignedOperand(validOperandBytes) == s);
        }

        [Test]
        [TestCase(ushort.MinValue)]
        [TestCase(ushort.MaxValue)]
        public void ParsesUnsignedOperandCorrectly1(ushort u)
        {
            var validOperandBytes = BitConverter.GetBytes(u);
            Assert.That(InstructionParser.ParseUnsignedOperand(validOperandBytes) == u);
        }
    }
}