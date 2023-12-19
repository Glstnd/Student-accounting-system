using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Npgsql;

namespace Login_and_Register_System
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.AppSettings.Get("MyConnection"));
        NpgsqlCommand cmd = new NpgsqlCommand();

        private void label6_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            this.Hide();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtUsername.Focus();
        }

        private void registrationButton_Click(object sender, EventArgs e)
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            string login = ("SELECT * FROM csharp_user WHERE username =  '" + txtUsername.Text + "' and password = '" + txtPassword.Text + "' ");
            cmd = new NpgsqlCommand(login, conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();


            if (dr.Read() == true)
            {
                string admin_faculty = dr["Факультет"].ToString().TrimEnd();
                string type = dr["Должность"].ToString().TrimEnd();
                conn.Close();
                new Dashboard().Show();
                Dashboard.instance.lbl.Text = txtUsername.Text;
                Dashboard.instance.faculty = admin_faculty;
                Dashboard.instance.typeUser = type;
                Dashboard.instance.lbl_faculty.Text = Dashboard.instance.faculty;
                Dashboard.instance.BeginData();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Credentials, please try Again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Text = "";
                txtPassword.Text = "";
                txtUsername.Focus();
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
            }
        }

        private void checkboxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkboxShowPass.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '*';

            }
        }
    }
}
