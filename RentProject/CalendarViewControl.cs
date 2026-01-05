using DevExpress.XtraEditors;
using DevExpress.XtraScheduler;
using RentProject.Shared.UIModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RentProject
{
    public partial class CalendarViewControl : DevExpress.XtraEditors.XtraUserControl
    {
        private DateTime _currentMonth; // 永遠存「當月 1 號」
        private List<CalendarRentTimeDetailItem> _detailList = new();
        private Dictionary<DateTime, List<CalendarRentTimeDetailItem>> _detailByDate = new();
        private Dictionary<int, CalendarRentTimeDetailItem> _detailById = new();

        public CalendarViewControl()
        {
            InitializeComponent();
        }

        private void CalendarViewControl_Load(object sender, EventArgs e)
        {
            schedulerControl1.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Month;

            // 關掉右側日期/時間捲軸（你畫面右邊那條)
            schedulerControl1.MonthView.DateTimeScrollbarVisible = false;
            // 關掉內建的「上方導覽列」（ 2026年1月 這種）
            schedulerControl1.DateNavigationBar.Visible = false;

            var today = DateTime.Today;

            _currentMonth = new DateTime(today.Year, today.Month, 1); // 把日期固定成1號

            schedulerControl1.Start = _currentMonth;

            // 綁 DataStorage（很重要）
            schedulerControl1.DataStorage = schedulerDataStorage1;

            // 設定 Appointment 欄位對應 (Mapping)
            schedulerDataStorage1.Appointments.Mappings.AppointmentId =
                nameof(CalendarRentTimeItem.RentTimeId);
            schedulerDataStorage1.Appointments.Mappings.Start =
                nameof(CalendarRentTimeItem.StartAt);
            schedulerDataStorage1.Appointments.Mappings.End =
                nameof(CalendarRentTimeItem.EndAt);
            schedulerDataStorage1.Appointments.Mappings.Subject =
                nameof(CalendarRentTimeItem.Subject);

            schedulerDataStorage1.Appointments.CustomFieldMappings.Clear();
            schedulerDataStorage1.Appointments.CustomFieldMappings.Add(
                new DevExpress.XtraScheduler.AppointmentCustomFieldMapping("Location", nameof(CalendarRentTimeItem.Location))
            );

            UpdateMonthTitle();

            // 讓月曆的行程可以自動長高，才放得下多行文字
            schedulerControl1.MonthView.AppointmentDisplayOptions.AppointmentAutoHeight = true;

            // 讓月曆格子的行程不要顯示時間（只顯示 Subject）
            schedulerControl1.MonthView.AppointmentDisplayOptions.StartTimeVisibility = AppointmentTimeVisibility.Never;
            schedulerControl1.MonthView.AppointmentDisplayOptions.EndTimeVisibility = AppointmentTimeVisibility.Never;

            LoadDemoAppointments();
            LoadDemoDetails();
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

        private void LoadDemoAppointments()
        {
            var list = new List<CalendarRentTimeItem>
            {
                new CalendarRentTimeItem
                {
                    RentTimeId = 1,
                    StartAt = _currentMonth.AddDays(2).AddHours(10),  // 當月第3天 10:00
                    EndAt   = _currentMonth.AddDays(2).AddHours(15),  // 當月第3天 15:00
                    Subject = "TE251130001\r\n好厲害科技\r\nConducted 2",
                    Location = "SAC D"
                },
                new CalendarRentTimeItem
                {
                    RentTimeId = 2,
                    StartAt = _currentMonth.AddDays(9).AddHours(9),
                    EndAt   = _currentMonth.AddDays(9).AddHours(12),
                    Subject = "TE251125001\r\n好厲害科技\r\nConducted 1",
                    Location = "Conducted 1"
                }
            };

            schedulerDataStorage1.Appointments.DataSource = list;

            // 保險：強制刷新一次畫面
            schedulerControl1.Refresh();
        }

        private void BuildDetailIndex()
        {
            _detailByDate = _detailList
                .GroupBy(x => x.StartAt.Date)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.StartAt).ToList());

            _detailById = _detailList.ToDictionary(x => x.RentTimeId, x => x);
        }

        private void LoadDemoDetails()
        {
            _detailList = new List<CalendarRentTimeDetailItem>
            {
                new CalendarRentTimeDetailItem
                {
                    RentTimeId = 1,
                    BookingNo = "TE251130001",
                    StartAt = _currentMonth.AddDays(2).AddHours(10),
                    EndAt   = _currentMonth.AddDays(2).AddHours(15),

                    ProjectNo = "TE251130001",
                    ProjectName = "專案A",
                    CustomerName = "好厲害科技",
                    ContactName = "王小明",
                    Phone = "0800-080-128",
                    PE = "Martin_Liu",
                    Location = "SAC D",
                    TestItem = "Conducted 2",
                    Notes = "Demo備註A"
                },
                new CalendarRentTimeDetailItem
                {
                    RentTimeId = 2,
                    BookingNo = "TE251125001",
                    StartAt = _currentMonth.AddDays(9).AddHours(9),
                    EndAt   = _currentMonth.AddDays(9).AddHours(12),

                    ProjectNo = "TE251125001",
                    ProjectName = "專案B",
                    CustomerName = "好厲害科技",
                    ContactName = "王小明",
                    Phone = "0800-080-128",
                    PE = "Martin_Liu",
                    Location = "Conducted 1",
                    TestItem = "Conducted 1",
                    Notes = "Demo備註B"
                }
            };

            BuildDetailIndex();
        }
    }
}
