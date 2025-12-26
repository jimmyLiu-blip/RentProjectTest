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
        private readonly ProjectService _projectService;
        private readonly DapperProjectRepository _projectRepo;

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

            _projectView = new ProjectViewControl(_rentTimeservice) { Dock = DockStyle.Fill };
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
            var form = new Project(_rentTimeservice, _projectService);

            form.ShowDialog();
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
