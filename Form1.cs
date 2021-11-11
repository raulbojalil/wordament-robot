﻿using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace WordamentCheater
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int BOARD_ROWS = 4;
        private const int BOARD_COLS = 4;

        private bool pendingStop;
        private IKeyboardMouseEvents globalHook;

        public class BoardSlot
        {
            public string Letter { get; set; }
            public BoardSlot[] Neighbors { get; set; }
            public double MouseX { get; set; }
            public double MouseY { get; set; }
        }

        #region Event Handlers

        private void Form1_Load(object sender, EventArgs e)
        {
            globalHook = Hook.GlobalEvents();
            globalHook.KeyPress += GlobalHookKeyPress;
            cmbLanguage.SelectedIndex = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            globalHook.KeyPress -= GlobalHookKeyPress;
            globalHook.Dispose();
        }

        private void btnOcr_Click(object sender, EventArgs e)
        {
            using (var overlay = new SelectionOverlay("Draw a rectangle around the game area to read the characters"))
            {
                var textBoxes = new List<TextBox>() {
                    slot11, slot12, slot13, slot14,
                    slot21, slot22, slot23, slot24,
                    slot31, slot32, slot33, slot34,
                    slot41, slot42, slot43, slot44,
                };

                var textBoxIndex = 0;

                if (overlay.ShowDialog() == DialogResult.OK)
                {
                    var gameArea = overlay.GetSelectionArea();
                    var slotWidth = gameArea.Width / BOARD_COLS;
                    var slotHeight = gameArea.Height / BOARD_ROWS;

                    for (var i = 0; i < BOARD_ROWS; i++)
                    {
                        for (var j = 0; j < BOARD_COLS; j++)
                        {
                            var slotScreenshot = TakeScreenshot(new Rectangle()
                            {
                                X = gameArea.X + (slotWidth * j),
                                Y = gameArea.Y + (slotHeight * i),
                                Height = slotHeight,
                                Width = slotWidth
                            });

                            textBoxes[textBoxIndex].Text = PerformOcr(slotScreenshot);
                            textBoxIndex++;
                        }
                    }

                }
            }
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 27)
            {
                pendingStop = true;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (var overlay = new SelectionOverlay("Draw a rectangle around the game area to start"))
            {
                if (overlay.ShowDialog() == DialogResult.OK)
                {
                    var language = cmbLanguage.Text;
                    btnStart.Enabled = false;
                    btnOcr.Enabled = false;
                    pendingStop = false;
                    tslStatus.Text = "Initializing board...";
                    

                    var task = new Task(() =>
                    {
                        var board = InitBoard(overlay.GetSelectionArea());
                        var dictionary = ReadDictionary($"{language.ToLower()}.txt");
                        var solutionWords = GetSolutionWords(board, dictionary);

                        var orderedWords = solutionWords.OrderByDescending(x => x.Key.Length);

                        BeginInvoke((Action)(() =>
                        {
                            txtSolutions.Lines = orderedWords.Select(x => x.Key).ToArray();
                        }));

                        foreach (var word in orderedWords)
                        {
                            if (pendingStop)
                            {
                                break;
                            }
                            ExecuteWord(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, word.Value);
                        }

                        BeginInvoke((Action)(() =>
                        {
                            tslStatus.Text = "Completed";
                            btnStart.Enabled = true;
                            btnOcr.Enabled = true;
                        }));
                    });
                    task.Start();
                }
            }
        }

        #endregion

        #region Private Methods

        private BoardSlot GetBoardSlot(BoardSlot[,] boardSlot, int row, int col)
        {
            if (row < 0 || col < 0) return null;
            if (row >= BOARD_ROWS) return null;
            if (col >= BOARD_ROWS) return null;

            return boardSlot[row, col];
        }

        private string CheckWord(Dictionary<string, string> dict, BoardSlot slot, List<BoardSlot> usedSlots, string currentWord, Dictionary<string, BoardSlot[]> resultingWords)
        {
           
            if(currentWord.Length > 2 && dict.ContainsKey(currentWord))
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

        private Dictionary<string, string> ReadDictionary(string dictionaryName)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var word in File.ReadAllLines(dictionaryName))
            {
                var key = word.ToUpper().Replace("Á", "A").Replace("É", "E").Replace("Í", "I")
                    .Replace("Ó", "O").Replace("Ú", "U").Replace("È", "E").Replace("Ê", "E")
                    .Replace("À", "A").Replace("Ï", "I");

                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, word);
            }

            return dictionary;
        }

        private Dictionary<string, BoardSlot[]> GetSolutionWords(BoardSlot[,] board, Dictionary<string, string> dictionary)
        {
            
            var resultingWords = new Dictionary<string, BoardSlot[]>();

            for (var i = 0; i < BOARD_ROWS; i++)
            {
                for (var j = 0; j < BOARD_COLS; j++)
                {
                    CheckWord(dictionary, board[i, j], new List<BoardSlot>(), "", resultingWords);
                }
            }

            return resultingWords;
        }

        private void ExecuteWord(double screenWidth, double screenHeight, BoardSlot[] word)
        {
            BeginInvoke((Action)(() =>
            {
                tslStatus.Text = "Executing " + string.Join("", word.Select(x => x.Letter));
            }));

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
            var board = new BoardSlot[BOARD_ROWS, BOARD_COLS];
            var letters = new string[BOARD_ROWS, BOARD_COLS]
            {
                { slot11.Text, slot12.Text, slot13.Text, slot14.Text },
                { slot21.Text, slot22.Text, slot23.Text, slot24.Text },
                { slot31.Text, slot32.Text, slot33.Text, slot34.Text },
                { slot41.Text, slot42.Text, slot43.Text, slot44.Text },
            };

            var slotWidth = gameArea.Width / BOARD_COLS;
            var slotHeight = gameArea.Height / BOARD_ROWS;

            for (var i = 0; i < BOARD_ROWS; i++)
            {
                for (var j = 0; j < BOARD_COLS; j++)
                {
                    board[i, j] = new BoardSlot()
                    {
                        Letter = letters[i, j],
                        MouseX = gameArea.X + (slotWidth * j) + (slotWidth / 2),
                        MouseY = gameArea.Y + (slotHeight * i) + (slotHeight / 2)
                    };
                }
            }

            for (var i = 0; i < BOARD_ROWS; i++)
            {
                for (var j = 0; j < BOARD_COLS; j++)
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

        private Bitmap TakeScreenshot(Rectangle rectangle) {
           
            using (Image image = new Bitmap(rectangle.Width, rectangle.Height))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.CopyFromScreen(new Point
                    (rectangle.Left, rectangle.Top), Point.Empty, rectangle.Size);
                }
                return new Bitmap(image);
            }
        }

        private byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private string PerformOcr(Bitmap slotScreenshot)
        {
            try
            {
                using (var engine = new TesseractEngine(@"C:/tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromMemory(ImageToByte(slotScreenshot)))
                    {
                        using (var page = engine.Process(img, PageSegMode.SingleChar))
                        {

                            using (var iterator = page.GetIterator())
                            {

                                iterator.Begin();
                                do
                                {
                                    string currentWord = iterator.GetText(PageIteratorLevel.Word);
                                    string result = Regex.Replace(
                                        currentWord.Replace("1","I").Replace("0","O").Replace("|", "I"), 
                                        @"[^a-zA-Z\-]", "").ToUpper();
                                    if (result.Length == 2) return result[0] + "/" + result[1];
                                    return result;

                                }
                                while (iterator.Next(PageIteratorLevel.Word));
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OCR Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return "";
        }

        #endregion

        private void textBoxSlot_Enter(object sender, EventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
