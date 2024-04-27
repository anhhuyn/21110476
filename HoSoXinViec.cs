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
using XinViec.Resources;
using XinViec.XinViec;

namespace XinViec
{
    public partial class HoSoXinViec : Form
    { 
        private TaoSuaHoSoMoi thsm;
        public delegate void TaoHSEventHandler();
        public delegate void SuaEventHandler(string tenHS);
        public event TaoHSEventHandler TaoHSMoi;
        public event SuaEventHandler SuaHS;
        public ucHienHoSo uc;
        string sqlStr;
        string tenHS;
        int value = 0;
        bool hienThi = true;
        string EmailDangNhap = StateStorage.GetInstance().SharedValue;
        DAO dao = new DAO();

        public HoSoXinViec()
        {
            InitializeComponent();
            thsm = new TaoSuaHoSoMoi(this);
            Load_cbbTinhChatLoc();
        }

        private void Load_cbbTinhChatLoc()
        {
            cbbTinhChatLoc.Items.Clear();
            cbbDanhSachLoc.Items.Clear();
            cbbTinhChatLoc.Items.Add("Tất cả");
            cbbTinhChatLoc.Items.Add("Tên hồ sơ");
            cbbTinhChatLoc.Items.Add("Vị trí tuyển dụng");
            cbbTinhChatLoc.Text = cbbTinhChatLoc.Items[0].ToString();
        }

        private void Xoa(object sender, EventArgs e)
        {
            dao.tb.Close();
            sqlStr = "DELETE FROM HoSoXinViec WHERE tenHoSo = @tenHS AND EmailDangNhap = @EmailDangNhap";
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlStr, connection))
                    {
                        // Assuming you have parameters to set for your query
                        command.Parameters.AddWithValue("@tenHS", tenHS);
                        command.Parameters.AddWithValue("@EmailDangNhap ", EmailDangNhap);
                        int k = command.ExecuteNonQuery();

                        // Check if rows were affected
                        if (k > 0)
                        {
                            dao.ThongBao("Xóa thành công");
                            plChuaHoSo.Controls.Clear();
                            HoSoXinViec_Load(sender, e); // Gọi sự kiện load dữ liệu sau khi xóa
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dao.ThongBao("Xóa thất bại: " + ex);
            }
        }

        private void XoaHoSo(string tenHS)
        {
            dao.ThongBao_LuaChon("Bạn muốn xóa hồ sơ?", Xoa);
            this.tenHS = tenHS;
        }
        private void SuaHoSo(string tenHS)
        {
            dao.MoFormCon(new TaoSuaHoSoMoi(this), plFormCha);
            SuaHS?.Invoke(tenHS);
        }

        private void QuayLai(object sender, EventArgs e)
        {
            dao.MoFormCon(new HoSoXinViec(), plFormCha);
        }

        private void XemHoSo(string tenHS)
        {
            XemHoSo xem = new XemHoSo(EmailDangNhap, tenHS);
            xem.QuayLai += QuayLai;
            dao.MoFormCon(xem, plFormCha);
        }
        private void HienThiHS(string tenHoSo, string viTriUngTuyen, DateTime ngayCapNhat)
        {
            ucHienHoSo uc = new ucHienHoSo();
            uc.ChonButtonSua += SuaHoSo;
            uc.ChonButtonXem += XemHoSo;
            uc.ChonButtonXoa += XoaHoSo;
            if (hienThi == true)
            {
                uc.Location = new Point(45, value);
                hienThi = false;
            }
            else
            {
                uc.Location = new Point(460, value);
                value += 100;
                hienThi = true;
            }
            uc.lblTenHoSo.Text = tenHoSo.ToUpper();
            uc.lblViTriUngTuyen.Text = viTriUngTuyen.ToUpper();
            uc.lblNgayCapNhat.Text = ngayCapNhat.ToString("dd/MM/yyyy   HH:mm:ss");
            plChuaHoSo.Controls.Add(uc);
        }

        private void HoSoDaTao()
        {
            sqlStr = string.Format("SELECT COUNT(tenHoSo) AS soHoSoDaTao FROM HoSoXinViec WHERE EmailDangNhap = @EmailDangNhap");
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlStr, connection);
                    command.Parameters.AddWithValue("@EmailDangNhap", EmailDangNhap);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txbSoHoSoDuocTao.Text = reader["soHoSoDaTao"].ToString();
                        }
                    }
                    connection.Close();
                } 
            }
            catch (Exception ex)
            {
                dao.BaoLoi("Lỗi khi lấy dữ liệu từ cơ sở dữ liệu: " + ex.Message);
            }
        }

        private void HoSoXinViec_Load(object sender, EventArgs e)
        {
            hienThi = true;
            value = 0;
            sqlStr = "SELECT tenHoSo, viTriUngTuyen, ngayCapNhat FROM HoSoXinViec WHERE EmailDangNhap = @EmailDangNhap ORDER BY ngayCapNhat DESC";
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlStr, connection))
                    {
                        command.Parameters.AddWithValue("@EmailDangNhap", EmailDangNhap);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tenHoSo = reader["tenHoSo"].ToString();
                                string viTriUngTuyen = reader["viTriUngTuyen"].ToString();
                                DateTime ngayCapNhat = reader.GetDateTime(reader.GetOrdinal("ngayCapNhat"));
                                HienThiHS(tenHoSo, viTriUngTuyen, ngayCapNhat);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dao.BaoLoi("Lỗi khi lấy dữ liệu từ cơ sở dữ liệu: " + ex.Message);
            }
            HoSoDaTao();
        }
        private void cbbTinhChatLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            Load_cbbDanhSachLoc();
        }

        private void Load_cbbDanhSachLoc()
        {
            cbbDanhSachLoc.Items.Clear();
            if (cbbTinhChatLoc.Text == "Tất cả")
            {
                cbbDanhSachLoc.Items.Add("Danh sách lọc trống");
            }
            else if (cbbTinhChatLoc.Text == "Vị trí tuyển dụng")
            {
                sqlStr = "SELECT DISTINCT viTriUngTuyen FROM HoSoXinViec WHERE EmailDangNhap = @EmailDangNhap";
                try
                {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sqlStr, connection))
                        {
                            command.Parameters.AddWithValue("@EmailDangNhap", EmailDangNhap);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cbbDanhSachLoc.Items.Add(reader["viTriUngTuyen"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    dao.BaoLoi("Lỗi khi lấy dữ liệu từ cơ sở dữ liệu: " + ex.Message);
                }
            }
            else
            {
                sqlStr = "SELECT tenHoSo FROM HoSoXinViec WHERE EmailDangNhap = @EmailDangNhap ORDER BY ngayCapNhat DESC";
                try
                {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sqlStr, connection))
                        {
                            command.Parameters.AddWithValue("@EmailDangNhap", EmailDangNhap);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cbbDanhSachLoc.Items.Add(reader["tenHoSo"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    dao.BaoLoi("Lỗi khi lấy dữ liệu từ cơ sở dữ liệu: " + ex.Message);
                }
            }
            if (cbbDanhSachLoc.Items.Count > 0)
            {
                cbbDanhSachLoc.Text = cbbDanhSachLoc.Items[0].ToString();
            }
        }

        private void btnBoLoc_Click(object sender, EventArgs e)
        {
            dao.MoFormCon(new HoSoXinViec(), plFormCha);
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            value = 0;
            hienThi = true;
            plChuaHoSo.Controls.Clear();
            if (cbbTinhChatLoc.Text == "Tất cả")
            {
                HoSoXinViec_Load(sender, e);
            }
            else if (cbbTinhChatLoc.Text == "Vị trí tuyển dụng")
            {
                sqlStr = "SELECT tenHoSo, viTriUngTuyen, ngayCapNhat FROM HoSoXinViec WHERE EmailDangNhap = @EmailDangNhap AND viTriUngTuyen = @viTriUngTuyen ORDER BY ngayCapNhat DESC";
                try
                {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sqlStr, connection))
                        {
                            command.Parameters.AddWithValue("@EmailDangNhap", EmailDangNhap);
                            command.Parameters.AddWithValue("@viTriUngTuyen", cbbDanhSachLoc.Text);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string tenHoSo = reader["tenHoSo"].ToString();
                                    string viTriUngTuyen = reader["viTriUngTuyen"].ToString();
                                    DateTime ngayCapNhat = reader.GetDateTime(reader.GetOrdinal("ngayCapNhat"));
                                    HienThiHS(tenHoSo, viTriUngTuyen, ngayCapNhat);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    dao.BaoLoi("Lỗi khi lấy dữ liệu từ cơ sở dữ liệu: " + ex.Message);
                }
            }
            else
            {
                sqlStr = "SELECT tenHoSo, viTriUngTuyen, ngayCapNhat FROM HoSoXinViec WHERE EmailDangNhap = @EmailDangNhap AND tenHoSo = @tenHoSo ORDER BY ngayCapNhat DESC";
                try
                {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sqlStr, connection))
                        {
                            command.Parameters.AddWithValue("@EmailDangNhap", EmailDangNhap);
                            command.Parameters.AddWithValue("@tenHoSo", cbbDanhSachLoc.Text);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string tenHoSo = reader["tenHoSo"].ToString();
                                    string viTriUngTuyen = reader["viTriUngTuyen"].ToString();
                                    DateTime ngayCapNhat = reader.GetDateTime(reader.GetOrdinal("ngayCapNhat"));
                                    HienThiHS(tenHoSo, viTriUngTuyen, ngayCapNhat);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    dao.BaoLoi("Lỗi khi lấy dữ liệu từ cơ sở dữ liệu: " + ex.Message);
                }
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            dao.MoFormCon(new TaoSuaHoSoMoi(this), plFormCha);
            TaoHSMoi?.Invoke();
        }
    }
}
