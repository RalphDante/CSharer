using System;
using System.Windows.Forms;
using CSharer.Database;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private readonly BaseParameterRepository _titleRepo;
        private readonly BaseParameterRepository _hashtagRepo;
        private readonly BaseParameterRepository _descriptionRepo;

        public Form1()
        {
            InitializeComponent();

            try
            {
                _titleRepo = new BaseParameterRepository("title_parameters");
                _hashtagRepo = new BaseParameterRepository("hashtag_parameters");
                _descriptionRepo = new BaseParameterRepository("description_parameters");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadData()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Title";
            dataGridView1.Columns[1].Name = "Hashtag";
            dataGridView1.Columns[2].Name = "Description";

            var titles = _titleRepo.GetAll();
            var hashtags = _hashtagRepo.GetAll();
            var descriptions = _descriptionRepo.GetAll();

            int maxRows = Math.Max(
                titles.Count,
                Math.Max(hashtags.Count, descriptions.Count)
            );

            for (int i = 0; i < maxRows; i++)
            {
                dataGridView1.Rows.Add(
                    i < titles.Count ? titles[i].Content : "",
                    i < hashtags.Count ? hashtags[i].Content : "",
                    i < descriptions.Count ? descriptions[i].Content : ""
                );
            }
        }

        // BUTTON 1 = EDIT (Title)
        private void button1_Click(object sender, EventArgs e)
        {
        }

        // BUTTON 2 = ADD PARAMETER (Title)
       private void button2_Click(object sender, EventArgs e)
        {
        if (string.IsNullOrWhiteSpace(textBox1.Text))
        return;

        _titleRepo.Add(textBox1.Text);
        textBox1.Clear();
        LoadData();
        }

        // BUTTON 3 = EDIT (Hashtag)
        private void button3_Click(object sender, EventArgs e)
        {
        }

        // BUTTON 4 = ADD PARAMETER (Hashtag)
        private void button4_Click(object sender, EventArgs e)
        {
        if (string.IsNullOrWhiteSpace(textBox2.Text))
        return;

        _hashtagRepo.Add(textBox2.Text);
        textBox2.Clear();
        LoadData();
        }

        // BUTTON 5 = EDIT (Description)
        private void button5_Click(object sender, EventArgs e)
        {
        }

        // BUTTON 6 = ADD PARAMETER (Description)
        private void button6_Click(object sender, EventArgs e)
        {
        if (string.IsNullOrWhiteSpace(textBox3.Text))
        return;

        _descriptionRepo.Add(textBox3.Text);
        textBox3.Clear();
        LoadData();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}


