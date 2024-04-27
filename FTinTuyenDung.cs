using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XinViec.XinViec;

namespace XinViec
{
    
    public partial class FTinTuyenDung : Form
    {

        SqlConnection conn = new SqlConnection(Properties.Settings.Default.stringConn);
        string sqlStr;
        //string Email = StateStorage.GetInstance().SharedValue;
        string Email;

        int kiemTra = 1;
        public FTinTuyenDung(string Email)
        {
            InitializeComponent();
            this.Email = Email;
            
        }

        private void themUCQLCongViec(string trangThai, string hinhThuc, string mucLuong, string tieuDe, string viTri, DateTime ngayDang, DateTime ngayHetHan, int slHoSo)
        {
            UCQLCongViec uc = new UCQLCongViec();
            uc.Dock = DockStyle.Top;
            uc.btnTieuDe.Text = tieuDe;

            uc.txtViTri.Text = viTri;
            uc.txtNgayDang.Text = ngayDang.Day + "/" + ngayDang.Month + "/" + ngayDang.Year;
            uc.txtNgayHetHan.Text = ngayHetHan.Day + "/" + ngayHetHan.Month + "/" + ngayHetHan.Year;
            uc.txtHinhThuc.Text = hinhThuc;
            uc.txtMucLuong.Text = mucLuong;
            uc.txtSLHoSo.Text = slHoSo.ToString();
            uc.cbbTrangThaiTuyen.Text = trangThai;
            uc.txtTrangThaiTuyen.Text = trangThai;
            uc.ChonButtonXemTin += XemTin;
            uc.btnMenu.Visible = false;
            uc.btnXemHS.Visible = false;
            uc.plHinhThuc.Visible = true;
            uc.plMucLuong.Visible = true;
            uc.txtTrangThaiTuyen.Visible = true;
            uc.cbbTrangThaiTuyen.Visible = false;
            uc.btnSuaTrangThaiTuyen.Visible = false;
            //uc.ChonButtonXemHS += XemHS;
            //uc.ChonButtonSua += SuaTin;

            //uc.ChonButtonXemHS += (sender, e) => XemHS(tieuDe);
            plUC.Controls.Add(uc);

        }

        private void XemTin(string tieuDe)
        {

            FThongTinTuyenDung tttd = new FThongTinTuyenDung(Email, tieuDe);
            tttd.Show();

        }

        private void FTinTuyenDung_Load(object sender, EventArgs e)
        {
            sqlStr = "SELECT LoaiHinhCongViec, MucLuong, TieuDe, TenCongViec, CapBac, NgayDang, NgayHetHan, SoLuongTuyen,TrangThai, (SELECT COUNT(*) FROM UngTuyen WHERE TieuDe = DangTinTuyenDung.TieuDe) AS SoHoSo FROM DangTinTuyenDung WHERE Email = @Email ORDER BY NgayDang DESC";


            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlStr, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tieuDe = reader["TieuDe"].ToString();
                                string tenCV = reader["TenCongViec"].ToString();
                                string viTri = reader["CapBac"].ToString();
                                string hinhThuc = reader["LoaiHinhCongViec"].ToString();
                                string mucLuong = reader["MucLuong"].ToString();
                                DateTime ngayDang = reader.GetDateTime(reader.GetOrdinal("NgayDang"));
                                DateTime ngayHetHan = reader.GetDateTime(reader.GetOrdinal("NgayHetHan"));
                                string trangThai = reader["TrangThai"].ToString();
                                int slConLai = Convert.ToInt32(1);
                                int slHoSo = Convert.ToInt32(reader["SoHoSo"]);
                                themUCQLCongViec(trangThai, hinhThuc, mucLuong, tieuDe, viTri, ngayDang, ngayHetHan, slHoSo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy dữ liệu từ cơ sở dữ liệu: " + ex.Message);
            }
        }

        

        private void vScrollBar1_Scroll_1(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                plUCTrai.VerticalScroll.Value = e.NewValue;
                plUCPhai.VerticalScroll.Value = e.NewValue;
            }
        }
    }
    

    
}
