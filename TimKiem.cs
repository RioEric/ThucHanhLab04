using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThucHanhLab4.Data;

namespace ThucHanhLab4
{
    public partial class frmTimKiem : Form
    {
        public Model1 dbContext;
        public frmTimKiem()
        {
            InitializeComponent();
            
        }
        private void frmTimKiem_Load(object sender, EventArgs e)
        {
            try
            {
                using (Model1 model1 = new Model1())
                {
                    List<Faculty> listFaculties = model1.Faculties.ToList();
                    List<Student> listStudents = model1.Students.ToList();
                    FillFacultyCombobox(listFaculties);
                    BindGrid(listStudents);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            cmbKhoa.DataSource = listFaculties;
            cmbKhoa.DisplayMember = "FacultyName";
            cmbKhoa.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (Student student in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = student.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = student.StudentName;
                dataGridView1.Rows[index].Cells[2].Value = student.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[3].Value = student.AverageScore;
            }
        }

        private void loadDaTa()
        {
            using (Model1 model1 = new Model1())
            {
                List<Student> listStudent = model1.Students.ToList();
                BindGrid(listStudent);
            }
        }
       

        private int GetSelectedRow(string StudentID)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null &&
                    dataGridView1.Rows[i].Cells[0].Value.ToString() == StudentID)
                {
                    return i;
                }

            }
            return -1;
        }

        private void InsertUpdate(int selectedRow)
        {
            dataGridView1.Rows[selectedRow].Cells[0].Value = txtMa.Text;
            dataGridView1.Rows[selectedRow].Cells[1].Value = txtTen.Text;
            dataGridView1.Rows[selectedRow].Cells[2].Value = cmbKhoa.Text;
            

        }

        private void btnTroLai_Click(object sender, EventArgs e)
        {
            frmQuanLy f1 = new frmQuanLy();
            f1.Show();
            this.Hide();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            timTheoKhoa();
        }

        private void timTheoKhoa()
        {
            using (var dbContext = new Model1())
            {
                string selectedKhoa = cmbKhoa.Text;
                string searchTen = txtTen.Text; // ô nhập tên sinh viên
                string searchID = txtMa.Text;   // ô nhập mã sinh viên

                var results = dbContext.Students.AsQueryable();

                if (!string.IsNullOrEmpty(selectedKhoa))
                {
                    results = results.Where(s => s.Faculty.FacultyName == selectedKhoa);
                }

                if (!string.IsNullOrEmpty(searchTen))
                {
                    results = results.Where(s => s.StudentName.Contains(searchTen));
                }

                if (!string.IsNullOrEmpty(searchID))
                {
                    if (int.TryParse(searchID, out int studentID))
                    {
                        results = results.Where( s => s.StudentID == searchID);
                    }
                    else
                    {
                        MessageBox.Show("ID sinh viên phải là số nguyên hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var studentResults = results.ToList();
                if (studentResults.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên với tiêu chí đã chọn", "Thông báo", MessageBoxButtons.OK);
                    dataGridView1.DataSource = null;
                }
                else
                {
                    BindGrid(studentResults);
                }
            }


        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            xoaTimKiem();
        }

        private void xoaTimKiem()
        {
            if(dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string studentID = selectedRow.Cells[0].Value.ToString();
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn xóa tìm kiếm này", "Thông Báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    using (var dbcontext = new Model1())
                    {
                        var studentDeLeTe = dbcontext.Students.FirstOrDefault(s => s.StudentID == studentID);
                        if (studentDeLeTe != null) 
                        {
                            dbcontext.Students.Remove(studentDeLeTe);

                            dbcontext.SaveChanges();

                            List<Student> listStudent = dbcontext.Students.ToList();

                            BindGrid(listStudent);

                            MessageBox.Show("Xóa tiềm kiếm xong", "Thông Báo", MessageBoxButtons.OK);

                        }
                        else
                        {
                            MessageBox.Show("Không Timg Thấy Lựa Chọn Để Xóa", "Thông Báo",MessageBoxButtons.OK);
                        }    
                    }    
                }
            } 
            else
            {
                MessageBox.Show("Vui Lòng Lựa Chọn Để Xóa","Thông Báo",MessageBoxButtons.OK);
            }    
        }
    }
}
