﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LKS_Vaksin
{
    public partial class MasterDokter : Form
    {
        int id, cond;
        SqlConnection connection = new SqlConnection(Utils.conn);

        public MasterDokter()
        {
            InitializeComponent();
            lblname.Text = Session.nama;
            lbltime.Text = DateTime.Now.ToString("dddd, dd-MM-yyyy / HH:mm:ss");

            dis();
            loadgrid();
        }

        void loadgrid()
        {
            string com = "select * from admin where level = 2";
            dataGridView1.DataSource = Command.getdata(com);
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[4].Visible = false;
        }

        void dis()
        {
            btncancel.Enabled = false;
            btnsave.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox4.Enabled = false;
            btn_tambah.Enabled = true;
            btn_edit.Enabled = true;
            btnhapus.Enabled = true;
        }
        void enable()
        {
            btncancel.Enabled = true;
            btnsave.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox4.Enabled = true;
            btn_tambah.Enabled = false;
            btn_edit.Enabled = false;
            btnhapus.Enabled = false;
        }

        bool val()
        {
            if (textBox1.TextLength < 1 || textBox2.TextLength < 1 || textBox3.TextLength < 1 || textBox3.TextLength < 1 || textBox4.TextLength < 1)
            {
                MessageBox.Show("Semua field harus diisi!", "Terjadi Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (textBox3.TextLength < 8 || textBox4.TextLength < 8)
            {
                MessageBox.Show("Password Minimal 8!", "Terjadi Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (textBox4.Text != textBox3.Text)
            {
                MessageBox.Show("Konfirmasi Password Tidak Tepat!", "Terjadi Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            SqlCommand command = new SqlCommand("select * from admin where username = '" + textBox2.Text + "'", connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                connection.Close();
                MessageBox.Show("Username telah digunakan!", "Terjadi Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            connection.Close();

            return true;
        }

        bool val_up()
        {
            if (textBox1.TextLength < 1 || textBox2.TextLength < 1)
            {
                MessageBox.Show("Semua field harus diisi!", "Terjadi Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            SqlCommand command = new SqlCommand("select * from admin where username = '" + textBox2.Text + "'", connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                if (Convert.ToInt32(reader["id"]) != id)
                {
                    connection.Close();
                    MessageBox.Show("Username telah digunakan!", "Terjadi Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            connection.Close();

            return true;
        }

        private void textboxsearch_TextChanged(object sender, EventArgs e)
        {
            string com = "select * from admin nama where nama like '%" + textboxsearch.Text + "%' and level = 2 ";
            dataGridView1.DataSource = Command.getdata(com);
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[4].Visible = false;
        }

        private void btn_tambah_Click(object sender, EventArgs e)
        {
            cond = 1;
            enable();
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow.Selected != false)
            {
                cond = 2;
                enable();
                textBox3.Enabled = false;
                textBox4.Enabled = false;
            }
        }

        private void btnhapus_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow.Selected)
            {
                if (btnhapus.Text.ToLower() == "aktifkan")
                {
                    DialogResult result = MessageBox.Show("Apakah anda yakin ingin mengaktifkan?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string com = "update admin set status_aktif = 1 where id = " + id;
                        try
                        {
                            Command.exec(com);
                            MessageBox.Show("Berhasil Mengaktifkan", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            loadgrid();
                            clear();
                            dis();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            connection.Close();
                        }
                    }

                }
                else if (btnhapus.Text.ToLower() == "non-aktifkan")
                {
                    DialogResult result = MessageBox.Show("Apakah anda yakin ingin menonaktifkan?", "Konfirmasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        string com = "update admin set status_aktif = 0 where id = " + id;
                        try
                        {
                            Command.exec(com);
                            MessageBox.Show("Berhasil Mengaktifkan", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            loadgrid();
                            clear();
                            dis();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void clear()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            if (cond == 1 && val())
            {
                string pass = Encrypt.enc(textBox3.Text);
                string com = "insert into admin values('" + textBox2.Text + "', '" + pass + "', 1, 2, '" + textBox1.Text + "')";
                try
                {
                    Command.exec(com);
                    MessageBox.Show("Berhasil Menambahkan", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clear();
                    dis();
                    loadgrid();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    MessageBox.Show(ex.Message);
                }
            }
            if (cond == 2 && val_up())
            {
                string com = "update admin set nama = '" + textBox1.Text + "', username = '" + textBox2.Text + "' where id = " + id;
                try
                {
                    Command.exec(com);
                    MessageBox.Show("Berhasil Mengubah", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clear();
                    dis();
                    loadgrid();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            clear();
            dis();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;
            id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            textBox1.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            if (Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[3].Value) == 0)
            {
                btnhapus.Text = "Aktifkan";
            }
            else if (Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[3].Value) == 1)
            {
                btnhapus.Text = "Non-Aktifkan";
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) || e.KeyChar == 8);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == 8);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah anda yakin ingin logout?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MainLogin login = new MainLogin();
                this.Hide();
                login.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah anda yakin ingin menutup aplikasi?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        private void panel_dokter_Click(object sender, EventArgs e)
        {
            MasterDokter master = new MasterDokter();
            this.Hide();
            master.ShowDialog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox3.PasswordChar = '\0';
            else if (!checkBox1.Checked)
                textBox3.PasswordChar = '*';
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            MasterVaksin vaksin = new MasterVaksin();
            this.Hide();
            vaksin.ShowDialog();
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            MasterWarga warga = new MasterWarga();
            this.Hide();
            warga.ShowDialog();
        }

        private void panel_master_Click(object sender, EventArgs e)
        {
            MasterAdmin master = new MasterAdmin();
            this.Hide();
            master.ShowDialog();
        }
    }
}
