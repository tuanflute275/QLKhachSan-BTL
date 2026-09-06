using System.Linq;
using System.Windows.Forms;

namespace QLKhachSan.Common
{
    /// <summary>Hàm kiểm tra input dùng chung cho các form (đề bài yêu cầu kiểm tra dữ liệu chặt chẽ).</summary>
    public static class Validator
    {
        public static bool IsAllDigits(string s) => !string.IsNullOrEmpty(s) && s.All(char.IsDigit);

        public static bool IsValidCmnd(string s) => IsAllDigits(s) && (s.Length == 9 || s.Length == 12);

        public static bool IsValidPhone(string s) => IsAllDigits(s) && s.Length >= 9 && s.Length <= 11;

        public static bool IsValidEmail(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return true; // email không bắt buộc
            return s.Contains("@") && s.Contains(".") && !s.Contains(" ");
        }

        /// <summary>Kiểm tra 1 TextBox không rỗng, gắn lỗi vào ErrorProvider nếu có.</summary>
        public static bool CheckRequired(TextBox tb, ErrorProvider ep, string message)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                ep.SetError(tb, message);
                return false;
            }
            ep.SetError(tb, string.Empty);
            return true;
        }

        public static bool CheckCondition(Control control, ErrorProvider ep, bool condition, string message)
        {
            ep.SetError(control, condition ? string.Empty : message);
            return condition;
        }
    }
}
