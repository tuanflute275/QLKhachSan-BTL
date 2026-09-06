namespace QLKhachSan.Common
{
    /// <summary>Cặp Mã/Tên hiển thị dùng chung cho các ComboBox trạng thái (không lưu CSDL).</summary>
    public class ComboItem
    {
        public string Value { get; }
        public string Text { get; }

        public ComboItem(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public override string ToString() => Text;
    }
}
