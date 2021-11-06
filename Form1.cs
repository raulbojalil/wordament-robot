using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WordamentCheater
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int ROWS = 4;
        private const int COLS = 4;

        public class BoardSlot
        {
            public string Letter { get; set; }
            public BoardSlot[] Neighbors { get; set; }
            public double MouseX { get; set; }
            public double MouseY { get; set; }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SelectionOverlay canvas = new SelectionOverlay())
            {
                if (canvas.ShowDialog() == DialogResult.OK)
                {
                    var board = InitBoard(canvas.GetRectangle());
                    var dictionary = ReadDictionary();
                    var solutionWords = GetSolutionWords(board, dictionary);

                    foreach (var word in solutionWords)
                    {
                        ExecuteWord(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, word.Value);
                    }
                }
            }
        }

        private BoardSlot GetBoardSlot(BoardSlot[,] boardSlot, int row, int col)
        {
            if (row < 0 || col < 0) return null;
            if (row >= ROWS) return null;
            if (col >= ROWS) return null;

            return boardSlot[row, col];
        }

        private string CheckWord(Dictionary<string, string> dict, BoardSlot slot, List<BoardSlot> usedSlots, string currentWord, Dictionary<string, BoardSlot[]> resultingWords)
        {
            
            currentWord += slot.Letter;

            if(currentWord.Length > 2 && dict.ContainsKey(currentWord))
            {
                if (!resultingWords.ContainsKey(currentWord))
                {
                    resultingWords.Add(currentWord, usedSlots.Append(slot).ToArray());
                }
            }

            foreach (var neighbor in slot.Neighbors)
            {
                if (!usedSlots.Contains(neighbor))
                {
                    CheckWord(dict, neighbor, usedSlots.Append(slot).ToList(), "" + currentWord, resultingWords);
                }
            }

            return null;
        }


        private Dictionary<string, string> ReadDictionary()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var word in File.ReadAllLines("spanish.txt"))
            {
                var key = word.ToUpper().Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U");

                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, word);
            }

            return dictionary;
        }

        private Dictionary<string, BoardSlot[]> GetSolutionWords(BoardSlot[,] board, Dictionary<string, string> dictionary)
        {
            
            var resultingWords = new Dictionary<string, BoardSlot[]>();

            for (var i = 0; i < ROWS; i++)
            {
                for (var j = 0; j < COLS; j++)
                {
                    CheckWord(dictionary, board[i, j], new List<BoardSlot>(), "", resultingWords);
                }
            }

            return resultingWords;
        }

        private void ExecuteWord(double screenWidth, double screenHeight, BoardSlot[] word)
        {
            tssLabel.Text = "Executing " + string.Join("", word.Select(x => x.Letter));

            var input = new WindowsInput.InputSimulator();

            input.Mouse.Sleep(2000);

            input.Mouse.MoveMouseTo(65535 * word[0].MouseX / screenWidth, 65535 * word[0].MouseY / screenHeight);
            input.Mouse.Sleep(100);
            input.Mouse.LeftButtonDown();
            input.Mouse.Sleep(100);

            for (var i=1; i < word.Length; i++)
            {
                input.Mouse.MoveMouseTo(65535 * word[i].MouseX / screenWidth, 65535 * word[i].MouseY / screenHeight);
                input.Mouse.Sleep(100);
            }

            input.Mouse.LeftButtonUp();
        }

        private BoardSlot[,] InitBoard(Rectangle gameArea)
        {
            var board = new BoardSlot[ROWS, COLS];
            var letters = new string[ROWS, COLS]
            {
                { slot11.Text, slot12.Text, slot13.Text, slot14.Text },
                { slot21.Text, slot22.Text, slot23.Text, slot24.Text },
                { slot31.Text, slot32.Text, slot33.Text, slot34.Text },
                { slot41.Text, slot42.Text, slot43.Text, slot44.Text },
            };

            var slotWidth = gameArea.Width / COLS;
            var slotHeight = gameArea.Height / ROWS;

            for (var i = 0; i < ROWS; i++)
            {
                for (var j = 0; j < COLS; j++)
                {
                    board[i, j] = new BoardSlot()
                    {
                        Letter = letters[i, j],
                        MouseX = gameArea.X + (slotWidth * j) + (slotWidth / 2),
                        MouseY = gameArea.Y + (slotHeight * i) + (slotHeight / 2)
                    };
                }
            }

            for (var i = 0; i < ROWS; i++)
            {
                for (var j = 0; j < COLS; j++)
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
        
        
    }
}
