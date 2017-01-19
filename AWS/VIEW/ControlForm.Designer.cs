namespace AWS.VIEW
{
    partial class ControlForm
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
            this.BufferClearButton = new DevComponents.DotNetBar.ButtonX();
            this.PowerResetButton = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.txtLocationNumber = new System.Windows.Forms.TextBox();
            this.LocationNumberButton = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.PasswordButton = new DevComponents.DotNetBar.ButtonX();
            this.TimeSyncButton = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel3 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.endNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.startNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1.SuspendLayout();
            this.groupPanel2.SuspendLayout();
            this.groupPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // BufferClearButton
            // 
            this.BufferClearButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BufferClearButton.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.BufferClearButton.Location = new System.Drawing.Point(28, 81);
            this.BufferClearButton.Name = "BufferClearButton";
            this.BufferClearButton.Size = new System.Drawing.Size(117, 39);
            this.BufferClearButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.BufferClearButton.TabIndex = 1;
            this.BufferClearButton.Text = "버퍼 클리어";
            this.BufferClearButton.Click += new System.EventHandler(this.BufferClearButton_Click);
            // 
            // PowerResetButton
            // 
            this.PowerResetButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.PowerResetButton.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.PowerResetButton.Location = new System.Drawing.Point(28, 12);
            this.PowerResetButton.Name = "PowerResetButton";
            this.PowerResetButton.Size = new System.Drawing.Size(117, 39);
            this.PowerResetButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.PowerResetButton.TabIndex = 2;
            this.PowerResetButton.Text = "재시작";
            this.PowerResetButton.Click += new System.EventHandler(this.buttonX3_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.txtLocationNumber);
            this.groupPanel1.Controls.Add(this.LocationNumberButton);
            this.groupPanel1.Location = new System.Drawing.Point(234, 12);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(253, 108);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 5;
            this.groupPanel1.Text = "Location Number";
            // 
            // txtLocationNumber
            // 
            this.txtLocationNumber.Location = new System.Drawing.Point(26, 30);
            this.txtLocationNumber.Name = "txtLocationNumber";
            this.txtLocationNumber.Size = new System.Drawing.Size(137, 21);
            this.txtLocationNumber.TabIndex = 5;
            // 
            // LocationNumberButton
            // 
            this.LocationNumberButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.LocationNumberButton.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LocationNumberButton.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.LocationNumberButton.Location = new System.Drawing.Point(169, 19);
            this.LocationNumberButton.Name = "LocationNumberButton";
            this.LocationNumberButton.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2);
            this.LocationNumberButton.Size = new System.Drawing.Size(62, 43);
            this.LocationNumberButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.Metro;
            this.LocationNumberButton.TabIndex = 4;
            this.LocationNumberButton.Text = "SEND";
            this.LocationNumberButton.Click += new System.EventHandler(this.LocationNumberButton_Click);
            // 
            // groupPanel2
            // 
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.txtPassword);
            this.groupPanel2.Controls.Add(this.PasswordButton);
            this.groupPanel2.Location = new System.Drawing.Point(234, 138);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(253, 108);
            // 
            // 
            // 
            this.groupPanel2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel2.Style.BackColorGradientAngle = 90;
            this.groupPanel2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderBottomWidth = 1;
            this.groupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderLeftWidth = 1;
            this.groupPanel2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderRightWidth = 1;
            this.groupPanel2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderTopWidth = 1;
            this.groupPanel2.Style.CornerDiameter = 4;
            this.groupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel2.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel2.TabIndex = 6;
            this.groupPanel2.Text = "Password ";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(26, 35);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(137, 21);
            this.txtPassword.TabIndex = 6;
            // 
            // PasswordButton
            // 
            this.PasswordButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.PasswordButton.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.PasswordButton.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.PasswordButton.Location = new System.Drawing.Point(169, 23);
            this.PasswordButton.Name = "PasswordButton";
            this.PasswordButton.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2);
            this.PasswordButton.Size = new System.Drawing.Size(62, 43);
            this.PasswordButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.Metro;
            this.PasswordButton.TabIndex = 3;
            this.PasswordButton.Text = "SEND";
            this.PasswordButton.Click += new System.EventHandler(this.PasswordButton_Click);
            // 
            // TimeSyncButton
            // 
            this.TimeSyncButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.TimeSyncButton.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.TimeSyncButton.Location = new System.Drawing.Point(28, 154);
            this.TimeSyncButton.Name = "TimeSyncButton";
            this.TimeSyncButton.Size = new System.Drawing.Size(117, 39);
            this.TimeSyncButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.TimeSyncButton.TabIndex = 7;
            this.TimeSyncButton.Text = "시간 동기화";
            this.TimeSyncButton.Click += new System.EventHandler(this.TimeSyncButton_Click);
            // 
            // groupPanel3
            // 
            this.groupPanel3.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel3.Controls.Add(this.endNumericUpDown);
            this.groupPanel3.Controls.Add(this.startNumericUpDown);
            this.groupPanel3.Controls.Add(this.labelX2);
            this.groupPanel3.Controls.Add(this.labelX1);
            this.groupPanel3.Controls.Add(this.monthCalendar1);
            this.groupPanel3.Controls.Add(this.buttonX1);
            this.groupPanel3.Location = new System.Drawing.Point(28, 271);
            this.groupPanel3.Name = "groupPanel3";
            this.groupPanel3.Size = new System.Drawing.Size(459, 207);
            // 
            // 
            // 
            this.groupPanel3.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel3.Style.BackColorGradientAngle = 90;
            this.groupPanel3.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel3.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderBottomWidth = 1;
            this.groupPanel3.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel3.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderLeftWidth = 1;
            this.groupPanel3.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderRightWidth = 1;
            this.groupPanel3.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel3.Style.BorderTopWidth = 1;
            this.groupPanel3.Style.CornerDiameter = 4;
            this.groupPanel3.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel3.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel3.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel3.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel3.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel3.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel3.TabIndex = 7;
            this.groupPanel3.Text = "과거자료";
            // 
            // endNumericUpDown
            // 
            this.endNumericUpDown.Location = new System.Drawing.Point(241, 110);
            this.endNumericUpDown.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.endNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.endNumericUpDown.Name = "endNumericUpDown";
            this.endNumericUpDown.Size = new System.Drawing.Size(75, 21);
            this.endNumericUpDown.TabIndex = 8;
            this.endNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // startNumericUpDown
            // 
            this.startNumericUpDown.Location = new System.Drawing.Point(241, 38);
            this.startNumericUpDown.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.startNumericUpDown.Name = "startNumericUpDown";
            this.startNumericUpDown.Size = new System.Drawing.Size(75, 21);
            this.startNumericUpDown.TabIndex = 7;
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(241, 72);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 6;
            this.labelX2.Text = "종료시간";
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(241, 9);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 5;
            this.labelX1.Text = "시작시간";
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(9, 9);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 4;
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.buttonX1.Location = new System.Drawing.Point(380, 138);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2);
            this.buttonX1.Size = new System.Drawing.Size(62, 33);
            this.buttonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.Metro;
            this.buttonX1.TabIndex = 3;
            this.buttonX1.Text = "SEND";
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 504);
            this.ControlBox = false;
            this.Controls.Add(this.groupPanel3);
            this.Controls.Add(this.TimeSyncButton);
            this.Controls.Add(this.groupPanel2);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.PowerResetButton);
            this.Controls.Add(this.BufferClearButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Control";
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            this.groupPanel2.ResumeLayout(false);
            this.groupPanel2.PerformLayout();
            this.groupPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.endNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevComponents.DotNetBar.ButtonX BufferClearButton;
        private DevComponents.DotNetBar.ButtonX PowerResetButton;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.TextBox txtLocationNumber;
        private DevComponents.DotNetBar.ButtonX LocationNumberButton;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        private System.Windows.Forms.TextBox txtPassword;
        private DevComponents.DotNetBar.ButtonX PasswordButton;
        private DevComponents.DotNetBar.ButtonX TimeSyncButton;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel3;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private System.Windows.Forms.NumericUpDown endNumericUpDown;
        private System.Windows.Forms.NumericUpDown startNumericUpDown;
    }
}