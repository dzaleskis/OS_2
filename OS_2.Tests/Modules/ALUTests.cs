using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Modules;

namespace OS_2.Tests.Modules
{
    public class ALUTests
    {
        private ALU alu;

        [SetUp]
        public void Setup()
        {
            alu = new ALU();
        }

        [Test]
        [TestCase(1, 4, 5)]
        [TestCase(1, -4, -3)]
        [TestCase(short.MinValue, -1, 32767)]
        [TestCase(short.MaxValue, 1, short.MinValue)]
        public void AddsSignedCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.ADD
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }

        [Test]
        [TestCase(1, 4, -3)]
        [TestCase(1, -4, 5)]
        [TestCase(short.MinValue, 1, 32767)]
        [TestCase(short.MaxValue, -1, short.MinValue)]
        public void SubtractsSignedCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.SUB
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }

        [Test]
        [TestCase(32, 4, 128)]
        [TestCase(55, 0, 0)]
        [TestCase(-1, 5, -5)]
        public void MultipliesSignedCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.IMUL
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }
        
        [Test]
        [TestCase(32, 4, 128)]
        [TestCase(55, 0, 0)]
        public void MultipliesUnsignedCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.MUL
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }
        
        [Test]
        [TestCase(32, 4, 8)]
        [TestCase(-10, -2, 5)]
        [TestCase(5, -1, -5)]
        [TestCase(-5, 1, -5)]
        public void DividesSignedCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.IDIV
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }
        
        [Test]
        [TestCase(32, 4, 8)]
        [TestCase(2000, 50, 40)]
        public void DividesUnsignedCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.DIV
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }
        
        [Test]
        [TestCase(5, 4, 4)]
        [TestCase(0, 5, 0)]
        public void ANDsCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.AND
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }
        
        [Test]
        [TestCase(0, 4, 4)]
        [TestCase(16, 4, 20)]
        public void ORsCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.OR
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }
        
        [Test]
        [TestCase(7, 4, 3)]
        [TestCase(0, 50, 50)]
        public void XORsCorrectly(int a, int b, int c)
        {
            var op = new BinaryInstruction()
            {
                A = a,
                B = b,
                Opcode = Opcode.XOR
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(c));
        }
        
        [Test]
        [TestCase(0, ushort.MaxValue)]
        [TestCase(ushort.MaxValue, 0)]
        public void NOTsCorrectly(int a, int b)
        {
            var op = new UnaryInstruction()
            {
                A = a,
                Opcode = Opcode.NOT
            };
            var result = alu.Process(op);
            Assert.That(result.Equals(b));
        }

        [Test]
        public void SetsZeroFlagCorrectly()
        {
            var op = new BinaryInstruction()
            {
                A = -5,
                B = 6,
                Opcode = Opcode.ADD
            };
            Assert.That(alu.Process(op) == 1);
            Assert.That(!alu.Flags.HasFlag(Flags.ZF));
            
            op = new BinaryInstruction()
            {
                A = -5,
                B = 5,
                Opcode = Opcode.ADD
            };
            Assert.That(alu.Process(op) == 0);
            Assert.That(alu.Flags.HasFlag(Flags.ZF));
        }
        
        [Test]
        public void SetsSignFlagCorrectly()
        {
            var op = new BinaryInstruction()
            {
                A = 5,
                B = 1,
                Opcode = Opcode.ADD
            };
            Assert.That(alu.Process(op) == 6);
            Assert.That(!alu.Flags.HasFlag(Flags.SF));
            
            op = new BinaryInstruction()
            {
                A = -5,
                B = 1,
                Opcode = Opcode.ADD
            };
            Assert.That(alu.Process(op) == -4);
            Assert.That(alu.Flags.HasFlag(Flags.SF));
        }
        
        [Test]
        public void SetsOverflowFlagCorrectly()
        {
            var op = new BinaryInstruction()
            {
                A = 5,
                B = 1,
                Opcode = Opcode.ADD
            };
            alu.Process(op);
            Assert.That(!alu.Flags.HasFlag(Flags.OF));
            
            op = new BinaryInstruction()
            {
                A = short.MaxValue,
                B = 1,
                Opcode = Opcode.ADD
            };
            alu.Process(op);
            Assert.That(alu.Flags.HasFlag(Flags.OF));
            
            op = new BinaryInstruction()
            {
                A = short.MinValue,
                B = -1,
                Opcode = Opcode.ADD
            };
            alu.Process(op);
            Assert.That(alu.Flags.HasFlag(Flags.OF));
            
            op = new BinaryInstruction()
            {
                A = short.MinValue,
                B = 1,
                Opcode = Opcode.SUB
            };
            alu.Process(op);
            Assert.That(alu.Flags.HasFlag(Flags.OF));
            
            op = new BinaryInstruction()
            {
                A = short.MaxValue,
                B = -1,
                Opcode = Opcode.SUB
            };
            alu.Process(op);
            Assert.That(alu.Flags.HasFlag(Flags.OF));
        }
        
        [Test]
        public void SetsCarryFlagCorrectly()
        {
            var op = new BinaryInstruction()
            {
                A = 5,
                B = 1,
                Opcode = Opcode.ADD
            };
            alu.Process(op);
            Assert.That(!alu.Flags.HasFlag(Flags.CF));
            
            op = new BinaryInstruction()
            {
                A = -1, // this is 0xFFFF in 2's complement
                B = 1,
                Opcode = Opcode.ADD
            };
            alu.Process(op);
            Assert.That(alu.Flags.HasFlag(Flags.CF));
            
            op = new BinaryInstruction()
            {
                A = 1,
                B = 2,
                Opcode = Opcode.SUB
            };
            alu.Process(op);
            Assert.That(alu.Flags.HasFlag(Flags.CF));
        }
    }
}