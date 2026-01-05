using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using RentProject.Repository;
using RentProject.Service;
using System.Configuration;
using System.Windows.Forms;

namespace RentProject
{
    public partial class Form1 : RibbonForm
    {
        private readonly DapperRentTimeRepository _rentTimeRepo;
        private readonly RentTimeService _rentTimeservice;
        private readonly DapperProjectRepository _projectRepo;
        private readonly ProjectService _projectService;
        private readonly DapperTestLocationRepository _testLocationRepo;
        private readonly TestLocationService _testLocationService;

        private ProjectViewControl _projectView;
        private CalendarViewControl _calendarView;

        // true = 目前顯示 CalendarView；false = 目前顯示 ProjectView
        private bool _isCalendarView = true;

        public Form1()
        {
            InitializeComponent();

            var connectionString =
                ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            _rentTimeRepo = new DapperRentTimeRepository(connectionString);
            _rentTimeservice = new RentTimeService(_rentTimeRepo);
            _projectRepo = new DapperProjectRepository(connectionString);
            _projectService = new ProjectService(_projectRepo);
            _testLocationRepo = new DapperTestLocationRepository(connectionString);
            _testLocationService = new TestLocationService(_testLocationRepo);

            _projectView = new ProjectViewControl(_rentTimeservice, _projectService, _testLocationService) { Dock = DockStyle.Fill };

            _projectView.RentTimeSaved += RefreshProjectView; //ProjectViewControl 說「存好了」→ Form1 刷新列表

            _calendarView = new CalendarViewControl { Dock = DockStyle.Fill };
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            mainPanel.Controls.Add(_projectView);
            mainPanel.Controls.Add(_calendarView);

            ShowCalendarView();
        }

        private void btnAddRentTime_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var form = new Project(_rentTimeservice, _projectService, _testLocationService);

            var dr = form.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                RefreshProjectView();
                ShowProjectView();
            }
        }

        private void btnTestConnection_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string msg = _rentTimeRepo.TestConnection();

            XtraMessageBox.Show(msg, "TestConnection");
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

        private void btnView_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_isCalendarView)
            {
                ShowProjectView();
            }
            else
                ShowCalendarView();
        }

        private void RefreshProjectView()
        { 
            var list = _rentTimeservice.GetProjectViewList();
            _projectView.LoadData(list);
        }
    }
}
