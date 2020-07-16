using ParkingLotProject.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParkingLotProject.UI
{
    public partial class Form1 : Form
    {
        private LogicManager m_LogicManager = new LogicManager();

        private static readonly List<string> m_Messages = new List<string>()
        {
            "Success!\nVehicle number ",
            "Decline!\nVehicle number ",
            " was allowed to enter our parking lot,\n and was loaded successfully to database.",
            " was not allowed to enter our parking lot for some reason,\n however, it was loaded successfully to database.",
            " is allready exists in the dataBase."
        };
        public Form1()
        {
            InitializeComponent();
            LoadVehiclesList();
        }
        private void LoadVehiclesList()
        {
            m_LogicManager.Vehicles = SqliteDataAccess.LoadVehicles();
            WireUpVehiclesList();
        }
        private void WireUpVehiclesList()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = m_LogicManager.Vehicles;
        }
        private void buttonUploadFromDevice_Click(object sender, EventArgs e)
        {
            m_LogicManager.ImagePath = "";
            pictureBox1.BackgroundImage = null;
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "jpeg and png files|*.png;*.jpg;*.JPG";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(fileDlg.FileName);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    //Size limit depends: Free API 1 MB, PRO API 5 MB and more
                    MessageBox.Show("Image file size limit reached (1MB free API)");
                    return;
                }
                pictureBox1.BackgroundImage = Image.FromFile(fileDlg.FileName);
                m_LogicManager.ImagePath = fileDlg.FileName;
            }
        }
        private async void buttonLoadToDatabase_Click(object sender, EventArgs e)
        {
            VehicleModel vehicle = new VehicleModel();
            try
            {
                vehicle.Id = await m_LogicManager.ExtractIdNumberFromImage(); //;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            vehicle.Decision = m_LogicManager.MakeDecision(vehicle.Id);//;

            vehicle.TimeStamp = DateTime.Now.ToString();
            try
            {
                SqliteDataAccess.SaveVehicle(vehicle);
                if (vehicle.Decision == 1)
                {
                    MessageBox.Show(m_Messages[0] +  vehicle.Id + m_Messages[2]);
                }
                else
                {
                    MessageBox.Show(m_Messages[1] + vehicle.Id + m_Messages[3]);
                }

            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(m_Messages[1] + vehicle.Id + m_Messages[4]);

            }
            LoadVehiclesList();
        }

    }
}
