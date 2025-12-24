using RentProject.Repository;
using System.Configuration;
using DevExpress.XtraEditors;
using RentProject.Service;
using DevExpress.XtraBars.Ribbon;

namespace RentProject
{
    public partial class Form1 : RibbonForm
    {
        private readonly DapperRentTimeRepository _rentTimeRepo;
        private readonly RentTimeService _service;
        private readonly DapperProjectRepository _projectRepo;

        public Form1()
        {
            InitializeComponent();

            var connectionString =
                ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            _rentTimeRepo = new DapperRentTimeRepository(connectionString);
            _service = new RentTimeService(_rentTimeRepo);
            _projectRepo = new DapperProjectRepository(connectionString);
        }

        private void btnAddRentTime_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var form = new Project(_service, _projectRepo);

            form.ShowDialog();
        }

        private void btnTestConnection_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string msg = _rentTimeRepo.TestConnection();

            XtraMessageBox.Show(msg, "TestConnection");
        }
    }
}
