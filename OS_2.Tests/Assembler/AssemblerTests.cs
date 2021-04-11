using System;
using System.Collections.Generic;
using System.Text;
using Assembler;
using Newtonsoft.Json;
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
                Variables = new List<NotAssembledVariable>()
                {
                    new NotAssembledVariable("var1",  1),
                    new NotAssembledVariable("var2", 500),
                    new NotAssembledVariable("var3",5500),
                },
                Instructions = new List<NotAssembledInstruction>()
                {
                    new NotAssembledInstruction("ADD"),
                    new NotAssembledInstruction("LOD", "var2"),
                    new NotAssembledInstruction("POP"),
                    new NotAssembledInstruction("STO", "var3"),
                    new NotAssembledInstruction("SUB", "551"),
                }
            };

            var expected = new AssembledProgram()
            {
                AssembledInstructions = new List<AssembledInstruction>()
                {
                    new AssembledInstruction(Opcode.ADD),
                    new AssembledInstruction(Opcode.LOD, 2),
                    new AssembledInstruction(Opcode.POP),
                    new AssembledInstruction(Opcode.STO, 4),
                    new AssembledInstruction(Opcode.SUB)
                },
                AssembledVariables = new List<AssembledVariable>()
                {
                    new AssembledVariable(1),
                    new AssembledVariable(500),
                    new AssembledVariable(5500),
                }
            };

            var assembled = OSAssembler.Assemble(notAssembled);
            Assert.That(JsonConvert.SerializeObject(assembled.AssembledInstructions), Is.EqualTo(JsonConvert.SerializeObject(expected.AssembledInstructions)));
            Assert.That(JsonConvert.SerializeObject(assembled.AssembledVariables), Is.EqualTo(JsonConvert.SerializeObject(expected.AssembledVariables)));
        }

        [Test]
        public void SerializesProgramCorrectly()
        {
            var assembledProgram = new AssembledProgram()
            {
                AssembledInstructions = new List<AssembledInstruction>()
                {
                    new AssembledInstruction(Opcode.ADD),
                    new AssembledInstruction(Opcode.SUB),
                    new AssembledInstruction(Opcode.HALT)
                },
                AssembledVariables = new List<AssembledVariable>()
                {
                    new AssembledVariable(5)
                }
            };
            SerializedProgram serialized = SerializedProgram.FromAssembledProgram(assembledProgram);
            
            Assert.AreEqual(serialized.GetInstructionsAsBytes(), new byte[] {1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0});
            Assert.AreEqual(serialized.GetVariablesAsBytes(), new byte[] {5, 0});
        }
    }
}