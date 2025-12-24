using DevExpress.XtraEditors;
using RentProject.Domain;
using RentProject.Repository;
using RentProject.Service;
using RentProject.UIModels;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RentProject
{
    public partial class Project : XtraForm
    {
        private readonly RentTimeService _service;

        // =========================
        // 1) 欄位 / 建構子
        // =========================
        public Project(RentTimeService service)
        {
            InitializeComponent();
            _service = service;
        }

        // =========================
        // 2) Form Load：初始化 UI
        // =========================
        private void Project_Load(object sender, EventArgs e)
        {
            startDateEdit.EditValue = DateTime.Today;
            endDateEdit.EditValue = DateTime.Today;

            startTimeEdit.EditValue = DateTime.Now;
            endTimeEdit.EditValue = DateTime.Now;

            cmbProjectNo.Properties.Items.Clear();
            cmbProjectNo.Properties.Items.AddRange(_projects.Select(x => x.ProjectNo).ToArray());

            ApplyLunchUI();
            ApplyDinnerUI();
            UpdateEstimatedUI();
        }

        // =========================
        // 3) 主要流程：按鈕事件
        // =========================
        private void btnCreatedRentTime_Click(object sender, EventArgs e)
        {
            try
            {
                var model = BuildModelFormUI();

                var result = _service.CreateRentTime(model);

                txtBookingNo.Text = result.BookingNo;

                XtraMessageBox.Show(
                    $"建立成功! \nRentTimeId：{result.RentTimeId}\nBookingNo：{result.BookingNo}", "CreateRentTime");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Error");
            }
        }

        // =========================
        // 4) 控制項事件：只負責觸發更新
        // =========================

        private void chkHasLunch_CheckedChanged(object sender, EventArgs e)
        {
            ApplyLunchUI();
            UpdateEstimatedUI();
        }

        private void chkHasDinner_CheckedChanged(object sender, EventArgs e)
        {
            ApplyDinnerUI();
            UpdateEstimatedUI();
        }

        private void startDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            UpdateEstimatedUI();
        }

        private void endDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            UpdateEstimatedUI();
        }

        private void startTimeEdit_EditValueChanged(object sender, EventArgs e)
        {
            UpdateEstimatedUI();
        }

        private void endTimeEdit_EditValueChanged(object sender, EventArgs e)
        {
            UpdateEstimatedUI();
        }

        private void cmbLocation_EditValueChanged(object sender, EventArgs e)
        {
            var location = cmbLocation.Text?.Trim() ?? "";

            var item = _locations.FirstOrDefault(x => x.Location == location);
            txtArea.Text = item?.Area ?? "";   // 找不到就清空（或你也可顯示 "未知"）
        }

        private void cmbProjectNo_EditValueChanged(object sender, EventArgs e)
        {
            var projectNo = cmbProjectNo.Text?.Trim() ?? "";
            var p = _projects.FirstOrDefault(x => x.ProjectNo == projectNo);

            txtProjectName.Text = p?.ProjectName ?? "";
            txtPE.Text = p?.PE ?? "";
        }

        // =========================
        // 5) UI 套用：午餐 / 晚餐規則
        // =========================
        private void ApplyLunchUI()
        {
            txtLunchMinutes.Properties.ReadOnly = true;
            txtLunchMinutes.Text = chkHasLunch.Checked ? "60" : "0";
        }

        private void ApplyDinnerUI()
        {
            txtDinnerMinutes.Properties.ReadOnly = !chkHasDinner.Checked;

            if (!chkHasDinner.Checked)
            {
                txtDinnerMinutes.Text = "0";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDinnerMinutes.Text))
            {
                txtDinnerMinutes.Text = "0";
                return;
            }
        }

        // =========================
        // 6) 從 UI 組出 Model
        // =========================
        private RentTime BuildModelFormUI()
        {
            return new RentTime
            {
                CreatedBy = txtCreatedBy.Text.Trim(),
                Area = txtArea.Text.Trim(),
                CustomerName = txtCustomerName.Text.Trim(),
                Sales = txtSales.Text.Trim(),
                ProjectName = txtProjectName.Text.Trim(),
                PE = txtPE.Text.Trim(),
                ProjectNo = cmbProjectNo.Text.Trim(),
                Location = cmbLocation.Text.Trim(),

                ContactName = txtContact.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                TestInformation = memoTestInformation.Text.Trim(),
                EngineerName = txtEngineer.Text.Trim(),
                SampleModel = txtSampleModel.Text.Trim(),
                SampleNo = txtSampleNo.Text.Trim(),
                TestMode = txtTestMode.Text.Trim(),
                TestItem = txtTestItem.Text.Trim(),
                Notes = memoNote.Text.Trim(),

                // 午餐/晚餐
                HasLunch = chkHasLunch.Checked,
                LunchMinutes = chkHasLunch.Checked ? 60 : 0,

                HasDinner = chkHasDinner.Checked,
                DinnerMinutes = chkHasDinner.Checked ? ParseIntOrZero(txtDinnerMinutes.Text) : 0,

                StartDate = startDateEdit.EditValue as DateTime?,
                EndDate = endDateEdit.EditValue as DateTime?,
                StartTime = startTimeEdit.EditValue is DateTime t1 ? t1.TimeOfDay : (TimeSpan?)null,
                EndTime = endTimeEdit.EditValue is DateTime t2 ? t2.TimeOfDay : (TimeSpan?)null,
            };
        }

        // =========================
        // 7) 預估時間：只負責顯示在 UI
        // =========================
        private void UpdateEstimatedUI()
        {
            var startDate = startDateEdit.EditValue as DateTime?;
            var endDate = endDateEdit.EditValue as DateTime?;
            var startTime = startTimeEdit.EditValue is DateTime t1 ? t1.TimeOfDay : (TimeSpan?)null;
            var endTime = endTimeEdit.EditValue is DateTime t2 ? t2.TimeOfDay : (TimeSpan?)null;

            if (startDate is null || endDate is null || startTime is null || endTime is null)
            {
                txtEstimatedHours.Text = "請輸入開始/結束日期時間";
                return;
            }

            var start = startDate.Value.Date + startTime.Value;
            var end = endDate.Value.Date + endTime.Value;

            if (end < start)
            {
                txtEstimatedHours.Text = "結束時間不能早於開始時間";
                return;
            }

            var minutes = (int)(end - start).TotalMinutes;

            if (chkHasLunch.Checked) minutes -= 60;
            if (chkHasDinner.Checked) minutes -= ParseIntOrZero(txtDinnerMinutes.Text);

            if (minutes < 0) minutes = 0;

            var hours = Math.Round(minutes / 60m, 2);

            txtEstimatedHours.Text = $"{minutes} 分鐘 ({hours} 小時)";
        }

        // =========================
        // 8) 小工具
        // =========================
        private static int ParseIntOrZero(string? s)
        {
            return int.TryParse(s?.Trim(), out var v) ? v : 0;
        }

        // =========================
        // 9) 區域綁定
        // =========================

        private readonly List<LocationItem> _locations = new()
        {
            new LocationItem { Location = "Conducted 1", Area = "五股" },
            new LocationItem { Location = "Conducted 2", Area = "五股" },
            new LocationItem { Location = "Conducted 3", Area = "五股" },
            new LocationItem { Location = "Conducted 4", Area = "五股" },
            new LocationItem { Location = "Conducted 5", Area = "五股" },
            new LocationItem { Location = "Conducted 6", Area = "五股" },
            new LocationItem { Location = "SAC 1", Area = "五股" },
            new LocationItem { Location = "SAC 2", Area = "五股" },
            new LocationItem { Location = "SAC 3", Area = "五股" },
            new LocationItem { Location = "FAC 1", Area = "五股" },
            new LocationItem { Location = "Setup Room 1", Area = "五股" },
            new LocationItem { Location = "Conducted A", Area = "華亞" },
            new LocationItem { Location = "Conducted B", Area = "華亞" },
            new LocationItem { Location = "Conducted C", Area = "華亞" },
            new LocationItem { Location = "Conducted D", Area = "華亞" },
            new LocationItem { Location = "Conducted E", Area = "華亞" },
            new LocationItem { Location = "Conducted F", Area = "華亞" },
            new LocationItem { Location = "SAC C", Area = "華亞" },
            new LocationItem { Location = "SAC D", Area = "華亞" },
            new LocationItem { Location = "SAC G", Area = "華亞" },
            new LocationItem { Location = "FAC A", Area = "華亞" },
            new LocationItem { Location = "Setup Room A", Area = "華亞" },
        };

        private readonly List<ProjectItem> _projects = new()
        {
            new() { ProjectNo = "TE25113000123456", ProjectName = "Smart 5G WPC", PE = "Martin_Liu" },
            new() { ProjectNo = "TE25113000123789", ProjectName = "Smart 2.4G WPC", PE = "Martin_Liu" },
        };

    }
}