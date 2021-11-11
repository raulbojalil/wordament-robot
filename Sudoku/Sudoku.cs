using System.Drawing;

namespace WordamentCheater.Sudoku
{
    public class Sudoku
    {
        public SudokuSlot[,] Solve(Rectangle gameArea, char[,] initialValues)
        {
            var board = InitBoard(gameArea, initialValues);
            Solve(board);
            return board;
        }

        private SudokuSlot[,] InitBoard(Rectangle gameArea, char[,] board)
        {
            var slots = new SudokuSlot[9, 9];

            var slotWidth = gameArea.Width / 9;
            var slotHeight = gameArea.Height / 9;

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    slots[i, j] = new SudokuSlot()
                    {
                        Number = board[i, j],
                        IsInitialValue = board[i,j] != ' ',
                        X = gameArea.X + (slotWidth * j) + (slotWidth / 2),
                        Y = gameArea.Y + (slotHeight * i) + (slotHeight / 2)
                    };
                }
            }

            return slots;
        }

        private bool Solve(SudokuSlot[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j].Number == ' ')
                    {
                        for (char c = '1'; c <= '9'; c++)
                        {
                            if (IsValid(board, i, j, c))
                            {
                                board[i, j].Number = c;

                                if (Solve(board))
                                    return true;
                                else
                                    board[i, j].Number = ' ';
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValid(SudokuSlot[,] board, int row, int col, char c)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i, col].Number != ' ' && board[i, col].Number == c)
                    return false;

                if (board[row, i].Number != ' ' && board[row, i].Number == c)
                    return false;

                if (board[3 * (row / 3) + i / 3, 3 * (col / 3) + i % 3].Number != ' ' && board[3 * (row / 3) + i / 3, 3 * (col / 3) + i % 3].Number == c)
                    return false;
            }
            return true;
        }
    }
    
}
