using System.Windows.Forms;

namespace QLKhachSan.Common
{
    /// <summary>
    /// Chuẩn phím tắt dùng chung cho mọi form (đề bài yêu cầu hạn chế thao tác chuột):
    /// Enter = chuyển control kế tiếp, F2 = Thêm, F3 = Sửa, F4/Delete = Xóa, F5 = Làm mới, Esc = Hủy/Đóng.
    /// </summary>
    public static class KeyboardHelper
    {
        /// <summary>Gắn vào KeyDown của TextBox/ComboBox/DateTimePicker để Enter hoạt động như Tab.</summary>
        public static void EnterAsTab(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                var control = sender as Control;
                var form = control?.FindForm();
                form?.SelectNextControl(control, true, true, true, true);
            }
        }
    }
}
