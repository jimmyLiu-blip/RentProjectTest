using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Microsoft.Extensions.DependencyInjection;
using RentProject.Service;
using System;
using System.Windows.Forms;

namespace RentProject
{
    public partial class Form1 : RibbonForm
    {
        private readonly IRentTimeService _rentTimeservice;
        private readonly IProjectService _projectService;
        private readonly ITestLocationService _testLocationService;

        private ProjectViewControl _projectView;
        private CalendarViewControl _calendarView;

        // true = 目前顯示 CalendarView；false = 目前顯示 ProjectView
        private bool _isCalendarView = true;

        // 在 Form1 建構子加進來 DI 容器（IServiceProvider）
        // IServiceProvider 是什麼? 它就是 DI容器本身的取貨櫃台
        private readonly IServiceProvider _sp;
        private readonly Func<int, Project> _projectFactory;

        public Form1(IRentTimeService rentTimeService, IProjectService projectService, ITestLocationService testLocationService, IServiceProvider sp, Func<int,Project> projectFactory)
        {
            InitializeComponent();

            _rentTimeservice = rentTimeService;
            _projectService = projectService;
            _testLocationService = testLocationService;
            _sp = sp;
            _projectFactory = projectFactory;

            _projectView = new ProjectViewControl(_rentTimeservice, _projectService, _testLocationService, _projectFactory) { Dock = DockStyle.Fill };

            _projectView.RentTimeSaved += RefreshProjectView; //ProjectViewControl 說「存好了」→ Form1 刷新列表

            _calendarView = new CalendarViewControl { Dock = DockStyle.Fill };
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            mainPanel.Controls.Add(_projectView);
            mainPanel.Controls.Add(_calendarView);

            ShowCalendarView();
        }

        // 點擊新增租時單綁定按鈕事件
        private void btnAddRentTime_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var form = _sp.GetRequiredService<Project>();

            var dr = form.ShowDialog();

            if (dr == DialogResult.OK)
            {
                RefreshProjectView();
                ShowProjectView();
            }
        }

        // 點擊測試連線綁定按鈕事件
        private void btnTestConnection_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string msg = _rentTimeservice.TestConnection();

            XtraMessageBox.Show(msg, "TestConnection");
        }

        // 點擊 View 切換視圖
        private void btnView_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_isCalendarView)
            {
                ShowProjectView();
            }
            else
                ShowCalendarView();
        }

        private void ShowProjectView()
        {
            RefreshProjectView();
            _projectView.BringToFront();

            _isCalendarView = false;
            btnView.Caption = "切換到日曆";
        }

        private void ShowCalendarView()
        {
            _calendarView.BringToFront();
            _isCalendarView = true;

            btnView.Caption = "切換到案件";
        }

        private void RefreshProjectView()
        { 
            var list = _rentTimeservice.GetProjectViewList();
            _projectView.LoadData(list);
        }
    }
}
