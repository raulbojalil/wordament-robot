namespace WordamentCheater.Sudoku
{
    public class SudokuSlot
    {
        public char Number { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsInitialValue { get; set; }
        public int NumberAsInt 
        { 
            get 
            {
                if (Number == ' ') return -1;

                int asInt;

                if (int.TryParse(Number.ToString(), out asInt))
                    return asInt;

                return -1;
            } 
        }
    }
}
