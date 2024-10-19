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
    public partial class frmQuanLy : Form
    {
        public frmQuanLy()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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
        private void ResetFom()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtDiem.Clear();
            cmbKhoa.SelectedIndex = -1;
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
            dataGridView1.Rows[selectedRow].Cells[3].Value = float.Parse(txtDiem.Text).ToString();

        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtMa.Text = row.Cells[0].Value.ToString();
                txtTen.Text= row.Cells[1].Value.ToString();
                cmbKhoa.Text = row.Cells[2].Value.ToString();
                txtDiem.Text = row.Cells[3].Value.ToString();

            }    
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtMa.Text == ""|| txtTen.Text == ""|| txtDiem.Text == "")
                {
                    throw new Exception("Vui Lòng Nhập Đầy Đủ Thông Tin");
                }
                int selected = GetSelectedRow(txtMa.Text);
                if (selected == -1)
                {
                    using (Model1 model1  = new Model1())
                    {
                        var faculty = model1.Faculties.FirstOrDefault(f => f.FacultyName == cmbKhoa.Text);
                        if (faculty == null)
                        {
                            throw new Exception("Khoa Không Tồn Tại");
                        }
                        var student = new Student
                        {
                            StudentID = txtMa.Text,
                            StudentName = txtTen.Text,
                            AverageScore = float.Parse(txtDiem.Text),
                            Faculty = faculty,

                        };
                        model1.Students.Add(student);
                        model1.SaveChanges();
                    }

                    selected = dataGridView1.Rows.Add();
                    InsertUpdate(selected);
                    MessageBox.Show("Thêm Thành Công","Thông Báo",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Sinh Viên Đã Tồn Tại","Thông Báo",MessageBoxButtons.OK);
                }    
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Mới bạn chọn Sinh Viên Để Sửa", "Thông Báo", MessageBoxButtons.OK);
                    return;
                }
                
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string studentID = selectedRow.Cells[0].Value.ToString();

                
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn Sửa sinh viên này?", "Xác nhận sửa", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    using (var model1 = new Model1())
                    {
                        var student = model1.Students.FirstOrDefault(s => s.StudentID == studentID);
                        if (student != null)
                        {
                            model1.Students.Add(student);
                            model1.SaveChanges(); 

                            MessageBox.Show("Sửa sinh viên thành công!", "Thông Báo", MessageBoxButtons.OK);

                            
                            dataGridView1.Rows.Add();
                            model1.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sinh viên để sửa.", "Lỗi", MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string studentID = selectedRow.Cells[0].Value.ToString();

                
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    using (var model1 = new Model1())
                    {
                        var student = model1.Students.FirstOrDefault(s => s.StudentID == studentID);
                        if (student != null)
                        {
                            model1.Students.Remove(student);
                            model1.SaveChanges();  

                            MessageBox.Show("Xóa sinh viên thành công!", "Thông Báo", MessageBoxButtons.OK);

                            
                            dataGridView1.Rows.Remove(selectedRow);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sinh viên để xóa.", "Lỗi", MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn Đang Muốn Thoát","Thông Báo",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
            if (result == DialogResult.OK) 
            {
                Close();
            }

        }

        private void mnstripQuanLy_Click(object sender, EventArgs e)
        {
            QuanLyKhoa quanLyKhoa = new QuanLyKhoa();
            quanLyKhoa.Show();
        }

        private void mnstripTimKiem_Click(object sender, EventArgs e)
        {
            frmTimKiem timKiem = new frmTimKiem();
            timKiem.Show();
        }
    }
}
