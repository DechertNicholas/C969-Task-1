using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using C969_Task_1.Models;

namespace C969_Task_1
{
    public partial class FormScheduling : Form
    {
        SQLHandler Handler = new SQLHandler();
        string CurrentUser = null;
        // Hours the business is open. Applies to all appointments
        List<string> OpenHours = new List<string>();
        // Updated per each consultant, contains hours available to the specific selected consultant
        BindingList<string> AvailableHours = new BindingList<string>();
        // Store customer data locally so we don't need to keep querying the server
        List<Customer> Customers = new List<Customer>();
        List<string> CustomerNames = new List<string>();
        BindingList<Appointment> Appointments = new BindingList<Appointment>();

        bool EditCustomer = false;
        bool EditAppt = false;

        int SelectedCustomer = 0;
        int SelectedCustomerForAppt = 0;

        int SelectedAppt = 0;

        enum UISection {APPT, CUSTOMER, ALL}

        public FormScheduling()
        {
            InitializeComponent();
            PopulateData();
            ToggleEditableControls(false, UISection.ALL);
            AssignOpenHours();
            SetUIInitialState();
            // Invoke the DateSelected event, so that the table is populated when the form is displayed
            // The sender argument doesn't really matter here, so just send an empty object
            monthCalendarAppts_DateSelected(new object { }, new DateRangeEventArgs(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd));
        }

        public FormScheduling(string userName) : this()
        {
            CurrentUser = userName;
            Console.WriteLine($"Scheduling started as {CurrentUser}");
        }

        public void SetUIInitialState()
        {
            buttonAddSaveCustomer.Text = "Add";
            buttonRemoveCancelCustomer.Text = "Remove";
        }

        public void AssignOpenHours()
        {
            var apptInterval = new TimeSpan(0, 30, 0); // 30 minute appointments
            var open = DateTime.ParseExact("07:00", "HH:mm", null); // 8 AM
            var close = DateTime.ParseExact("18:00", "HH:mm", null); // 6 PM

            OpenHours.Add(open.ToString("hh:mm tt")); // Add the first value

            while (OpenHours.Last() != close.ToString("hh:mm tt")) // Add remaining values
            {
                OpenHours.Add(DateTime.ParseExact(OpenHours.Last(), "hh:mm tt", CultureInfo.InvariantCulture)
                    .Add(apptInterval)
                    .ToString("hh:mm tt"));
            }

            comboBoxTimeForAppt.DataSource = OpenHours;
        }

        private void ToggleEditableControls(bool state, UISection section)
        {
            switch (section)
            {
                case UISection.APPT:
                    {
                        // I want these boxes to be read only until the user wants to edit the selected item, or add/delete an item
                        // Appointments
                        comboBoxCustomerForAppt.Enabled = state;
                        dateTimePickerForAppt.Enabled = state;
                        comboBoxTimeForAppt.Enabled = state;
                        comboBoxConsultant.Enabled = state;
                        textBoxApptType.Enabled = state;
                        break;
                    }
                case UISection.CUSTOMER:
                    {
                        // Customers - NOT state, since readonly will be true when control enabled state should be false
                        textBoxName.ReadOnly = !state;
                        textBoxPhoneNumber.ReadOnly = !state;
                        textBoxAddress.ReadOnly = !state;
                        textBoxCity.ReadOnly = !state;
                        textBoxZip.ReadOnly = !state;
                        textBoxCountry.ReadOnly = !state;
                        break;
                    }
                case UISection.ALL:
                    {
                        ToggleEditableControls(state, UISection.APPT);
                        ToggleEditableControls(state, UISection.CUSTOMER);
                        break;
                    }
            }
        }

        public void PopulateData()
        {
            var consultants = Handler.GetConsultants();
            comboBoxConsultant.DataSource = consultants;

            Customers = Handler.GetCustomers();

            foreach (var customer in Customers)
            {
                CustomerNames.Add(customer.Name);
            }

            comboBoxCustomerForAppt.DataSource = CustomerNames;
        }

        public void PopulateApptTable(DateTime startDate, DateTime endDate)
        {
            //dataGridViewAppts.DataSource = null;
            Appointments = Handler.GetAppointments(startDate, endDate);

            // I'm using a lambda here to select only columns I want into the dataGridVew, as it is much faster than creating a new model
            // and assigning the properties to the new model. Having the full appointment data in Appointments will still be useful,
            // since other existing properties will need to be available when an appointment is edited.
            dataGridViewAppts.DataSource = Appointments.Select(a => new { a.AppointmentId, a.Start, a.End, a.Type, a.CreatedBy }).ToList();
            // Keep the appointmentId data, but don't show it
            dataGridViewAppts.Columns["AppointmentId"].Visible = false;
        }

        private void buttonAddSaveCustomer_Click(object sender, EventArgs e)
        {
            if (EditCustomer == false)
            {
                // Not editing a customer, we're adding a new customer
                // The button is meant to be clicked again to save
                EditCustomer = true;

                buttonAddSaveCustomer.Text = "Save";
                buttonRemoveCancelCustomer.Text = "Cancel";

                // Clear the text boxes for data entry
                textBoxName.Text = null;
                textBoxPhoneNumber.Text = null;
                textBoxAddress.Text = null;
                textBoxCity.Text = null;
                textBoxZip.Text = null;
                textBoxCountry.Text = null;

                ToggleEditableControls(EditCustomer, UISection.CUSTOMER);
                buttonEditCustomer.Enabled = false;
            }
            else
            {
                // customer is meant to be saved now
                try
                {
                    var validatedCustomer = Validations.ValidateCustomerData(new UnvalidatedCustomer
                    (
                        null, // ID is assigned when updating in the database
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text
                    ));

                    Handler.AddCustomer(validatedCustomer, CurrentUser);

                    EditCustomer = false;
                    buttonAddSaveCustomer.Text = "Add";
                    buttonRemoveCancelCustomer.Text = "Remove";
                    ToggleEditableControls(EditCustomer, UISection.CUSTOMER);

                    // Refresh customer data list
                    PopulateData();
                    buttonEditCustomer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {

                }
            }
        }

        private void monthCalendarAppts_DateSelected(object sender, DateRangeEventArgs e)
        {
            SelectDateRange();
            PopulateApptTable(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd);
        }

        private void SelectDateRange()
        {
            if (radioButtonByWeek.Checked)
            {
                var today = monthCalendarAppts.SelectionStart;
                var startOfWeek = today;
                var cultureFirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                while (startOfWeek.DayOfWeek != cultureFirstDayOfWeek)
                {
                    // Just walk the calendar backwards until we find the start of the week
                    startOfWeek = startOfWeek.AddDays(-1);
                }

                var endOfWeek = startOfWeek.AddDays(6);
                monthCalendarAppts.MaxSelectionCount = 7;
                monthCalendarAppts.SelectionStart = startOfWeek;
                monthCalendarAppts.SelectionEnd = endOfWeek;
            }
            else
            {
                var year = monthCalendarAppts.SelectionStart.Year;
                var month = monthCalendarAppts.SelectionStart.Month;
                monthCalendarAppts.MaxSelectionCount = 31; // No month has more than 31 days
                monthCalendarAppts.SelectionStart = new DateTime(year, month, 1);
                monthCalendarAppts.SelectionEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            }
            Console.WriteLine($"Selected dates between {monthCalendarAppts.SelectionStart} and {monthCalendarAppts.SelectionEnd}");
        }

        private void PopulateCustomerData(int customerId)
        {
            // I use a simple lambda here instead of a foreach loop to find the first matching customerId in the list of Customers
            // A foreach loop would use many more lines to do something very simple and easily understood
            var customer = Customers.Where(c => c.Id == customerId).First();

            textBoxName.Text = customer.Name;
            textBoxPhoneNumber.Text = customer.PhoneNumber;
            textBoxAddress.Text = customer.Address;
            textBoxCity.Text = customer.City;
            textBoxZip.Text = customer.ZipCode.ToString();
            textBoxCountry.Text = customer.Country;

            // This field is technically part of the appointment data, but it's easier to populate it here and should be the
            // only exception
            comboBoxCustomerForAppt.SelectedIndex = comboBoxCustomerForAppt.Items.IndexOf(customer.Name);
        }

        public void PopulateAppointmentData(Appointment appt)
        {
            dateTimePickerForAppt.Value = appt.Start;
            Console.WriteLine($"Time Slot: {appt.Start:hh:mm tt}");
            comboBoxTimeForAppt.SelectedItem = appt.Start.ToString("hh:mm tt");
            textBoxApptType.Text = appt.Type;
        }

        private void dataGridViewAppts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAppts.SelectedRows.Count == 0)
            {
                // This happens sometimes, but isn't a cause for alarm
                Console.WriteLine("DGV Row is null, this is expected on first startup (but may be unexpected if this happens later!)");
                return;
            }

            // Multiselect should not be allowed, so the selected row will always be the first
            var row = dataGridViewAppts.SelectedRows[0];

            var appt = Appointments.Where(a => a.AppointmentId == int.Parse(row.Cells["AppointmentId"].Value.ToString())).First();
            
            Console.WriteLine($"Got customerId: {appt.CustomerId}");

            PopulateCustomerData(appt.CustomerId);
            PopulateAppointmentData(appt);
        }

        private void buttonRemoveCancelCustomer_Click(object sender, EventArgs e)
        {
            if (EditCustomer == true)
            {
                // Cancel an edit
                EditCustomer = false;

                buttonAddSaveCustomer.Text = "Add";
                buttonRemoveCancelCustomer.Text = "Remove";

                ToggleEditableControls(EditCustomer, UISection.CUSTOMER);

                // pretend to change the selection in the appointment list to refresh the data
                dataGridViewAppts_SelectionChanged(new object(), new EventArgs());
                buttonEditCustomer.Enabled = true;
            }
            else
            {
                // Button is remove
                if (string.IsNullOrWhiteSpace(textBoxName.Text))
                {
                    MessageBox.Show("Cannot delete an empty user", "No user selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var choice = MessageBox.Show("Are you sure you want to delete this user?", "Confirm delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (choice != DialogResult.OK)
                {
                    return;
                }

                // this isn't the best way to select a unique customer, but it should be good enough for this
                var customer = Customers.Where(c => c.Name == textBoxName.Text).Where(c => c.Address == textBoxAddress.Text).First();
                var appointments = Handler.GetAppointmentsById(customer.Id);

                // Check if there are any appointments for the user in the database. They'll need to be deleted
                if (appointments.Count != 0)
                {
                    var apptString = "User has outstanding appointments. Confirm deletion:\n\n";
                    foreach (var appt in appointments)
                    {
                        apptString += $"{appt.Type} - {appt.Start}\n";
                    }

                    choice = MessageBox.Show(apptString, "User has outstanding appointments", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (choice != DialogResult.OK)
                    {
                        return;
                    }
                }

                foreach (var appt in appointments)
                {
                    Handler.DeleteAppointmentById(appt.AppointmentId);
                }

                Handler.DeleteCustomerById(customer.Id);

                PopulateData();
                monthCalendarAppts_DateSelected(new object { }, new DateRangeEventArgs(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd));
            }
        }

        private void buttonEditCustomer_Click(object sender, EventArgs e)
        {
            if (EditCustomer == false)
            {
                // Not editing a customer, we're starting to edit a customer
                // Save will need to be clicked to finish
                EditCustomer = true;

                buttonAddSaveCustomer.Text = "Save";
                buttonRemoveCancelCustomer.Text = "Cancel";

                ToggleEditableControls(EditCustomer, UISection.CUSTOMER);
                buttonEditCustomer.Enabled = false;
            }
            else
            {
                // customer is meant to be saved now
                try
                {
                    var customer = new UnvalidatedCustomer
                    (
                        null,
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text
                    );

                    var validatedCustomer = Validations.ValidateCustomerData(customer);

                    Handler.UpdateCustomer(validatedCustomer);

                    EditCustomer = false;
                    buttonAddSaveCustomer.Text = "Add";
                    buttonRemoveCancelCustomer.Text = "Remove";
                    ToggleEditableControls(EditCustomer, UISection.CUSTOMER);

                    // Refresh customer data list
                    PopulateData();
                    buttonEditCustomer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {

                }
            }
        }

        private void buttonAddSaveAppt_Click(object sender, EventArgs e)
        {
            if (EditAppt == false)
            {
                // Not editing an appointment, we're adding a new appointment
                // The button is meant to be clicked again to save
                EditAppt = true;

                buttonAddSaveAppt.Text = "Save";
                buttonRemoveCancelAppt.Text = "Cancel";

                comboBoxCustomerForAppt.SelectedItem = null;
                dateTimePickerForAppt.Value = DateTime.Today;
                comboBoxTimeForAppt.SelectedItem = null;
                comboBoxConsultant.SelectedItem = null;
                textBoxApptType.Text = null;

                ToggleEditableControls(EditAppt, UISection.APPT);
                buttonEditAppt.Enabled = false;
            }
            else
            {
                // Appointment is meant to be saved now
                try
                {
                    Validations.ValidateCustomerData(new UnvalidatedCustomer
                    (
                        null,
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text
                    ));

                    Handler.AddCustomer(
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text);

                    EditCustomer = false;
                    buttonAddSaveCustomer.Text = "Add";
                    buttonRemoveCancelCustomer.Text = "Remove";
                    ToggleEditableControls(EditCustomer, UISection.CUSTOMER);

                    // Refresh customer data list
                    PopulateData();
                    buttonEditCustomer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {

                }
            }
        }
    }
}
