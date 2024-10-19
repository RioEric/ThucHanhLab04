using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThucHanhLab4.Data;

namespace ThucHanhLab4
{
    public partial class QuanLyKhoa : Form
    {
        public Model1 dbContext;
        public QuanLyKhoa()
        {
            InitializeComponent();
            dbContext = new Model1();
        }
        private void LoadData()
        {
            try
            {
                var faculties = dbContext.Faculties
                    .Select(f => new
                    {
                        f.FacultyID,
                        f.FacultyName,
                        f.TotalProfessor
                    }).ToList();

                if (faculties != null && faculties.Any())
                {
                    dataGridView1.DataSource = faculties;
                }
                else
                {
                    MessageBox.Show("No data found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(txtTenKhoa.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên khoa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                int totalProfessor = 0;
                if (!string.IsNullOrWhiteSpace(txtTong.Text))
                {
                    if (!int.TryParse(txtTong.Text, out totalProfessor))
                    {
                        MessageBox.Show("Tổng số giáo sư phải là số nguyên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                
                var faculty = new Faculty
                {
                    FacultyName = txtTenKhoa.Text,
                    FacultyID = txtMaKhoa.Text,
                    TotalProfessor = totalProfessor
                };

                
                dbContext.Faculties.Add(faculty);
                dbContext.SaveChanges();

                
                LoadData();
                ClearFields();

                MessageBox.Show("Thêm khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedFaculty = dataGridView1.CurrentRow.DataBoundItem as dynamic;
            if (selectedFaculty != null)
            {
                var faculty = dbContext.Faculties.Find(selectedFaculty.FacultyID);
                if (faculty != null)
                {
                    faculty.FacultyName = txtTenKhoa.Text;
                    faculty.TotalProfessor = int.TryParse(txtTong.Text, out int professors) ? professors : (int?)null;

                    dbContext.SaveChanges();
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var selectedFaculty = dataGridView1.CurrentRow.DataBoundItem as dynamic;
            if (selectedFaculty != null)
            {
                var faculty = dbContext.Faculties.Find(selectedFaculty.FacultyID);
                if (faculty != null)
                {
                    dbContext.Faculties.Remove(faculty);
                    dbContext.SaveChanges();
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void QuanLyKhoa_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void ClearFields()
        {
            txtMaKhoa.Clear();
            txtTenKhoa.Clear();
            txtTong.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn Đang Muốn Thoát", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Close();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtMaKhoa.Text = row.Cells[0].Value.ToString();
                txtTenKhoa.Text = row.Cells[1].Value.ToString();
                txtTong.Text = row.Cells[2].Value.ToString();
            }    
        }
    }
}
