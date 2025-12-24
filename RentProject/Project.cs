using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
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
        private readonly DapperProjectRepository _projectRepo;
        private List<ProjectItem> _projects = new();

        private static readonly TimeSpan LunchEnableAt = new(13, 0, 0);
        private static readonly TimeSpan DinnerEnableAt = new(18, 0, 0);

        // =========================
        // 1) 欄位 / 建構子
        // =========================
        public Project(RentTimeService service, DapperProjectRepository projectRepo)
        {
            InitializeComponent();
            _service = service;
            _projectRepo = projectRepo;
        }

        // =========================
        // 2) Form Load：初始化 UI
        // =========================
        private void Project_Load(object sender, EventArgs e)
        {
            _projects = _projectRepo.GetActiveProject();

            cmbProjectNo.Properties.Items.Clear();
            cmbProjectNo.Properties.Items.AddRange(_projects.Select(p => p.ProjectNo).ToArray());

            startDateEdit.EditValue = DateTime.Today;
            endDateEdit.EditValue = DateTime.Today;

            startTimeEdit.EditValue = DateTime.Now;
            endTimeEdit.EditValue = DateTime.Now;

            ApplyMealEnableByEndTime();
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
            ApplyMealEnableByEndTime();
            UpdateEstimatedUI();
        }

        private void endDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            ApplyMealEnableByEndTime();
            UpdateEstimatedUI();
        }

        private void startTimeEdit_EditValueChanged(object sender, EventArgs e)
        {
            ApplyMealEnableByEndTime();
            UpdateEstimatedUI();
        }

        private void endTimeEdit_EditValueChanged(object sender, EventArgs e)
        {
            ApplyMealEnableByEndTime();
            UpdateEstimatedUI();
        }

        private void txtDinnerMinutes_EditValueChanged(object sender, EventArgs e)
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
            txtPE.Text = p?.JobPM ?? "";
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

        private void ApplyMealEnableByEndTime()
        {
            // 取結束日期 + 結束時間
            var endDate = endDateEdit.EditValue as DateTime?;
            var endTime = endTimeEdit.EditValue is DateTime t ? t.TimeOfDay : (TimeSpan?)null;

            // 預設：不可勾選
            bool canLunch = false;
            bool canDinner = false;

            // 只要結束日期/結束時間有效，才能判斷門檻
            if (endDate is not null && endTime is not null)
            {
                var end = endDate.Value.Date + endTime.Value;
                canLunch = end.TimeOfDay >= LunchEnableAt;
                canDinner = end.TimeOfDay >= DinnerEnableAt;
            }

            // 午餐：不到 13:00 → 禁用 + 強制取消
            chkHasLunch.Enabled = canLunch;
            if (!canLunch) chkHasLunch.Checked = false;

            // 晚餐：不到 18:00 → 禁用 + 強制取消
            chkHasDinner.Enabled = canDinner;
            if (!canDinner) chkHasDinner.Checked = false;

            // 依目前勾選狀態，把分鐘/唯讀套回去
            ApplyLunchUI();
            ApplyDinnerUI();
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
    }
}