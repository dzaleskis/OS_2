using System.Collections.Generic;
using Assembler;
using NUnit.Framework;
using OS_2.Concepts;

namespace OS_2.Tests.Assembler
{
    public class AssemblerTests
    {
        [Test]
        public void AssemblesProgramCorrectly()
        {
            var notAssembled = new NotAssembledProgram()
            {
                Variables = new List<StaticVariable>()
                {
                    new StaticVariable(){ Name = "var1", Value = 1 },
                    new StaticVariable(){ Name = "var2", Value = 500 },
                    new StaticVariable(){ Name = "var3", Value = 5500 },
                },
                Instructions = new List<NotAssembledInstruction>()
                {
                    new NotAssembledInstruction(){ Name = "ADD", Operand = null },
                    new NotAssembledInstruction(){ Name = "LOD", Operand = "var2" },
                    new NotAssembledInstruction(){ Name = "POP", Operand = null },
                    new NotAssembledInstruction(){ Name = "STO", Operand = "var3" },
                    new NotAssembledInstruction(){ Name = "SUB", Operand = "551" },
                }
            };

            var assembled = OSAssembler.Assemble(notAssembled);
            
            Assert.That(assembled.AssembledVariables.Count == notAssembled.Variables.Count);

            for(int i = 0; i < notAssembled.Variables.Count; i++)
            {
                Assert.That(assembled.AssembledVariables[i].Value == notAssembled.Variables[i].Value);
            }
            
            Assert.That(assembled.AssembledInstructions.Count == notAssembled.Instructions.Count);
            
            Assert.That(assembled.AssembledInstructions[0].Opcode == Opcode.ADD);
            Assert.That(assembled.AssembledInstructions[0].Operand == 0);
            
            Assert.That(assembled.AssembledInstructions[1].Opcode == Opcode.LOD);
            Assert.That(assembled.AssembledInstructions[1].Operand == 2);
            
            Assert.That(assembled.AssembledInstructions[2].Opcode == Opcode.POP);
            Assert.That(assembled.AssembledInstructions[2].Operand == 0);
            
            Assert.That(assembled.AssembledInstructions[3].Opcode == Opcode.STO);
            Assert.That(assembled.AssembledInstructions[3].Operand == 4);
            
            Assert.That(assembled.AssembledInstructions[4].Opcode == Opcode.SUB);
            Assert.That(assembled.AssembledInstructions[4].Operand == 0);
        }
    }
}