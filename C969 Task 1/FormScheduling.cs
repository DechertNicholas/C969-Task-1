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
        // Store customer data locally so we don't need to keep querying the server
        List<string> OpenHours = new List<string>();
        // Updated per each consultant, contains hours available to the specific selected consultant
        BindingList<string> AvailableHours = new BindingList<string>();
        List<Customer> Customers = new List<Customer>();
        List<string> CustomerNames = new List<string>();
        BindingList<Appointment> Appointments = new BindingList<Appointment>();
        bool EditClient = false;
        bool EditAppt = false;
        enum UISection {APPT, CLIENT, ALL}

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
            buttonAddSaveClient.Text = "Add";
            buttonRemoveCancelClient.Text = "Remove";
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
                        comboBoxClientForAppt.Enabled = state;
                        dateTimePickerForAppt.Enabled = state;
                        comboBoxTimeForAppt.Enabled = state;
                        comboBoxConsultant.Enabled = state;
                        textBoxApptType.Enabled = state;
                        break;
                    }
                case UISection.CLIENT:
                    {
                        // Clients - NOT state, since readonly will be true when control enabled state should be false
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
                        ToggleEditableControls(state, UISection.CLIENT);
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

            comboBoxClientForAppt.DataSource = CustomerNames;
        }

        public void PopulateApptTable(DateTime startDate, DateTime endDate)
        {
            //dataGridViewAppts.DataSource = null;
            Appointments = Handler.GetAppointments(startDate, endDate);
            // I'm using a lambda here to select only columns I want into the dataGridVew, as it is much faster than creating a new model
            // and assigning the properties to the new model. Having the full appointment data in Appointments will still be useful,
            // since other existing properties will need to be available when an appointment is edited.
            dataGridViewAppts.DataSource = Appointments.Select(a => new { Start = a.Start, End = a.End, Type = a.Type, CreatedBy = a.CreatedBy }).ToList();
        }

        private void buttonAddSaveClient_Click(object sender, EventArgs e)
        {
            if (EditClient == false)
            {
                // Not editing a client, we're adding a new client
                // The button is meant to be clicked again to save
                EditClient = true;

                buttonAddSaveClient.Text = "Save";
                buttonRemoveCancelClient.Text = "Cancel";

                textBoxName.Text = null;
                textBoxPhoneNumber.Text = null;
                textBoxAddress.Text = null;
                textBoxCity.Text = null;
                textBoxZip.Text = null;
                textBoxCountry.Text = null;

                ToggleEditableControls(EditClient, UISection.CLIENT);
                buttonEditClient.Enabled = false;
            }
            else
            {
                // Client is meant to be saved now
                try
                {
                    Validations.ValidateCustomerData(new UnvalidatedCustomer
                    (
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text
                    ));

                    Handler.AddClient(
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text);

                    EditClient = false;
                    buttonAddSaveClient.Text = "Add";
                    buttonRemoveCancelClient.Text = "Remove";
                    ToggleEditableControls(EditClient, UISection.CLIENT);

                    // Refresh customer data list
                    PopulateData();
                    buttonEditClient.Enabled = true;
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
            comboBoxClientForAppt.SelectedIndex = comboBoxClientForAppt.Items.IndexOf(customer.Name);
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
                Console.WriteLine("DGV Row is null, this is expected on first startup (but may be unexpected if this happens later!)");
                return;
            }
            var row = dataGridViewAppts.SelectedRows[0];
            var apptTime = DateTime.Parse(row.Cells["Start"].Value.ToString());
            var consultant = row.Cells["CreatedBy"].Value.ToString();
            // I use a lambda here in place of a foreach loop to find which appointment in the appointments collections has a matching start time
            // Since no two appointments should overlap, this value should be unique
            var appt = Appointments.Where(a => a.Start == apptTime).Where(a => a.CreatedBy == consultant).First();
            
            Console.WriteLine($"Got customerId: {appt.CustomerId}");

            PopulateCustomerData(appt.CustomerId);
            PopulateAppointmentData(appt);
        }

        private void buttonRemoveCancelClient_Click(object sender, EventArgs e)
        {
            if (EditClient == true)
            {
                // Cancel an edit
                EditClient = false;

                buttonAddSaveClient.Text = "Add";
                buttonRemoveCancelClient.Text = "Remove";

                ToggleEditableControls(EditClient, UISection.CLIENT);

                // pretend to change the selection in the appointment list to refresh the data
                dataGridViewAppts_SelectionChanged(new object(), new EventArgs());
                buttonEditClient.Enabled = true;
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
    }
}
