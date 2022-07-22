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
        // customer list for new appointments
        List<Customer> Customers = new List<Customer>();
        List<string> CustomerNames = new List<string>();
        BindingList<Appointment> Appointments = new BindingList<Appointment>();

        bool EditCustomer = false;
        bool EditAppt = false;

        //int SelectedCustomer = 0;
        //int SelectedCustomerForAppt = 0;

        //int SelectedAppt = 0;

        enum UISection {APPT, CUSTOMER, ALL}

        public FormScheduling()
        {
            InitializeComponent();
            PopulateData();
            ToggleEditableControls(false, UISection.ALL);
            AssignOpenHours();
            SetUIInitialState();

            // set the initial calendar state and appointment list state
            SelectDateRange();
            PopulateApptTable(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd);
            //monthCalendarAppts_DateSelected(new object { }, new DateRangeEventArgs(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd));
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
            comboBoxConsultant.DataSource = Handler.GetAllConsultants();

            Customers = Handler.GetAllCustomers();

            // this is a quick lambda to generate a list from one property of each element in a collection. This would normally need a foreach loop
            // and would take up around 4-5 lines of code, but can be easily consolidated into a single, readable line
            CustomerNames = Customers.Select(c => c.CustomerName).ToList();

            comboBoxCustomerForAppt.DataSource = CustomerNames;
        }

        public void PopulateApptTable(DateTime startDate, DateTime endDate)
        {
            Appointments = Handler.GetAppointmentsByRange(startDate, endDate);

            // I'm using a lambda here to select only columns I want into the dataGridVew, as it is much faster than creating a new model
            // and assigning the properties to the new model. Having the full appointment data in Appointments will still be useful,
            // since other existing properties will need to be available when an appointment is edited.
            dataGridViewAppts.DataSource = Appointments.Select(a => new { a.AppointmentId, a.Start, a.End, a.Type, a.CreatedBy }).ToList();
            // Keep the appointmentId data, but don't show it
            dataGridViewAppts.Columns["AppointmentId"].Visible = false;
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
            var address = Handler.GetAllAddresses().Where(a => a.AddressId == customer.AddressId).First();
            var city = Handler.GetAllCities().Where(c => c.CityId == address.CityId).First();
            var country = Handler.GetAllCountries().Where(c => c.CountryId == city.CountryId).First();

            textBoxName.Text = customer.CustomerName;
            textBoxPhoneNumber.Text = address.Phone;
            textBoxAddress.Text = address.AddressName;
            textBoxCity.Text = city.CityName;
            textBoxZip.Text = address.PostalCode;
            textBoxCountry.Text = country.CountryName;

            // This field is technically part of the appointment data, but it's easier to populate it here in the "CustomerData" section and should be the
            // only exception
            comboBoxCustomerForAppt.SelectedIndex = comboBoxCustomerForAppt.Items.IndexOf(customer.CustomerName);
        }

        public void PopulateAppointmentData(Appointment appt)
        {
            dateTimePickerForAppt.Value = appt.Start;
            Console.WriteLine($"Time Slot: {appt.Start:hh:mm tt}");
            comboBoxTimeForAppt.SelectedItem = appt.Start.ToString("hh:mm tt");
            textBoxApptType.Text = appt.Type;
        }

        private void SelectAppointment()
        {
            if (dataGridViewAppts.SelectedRows.Count == 0)
            {
                // This happens sometimes, but isn't a cause for alarm
                Console.WriteLine("DGV Row is null, this is expected on first startup (but may be unexpected if this happens later!)");
                return;
            }

            // multiselect should not be allowed, so the selected row will always be the first
            var row = dataGridViewAppts.SelectedRows[0];

            // this lambda allows for quick selection of the appointment from the grid to the appointment in the list. 
            var appt = Appointments.Where(a => a.AppointmentId == int.Parse(row.Cells["AppointmentId"].Value.ToString())).First();

            Console.WriteLine($"Got customerId: {appt.CustomerId}");

            PopulateCustomerData(appt.CustomerId);
            PopulateAppointmentData(appt);
        }

        private void UpdateCustomer()
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
                    // validate user input before doing anything
                    var customerSet = new Validations.CustomerValidationSet
                    (
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text
                    );

                    Validations.ValidateCustomerData(customerSet);

                    // any part of customer could have changed, from name to address to city to country
                    // all records that are not "customer" will just be added, not updated
                    // this is because other users could be using the same address (siblings), and it isn't worth going through and delting the old address

                    // get the appointment data
                    var row = dataGridViewAppts.SelectedRows[0];
                    var appt = Appointments.Where(a => a.AppointmentId == int.Parse(row.Cells["AppointmentId"].Value.ToString())).First();

                    // get the data for the customer related classes
                    var customer = Handler.GetCustomerById(appt.CustomerId);
                    var address = Handler.GetAddressById(customer.AddressId);
                    var city = Handler.GetCityById(address.CityId);
                    var country = Handler.GetCountryById(city.CountryId);

                    // informs us if the record that relies on the changed record needs to be changed
                    // if country is changed, the new country may need to be added to the database, or gotten from the database
                    var countryUpdated = false;
                    var cityUpdated = false;
                    var addressUpdated = false;

                    // countryId will be updated when adding the new country, if needed
                    if (country.CountryName != textBoxCountry.Text)
                    {
                        country.CountryName = textBoxCountry.Text;

                        // check if the country exists in the database
                        var existingCountry = Handler.GetAllCountries().Where(c => c.CountryName == country.CountryName).FirstOrDefault();

                        // if this is a new country, add it
                        if (existingCountry == null)
                        {
                            country = Handler.AddCountry(country, CurrentUser);
                        }
                        else
                        {
                            // update our reference to that existing country
                            country = existingCountry;
                        }
                        countryUpdated = true;
                        // update the reference in city
                        city.CountryId = country.CountryId;
                    }

                    // the city name could stay the same, but if the country updates, we need to ensure we update the city with that country
                    // if the name changes but the country stays the same, the country will not have been updated and the countryId will remain the same
                    if (city.CityName != textBoxCity.Text || countryUpdated)
                    {
                        city.CityName = textBoxCity.Text;

                        var existingCity = Handler.GetAllCities().Where(c => c.CityName == city.CityName).Where(c => c.CountryId == city.CountryId).FirstOrDefault();

                        // if no matching city name + countryId is found
                        if (existingCity == null)
                        {
                            city = Handler.AddCity(city, CurrentUser);
                        }
                        else
                        {
                            // update our reference to city
                            city = existingCity;
                        }
                        cityUpdated = true;
                        // update the reference in address
                        address.CityId = city.CityId;
                    }

                    // address has the same issues as above
                    if (address.AddressName != textBoxAddress.Text || address.Phone != textBoxPhoneNumber.Text || address.PostalCode != textBoxZip.Text || cityUpdated)
                    {
                        address.AddressName = textBoxAddress.Text;
                        address.Phone = textBoxPhoneNumber.Text;
                        address.PostalCode = textBoxZip.Text;

                        var existingAddress = Handler.GetAllAddresses().Where(a => a.AddressName == address.AddressName)
                            .Where(a => a.Phone == address.Phone)
                            .Where(a => a.PostalCode == address.PostalCode)
                            .Where(a => a.CityId == city.CityId)
                            .FirstOrDefault();

                        // if no matching address is found, add it
                        if (existingAddress == null)
                        {
                            address = Handler.AddAddress(address, CurrentUser);
                        }
                        else
                        {
                            // update our reference to the existing address
                            address = existingAddress;
                        }
                        addressUpdated = true;
                        // update our reference in customer
                        customer.AddressId = address.AddressId;
                    }

                    if (customer.CustomerName != textBoxName.Text || addressUpdated)
                    {
                        customer.CustomerName = textBoxName.Text;
                        Handler.UpdateCustomer(customer, CurrentUser);
                    }

                    // reset the UI state
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

        // ui methods
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
                    var customerSet = new Validations.CustomerValidationSet(
                        textBoxName.Text,
                        textBoxPhoneNumber.Text,
                        textBoxAddress.Text,
                        textBoxCity.Text,
                        textBoxZip.Text,
                        textBoxCountry.Text
                        );
                    Validations.ValidateCustomerData(customerSet);

                    Handler.AddCustomer(customerSet, CurrentUser);

                    // set our UI back to normal
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

        private void dataGridViewAppts_SelectionChanged(object sender, EventArgs e)
        {
            SelectAppointment();
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

                // repopulate the customer data into the appropriate text fields
                SelectAppointment();
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
                var row = dataGridViewAppts.SelectedRows[0];
                // cust is where the selected appointmentId in row -> appointments -> customerId -> get customer
                var apptId = int.Parse(row.Cells["AppointmentId"].Value.ToString());
                var customerId = Appointments.Where(a => a.AppointmentId == apptId).Select(a => a.CustomerId).First();
                var customer = Handler.GetCustomerById(customerId);
                // find all appointments for this customer
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

                // need to refresh the local lists of appointments and customers
                PopulateData();
                //monthCalendarAppts_DateSelected(new object { }, new DateRangeEventArgs(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd));
                PopulateApptTable(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd);
            }
        }

        private void buttonEditCustomer_Click(object sender, EventArgs e)
        {
            UpdateCustomer();
        }

        private void monthCalendarAppts_DateSelected(object sender, DateRangeEventArgs e)
        {
            SelectDateRange();
            PopulateApptTable(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd);
        }

        // needs fixing
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
                    // Actually this is for an appointment,not a customer
                    //----------------------------------------------------------------------------------------------------
                    // validate the data entered into the customer data field
                    
                    // validate

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
