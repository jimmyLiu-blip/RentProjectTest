namespace RentProject
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            btnAddRentTime = new DevExpress.XtraBars.BarButtonItem();
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            barManager1 = new DevExpress.XtraBars.BarManager(components);
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            btnTestConnection = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).BeginInit();
            SuspendLayout();
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.ItemLinks.Add(btnAddRentTime);
            ribbonPageGroup1.ItemLinks.Add(btnTestConnection);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "管理租時單";
            // 
            // btnAddRentTime
            // 
            btnAddRentTime.Caption = "新增租時單";
            btnAddRentTime.Id = 1;
            btnAddRentTime.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAddRentTime.ImageOptions.Image");
            btnAddRentTime.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnAddRentTime.ImageOptions.LargeImage");
            btnAddRentTime.Name = "btnAddRentTime";
            btnAddRentTime.ItemClick += btnAddRentTime_ItemClick;
            // 
            // ribbonControl1
            // 
            ribbonControl1.AllowMinimizeRibbon = false;
            ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(50, 51, 50, 51);
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, btnAddRentTime, btnTestConnection });
            ribbonControl1.Location = new System.Drawing.Point(0, 0);
            ribbonControl1.Margin = new System.Windows.Forms.Padding(5);
            ribbonControl1.MaxItemId = 3;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.OptionsMenuMinWidth = 550;
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2019;
            ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowPageHeadersInFormCaption = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            ribbonControl1.ShowToolbarCustomizeItem = false;
            ribbonControl1.Size = new System.Drawing.Size(1859, 237);
            ribbonControl1.Toolbar.ShowCustomizeItem = false;
            // 
            // barManager1
            // 
            barManager1.DockControls.Add(barDockControlTop);
            barManager1.DockControls.Add(barDockControlBottom);
            barManager1.DockControls.Add(barDockControlLeft);
            barManager1.DockControls.Add(barDockControlRight);
            barManager1.Form = this;
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlTop.Location = new System.Drawing.Point(0, 0);
            barDockControlTop.Manager = barManager1;
            barDockControlTop.Size = new System.Drawing.Size(1859, 0);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlBottom.Location = new System.Drawing.Point(0, 984);
            barDockControlBottom.Manager = barManager1;
            barDockControlBottom.Size = new System.Drawing.Size(1859, 0);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            barDockControlLeft.Manager = barManager1;
            barDockControlLeft.Size = new System.Drawing.Size(0, 984);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            barDockControlRight.Location = new System.Drawing.Point(1859, 0);
            barDockControlRight.Manager = barManager1;
            barDockControlRight.Size = new System.Drawing.Size(0, 984);
            // 
            // btnTestConnection
            // 
            btnTestConnection.Caption = "連線測試";
            btnTestConnection.Id = 2;
            btnTestConnection.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnTestConnection.ImageOptions.Image");
            btnTestConnection.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("btnTestConnection.ImageOptions.LargeImage");
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.ItemClick += btnTestConnection_ItemClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1859, 984);
            Controls.Add(ribbonControl1);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            IconOptions.LargeImage = (System.Drawing.Image)resources.GetObject("Form1.IconOptions.LargeImage");
            Margin = new System.Windows.Forms.Padding(5);
            Name = "Form1";
            Ribbon = ribbonControl1;
            Text = "RentalSystem";
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem btnAddRentTime;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem btnTestConnection;
    }
}

