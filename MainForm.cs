/*
 * MainForm.cs
 * Assignment 1
 * Section 2
 * Group 42 (Lena Brokaw, Ana Isakov)
*/

// Imports
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace _42Assignment1
{
    public partial class MainForm : Form
    {
        // Global variables
        public bool win = false;
        const string X_WIN = "xxx"; // pattern needed for x to win
        const string O_WIN = "ooo"; // pattern needed for o to win
        public int player = 0; // variable to control whose turn it is (0 is O, 1 is X)
        public char[] output = { '.', '.', '.' }; // array to hold row, column, symbol of picturebox square
        // array to hold contents of 3 by 3 tic-tac-toe grid
        public char[,] grid = {
            {'0','0','0'},
            {'0','0','0'},
            {'0','0','0'}
        };

        /// <summary>
        /// Method to check for a horizontal win (loops through grid by its rows).
        /// </summary>
        public void RowCheck()
        {
            string horizontal = "";
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    horizontal += grid[i, j]; // loops by row
                }
                // check against x pattern and o pattern
                if (horizontal == X_WIN || horizontal== O_WIN)
                {
                    win = true; // set win to true
                }
                horizontal = ""; // reset horizontal check to empty after each row
            }
        }
        /// <summary>
        /// Method to check for a vertical win (loops through grid by its columns).
        /// </summary>
        public void ColumnCheck() {
            string vertical = "";
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    vertical += grid[j, i]; // loops by column
                }
                // check against x pattern and o pattern
                if (vertical == X_WIN || vertical == O_WIN)
                { 
                    win = true; // set win to true
                }
                vertical = ""; // reset vertical check to empty after each column
            }
        }
        /// <summary>
        /// Method to check for a diagonal win (loops through only one dimension of the grid).
        /// </summary>
        public void DiagonalCheck() {
            string diagonal1 = ""; // diagonal starting from top left
            string diagonal2 = ""; // diagonal starting from top right
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                diagonal1 += grid[i, i]; // checks [0, 0], [1, 1], [2, 2]
                if (diagonal1 == X_WIN || diagonal1 == O_WIN)
                {
                    win = true;         
                    break;
                }
                diagonal2 += grid[i, grid.GetLength(1) - 1 - i]; // checks [0, 2], [1, 1], [2, 0]
                if (diagonal2 == X_WIN || diagonal2 == O_WIN)
                {
                    win = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Method to change whose turn it is (player X or O).
        /// </summary>
        public void PlayerChange()
        {
            if (player == 0) { 
                player = 1; 
            }
            else if (player == 1) {
                player = 0;
            }
        }

        /// <summary>
        /// Method that allows the player to complete a turn by extracting the row and column 
        /// numbers of the move, along with the player's symbol, and adding it to the 3 by 3 grid
        /// </summary>
        /// <param name="output"></param>
        public void CellInput(char[] output)
        {
            int row = int.Parse(output[0].ToString());
            int col = int.Parse(output[1].ToString());
            char symbol = output[2];
            grid[row, col] = symbol; // adds the player's symbol to the grid
        }

        /// <summary>
        /// Clears and resets all controls and variables related to the game. User can then restart.
        /// </summary>
        public void ClearAll()
        {
            Task.Delay(2500).Wait(); // wait for a few seconds (2.5) before resetting the game so the
                                     // user has time to read the output message
            // loop through all pictureboxes
            foreach (var square in Controls.OfType<PictureBox>())
            {
                square.Image = null; // set each image to null ("none")
                square.Enabled = true; // enable all pictureboxes
            }
            // reset variables
            win = false;
            player = 0;

            lblMessage.Text = "Game Reset"; // let user know that we've reset

            GridReset(); // reset grid
        }

        /// <summary>
        /// Method to reset the grid array to empty
        /// </summary>
        public void GridReset()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = '0'; // sets each element to 0, rather than O or X
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        // Event handler for all pictureboxes
        private void Square_Click(object sender, EventArgs e)
        {
            //extracts the picturebox name and co-ordinates from the sender
            PictureBox picBox = (PictureBox)sender;
            string picBoxName = picBox.Name;
            char x_coord = picBoxName[picBoxName.Length-1]; 
            char y_coord = picBoxName[picBoxName.Length-2];

            //Sets the image of the picturebox that was clicked, dependant on the player
            if (player == 0) { picBox.Image = _42Assignment1.Properties.Resources.letter_o; }
            else { picBox.Image = _42Assignment1.Properties.Resources.letter_x; }

            //disables the control, ensuring the square cannot be stolen by the other player
            picBox.Enabled = false;

            //sends the coordinates of the square and the player symbol to an array
            output[0] = y_coord;
            output[1] = x_coord;
            if (player == 0)
            { 
                output[2] = 'o';
            }
            else 
            { 
                output[2] = 'x';
            }

            //adds x or o to the array at the coordinate of the clicked square
            CellInput(output);

            // -- Check Logic --- 
            RowCheck(); // horizontal win
            ColumnCheck(); // vertical win
            DiagonalCheck(); // diagonal win

            // loop to check if all squares on the grid are full
            bool isFull = true;
            foreach (var square in Controls.OfType<PictureBox>())
            {
                if (square.Image == null) // if any of them don't have a picture
                { 
                    isFull = false; // the grid is not full
                }
            }

            // --- GAME END LOGIC ---

            if (win) // check for a win
            { 
                // Code to get player name
                string playerName;
                if (player == 0) 
                { 
                    playerName = "O";
                }
                else 
                { 
                    playerName = "X";
                }

                // output winner!
                lblMessage.Text = $"Player {playerName} wins!";
                
                ClearAll(); // reset the game
                return; // exit event handler
            }
            else if (isFull) // check if all spaces are full (tie)
            {
                // if no one has won, and all spaces are full, there is a tie
                // output tie status
                lblMessage.Text = "It's a tie!";
                ClearAll(); // reset the game
                return; // exit event handler
            }

            // if neither of these conditions were satisfied, keep playing
            
            // changes the player, 0 to 1 and vice versa
            PlayerChange();
        }

        // Changes form to light mode
        private void btnLightMode_Click(object sender, EventArgs e)
        {
            MainForm.ActiveForm.BackColor = Color.White;
            lblTitle.ForeColor = Color.Black;
            lblMessage.ForeColor = Color.DarkBlue;
        }
        // Changes form to dark mode
        private void btnDarkMode_Click(object sender, EventArgs e)
        {
            MainForm.ActiveForm.BackColor = Color.Black;
            lblTitle.ForeColor = Color.White;
            lblMessage.ForeColor = Color.FromArgb(128, 255, 255);
        }
        // Exit the form
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
