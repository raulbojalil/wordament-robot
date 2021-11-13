using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using WordamentCheater.Sudoku;
using WordamentCheater.Wordament;

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

        private const int SUDOKU_ROWS = 9;
        private const int SUDOKU_COLS = 9;

        private bool pendingStop;
        private IKeyboardMouseEvents globalHook;
        private WindowsInput.InputSimulator input;


        #region Event Handlers

        private void Form1_Load(object sender, EventArgs e)
        {
            input = new WindowsInput.InputSimulator();
            globalHook = Hook.GlobalEvents();
            globalHook.KeyPress += GlobalHookKeyPress;
            cmbLanguage.SelectedIndex = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            globalHook.KeyPress -= GlobalHookKeyPress;
            globalHook.Dispose();
        }

        private void textBoxSlot_Enter(object sender, EventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void PrintSudoku(char[,] board)
        {
            for (var i = 0; i < SUDOKU_ROWS; i++)
            {
                for (var j = 0; j < SUDOKU_COLS; j++)
                {
                    Console.Write(board[i, j] + " ");
                }

                Console.WriteLine();
            }
        }

        private void PrintSolvedSudoku(SudokuSlot[,] board)
        {
            for (var i = 0; i < SUDOKU_ROWS; i++)
            {
                for (var j = 0; j < SUDOKU_COLS; j++)
                {
                    Console.Write(board[i, j].Number + " ");
                }

                Console.WriteLine();
            }
        }

        private Bitmap CropImage(Bitmap image, Rectangle rectangle, float leftPaddingPc = 0, float rightPaddingPc = 0, float topPaddingPc = 0, float bottomPaddingPc = 0)
        {
            int leftPadding = (int)((float)rectangle.Width * leftPaddingPc);
            var topPadding = (int)((float)rectangle.Height * topPaddingPc);
            int rightPadding = (int)((float)rectangle.Width * rightPaddingPc);
            var bottomPadding = (int)((float)rectangle.Height * bottomPaddingPc);

            var paddedRectangle = new Rectangle()
            {
                X = rectangle.X + leftPadding,
                Y = rectangle.Y + topPadding,
                Width = rectangle.Width - leftPadding - rightPadding,
                Height = rectangle.Height - topPadding - bottomPadding
            };

            var croppedImage = new Bitmap(paddedRectangle.Width, paddedRectangle.Height);
            using (var g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(image, -paddedRectangle.X, -paddedRectangle.Y);
                return croppedImage;
            }
        }

        private void btnSolveSudoku_Click(object sender, EventArgs e)
        {
            pendingStop = false;

            using (var gameAreaOverlay = new SelectionOverlay("Draw a rectangle around the game area to start"))
            {
                if (gameAreaOverlay.ShowDialog() == DialogResult.OK)
                {
                    var gameArea = gameAreaOverlay.GetSelectionArea();

                    using (var numberButtonsOverlay = new SelectionOverlay("Draw a rectangle around the number buttons"))
                    {
                        if (numberButtonsOverlay.ShowDialog() == DialogResult.OK)
                        {
                            var numberButtons = numberButtonsOverlay.GetSelectionArea();

                            var boardScreenshot = TakeScreenshot(gameArea);

                            var slotWidth = gameArea.Width / SUDOKU_COLS;
                            var slotHeight = gameArea.Height / SUDOKU_ROWS;

                            var board = new char[,]
                            {
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' }
                            };

                            for (var i = 0; i < SUDOKU_ROWS; i++)
                            {
                                for (var j = 0; j < SUDOKU_COLS; j++)
                                {
                                    var slotImage = CropImage(boardScreenshot, new Rectangle()
                                    {
                                        X = (slotWidth * j),
                                        Y = (slotHeight * i),
                                        Height = slotHeight,
                                        Width = slotWidth
                                    });

                                    var extractedNumber = PerformOcr(slotImage);
                                    extractedNumber = extractedNumber.ToUpper().Replace(" ", "").Replace("I", "1").Replace("O", "").Replace("0", "").Replace("Z", "2");

                                    extractedNumber = Regex.Replace(extractedNumber, @"[^0-9\-]", "");

                                    board[i, j] = extractedNumber.Length > 0 ? extractedNumber[0] : ' ';
                                }
                            }

                            using (var sudokuBoardForm = new SudokuBoardForm())
                            {
                                sudokuBoardForm.Values = board;

                                if (sudokuBoardForm.ShowDialog() == DialogResult.OK)
                                {
                                    var sudoku = new Sudoku.Sudoku();
                                    var solvedSudoku = sudoku.Solve(gameArea, sudokuBoardForm.Values);

                                    PrintSolvedSudoku(solvedSudoku);

                                    Thread.Sleep(3000);

                                    for (var i = 0; i < SUDOKU_ROWS; i++)
                                    {
                                        for (var j = 0; j < SUDOKU_COLS; j++)
                                        {
                                            if (pendingStop) return;

                                            InputNumber(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, solvedSudoku[i, j], numberButtons);
                                        }
                                    }
                                }
                            }  
                        }
                    }
                }
            }
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

                    var gameAreaScreenshot = TakeScreenshot(gameArea);

                    for (var i = 0; i < BOARD_ROWS; i++)
                    {
                        for (var j = 0; j < BOARD_COLS; j++)
                        {
                            var slotImage = CropImage(gameAreaScreenshot, new Rectangle()
                            {
                                X = slotWidth * j,
                                Y = slotHeight * i,
                                Height = slotHeight,
                                Width = slotWidth
                            }, 0.2f, 0.2f, 0.2f, 0.2f);

                            //slotScreenshot.Save($"{i}-{j}.png");

                            var slotText = PerformOcr(slotImage);

                            slotText = Regex.Replace(
                                        slotText.Replace("1", "I").Replace("0", "O").Replace("|", "I"),
                                        @"[^a-zA-Z]", "").ToUpper();

                            textBoxes[textBoxIndex].Text = slotText;
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

        private async void btnStart_Click(object sender, EventArgs e)
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

                    var letters = new string[BOARD_ROWS, BOARD_COLS]
                    {
                        { slot11.Text, slot12.Text, slot13.Text, slot14.Text },
                        { slot21.Text, slot22.Text, slot23.Text, slot24.Text },
                        { slot31.Text, slot32.Text, slot33.Text, slot34.Text },
                        { slot41.Text, slot42.Text, slot43.Text, slot44.Text },
                    };

                    var wordament = new WordamentBoard(BOARD_ROWS, BOARD_COLS);
                    
                    var task = new Task(() =>
                    {
                        var dictionary = ReadDictionary($"{language.ToLower()}.txt");

                        var solutionWords = wordament.GetSolutionWords(overlay.GetSelectionArea(), letters, dictionary);
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

        private void InputNumber(double screenWidth, double screenHeight, SudokuSlot slot, Rectangle inputNumbersArea)
        {
            if (slot.IsInitialValue) return;

            var inputNumberWidth = inputNumbersArea.Width / 9;
            var inputNumberHeight = inputNumbersArea.Height;

            

            var number = slot.NumberAsInt;

            if (number == -1) return;

            input.Mouse.Sleep(300);

            input.Mouse.MoveMouseTo(65535 * slot.X / screenWidth, 65535 * slot.Y / screenHeight);
            input.Mouse.Sleep(50);
            input.Mouse.LeftButtonClick();
            input.Mouse.Sleep(50);
            input.Mouse.MoveMouseTo(65535 * ((inputNumbersArea.X + (inputNumberWidth * (number - 1))) + (inputNumberWidth / 2)) / screenWidth, 65535 * (inputNumbersArea.Y + (inputNumberHeight / 2)) / screenHeight);
            input.Mouse.Sleep(50);
            input.Mouse.LeftButtonClick();
        }

        private void ExecuteWord(double screenWidth, double screenHeight, BoardSlot[] word)
        {
            BeginInvoke((Action)(() =>
            {
                tslStatus.Text = "Executing " + string.Join("", word.Select(x => x.Letter));
            }));

            input.Mouse.Sleep(2000);

            input.Mouse.MoveMouseTo(65535 * word[0].X / screenWidth, 65535 * word[0].Y / screenHeight);
            input.Mouse.Sleep(100);
            input.Mouse.LeftButtonDown();
            input.Mouse.Sleep(100);

            for (var i=1; i < word.Length; i++)
            {
                input.Mouse.MoveMouseTo(65535 * word[i].X / screenWidth, 65535 * word[i].Y / screenHeight);
                input.Mouse.Sleep(100);
            }

            input.Mouse.LeftButtonUp();
        }

        private Bitmap TakeScreenshot(Rectangle rectangle, float leftPaddingPc = 0, float rightPaddingPc = 0, float topPaddingPc = 0, float bottomPaddingPc = 0) {

            int leftPadding = (int)((float)rectangle.Width * leftPaddingPc);
            var topPadding = (int)((float)rectangle.Height * topPaddingPc);
            int rightPadding = (int)((float)rectangle.Width * rightPaddingPc);
            var bottomPadding = (int)((float)rectangle.Height * bottomPaddingPc);

            var screenshotRectangle = new Rectangle() { 
                X = rectangle.X + leftPadding,
                Y = rectangle.Y + topPadding,
                Width = rectangle.Width - leftPadding - rightPadding,
                Height = rectangle.Height - topPadding - bottomPadding
            };

            using (Image image = new Bitmap(screenshotRectangle.Width, screenshotRectangle.Height))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.CopyFromScreen(new Point
                    (screenshotRectangle.Left, screenshotRectangle.Top), Point.Empty, screenshotRectangle.Size);
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
                using (var engine = new TesseractEngine(@"C:/tessdata", "deu", EngineMode.Default))
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

                                    if (currentWord == null) return "";

                                    return currentWord.Trim();

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
    }
}
