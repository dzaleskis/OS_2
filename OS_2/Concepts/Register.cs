namespace OS_2.Concepts
{
    public enum RealRegister
    {
        CR0,
        CR1,
        BP,
        SP,
        PT,
        FLAGS,
        IDT
    }
    
    public enum ProtectedRegister
    {
        BP = RealRegister.BP,
        SP = RealRegister.SP,
        FLAGS = RealRegister.FLAGS,
    }
}