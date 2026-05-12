using System;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        // STORE CURRENT ROW
        private int currentRow = -1;

        public Form1()
        {
            InitializeComponent();
        }

        // FORM LOAD
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;

            // CREATE 3 COLUMNS
            dataGridView1.ColumnCount = 3;

            dataGridView1.Columns[0].Name = "Column 1";
            dataGridView1.Columns[1].Name = "Column 2";
            dataGridView1.Columns[2].Name = "Column 3";

            // SELECT CELL
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        // CREATE ROW IF NEEDED
        private void CreateRowIfNeeded()
        {
            if (currentRow == -1)
            {
                currentRow = dataGridView1.Rows.Add();
            }
            else
            {
                bool col1 = dataGridView1.Rows[currentRow].Cells[0].Value != null;
                bool col2 = dataGridView1.Rows[currentRow].Cells[1].Value != null;
                bool col3 = dataGridView1.Rows[currentRow].Cells[2].Value != null;

                // IF ALL COLUMNS HAVE DATA
                if (col1 && col2 && col3)
                {
                    currentRow = dataGridView1.Rows.Add();
                }
            }
        }

        // REMOVE ROW IF EMPTY
        private void RemoveRowIfEmpty(int rowIndex)
        {
            bool col1 = dataGridView1.Rows[rowIndex].Cells[0].Value == null;
            bool col2 = dataGridView1.Rows[rowIndex].Cells[1].Value == null;
            bool col3 = dataGridView1.Rows[rowIndex].Cells[2].Value == null;

            // IF ALL CELLS EMPTY
            if (col1 && col2 && col3)
            {
                dataGridView1.Rows.RemoveAt(rowIndex);

                currentRow = -1;
            }
        }

        // SAVE TEXTBOX1
        private void SaveTextBox1()
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                CreateRowIfNeeded();

                dataGridView1.Rows[currentRow].Cells[0].Value = textBox1.Text;

                textBox1.Clear();
            }
            else
            {
                MessageBox.Show("Textbox1 is empty.");
            }
        }

        // SAVE TEXTBOX2
        private void SaveTextBox2()
        {
            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                CreateRowIfNeeded();

                dataGridView1.Rows[currentRow].Cells[1].Value = textBox2.Text;

                textBox2.Clear();
            }
            else
            {
                MessageBox.Show("Textbox2 is empty.");
            }
        }

        // SAVE TEXTBOX3
        private void SaveTextBox3()
        {
            if (!string.IsNullOrWhiteSpace(textBox3.Text))
            {
                CreateRowIfNeeded();

                dataGridView1.Rows[currentRow].Cells[2].Value = textBox3.Text;

                textBox3.Clear();

                // RESET ROW
                currentRow = -1;
            }
            else
            {
                MessageBox.Show("Textbox3 is empty.");
            }
        }

        // DELETE COLUMN 1
        private void DeleteColumn1()
        {
            if (dataGridView1.CurrentCell != null)
            {
                if (dataGridView1.CurrentCell.ColumnIndex == 0)
                {
                    int rowIndex = dataGridView1.CurrentCell.RowIndex;

                    dataGridView1.Rows[rowIndex].Cells[0].Value = null;

                    RemoveRowIfEmpty(rowIndex);
                }
                else
                {
                    MessageBox.Show("Select Column 1 only.");
                }
            }
            else
            {
                MessageBox.Show("No cell selected.");
            }
        }

        // DELETE COLUMN 2
        private void DeleteColumn2()
        {
            if (dataGridView1.CurrentCell != null)
            {
                if (dataGridView1.CurrentCell.ColumnIndex == 1)
                {
                    int rowIndex = dataGridView1.CurrentCell.RowIndex;

                    dataGridView1.Rows[rowIndex].Cells[1].Value = null;

                    RemoveRowIfEmpty(rowIndex);
                }
                else
                {
                    MessageBox.Show("Select Column 2 only.");
                }
            }
            else
            {
                MessageBox.Show("No cell selected.");
            }
        }

        // DELETE COLUMN 3
        private void DeleteColumn3()
        {
            if (dataGridView1.CurrentCell != null)
            {
                if (dataGridView1.CurrentCell.ColumnIndex == 2)
                {
                    int rowIndex = dataGridView1.CurrentCell.RowIndex;

                    dataGridView1.Rows[rowIndex].Cells[2].Value = null;

                    RemoveRowIfEmpty(rowIndex);
                }
                else
                {
                    MessageBox.Show("Select Column 3 only.");
                }
            }
            else
            {
                MessageBox.Show("No cell selected.");
            }
        }

        // BUTTON 1 = DELETE COLUMN 1
        private void button1_Click(object sender, EventArgs e)
        {
            DeleteColumn1();
        }

        // BUTTON 2 = SAVE TEXTBOX1
        private void button2_Click(object sender, EventArgs e)
        {
            SaveTextBox1();
        }

        // BUTTON 3 = DELETE COLUMN 2
        private void button3_Click(object sender, EventArgs e)
        {
            DeleteColumn2();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveTextBox2();
        }
        // BUTTON 5 = DELETE COLUMN 3
        private void button5_Click(object sender, EventArgs e)
        {
            DeleteColumn3();
        }

        // BUTTON 6 = SAVE TEXTBOX3
        private void button6_Click(object sender, EventArgs e)
        {
            SaveTextBox3();
        }

        // TEXTBOX EVENTS
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        // DATAGRIDVIEW EVENT
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
    }
}