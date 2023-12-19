using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login_and_Register_System
{
    public partial class Dashboard : Form
    {
        public static Dashboard instance;
        public Label lbl;
        public Label lbl_faculty;
        public string typeUser;

        private readonly string[] columns = { "id", "Фамилия", "Имя", "Отчество", "Группа", "Факультет", "Почта", "Телефон", "Пол", "Дата рождения", "Староста", "Номер студенческого", "Статус обучения", "Форма обучения", "Курс", "Тип обучения", "Язык обучения", "Активность", "Дата начала обучения", "Дата окончания обучения", "Кафедра", "Код специальности", "Стадия обучения", "Номер зачетки", "Номер персонального дела", "Номер паспорта", "Идентификационный номер паспорта", "Выдан кем паспорт", "Дата выдачи", "Срок действия", "Место рождения", "Гражданство", "Почтовый индекс" };
        private readonly string[] comboboxColumns = { "Пол", "Статус обучения", "Форма обучения", "Курс", "Тип обучения" };

        private readonly string[] types =
        {
            "gender", "status", "form", "course", "type_study"
        };

        private readonly string[] requestColumns =
        {
            "Имя", "Фамилия", "Должность", "Факультет"
        };

        NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.AppSettings.Get("MyConnection"));
        NpgsqlCommand cmd = new NpgsqlCommand();

        public string faculty;

        public Dashboard()
        {
            InitializeComponent();
            instance = this;
            lbl = label1;
            lbl_faculty = label2;
        }

        public void BeginData()
        {
            if (typeUser == "Ректорат" || typeUser == "Деканат")
            {
                button3.Hide();
                button4.Hide();
                button2.Hide();
            }

            switch (typeUser)
            {
                case "Ректорат":
                    {
                        lbl_faculty.Text = "Ректорат";
                        break;
                    }
                case "Деканат":
                    {
                        lbl_faculty.Text = faculty;
                        break;
                    }
                default:
                    {
                        lbl_faculty.Text = "Администратор";
                        break;
                    }
            }
            dataGridView1.ColumnCount = 1;
            dataGridView1.RowCount = 1;

            for (int i = 0; i < columns.Length; i++)
            {
                if (i >= dataGridView1.Columns.Count)
                {
                    if (comboboxColumns.Contains(columns[i]))
                    {
                        DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();

                        var ind = Array.IndexOf(comboboxColumns, columns[i]);

                        conn.Open();
                        string type_values = ($"SELECT unnest(enum_range(NULL::{types[ind]}))");
                        cmd = new NpgsqlCommand(type_values, conn);
                        NpgsqlDataReader dr_type = cmd.ExecuteReader();

                        List<string> typesColumnList = new List<string>();

                        while (dr_type.Read())
                        {
                            typesColumnList.Add(dr_type["unnest"].ToString());
                        }

                        conn.Close();

                        foreach (var type in typesColumnList)
                        {
                            column.Items.Add(type);
                        }

                        dataGridView1.Columns.Add(column);
                    }
                    else
                    {
                        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
                    }
                    dataGridView1.Columns[i].HeaderText = columns[i];
                }
            }

            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();

            string dataStudents = null;
            switch (typeUser)
            {
                case "Ректорат":
                    {
                        dataStudents = "SELECT * FROM students";
                        break;
                    }
                case "Деканат":
                    {
                        dataStudents = "SELECT * FROM students WHERE Факультет = '" + faculty + "' ";
                        break;
                    }
                default:
                    {
                        dataStudents = "SELECT * FROM students";
                        break;
                    }
            }
            cmd = new NpgsqlCommand(dataStudents, conn);
            NpgsqlDataReader dr_2 = cmd.ExecuteReader();

            int num_row = 0;

            while (dr_2.Read())
            {
                DataGridViewRow DGVR = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                for (int i = 0; i < columns.Length; i++)
                {
                    DGVR.Cells[i].Value = dr_2[columns[i]].ToString().TrimEnd();
                }

                num_row++;

                dataGridView1.Rows.Add(DGVR);
            }
            conn.Close();

            if (typeUser != "Ректорат" && typeUser != "Деканат")
            {
                dataGridView2.ColumnCount = requestColumns.Length;
                dataGridView2.RowCount = 1;

                for (int i = 0; i < requestColumns.Length; i++)
                {
                    dataGridView2.Columns[i].HeaderText = requestColumns[i];
                }

                conn.Open();
                string requests = ("SELECT * FROM requests");
                cmd = new NpgsqlCommand(requests, conn);
                NpgsqlDataReader dr_requests = cmd.ExecuteReader();

                while (dr_requests.Read())
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[0].Clone();
                    for (int i = 0; i < requestColumns.Length; i++)
                    {
                        row.Cells[i].Value = dr_requests[requestColumns[i]].ToString().TrimEnd();
                    }

                    num_row++;

                    dataGridView2.Rows.Add(row);
                }
                conn.Close();
            }

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            conn.Open();
            cmd = new NpgsqlCommand("DELETE FROM students", conn);
            cmd.ExecuteNonQuery();
            conn.Close();


            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                string row = "INSERT INTO students (\"";
                string values = " VALUES ('";
                for (int j = 1; j < columns.Length; j++)
                {
                    if (dataGridView1.Rows[i].Cells[j].Value != null && dataGridView1.Rows[i].Cells[j].Value.ToString() != "")
                    {
                        if (row != "INSERT INTO students (\"")
                        {
                            values += ", '";
                            row += "\", \"";
                        }
                        row += columns[j];
                        values += dataGridView1.Rows[i].Cells[j].Value.ToString() + "'";
                    }
                }

                row += "\")";
                row += values + ")";

                conn.Open();
                cmd = new NpgsqlCommand(row, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Hide();
            dataGridView2.Show();
            button3.Show();
            button4.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView2.Hide();
            button3.Hide();
            button4.Hide();
            dataGridView1.Show();
        }
    }
}
