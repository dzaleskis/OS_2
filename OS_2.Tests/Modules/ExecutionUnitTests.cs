using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Machines;
using OS_2.Modules;

namespace OS_2.Tests.Modules
{
    public class ExecutionUnitTests
    {
        public ExecutionUnit _executionUnit;

        public Mock<Func<int, int>> MemRead = new Mock<Func<int, int>>();
        public Mock<Action<int, int>> MemWrite  = new Mock<Action<int, int>>();
        public Mock<Func<int, int>> PortRead  = new Mock<Func<int, int>>();
        public Mock<Action<int, int>> PortWrite = new Mock<Action<int, int>>();
        public Mock<Func<int, int>> RegRead = new Mock<Func<int, int>>();
        public Mock<Action<int, int>> RegWrite = new Mock<Action<int, int>>();
        public Mock<Action<int>> PCWrite = new Mock<Action<int>>();
        public Mock<Func<int>> PCRead = new Mock<Func<int>>();
        
        
        [SetUp]
        public void Setup()
        {
            _executionUnit = new ExecutionUnit();
        }
        
        [TearDown]
        public void Teardown()
        {
            MemWrite.Reset();
            MemRead.Reset();
            PortRead.Reset();
            PortWrite.Reset();
            RegRead.Reset();
            RegWrite.Reset();
            PCWrite.Reset();
            PCRead.Reset();
        }

        private InstructionExecutionContext ConstructMockContext(Opcode opcode, int operand)
        {
            return new InstructionExecutionContext()
            {
                Opcode = opcode,
                Operand = operand,
                MemRead = MemRead.Object,
                MemWrite = MemWrite.Object,
                PortRead = PortRead.Object,
                PortWrite = PortWrite.Object,
                RegRead = RegRead.Object,
                RegWrite = RegWrite.Object,
                PCRead = PCRead.Object,
                PCWrite = PCWrite.Object
            };
        }

        [Test]
        public void DoesArithmeticCorrectly()
        {
            // let's simulate two items are on stack and SP points to the second one
            _executionUnit.SP = 2;
            
            var context = ConstructMockContext(Opcode.ADD, 0);
            MemRead.Setup(x => x(It.IsAny<int>())).Returns(10);
            
            _executionUnit.ExecuteInstruction(context);
            
            // should read from addresses 0 and 2
            MemRead.Verify(x => x(0), Times.Once);
            MemRead.Verify(x => x(2), Times.Once);
            
            // should write to address 0 with 10 + 10
            MemWrite.Verify(x => x(0, 20));
        }
        
        [Test]
        public void WorksWithJump()
        {
            var address = 534;
            var context = ConstructMockContext(Opcode.JMP, address);
            _executionUnit.ExecuteInstruction(context);
            
            PCWrite.Verify(x => x(address));
        }
        
        [Test]
        public void WorksWithConditionalJump()
        {
            MemRead.Setup(x => x(It.IsAny<int>())).Returns(100);
            var context = ConstructMockContext(Opcode.CMP, 0);
            _executionUnit.ExecuteInstruction(context);
            
            MemWrite.Verify(x => x(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            
            var address = 534;
            context = ConstructMockContext(Opcode.JE, address);
            _executionUnit.ExecuteInstruction(context);
            
            PCWrite.Verify(x => x(address), Times.Once);
        }

        [Test]
        public void WorksWithPortWrite()
        {
            var port = 52;
            var context = ConstructMockContext(Opcode.OUTB, port);
            MemRead.Setup(x => x(It.IsAny<int>())).Returns(25);
            _executionUnit.ExecuteInstruction(context);
            
            PortWrite.Verify(x => x(port, 25));
        }
        
        [Test]
        public void WorksWithPortRead()
        {
            var port = 52;
            var context = ConstructMockContext(Opcode.INB, port);
            PortRead.Setup(x => x(port)).Returns(25);
            _executionUnit.ExecuteInstruction(context);
            
            MemWrite.Verify(x => x(2, 25));
        }
        
        [Test]
        public void WorksWithRegWrite()
        {
            var register = RealRegister.CR0;
            var context = ConstructMockContext(Opcode.POPR, (int)register);
            MemRead.Setup(x => x(It.IsAny<int>())).Returns(1);
            _executionUnit.ExecuteInstruction(context);
            
            RegWrite.Verify(x => x((int)register, 1));
        }
        
        [Test]
        public void WorksWithRegRead()
        {
            var register = RealRegister.CR0;
            var regValue = 15;
            var context = ConstructMockContext(Opcode.PUSHR, (int)register);
            RegRead.Setup(x => x((int)register)).Returns(regValue);
            _executionUnit.ExecuteInstruction(context);
            
            MemWrite.Verify(x => x(2, regValue));
        }
        
    }
}