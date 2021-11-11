using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WordamentCheater.Wordament
{
    public class WordamentBoard
    {
        private readonly int Rows;
        private readonly int Cols;

        public WordamentBoard(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
        }

        public Dictionary<string, BoardSlot[]> GetSolutionWords(Rectangle gameArea, string[,] letters, Dictionary<string, string> dictionary)
        {
            var board = InitBoard(gameArea, letters);

            var resultingWords = new Dictionary<string, BoardSlot[]>();

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    CheckWord(dictionary, board[i, j], new List<BoardSlot>(), "", resultingWords);
                }
            }

            return resultingWords;
        }

        private BoardSlot[,] InitBoard(Rectangle gameArea, string[,] letters)
        {
            var board = new BoardSlot[Rows, Cols];

            var slotWidth = gameArea.Width / Cols;
            var slotHeight = gameArea.Height / Rows;

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    board[i, j] = new BoardSlot()
                    {
                        Letter = letters[i, j],
                        X = gameArea.X + (slotWidth * j) + (slotWidth / 2),
                        Y = gameArea.Y + (slotHeight * i) + (slotHeight / 2)
                    };
                }
            }

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    board[i, j].Neighbors = new BoardSlot[]
                    {
                        GetBoardSlot(board, i, j+1),
                        GetBoardSlot(board, i, j-1),
                        GetBoardSlot(board, i+1, j),
                        GetBoardSlot(board, i+1, j+1),
                        GetBoardSlot(board, i+1, j-1),
                        GetBoardSlot(board, i-1, j),
                        GetBoardSlot(board, i-1, j+1),
                        GetBoardSlot(board, i-1, j-1),

                    }.Where(x => x != null).ToArray();
                }
            }

            return board;
        }

        private BoardSlot GetBoardSlot(BoardSlot[,] boardSlot, int row, int col)
        {
            if (row < 0 || col < 0) return null;
            if (row >= Rows) return null;
            if (col >= Cols) return null;

            return boardSlot[row, col];
        }

        private string CheckWord(Dictionary<string, string> dict, BoardSlot slot, List<BoardSlot> usedSlots, string currentWord, Dictionary<string, BoardSlot[]> resultingWords)
        {

            if (currentWord.Length > 2 && dict.ContainsKey(currentWord))
            {
                if (!resultingWords.ContainsKey(currentWord))
                {
                    resultingWords.Add(currentWord, usedSlots.ToArray());
                }
            }

            foreach (var neighbor in slot.Neighbors)
            {
                if (!usedSlots.Contains(neighbor))
                {
                    if (slot.Letter.Contains("/"))
                    {
                        CheckWord(dict, neighbor, usedSlots.Append(slot).ToList(), currentWord + slot.Letter.Split('/')[0], resultingWords);
                        CheckWord(dict, neighbor, usedSlots.Append(slot).ToList(), currentWord + slot.Letter.Split('/')[1], resultingWords);
                    }
                    else
                    {
                        CheckWord(dict, neighbor, usedSlots.Append(slot).ToList(), currentWord + slot.Letter, resultingWords);
                    }
                }
            }

            return null;
        }
    }
}
