
namespace C969_Task_1
{
    partial class FormScheduling
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
            this.dataGridViewAppts = new System.Windows.Forms.DataGridView();
            this.groupBoxAppointments = new System.Windows.Forms.GroupBox();
            this.textBoxApptType = new System.Windows.Forms.TextBox();
            this.labelApptType = new System.Windows.Forms.Label();
            this.labelConsultant = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.labelCustomerSelect = new System.Windows.Forms.Label();
            this.comboBoxTimeForAppt = new System.Windows.Forms.ComboBox();
            this.comboBoxCustomerForAppt = new System.Windows.Forms.ComboBox();
            this.comboBoxConsultant = new System.Windows.Forms.ComboBox();
            this.dateTimePickerForAppt = new System.Windows.Forms.DateTimePicker();
            this.buttonRemoveCancelAppt = new System.Windows.Forms.Button();
            this.buttonEditAppt = new System.Windows.Forms.Button();
            this.buttonAddSaveAppt = new System.Windows.Forms.Button();
            this.groupBoxCustomer = new System.Windows.Forms.GroupBox();
            this.labelCountry = new System.Windows.Forms.Label();
            this.textBoxCountry = new System.Windows.Forms.TextBox();
            this.labelZip = new System.Windows.Forms.Label();
            this.textBoxZip = new System.Windows.Forms.TextBox();
            this.labelCity = new System.Windows.Forms.Label();
            this.textBoxCity = new System.Windows.Forms.TextBox();
            this.labelPhoneNumber = new System.Windows.Forms.Label();
            this.labelAddress = new System.Windows.Forms.Label();
            this.labelFirstName = new System.Windows.Forms.Label();
            this.buttonRemoveCancelCustomer = new System.Windows.Forms.Button();
            this.buttonEditCustomer = new System.Windows.Forms.Button();
            this.textBoxPhoneNumber = new System.Windows.Forms.TextBox();
            this.buttonAddSaveCustomer = new System.Windows.Forms.Button();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.menuStripScheduling = new System.Windows.Forms.MenuStrip();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numberOfAppointmentTypesByMonthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.theScheduleForEachConsultantToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oneOtherTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxCalendarView = new System.Windows.Forms.GroupBox();
            this.radioButtonByWeek = new System.Windows.Forms.RadioButton();
            this.radioButtonByMonth = new System.Windows.Forms.RadioButton();
            this.monthCalendarAppts = new System.Windows.Forms.MonthCalendar();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAppts)).BeginInit();
            this.groupBoxAppointments.SuspendLayout();
            this.groupBoxCustomer.SuspendLayout();
            this.menuStripScheduling.SuspendLayout();
            this.groupBoxCalendarView.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewAppts
            // 
            this.dataGridViewAppts.AllowUserToAddRows = false;
            this.dataGridViewAppts.AllowUserToDeleteRows = false;
            this.dataGridViewAppts.AllowUserToResizeRows = false;
            this.dataGridViewAppts.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dataGridViewAppts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewAppts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAppts.Location = new System.Drawing.Point(394, 207);
            this.dataGridViewAppts.MultiSelect = false;
            this.dataGridViewAppts.Name = "dataGridViewAppts";
            this.dataGridViewAppts.ReadOnly = true;
            this.dataGridViewAppts.RowHeadersVisible = false;
            this.dataGridViewAppts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAppts.ShowEditingIcon = false;
            this.dataGridViewAppts.Size = new System.Drawing.Size(608, 302);
            this.dataGridViewAppts.TabIndex = 0;
            this.dataGridViewAppts.SelectionChanged += new System.EventHandler(this.dataGridViewAppts_SelectionChanged);
            // 
            // groupBoxAppointments
            // 
            this.groupBoxAppointments.Controls.Add(this.textBoxApptType);
            this.groupBoxAppointments.Controls.Add(this.labelApptType);
            this.groupBoxAppointments.Controls.Add(this.labelConsultant);
            this.groupBoxAppointments.Controls.Add(this.labelTime);
            this.groupBoxAppointments.Controls.Add(this.labelDate);
            this.groupBoxAppointments.Controls.Add(this.labelCustomerSelect);
            this.groupBoxAppointments.Controls.Add(this.comboBoxTimeForAppt);
            this.groupBoxAppointments.Controls.Add(this.comboBoxCustomerForAppt);
            this.groupBoxAppointments.Controls.Add(this.comboBoxConsultant);
            this.groupBoxAppointments.Controls.Add(this.dateTimePickerForAppt);
            this.groupBoxAppointments.Controls.Add(this.buttonRemoveCancelAppt);
            this.groupBoxAppointments.Controls.Add(this.buttonEditAppt);
            this.groupBoxAppointments.Controls.Add(this.buttonAddSaveAppt);
            this.groupBoxAppointments.Location = new System.Drawing.Point(12, 75);
            this.groupBoxAppointments.Name = "groupBoxAppointments";
            this.groupBoxAppointments.Size = new System.Drawing.Size(376, 214);
            this.groupBoxAppointments.TabIndex = 1;
            this.groupBoxAppointments.TabStop = false;
            this.groupBoxAppointments.Text = "Appointments";
            // 
            // textBoxApptType
            // 
            this.textBoxApptType.Location = new System.Drawing.Point(133, 132);
            this.textBoxApptType.Name = "textBoxApptType";
            this.textBoxApptType.Size = new System.Drawing.Size(206, 20);
            this.textBoxApptType.TabIndex = 5;
            // 
            // labelApptType
            // 
            this.labelApptType.AutoSize = true;
            this.labelApptType.Location = new System.Drawing.Point(130, 116);
            this.labelApptType.Name = "labelApptType";
            this.labelApptType.Size = new System.Drawing.Size(31, 13);
            this.labelApptType.TabIndex = 14;
            this.labelApptType.Text = "Type";
            // 
            // labelConsultant
            // 
            this.labelConsultant.AutoSize = true;
            this.labelConsultant.Location = new System.Drawing.Point(3, 116);
            this.labelConsultant.Name = "labelConsultant";
            this.labelConsultant.Size = new System.Drawing.Size(57, 13);
            this.labelConsultant.TabIndex = 12;
            this.labelConsultant.Text = "Consultant";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(215, 60);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(30, 13);
            this.labelTime.TabIndex = 11;
            this.labelTime.Text = "Time";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Location = new System.Drawing.Point(6, 60);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(30, 13);
            this.labelDate.TabIndex = 10;
            this.labelDate.Text = "Date";
            // 
            // labelCustomerSelect
            // 
            this.labelCustomerSelect.AutoSize = true;
            this.labelCustomerSelect.Location = new System.Drawing.Point(7, 17);
            this.labelCustomerSelect.Name = "labelCustomerSelect";
            this.labelCustomerSelect.Size = new System.Drawing.Size(51, 13);
            this.labelCustomerSelect.TabIndex = 9;
            this.labelCustomerSelect.Text = "Customer";
            // 
            // comboBoxTimeForAppt
            // 
            this.comboBoxTimeForAppt.FormattingEnabled = true;
            this.comboBoxTimeForAppt.Location = new System.Drawing.Point(218, 76);
            this.comboBoxTimeForAppt.Name = "comboBoxTimeForAppt";
            this.comboBoxTimeForAppt.Size = new System.Drawing.Size(121, 21);
            this.comboBoxTimeForAppt.TabIndex = 3;
            // 
            // comboBoxCustomerForAppt
            // 
            this.comboBoxCustomerForAppt.FormattingEnabled = true;
            this.comboBoxCustomerForAppt.Location = new System.Drawing.Point(6, 36);
            this.comboBoxCustomerForAppt.Name = "comboBoxCustomerForAppt";
            this.comboBoxCustomerForAppt.Size = new System.Drawing.Size(121, 21);
            this.comboBoxCustomerForAppt.TabIndex = 1;
            // 
            // comboBoxConsultant
            // 
            this.comboBoxConsultant.FormattingEnabled = true;
            this.comboBoxConsultant.Location = new System.Drawing.Point(6, 132);
            this.comboBoxConsultant.Name = "comboBoxConsultant";
            this.comboBoxConsultant.Size = new System.Drawing.Size(121, 21);
            this.comboBoxConsultant.TabIndex = 4;
            // 
            // dateTimePickerForAppt
            // 
            this.dateTimePickerForAppt.Location = new System.Drawing.Point(7, 76);
            this.dateTimePickerForAppt.Name = "dateTimePickerForAppt";
            this.dateTimePickerForAppt.Size = new System.Drawing.Size(205, 20);
            this.dateTimePickerForAppt.TabIndex = 2;
            // 
            // buttonRemoveCancelAppt
            // 
            this.buttonRemoveCancelAppt.Location = new System.Drawing.Point(170, 185);
            this.buttonRemoveCancelAppt.Name = "buttonRemoveCancelAppt";
            this.buttonRemoveCancelAppt.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveCancelAppt.TabIndex = 8;
            this.buttonRemoveCancelAppt.Text = "Remove/Cancel";
            this.buttonRemoveCancelAppt.UseVisualStyleBackColor = true;
            // 
            // buttonEditAppt
            // 
            this.buttonEditAppt.Location = new System.Drawing.Point(89, 185);
            this.buttonEditAppt.Name = "buttonEditAppt";
            this.buttonEditAppt.Size = new System.Drawing.Size(75, 23);
            this.buttonEditAppt.TabIndex = 7;
            this.buttonEditAppt.Text = "Edit";
            this.buttonEditAppt.UseVisualStyleBackColor = true;
            // 
            // buttonAddSaveAppt
            // 
            this.buttonAddSaveAppt.Location = new System.Drawing.Point(7, 185);
            this.buttonAddSaveAppt.Name = "buttonAddSaveAppt";
            this.buttonAddSaveAppt.Size = new System.Drawing.Size(75, 23);
            this.buttonAddSaveAppt.TabIndex = 6;
            this.buttonAddSaveAppt.Text = "Add/Save";
            this.buttonAddSaveAppt.UseVisualStyleBackColor = true;
            this.buttonAddSaveAppt.Click += new System.EventHandler(this.buttonAddSaveAppt_Click);
            // 
            // groupBoxCustomer
            // 
            this.groupBoxCustomer.Controls.Add(this.labelCountry);
            this.groupBoxCustomer.Controls.Add(this.textBoxCountry);
            this.groupBoxCustomer.Controls.Add(this.labelZip);
            this.groupBoxCustomer.Controls.Add(this.textBoxZip);
            this.groupBoxCustomer.Controls.Add(this.labelCity);
            this.groupBoxCustomer.Controls.Add(this.textBoxCity);
            this.groupBoxCustomer.Controls.Add(this.labelPhoneNumber);
            this.groupBoxCustomer.Controls.Add(this.labelAddress);
            this.groupBoxCustomer.Controls.Add(this.labelFirstName);
            this.groupBoxCustomer.Controls.Add(this.buttonRemoveCancelCustomer);
            this.groupBoxCustomer.Controls.Add(this.buttonEditCustomer);
            this.groupBoxCustomer.Controls.Add(this.textBoxPhoneNumber);
            this.groupBoxCustomer.Controls.Add(this.buttonAddSaveCustomer);
            this.groupBoxCustomer.Controls.Add(this.textBoxAddress);
            this.groupBoxCustomer.Controls.Add(this.textBoxName);
            this.groupBoxCustomer.Location = new System.Drawing.Point(12, 295);
            this.groupBoxCustomer.Name = "groupBoxCustomer";
            this.groupBoxCustomer.Size = new System.Drawing.Size(376, 214);
            this.groupBoxCustomer.TabIndex = 2;
            this.groupBoxCustomer.TabStop = false;
            this.groupBoxCustomer.Text = "Customer";
            // 
            // labelCountry
            // 
            this.labelCountry.AutoSize = true;
            this.labelCountry.Location = new System.Drawing.Point(7, 110);
            this.labelCountry.Name = "labelCountry";
            this.labelCountry.Size = new System.Drawing.Size(43, 13);
            this.labelCountry.TabIndex = 27;
            this.labelCountry.Text = "Country";
            // 
            // textBoxCountry
            // 
            this.textBoxCountry.Location = new System.Drawing.Point(6, 126);
            this.textBoxCountry.Name = "textBoxCountry";
            this.textBoxCountry.Size = new System.Drawing.Size(100, 20);
            this.textBoxCountry.TabIndex = 6;
            // 
            // labelZip
            // 
            this.labelZip.AutoSize = true;
            this.labelZip.Location = new System.Drawing.Point(219, 63);
            this.labelZip.Name = "labelZip";
            this.labelZip.Size = new System.Drawing.Size(50, 13);
            this.labelZip.TabIndex = 25;
            this.labelZip.Text = "Zip Code";
            // 
            // textBoxZip
            // 
            this.textBoxZip.Location = new System.Drawing.Point(218, 79);
            this.textBoxZip.Name = "textBoxZip";
            this.textBoxZip.Size = new System.Drawing.Size(100, 20);
            this.textBoxZip.TabIndex = 5;
            // 
            // labelCity
            // 
            this.labelCity.AutoSize = true;
            this.labelCity.Location = new System.Drawing.Point(118, 63);
            this.labelCity.Name = "labelCity";
            this.labelCity.Size = new System.Drawing.Size(24, 13);
            this.labelCity.TabIndex = 23;
            this.labelCity.Text = "City";
            // 
            // textBoxCity
            // 
            this.textBoxCity.Location = new System.Drawing.Point(112, 79);
            this.textBoxCity.Name = "textBoxCity";
            this.textBoxCity.Size = new System.Drawing.Size(100, 20);
            this.textBoxCity.TabIndex = 4;
            // 
            // labelPhoneNumber
            // 
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Location = new System.Drawing.Point(219, 19);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new System.Drawing.Size(78, 13);
            this.labelPhoneNumber.TabIndex = 21;
            this.labelPhoneNumber.Text = "Phone Number";
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(6, 63);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(45, 13);
            this.labelAddress.TabIndex = 20;
            this.labelAddress.Text = "Address";
            // 
            // labelFirstName
            // 
            this.labelFirstName.AutoSize = true;
            this.labelFirstName.Location = new System.Drawing.Point(7, 20);
            this.labelFirstName.Name = "labelFirstName";
            this.labelFirstName.Size = new System.Drawing.Size(107, 13);
            this.labelFirstName.TabIndex = 18;
            this.labelFirstName.Text = "Name (First and Last)";
            // 
            // buttonRemoveCancelCustomer
            // 
            this.buttonRemoveCancelCustomer.Location = new System.Drawing.Point(170, 185);
            this.buttonRemoveCancelCustomer.Name = "buttonRemoveCancelCustomer";
            this.buttonRemoveCancelCustomer.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveCancelCustomer.TabIndex = 9;
            this.buttonRemoveCancelCustomer.Text = "Remove/Cancel";
            this.buttonRemoveCancelCustomer.UseVisualStyleBackColor = true;
            this.buttonRemoveCancelCustomer.Click += new System.EventHandler(this.buttonRemoveCancelCustomer_Click);
            // 
            // buttonEditCustomer
            // 
            this.buttonEditCustomer.Location = new System.Drawing.Point(89, 185);
            this.buttonEditCustomer.Name = "buttonEditCustomer";
            this.buttonEditCustomer.Size = new System.Drawing.Size(75, 23);
            this.buttonEditCustomer.TabIndex = 8;
            this.buttonEditCustomer.Text = "Edit";
            this.buttonEditCustomer.UseVisualStyleBackColor = true;
            this.buttonEditCustomer.Click += new System.EventHandler(this.buttonEditCustomer_Click);
            // 
            // textBoxPhoneNumber
            // 
            this.textBoxPhoneNumber.Location = new System.Drawing.Point(218, 38);
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new System.Drawing.Size(100, 20);
            this.textBoxPhoneNumber.TabIndex = 2;
            // 
            // buttonAddSaveCustomer
            // 
            this.buttonAddSaveCustomer.Location = new System.Drawing.Point(7, 185);
            this.buttonAddSaveCustomer.Name = "buttonAddSaveCustomer";
            this.buttonAddSaveCustomer.Size = new System.Drawing.Size(75, 23);
            this.buttonAddSaveCustomer.TabIndex = 7;
            this.buttonAddSaveCustomer.Text = "Add/Save";
            this.buttonAddSaveCustomer.UseVisualStyleBackColor = true;
            this.buttonAddSaveCustomer.Click += new System.EventHandler(this.buttonAddSaveCustomer_Click);
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(6, 79);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(100, 20);
            this.textBoxAddress.TabIndex = 3;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(6, 38);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(206, 20);
            this.textBoxName.TabIndex = 1;
            // 
            // menuStripScheduling
            // 
            this.menuStripScheduling.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportsToolStripMenuItem});
            this.menuStripScheduling.Location = new System.Drawing.Point(0, 0);
            this.menuStripScheduling.Name = "menuStripScheduling";
            this.menuStripScheduling.Size = new System.Drawing.Size(1014, 24);
            this.menuStripScheduling.TabIndex = 3;
            this.menuStripScheduling.Text = "menuStripScheduling";
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.numberOfAppointmentTypesByMonthToolStripMenuItem,
            this.theScheduleForEachConsultantToolStripMenuItem,
            this.oneOtherTypeToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // numberOfAppointmentTypesByMonthToolStripMenuItem
            // 
            this.numberOfAppointmentTypesByMonthToolStripMenuItem.Name = "numberOfAppointmentTypesByMonthToolStripMenuItem";
            this.numberOfAppointmentTypesByMonthToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.numberOfAppointmentTypesByMonthToolStripMenuItem.Text = "number of appointment types by month";
            // 
            // theScheduleForEachConsultantToolStripMenuItem
            // 
            this.theScheduleForEachConsultantToolStripMenuItem.Name = "theScheduleForEachConsultantToolStripMenuItem";
            this.theScheduleForEachConsultantToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.theScheduleForEachConsultantToolStripMenuItem.Text = "the schedule for each consultant";
            // 
            // oneOtherTypeToolStripMenuItem
            // 
            this.oneOtherTypeToolStripMenuItem.Name = "oneOtherTypeToolStripMenuItem";
            this.oneOtherTypeToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.oneOtherTypeToolStripMenuItem.Text = "One other type";
            // 
            // groupBoxCalendarView
            // 
            this.groupBoxCalendarView.Controls.Add(this.radioButtonByWeek);
            this.groupBoxCalendarView.Controls.Add(this.radioButtonByMonth);
            this.groupBoxCalendarView.Location = new System.Drawing.Point(12, 27);
            this.groupBoxCalendarView.Name = "groupBoxCalendarView";
            this.groupBoxCalendarView.Size = new System.Drawing.Size(154, 42);
            this.groupBoxCalendarView.TabIndex = 4;
            this.groupBoxCalendarView.TabStop = false;
            this.groupBoxCalendarView.Text = "Calendar View";
            // 
            // radioButtonByWeek
            // 
            this.radioButtonByWeek.AutoSize = true;
            this.radioButtonByWeek.Location = new System.Drawing.Point(82, 19);
            this.radioButtonByWeek.Name = "radioButtonByWeek";
            this.radioButtonByWeek.Size = new System.Drawing.Size(69, 17);
            this.radioButtonByWeek.TabIndex = 1;
            this.radioButtonByWeek.Text = "By Week";
            this.radioButtonByWeek.UseVisualStyleBackColor = true;
            // 
            // radioButtonByMonth
            // 
            this.radioButtonByMonth.AutoSize = true;
            this.radioButtonByMonth.Checked = true;
            this.radioButtonByMonth.Location = new System.Drawing.Point(6, 19);
            this.radioButtonByMonth.Name = "radioButtonByMonth";
            this.radioButtonByMonth.Size = new System.Drawing.Size(70, 17);
            this.radioButtonByMonth.TabIndex = 0;
            this.radioButtonByMonth.TabStop = true;
            this.radioButtonByMonth.Text = "By Month";
            this.radioButtonByMonth.UseVisualStyleBackColor = true;
            // 
            // monthCalendarAppts
            // 
            this.monthCalendarAppts.Location = new System.Drawing.Point(400, 33);
            this.monthCalendarAppts.Name = "monthCalendarAppts";
            this.monthCalendarAppts.TabIndex = 5;
            this.monthCalendarAppts.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendarAppts_DateSelected);
            // 
            // FormScheduling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 519);
            this.Controls.Add(this.monthCalendarAppts);
            this.Controls.Add(this.groupBoxCalendarView);
            this.Controls.Add(this.groupBoxCustomer);
            this.Controls.Add(this.groupBoxAppointments);
            this.Controls.Add(this.dataGridViewAppts);
            this.Controls.Add(this.menuStripScheduling);
            this.MainMenuStrip = this.menuStripScheduling;
            this.Name = "FormScheduling";
            this.Text = "Scheduling";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAppts)).EndInit();
            this.groupBoxAppointments.ResumeLayout(false);
            this.groupBoxAppointments.PerformLayout();
            this.groupBoxCustomer.ResumeLayout(false);
            this.groupBoxCustomer.PerformLayout();
            this.menuStripScheduling.ResumeLayout(false);
            this.menuStripScheduling.PerformLayout();
            this.groupBoxCalendarView.ResumeLayout(false);
            this.groupBoxCalendarView.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewAppts;
        private System.Windows.Forms.GroupBox groupBoxAppointments;
        private System.Windows.Forms.GroupBox groupBoxCustomer;
        private System.Windows.Forms.MenuStrip menuStripScheduling;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numberOfAppointmentTypesByMonthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem theScheduleForEachConsultantToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxConsultant;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.DateTimePicker dateTimePickerForAppt;
        private System.Windows.Forms.Button buttonRemoveCancelAppt;
        private System.Windows.Forms.Button buttonEditAppt;
        private System.Windows.Forms.Button buttonAddSaveAppt;
        private System.Windows.Forms.GroupBox groupBoxCalendarView;
        private System.Windows.Forms.RadioButton radioButtonByWeek;
        private System.Windows.Forms.RadioButton radioButtonByMonth;
        private System.Windows.Forms.MonthCalendar monthCalendarAppts;
        private System.Windows.Forms.Label labelConsultant;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label labelCustomerSelect;
        private System.Windows.Forms.ComboBox comboBoxTimeForAppt;
        private System.Windows.Forms.ComboBox comboBoxCustomerForAppt;
        private System.Windows.Forms.Label labelApptType;
        private System.Windows.Forms.Label labelPhoneNumber;
        private System.Windows.Forms.Label labelAddress;
        private System.Windows.Forms.Label labelFirstName;
        private System.Windows.Forms.Button buttonRemoveCancelCustomer;
        private System.Windows.Forms.Button buttonEditCustomer;
        private System.Windows.Forms.TextBox textBoxPhoneNumber;
        private System.Windows.Forms.Button buttonAddSaveCustomer;
        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.ToolStripMenuItem oneOtherTypeToolStripMenuItem;
        private System.Windows.Forms.Label labelCountry;
        private System.Windows.Forms.TextBox textBoxCountry;
        private System.Windows.Forms.Label labelZip;
        private System.Windows.Forms.TextBox textBoxZip;
        private System.Windows.Forms.Label labelCity;
        private System.Windows.Forms.TextBox textBoxCity;
        private System.Windows.Forms.TextBox textBoxApptType;
    }
}