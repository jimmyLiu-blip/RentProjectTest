using RentProject.Repository;
using System.Configuration;
using DevExpress.XtraEditors;
using RentProject.Service;
using DevExpress.XtraBars.Ribbon;

namespace RentProject
{
    public partial class Form1 : RibbonForm
    {
        private readonly DapperRentTimeRepository _repo;
        private readonly RentTimeService _service;

        public Form1()
        {
            InitializeComponent();

            var connectionString =
                ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            _repo = new DapperRentTimeRepository(connectionString);
            _service = new RentTimeService(_repo);
        }

        private void btnAddRentTime_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var form = new Project(_service);

            form.ShowDialog();
        }

        private void btnTestConnection_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string msg = _repo.TestConnection();

            XtraMessageBox.Show(msg, "TestConnection");
        }
    }
}
