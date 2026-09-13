using System;
using System.Linq;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;

namespace QLKhachSan.Reports
{
    /// <summary>
    /// Tìm report class do Visual Studio sinh ra khi "Add New Item -> Crystal Report" (xem
    /// docs/04-HuongDan-TichHop-CrystalReport.md), rồi khởi tạo bằng reflection thay vì "new rptHoaDon()"
    /// trực tiếp — nhờ vậy project vẫn build được kể cả khi report .rpt chưa được thiết kế xong trong VS.
    /// </summary>
    public static class CrystalReportLoader
    {
        public static ReportDocument TryCreate(string reportClassName)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => t.Name == reportClassName && typeof(ReportDocument).IsAssignableFrom(t));
            return type == null ? null : (ReportDocument)Activator.CreateInstance(type);
        }
    }
}
