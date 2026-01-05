using DevExpress.XtraEditors;
using RentProject.Domain;
using RentProject.Repository;
using RentProject.Service;
using RentProject.Shared.DTO;
using RentProject.UIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;


namespace RentProject
{
    public partial class Project : XtraForm
    {
        // =========================
        // 1) 欄位 / 常數 / 資料來源
        // =========================
        private readonly RentTimeService _rentTimeService;
        private readonly ProjectService _projectService;

        private List<ProjectLookupRow> _projects = new();

        private static readonly TimeSpan LunchEnableAt = new(13, 0, 0);
        private static readonly TimeSpan DinnerEnableAt = new(18, 0, 0);

        // 編輯租時單
        private readonly int? _editRentTimeId = null; // 要用 .Value 把「nullable 裡面的那個 int 值」拿出來。

        private readonly List<LocationItem> _locations = new()
        {
            new LocationItem { Location = "Conducted 1", Area = "WG" },
            new LocationItem { Location = "Conducted 2", Area = "WG" },
            new LocationItem { Location = "Conducted 3", Area = "WG" },
            new LocationItem { Location = "Conducted 4", Area = "WG" },
            new LocationItem { Location = "Conducted 5", Area = "WG" },
            new LocationItem { Location = "Conducted 6", Area = "WG" },
            new LocationItem { Location = "SAC 1", Area = "WG" },
            new LocationItem { Location = "SAC 2", Area = "WG" },
            new LocationItem { Location = "SAC 3", Area = "WG" },
            new LocationItem { Location = "FAC 1", Area = "WG" },
            new LocationItem { Location = "Setup Room 1", Area = "WG" },
            new LocationItem { Location = "Conducted A", Area = "HY" },
            new LocationItem { Location = "Conducted B", Area = "HY" },
            new LocationItem { Location = "Conducted C", Area = "HY" },
            new LocationItem { Location = "Conducted D", Area = "HY" },
            new LocationItem { Location = "Conducted E", Area = "HY" },
            new LocationItem { Location = "Conducted F", Area = "HY" },
            new LocationItem { Location = "SAC C", Area = "HY" },
            new LocationItem { Location = "SAC D", Area = "HY" },
            new LocationItem { Location = "SAC G", Area = "HY" },
            new LocationItem { Location = "FAC A", Area = "HY" },
            new LocationItem { Location = "Setup Room A", Area = "HY" },
        };

        // =========================
        // 2) 建構子
        // =========================
        public Project(RentTimeService rentTimeService, ProjectService projectService)
        {
            InitializeComponent();
            _rentTimeService = rentTimeService;
            _projectService = projectService;
        }

        public Project(RentTimeService rentTimeService, ProjectService projectService, int rentTimeId) : this(rentTimeService, projectService)
        {
            _editRentTimeId = rentTimeId;
        }

        // =========================
        // 3) Form Load：初始化 UI
        // =========================
        private void Project_Load(object sender, EventArgs e)
        {
            _projects = _projectService.GetProjectLookup();

            cmbProjectNo.Properties.Items.Clear();
            cmbProjectNo.Properties.Items.AddRange(_projects.Select(p => p.ProjectNo).ToArray());

            startDateEdit.EditValue = null;
            endDateEdit.EditValue = null;

            startTimeEdit.EditValue = null;
            endTimeEdit.EditValue = null;

            RefreshMealAndEstimateUI();

            btnDeletedRentTime.Visible = _editRentTimeId != null;

            if (_editRentTimeId != null)
            {
                var data = _rentTimeService.GetRentTimeById(_editRentTimeId.Value);

                FillUIFromModel(data);

                btnCreatedRentTime.Text = "儲存修改";

                this.Text = "修改租時單";
            }
        }

        // =========================
        // 4) 主要流程：按鈕事件
        // =========================
        private void btnCreatedRentTime_Click(object sender, EventArgs e)
        {
            try
            {
                var model = BuildModelFormUI();

                // 新增模式：照舊 Create
                if (_editRentTimeId == null)
                {
                    var result = _rentTimeService.CreateRentTime(model);

                    txtBookingNo.Text = result.BookingNo;

                    XtraMessageBox.Show(
                        $"建立成功! \nRentTimeId：{result.RentTimeId}\nBookingNo：{result.BookingNo}",
                        "CreateRentTime");

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                    return;
                }

                // 編輯模式：走 Update（最重要：要把 RentTimeId 帶回 model）
                model.RentTimeId = _editRentTimeId.Value;

                var confirm = XtraMessageBox.Show(
                    "確認儲存修改嗎?",
                    "確認儲存",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );

                if (confirm != DialogResult.Yes)
                { return; } // 使用者按 No，就不更新、也不關窗

                _rentTimeService.UpdateRentTimeById(model);

                this.DialogResult = System.Windows.Forms.DialogResult.OK; // 表示：「我這個對話框是成功完成的（使用者按了儲存）」
                this.Close(); // 把這個表單關掉，讓 ShowDialog() 結束並把結果回傳給呼叫端。
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Error");
            }
        }

        private void btnDeletedRentTime_Click(object sender, EventArgs e)
        {
            try
            {
                if (_editRentTimeId == null) return;

                var confirm = XtraMessageBox.Show(
                   "確認刪除嗎?",
                   "確認刪除",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
                   );

                if (confirm != DialogResult.Yes)
                { return; }

                var createdBy = txtCreatedBy.Text.Trim();

                _rentTimeService.DeletedRentTime(_editRentTimeId.Value, createdBy, DateTime.Now);

                this.DialogResult = System.Windows.Forms.DialogResult.OK; 
                this.Close(); 
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"{ex.GetType().Name} - {ex.Message}", "Error");
            }
        }

        // =========================
        // 5) 控制項事件：只負責觸發更新
        // =========================

        // 5-1 勾選午/晚餐
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

        // 5-2 日期/時間改變
        private void startDateEdit_EditValueChanged(object sender, EventArgs e) => RefreshMealAndEstimateUI();
        private void endDateEdit_EditValueChanged(object sender, EventArgs e) => RefreshMealAndEstimateUI();
        private void startTimeEdit_EditValueChanged(object sender, EventArgs e) => RefreshMealAndEstimateUI();
        private void endTimeEdit_EditValueChanged(object sender, EventArgs e) => RefreshMealAndEstimateUI();

        // 5-3 晚餐分鐘改變
        private void txtDinnerMinutes_EditValueChanged(object sender, EventArgs e)
        {
            UpdateEstimatedUI();
        }

        // 5-4 Location / ProjectNo 連動填值
        private void cmbLocation_EditValueChanged(object sender, EventArgs e)
        {
            var location = cmbLocation.Text?.Trim() ?? "";
            var item = _locations.FirstOrDefault(x => x.Location == location);
            txtArea.Text = item?.Area ?? "";
        }

        private void cmbProjectNo_EditValueChanged(object sender, EventArgs e)
        {
            var projectNo = cmbProjectNo.Text?.Trim() ?? "";
            var p = _projects.FirstOrDefault(x => x.ProjectNo == projectNo);

            txtProjectName.Text = p?.ProjectName ?? "";
            txtPE.Text = p?.ProjectEngineer ?? "";
        }

        // =========================
        // 6) UI 套用：午餐 / 晚餐規則
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
            var startDate = startDateEdit.EditValue as DateTime?;
            var startTime = startTimeEdit.EditValue is DateTime t1 ? t1.TimeOfDay : (TimeSpan?)null;
            var endDate = endDateEdit.EditValue as DateTime?;
            var endTime = endTimeEdit.EditValue is DateTime t2 ? t2.TimeOfDay : (TimeSpan?)null;

            bool canLunch = false;
            bool canDinner = false;

            if (startDate is not null && startTime is not null && endDate is not null && endTime is not null)
            {
                var start = startDate.Value.Date + startTime.Value;
                var end = endDate.Value.Date + endTime.Value;
                canLunch = end.TimeOfDay >= LunchEnableAt && start.TimeOfDay < LunchEnableAt;
                canDinner = end.TimeOfDay >= DinnerEnableAt && start.TimeOfDay < DinnerEnableAt;
            }

            chkHasLunch.Enabled = canLunch;
            if (!canLunch) chkHasLunch.Checked = false;

            chkHasDinner.Enabled = canDinner;
            if (!canDinner) chkHasDinner.Checked = false;

            ApplyLunchUI();
            ApplyDinnerUI();
        }

        // =========================
        // 7) 從 UI 組出 Model
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

        private void FillUIFromModel(RentTime data)
        {
            // 文字訊息
            txtBookingNo.Text = data.BookingNo ?? "";
            txtCreatedBy.Text = data.CreatedBy ?? "";
            txtArea.Text = data.Area ?? "";
            txtCustomerName.Text = data.CustomerName ?? "";
            txtSales.Text = data.Sales ?? "";
            cmbProjectNo.Text = data.ProjectNo ?? "";
            txtProjectName.Text = data.ProjectName ?? "";
            txtPE.Text = data.PE ?? "";
            cmbLocation.Text = data.Location ?? "";

            txtContact.Text = data.ContactName ?? "";
            txtPhone.Text = data.Phone ?? "";
            memoTestInformation.Text = data.TestInformation ?? "";
            txtEngineer.Text = data.EngineerName ?? "";
            txtSampleModel.Text = data.SampleModel ?? "";
            txtSampleNo.Text = data.SampleNo ?? "";
            txtTestMode.Text = data.TestMode ?? "";
            txtTestItem.Text = data.TestItem ?? "";
            memoNote.Text = data.Notes ?? "";

            //日期
            startDateEdit.EditValue = data.StartDate;
            endDateEdit.EditValue = data.EndDate;

            //時間：TimeEdit 的 EditValue 通常要 DateTime，所以把 TimeSpan 轉成「今天日期 + TimeSpan」
            startTimeEdit.EditValue = data.StartTime.HasValue ? DateTime.Today.Add(data.StartTime.Value) : null;
            endTimeEdit.EditValue = data.EndTime.HasValue ? DateTime.Today.Add(data.EndTime.Value) : null;

            // 午餐/晚餐
            chkHasLunch.Checked = data.HasLunch;
            chkHasDinner.Checked = data.HasDinner;

            txtLunchMinutes.Text = data.HasLunch ? data.LunchMinutes.ToString() : "0";
            txtDinnerMinutes.Text = data.HasDinner ? data.DinnerMinutes.ToString() : "0";

            // 讓 UI 規則與預估時間重新刷新一次
            RefreshMealAndEstimateUI();
        }

        // =========================
        // 8) 預估時間：只負責顯示在 UI
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
        // 9) 小工具 / 集中刷新
        // =========================
        private static int ParseIntOrZero(string? s)
            => int.TryParse(s?.Trim(), out var v) ? v : 0;  //TryParse(...) 的回傳值是「有沒有成功」→ bool

        private void RefreshMealAndEstimateUI()
        {
            ApplyMealEnableByEndTime();
            UpdateEstimatedUI();
        }


    }
}
