using Guna.UI2.WinForms;
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
    public partial class LuuHoSoMoi : Form
    {
        public event EventHandler DaLuu;
        DAO dao = new DAO();
        string sqlStr;
        string EmailDangNhap = StateStorage.GetInstance().SharedValue;
        string ViTriUngTuyen;
        string MucTieuNgheNghiep;
        string KinhNghiemLamViec;
        string SoThich;
        string KyNang;


        public LuuHoSoMoi(Guna2TextBox ViTriUngTuyen, Guna2TextBox MucTieuNgheNghiep, Guna2TextBox KinhNghiemLamViec, Guna2TextBox SoThich, Guna2TextBox KyNang)
        {
            InitializeComponent();
            this.ViTriUngTuyen = ViTriUngTuyen.Text;
            this.MucTieuNgheNghiep = MucTieuNgheNghiep.Text;
            this.KinhNghiemLamViec = KinhNghiemLamViec.Text;
            this.SoThich = SoThich.Text;
            this.KyNang = KyNang.Text;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            sqlStr = "INSERT INTO HoSoXinViec (tenHoSo, ngayCapNhat, viTriUngTuyen, mucTieuNgheNghiep, kinhNghiemLamViec, kyNang, soThich, EmailDangNhap) " +
                "VALUES (@tenHoSo, @ngayCapNhat, @viTriUngTuyen, @mucTieuNgheNghiep, @kinhNghiemLamViec, @kyNang, @soThich, @EmailDangNhap)";
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.stringConn))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlStr, connection))
                    {
                        // Assuming you have parameters to set for your query
                        command.Parameters.AddWithValue("@tenHoSo", txbTenHoSo.Text);
                        command.Parameters.AddWithValue("@ngayCapNhat", DateTime.Now);
                        command.Parameters.AddWithValue("@viTriUngTuyen", ViTriUngTuyen);
                        command.Parameters.AddWithValue("@mucTieuNgheNghiep", MucTieuNgheNghiep);
                        command.Parameters.AddWithValue("@kinhNghiemLamViec", KinhNghiemLamViec);
                        command.Parameters.AddWithValue("@kyNang", KyNang);
                        command.Parameters.AddWithValue("@soThich", SoThich);
                        command.Parameters.AddWithValue("@EmailDangNhap", EmailDangNhap);
                        // Execute the INSERT query
                        int k = command.ExecuteNonQuery();

                        // Check if rows were affected
                        if (k > 0)
                        {
                            dao.ThongBao("Lưu thành công");
                            DaLuu?.Invoke(this, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dao.BaoLoi("Lưu thất bại: " + ex.Message);
            }
            this.Close();
        }
    }
}
