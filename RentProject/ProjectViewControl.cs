using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using RentProject.Domain;
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
    public partial class ProjectViewControl : DevExpress.XtraEditors.XtraUserControl
    {
        public ProjectViewControl()
        {
            InitializeComponent();
        }

        public void LoadData(List<RentTime> list)
        {
            gridControl1.DataSource = list;
            gridView1.PopulateColumns();

            var show = new[]
            {
                "BookingNo", "Area", "Location", "CustomerName", "PE",
                "StartDate", "EndDate", "ProjectNo", "ProjectName"
            };

            foreach (GridColumn col in gridView1.Columns)
                col.Visible = show.Contains(col.FieldName);

            gridView1.BestFitColumns();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
    }
}
