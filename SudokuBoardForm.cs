using System;
using System.Windows.Forms;

namespace WordamentCheater
{
    public partial class SudokuBoardForm : Form
    {
        public char[,] Values { 
            get 
            { 
                return new char[9, 9] {
                    { GetSlotNumber(txtSlot11), GetSlotNumber(txtSlot12), GetSlotNumber(txtSlot13), GetSlotNumber(txtSlot14), GetSlotNumber(txtSlot15), GetSlotNumber(txtSlot16), GetSlotNumber(txtSlot17), GetSlotNumber(txtSlot18), GetSlotNumber(txtSlot19) },
                    { GetSlotNumber(txtSlot21), GetSlotNumber(txtSlot22), GetSlotNumber(txtSlot23), GetSlotNumber(txtSlot24), GetSlotNumber(txtSlot25), GetSlotNumber(txtSlot26), GetSlotNumber(txtSlot27), GetSlotNumber(txtSlot28), GetSlotNumber(txtSlot29) },
                    { GetSlotNumber(txtSlot31), GetSlotNumber(txtSlot32), GetSlotNumber(txtSlot33), GetSlotNumber(txtSlot34), GetSlotNumber(txtSlot35), GetSlotNumber(txtSlot36), GetSlotNumber(txtSlot37), GetSlotNumber(txtSlot38), GetSlotNumber(txtSlot39) },
                    { GetSlotNumber(txtSlot41), GetSlotNumber(txtSlot42), GetSlotNumber(txtSlot43), GetSlotNumber(txtSlot44), GetSlotNumber(txtSlot45), GetSlotNumber(txtSlot46), GetSlotNumber(txtSlot47), GetSlotNumber(txtSlot48), GetSlotNumber(txtSlot49) },
                    { GetSlotNumber(txtSlot51), GetSlotNumber(txtSlot52), GetSlotNumber(txtSlot53), GetSlotNumber(txtSlot54), GetSlotNumber(txtSlot55), GetSlotNumber(txtSlot56), GetSlotNumber(txtSlot57), GetSlotNumber(txtSlot58), GetSlotNumber(txtSlot59) },
                    { GetSlotNumber(txtSlot61), GetSlotNumber(txtSlot62), GetSlotNumber(txtSlot63), GetSlotNumber(txtSlot64), GetSlotNumber(txtSlot65), GetSlotNumber(txtSlot66), GetSlotNumber(txtSlot67), GetSlotNumber(txtSlot68), GetSlotNumber(txtSlot69) },
                    { GetSlotNumber(txtSlot71), GetSlotNumber(txtSlot72), GetSlotNumber(txtSlot73), GetSlotNumber(txtSlot74), GetSlotNumber(txtSlot75), GetSlotNumber(txtSlot76), GetSlotNumber(txtSlot77), GetSlotNumber(txtSlot78), GetSlotNumber(txtSlot79) },
                    { GetSlotNumber(txtSlot81), GetSlotNumber(txtSlot82), GetSlotNumber(txtSlot83), GetSlotNumber(txtSlot84), GetSlotNumber(txtSlot85), GetSlotNumber(txtSlot86), GetSlotNumber(txtSlot87), GetSlotNumber(txtSlot88), GetSlotNumber(txtSlot89) },
                    { GetSlotNumber(txtSlot91), GetSlotNumber(txtSlot92), GetSlotNumber(txtSlot93), GetSlotNumber(txtSlot94), GetSlotNumber(txtSlot95), GetSlotNumber(txtSlot96), GetSlotNumber(txtSlot97), GetSlotNumber(txtSlot98), GetSlotNumber(txtSlot99) },
                };
            } 
            set
            {
                var values = value;

                txtSlot11.Text = values[0, 0].ToString();
                txtSlot12.Text = values[0, 1].ToString();
                txtSlot13.Text = values[0, 2].ToString();
                txtSlot14.Text = values[0, 3].ToString();
                txtSlot15.Text = values[0, 4].ToString();
                txtSlot16.Text = values[0, 5].ToString();
                txtSlot17.Text = values[0, 6].ToString();
                txtSlot18.Text = values[0, 7].ToString();
                txtSlot19.Text = values[0, 8].ToString();

                txtSlot21.Text = values[1, 0].ToString();
                txtSlot22.Text = values[1, 1].ToString();
                txtSlot23.Text = values[1, 2].ToString();
                txtSlot24.Text = values[1, 3].ToString();
                txtSlot25.Text = values[1, 4].ToString();
                txtSlot26.Text = values[1, 5].ToString();
                txtSlot27.Text = values[1, 6].ToString();
                txtSlot28.Text = values[1, 7].ToString();
                txtSlot29.Text = values[1, 8].ToString();

                txtSlot31.Text = values[2, 0].ToString();
                txtSlot32.Text = values[2, 1].ToString();
                txtSlot33.Text = values[2, 2].ToString();
                txtSlot34.Text = values[2, 3].ToString();
                txtSlot35.Text = values[2, 4].ToString();
                txtSlot36.Text = values[2, 5].ToString();
                txtSlot37.Text = values[2, 6].ToString();
                txtSlot38.Text = values[2, 7].ToString();
                txtSlot39.Text = values[2, 8].ToString();

                txtSlot41.Text = values[3, 0].ToString();
                txtSlot42.Text = values[3, 1].ToString();
                txtSlot43.Text = values[3, 2].ToString();
                txtSlot44.Text = values[3, 3].ToString();
                txtSlot45.Text = values[3, 4].ToString();
                txtSlot46.Text = values[3, 5].ToString();
                txtSlot47.Text = values[3, 6].ToString();
                txtSlot48.Text = values[3, 7].ToString();
                txtSlot49.Text = values[3, 8].ToString();

                txtSlot51.Text = values[4, 0].ToString();
                txtSlot52.Text = values[4, 1].ToString();
                txtSlot53.Text = values[4, 2].ToString();
                txtSlot54.Text = values[4, 3].ToString();
                txtSlot55.Text = values[4, 4].ToString();
                txtSlot56.Text = values[4, 5].ToString();
                txtSlot57.Text = values[4, 6].ToString();
                txtSlot58.Text = values[4, 7].ToString();
                txtSlot59.Text = values[4, 8].ToString();

                txtSlot61.Text = values[5, 0].ToString();
                txtSlot62.Text = values[5, 1].ToString();
                txtSlot63.Text = values[5, 2].ToString();
                txtSlot64.Text = values[5, 3].ToString();
                txtSlot65.Text = values[5, 4].ToString();
                txtSlot66.Text = values[5, 5].ToString();
                txtSlot67.Text = values[5, 6].ToString();
                txtSlot68.Text = values[5, 7].ToString();
                txtSlot69.Text = values[5, 8].ToString();

                txtSlot71.Text = values[6, 0].ToString();
                txtSlot72.Text = values[6, 1].ToString();
                txtSlot73.Text = values[6, 2].ToString();
                txtSlot74.Text = values[6, 3].ToString();
                txtSlot75.Text = values[6, 4].ToString();
                txtSlot76.Text = values[6, 5].ToString();
                txtSlot77.Text = values[6, 6].ToString();
                txtSlot78.Text = values[6, 7].ToString();
                txtSlot79.Text = values[6, 8].ToString();

                txtSlot81.Text = values[7, 0].ToString();
                txtSlot82.Text = values[7, 1].ToString();
                txtSlot83.Text = values[7, 2].ToString();
                txtSlot84.Text = values[7, 3].ToString();
                txtSlot85.Text = values[7, 4].ToString();
                txtSlot86.Text = values[7, 5].ToString();
                txtSlot87.Text = values[7, 6].ToString();
                txtSlot88.Text = values[7, 7].ToString();
                txtSlot89.Text = values[7, 8].ToString();

                txtSlot91.Text = values[8, 0].ToString();
                txtSlot92.Text = values[8, 1].ToString();
                txtSlot93.Text = values[8, 2].ToString();
                txtSlot94.Text = values[8, 3].ToString();
                txtSlot95.Text = values[8, 4].ToString();
                txtSlot96.Text = values[8, 5].ToString();
                txtSlot97.Text = values[8, 6].ToString();
                txtSlot98.Text = values[8, 7].ToString();
                txtSlot99.Text = values[8, 8].ToString();
            } 
        }


        public SudokuBoardForm()
        {
            InitializeComponent();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private char GetSlotNumber(TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                return textBox.Text[0];
            }

            return ' ';
        }
    }
}
