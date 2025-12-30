using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentProject
{
    public partial class CalendarViewControl : DevExpress.XtraEditors.XtraUserControl
    {
        private DateTime _currentMonth; // 永遠存「當月 1 號」

        public CalendarViewControl()
        {
            InitializeComponent();
        }

        private void CalendarViewControl_Load(object sender, EventArgs e)
        {
            schedulerControl1.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Month;

            // 關掉右側日期/時間捲軸（你畫面右邊那條)
            schedulerControl1.MonthView.DateTimeScrollbarVisible = false;
            // 關掉內建的「上方導覽列」（< 2026年1月 這種）
            schedulerControl1.DateNavigationBar.Visible = false;

            var today = DateTime.Today;

            _currentMonth = new DateTime(today.Year, today.Month, 1); // 把日期固定成1號

            schedulerControl1.Start = _currentMonth;

            UpdateMonthTitle();
        }

        private void btnPrevMonth_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            schedulerControl1.Start = _currentMonth;
            UpdateMonthTitle();
        }

        private void btnNextMonth_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            schedulerControl1.Start = _currentMonth;
            UpdateMonthTitle();
        }

        private void UpdateMonthTitle()
        {
            lblMonthTitle.Text = $"{_currentMonth.Year}年{_currentMonth.Month}月";
        }
    }
}
