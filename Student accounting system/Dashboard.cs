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
    }
}
