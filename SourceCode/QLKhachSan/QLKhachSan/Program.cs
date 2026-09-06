using System;
using System.Windows.Forms;
using QLKhachSan.Forms.DangNhap;

namespace QLKhachSan
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Bắt đăng nhập trước khi vào MainForm; nếu người dùng bấm "Đăng xuất" trong MainForm,
            // vòng lặp quay lại màn hình đăng nhập thay vì thoát hẳn ứng dụng.
            bool tiepTuc = true;
            while (tiepTuc)
            {
                using (var frmLogin = new frmDangNhap())
                {
                    if (frmLogin.ShowDialog() != DialogResult.OK) return;
                }

                var main = new MainForm();
                Application.Run(main);
                tiepTuc = main.YeuCauDangXuat;
            }
        }
    }
}
