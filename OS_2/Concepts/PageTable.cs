namespace OS_2.Concepts
{
    public class PageTable
    {
        // default value of int is 0, so if address is not in table, we return 0
        private int[] pages = new int[128];
        
        public int this[int index]
        {
            get => pages[index];

            set => pages[index] = value;
        }
    }
}