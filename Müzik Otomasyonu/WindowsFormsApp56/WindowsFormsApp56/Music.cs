using MetroSet_UI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp56
{
    public partial class Music : MetroSetForm
    {
        public Music()
        {
            InitializeComponent();
        }
        MySqlConnection connection;
        string myConnectionString = "Server=localhost;Database=musicplayer;uid=root;pwd=2469";
        MySqlDataReader reader;

        private void Music_Load(object sender, EventArgs e)
        {
            label1.Visible = false;
            connection = new MySqlConnection(myConnectionString);
            kategoriListeleme();
            //vokalListeleme();
            veriListeleme();
        }

        /*public void vokalListeleme()
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("select * from authors", connection);
            //MySqlCommand cmd = new MySqlCommand("select author_name from muzikler", connection);
            reader = command.ExecuteReader();
            int rowcount = 0;
            while (reader.Read())
            {
                rowcount++;
                //cbAuthor.Items.Add(reader["author_name"].ToString());
            }
            connection.Close();
        }*/

        public void kategoriListeleme()
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("select * from cats", connection);
            //MySqlCommand cmd = new MySqlCommand("select music_cat from muzikler", connection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                cbCat.Items.Add(reader["cat_name"].ToString());
            }
            connection.Close();
        }

        public void veriListeleme()
        {
            connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("select musics.music_id, musics.music_played,musics.music_name, authors.author_name, musics.music_time,cats.cat_name, musics.music_link, musics.music_path from musics, authors, cats where musics.author_id=authors.author_id and musics.cat_id = cats.cat_id", connection);
            //MySqlDataAdapter adp = new MySqlDataAdapter("select * from muzikler", connection);
            
            DataTable table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            dataGridView1.Columns["music_id"].Visible = false;
            connection.Close();
        }

        public void veriEkleme()
        {
            MySqlDataReader myReader;
            MySqlCommand catekle = new MySqlCommand("insert ignore into cats (cat_name) values('" + cbCat.Text + "')", connection);
            MySqlCommand autekle = new MySqlCommand("insert ignore into authors(author_name) values ('" + txtVokal.Text + "')", connection);
            MySqlCommand catidal = new MySqlCommand("select cat_id from cats where cat_name = '" + cbCat.Text + "'", connection);
            MySqlCommand autidal = new MySqlCommand("select author_id from authors where author_name = '" + txtVokal.Text + "'", connection);
            connection.Open();
            catekle.ExecuteNonQuery();
            autekle.ExecuteNonQuery();
            
            myReader = catidal.ExecuteReader();
            myReader.Read();
            int kategori_id = (int)myReader["cat_id"];
            myReader.Close();

            reader = autidal.ExecuteReader();
            reader.Read();
            int vokal_id = (int)reader["author_id"];
            reader.Close();

            MySqlCommand command = new MySqlCommand("insert into musics(music_name, author_id,music_time, cat_id,music_link, music_path)" +
                "Values( '" + txtName.Text + "','" + vokal_id + "','" + txtTime.Text + "','" + kategori_id + "','" + txtLink.Text + "','" + txtPath.Text + "')", connection);

            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Kayıt başarıyla eklendi !", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Kayıt ekleme başarısız !", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            connection.Close();
            
        }


        public void veriSilme()
        {
            MySqlCommand command = new MySqlCommand("Delete from musics where music_id= '" + label1.Text + "'", connection);
            connection.Open();
            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Kayıt başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Kayıt silinemedi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        public void textTemizleme()
        {
            label1.Text = txtName.Text = txtVokal.Text = txtTime.Text = cbCat.Text = txtLink.Text = txtPath.Text = null;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            label1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtName.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            txtVokal.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            txtTime.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            cbCat.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            txtLink.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            txtPath.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            veriEkleme();
            veriListeleme();
            textTemizleme();
        }

        private void btnSil_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bu kayıtı silmek istediğinizden emin misiniz ?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                veriSilme();
                veriListeleme();
                textTemizleme();
            }
            else
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Media File(*.mpg,*.dat,*.avi,*.wmv,*.wav,*.mp3,*.mp4)|*.wav;*.mp3;*.mpg;*.dat;*.avi;*.wmv;*.mp4";
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Title = "Dosya Seç";

            try
            {
                if (dataGridView1.CurrentRow.Cells[7].Value.ToString() == string.Empty)
                {
                    DialogResult dialogResult = MessageBox.Show("Dosya yolu bulunamadı manuel olarak seçmek ister misiniz ?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        openFileDialog1.ShowDialog();
                        string FileName = openFileDialog1.FileName;
                        axWindowsMediaPlayer1.URL = FileName;
                        int oynatmaSayisi = int.Parse(dataGridView1.CurrentRow.Cells[7].Value.ToString());
                        oynatmaSayisi++;
                        MySqlCommand command = new MySqlCommand("update musics set music_played='" + oynatmaSayisi + "' where music_id='"+label1.Text+"'", connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        veriListeleme();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    axWindowsMediaPlayer1.URL = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                    int oynatmaSayisi = int.Parse(dataGridView1.CurrentRow.Cells[1].Value.ToString());
                    oynatmaSayisi++;
                    MySqlCommand command = new MySqlCommand("update musics set music_played='" + oynatmaSayisi + "'", connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    veriListeleme();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow.Cells[6].Value.ToString() == string.Empty)
                {
                    MessageBox.Show("Bu müziğin linki veritabanında kayıtlı değil!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    System.Diagnostics.Process.Start(dataGridView1.CurrentRow.Cells[6].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"muzik.pdf");
        }
    }
}
