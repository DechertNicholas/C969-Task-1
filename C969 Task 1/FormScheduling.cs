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
        Dictionary<string, Customer> CustomerMappings = new Dictionary<string, Customer>();
        BindingList<Appointment> Appointments = new BindingList<Appointment>();

        bool AddingCustomer = false;
        bool EditingCustomer = false;
        bool AddingAppt = false;
        bool EditingAppt = false;

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
            // I want the appointment to show up after the Scheduling form launches, but having this in the constructor 
            // will cause this to execute before the form appears. So, just delay the task
            Task.Delay(2000).ContinueWith(t => AlertUpcomingAppointments());
            //AlertUpcomingAppointments();
        }

        private void SetUIInitialState()
        {
            buttonAddSaveAppt.Text = "Add";
            buttonRemoveCancelAppt.Text = "Remove";

            buttonAddSaveCustomer.Text = "Add";
            buttonRemoveCancelCustomer.Text = "Remove";
        }

        private void AssignOpenHours()
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

        private void PopulateData()
        {
            comboBoxConsultant.DataSource = Handler.GetAllConsultants().Select(c => c.Name).ToList();

            // this is a quick lambda to generate a mapping of customer "id: names" to customer objects. I want to have a combobox dropdown to select the customer names
            // but also need the dictionary keys to be unique, which is why the id is prefixed. This would normally need a foreach loop
            // and would take up around 4-5 lines of code, but can be easily consolidated into a single, readable line
            CustomerMappings = Handler.GetAllCustomers().ToDictionary(x => $"{x.Id}: {x.CustomerName}", x => x);
            comboBoxCustomerForAppt.DataSource = CustomerMappings.Keys.ToList();
        }

        private void PopulateApptTable(DateTime startDate, DateTime endDate)
        {
            Appointments = Handler.GetAppointmentsByRange(startDate, endDate);
            var customers = Handler.GetAllCustomers();
            var consultants = Handler.GetAllConsultants();

            // I'm using a lambda here to select only columns I want into the dataGridVew, as it is much faster than creating a new model
            // and assigning the properties to the new model. Having the full appointment data in Appointments will still be useful,
            // since other existing properties will need to be available when an appointment is edited.
            dataGridViewAppts.DataSource = Appointments.Select(a => new { a.AppointmentId, a.Start, a.End, a.Type, 
                customers.Where(c => c.Id == a.CustomerId).First().CustomerName,
                consultants.Where(c => c.Id == a.UserId).First().Name})
                .ToList();
            // Keep the appointmentId data, but don't show it
            dataGridViewAppts.Columns["AppointmentId"].Visible = false;
            dataGridViewAppts.Columns["CustomerName"].HeaderText = "Customer";
            dataGridViewAppts.Columns["Name"].HeaderText = "Consultant";
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
            var search = new Regex("^" + $"{customerId}" + ":");
            var customer = CustomerMappings[CustomerMappings.Keys.Where(x => search.IsMatch(x)).First()];
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
            comboBoxCustomerForAppt.SelectedIndex = comboBoxCustomerForAppt.Items.IndexOf($"{customer.Id}: {customer.CustomerName}");
        }

        private void PopulateAppointmentData(Appointment appt)
        {
            // set the appt data
            comboBoxConsultant.SelectedItem = Handler.GetAllConsultants().Where(c => c.Id == appt.UserId).First().Name;
            comboBoxTimeForAppt.SelectedItem = appt.Start.ToString("hh:mm tt");
            dateTimePickerForAppt.Value = appt.Start;
            Console.WriteLine($"Time Slot: {appt.Start:hh:mm tt}");
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

        private void AlertUpcomingAppointments()
        {
            // where appt is for this consultant -> starts after the current time -> starts before 15 minutes from now
            var upcomingAppts = Appointments.Where(a => a.UserId == Handler.GetAllConsultants().Where(c => c.Name == CurrentUser).First().Id)
                .Where(a => a.Start >= DateTime.Now)
                .Where(a => a.Start <= DateTime.Now.AddMinutes(15))
                .ToList();
            if (upcomingAppts.Count == 0)
            {
                return;
            }

            var alertString = "";
            foreach (var appt in upcomingAppts)
            {
                alertString += $"{appt.Start.ToLocalTime()}: {Handler.GetCustomerById(appt.CustomerId).CustomerName} - {appt.Type}\n";
            }

            MessageBox.Show("You have the following upcoming appointments:\n" + alertString, "Upcoming appointments", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //var alertString = upcomingAppts.Select(a => $"{a.Start.ToLocalTime()}: {Handler.GetCustomerById(a.CustomerId).CustomerName} - {a.Type}\n").ToString();
        }

        private Country AddOrGetCountry(string countryName)
        {
            // check if the country exists in the database
            var existingCountry = Handler.GetAllCountries()
                .Where(c => c.CountryName == countryName)
                .FirstOrDefault();

            return existingCountry ?? Handler.AddCountry(countryName, CurrentUser);

            //// if this is a new country, add it
            //if (existingCountry == null)
            //{
            //    return Handler.AddCountry(countryName, CurrentUser);
            //}
            //else
            //{
            //    // update our reference to that existing country
            //    return existingCountry;
            //}
        }

        private City AddOrGetCity(string cityName, int countryId)
        {
            var existingCity = Handler.GetAllCities()
                .Where(c => c.CityName == cityName)
                .Where(c => c.CountryId == countryId)
                .FirstOrDefault();

            return existingCity ?? Handler.AddCity(cityName, countryId, CurrentUser);

            //// if no matching city name + countryId is found
            //if (existingCity == null)
            //{
            //    return Handler.AddCity(cityName, countryId,  CurrentUser);
            //}
            //else
            //{
            //    // update our reference to city
            //    return existingCity;
            //}
        }

        private Address AddOrGetAddress(string addressName, int cityId, string postalCode, string phone)
        {
            var existingAddress = Handler.GetAllAddresses()
                .Where(a => a.AddressName == addressName)
                .Where(a => a.Phone == phone)
                .Where(a => a.PostalCode == postalCode)
                .Where(a => a.CityId == cityId)
                .FirstOrDefault();

            return existingAddress ?? Handler.AddAddress(addressName, cityId, postalCode, phone, CurrentUser);

            //// if no matching address is found, add it
            //if (existingAddress == null)
            //{
            //    return Handler.AddAddress(addressName, cityId, postalCode, phone, CurrentUser);
            //}
            //else
            //{
            //    // update our reference to the existing address
            //    return existingAddress;
            //}
        }

        private void RemoveCustomer()
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
            var appointments = Handler.GetAppointmentsByCustomerId(customer.Id);

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

        private void UpdateCustomer()
        {
            if (EditingCustomer == false)
            {
                // Not editing a customer, we're starting to edit a customer
                // Save will need to be clicked to finish
                EditingCustomer = true;

                buttonAddSaveCustomer.Text = "Save";
                buttonRemoveCancelCustomer.Text = "Cancel";

                ToggleEditableControls(EditingCustomer, UISection.CUSTOMER);
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

                    // create a copy of originalCustomer that will be edited to hold the new data
                    //var customer = new Customer(originalCustomer.Id, originalCustomer.CustomerName, originalCustomer.AddressId,
                    //    originalCustomer.Active, originalCustomer.CreateDate, originalCustomer.CreatedBy, originalCustomer.LastUpdate,
                    //    originalCustomer.LastUpdateBy);

                    // informs us if the record that relies on the changed record needs to be changed
                    // if country is changed, the new country may need to be added to the database, or gotten from the database
                    var countryUpdated = false;
                    var cityUpdated = false;
                    var addressUpdated = false;

                    // countryId will be updated when adding the new country, if needed
                    if (country.CountryName != textBoxCountry.Text)
                    {
                        country = AddOrGetCountry(textBoxCountry.Text);

                        countryUpdated = true;
                        // update the reference in city
                        city.CountryId = country.CountryId;
                    }

                    // the city name could stay the same, but if the country updates, we need to ensure we update the city with that country
                    // if the name changes but the country stays the same, the country will not have been updated and the countryId will remain the same
                    if (city.CityName != textBoxCity.Text || countryUpdated)
                    {
                        city.CityName = textBoxCity.Text;

                        city = AddOrGetCity(textBoxCity.Text, country.CountryId);
                        
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

                        address = AddOrGetAddress(textBoxAddress.Text, city.CityId, textBoxZip.Text, textBoxPhoneNumber.Text);

                        addressUpdated = true;
                        // update our reference in customer
                        customer.AddressId = address.AddressId;
                    }

                    if (customer.CustomerName != textBoxName.Text || addressUpdated)
                    {
                        customer.CustomerName = textBoxName.Text;
                        customer = Handler.UpdateCustomer(customer, CurrentUser);
                    }

                    // reset the UI state
                    EditingCustomer = false;
                    buttonAddSaveCustomer.Text = "Add";
                    buttonRemoveCancelCustomer.Text = "Remove";
                    ToggleEditableControls(EditingCustomer, UISection.CUSTOMER);

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

        private void AddCustomer()
        {
            if (AddingCustomer == false)
            {
                // Not editing a customer, we're adding a new customer
                // The button is meant to be clicked again to save
                AddingCustomer = true;

                buttonAddSaveCustomer.Text = "Save";
                buttonRemoveCancelCustomer.Text = "Cancel";

                // Clear the text boxes for data entry
                textBoxName.Text = null;
                textBoxPhoneNumber.Text = null;
                textBoxAddress.Text = null;
                textBoxCity.Text = null;
                textBoxZip.Text = null;
                textBoxCountry.Text = null;

                ToggleEditableControls(AddingCustomer, UISection.CUSTOMER);
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

                    var country = AddOrGetCountry(customerSet.Country);
                    var city = AddOrGetCity(textBoxCity.Text, country.CountryId);
                    var address = AddOrGetAddress(textBoxAddress.Text, city.CityId, textBoxZip.Text, textBoxPhoneNumber.Text);

                    Handler.AddCustomer(textBoxName.Text, address.AddressId, CurrentUser);

                    // set our UI back to normal
                    AddingCustomer = false;
                    buttonAddSaveCustomer.Text = "Add";
                    buttonRemoveCancelCustomer.Text = "Remove";
                    ToggleEditableControls(AddingCustomer, UISection.CUSTOMER);

                    // Refresh customer data list
                    PopulateData();
                    buttonEditCustomer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddAppt()
        {
            if (AddingAppt == false)
            {
                // Not editing an appt, we're adding a new appt
                // The button is meant to be clicked again to save
                AddingAppt = true;
                dataGridViewAppts.Enabled = false;

                buttonAddSaveAppt.Text = "Save";
                buttonRemoveCancelAppt.Text = "Cancel";

                // Clear the text boxes for data entry
                comboBoxCustomerForAppt.SelectedItem = null;
                dateTimePickerForAppt.Value = DateTime.Now;
                comboBoxTimeForAppt.SelectedIndex = 0;
                comboBoxConsultant.SelectedItem = CurrentUser;
                textBoxApptType.Text = null;

                ToggleEditableControls(AddingAppt, UISection.APPT);
                buttonEditAppt.Enabled = false;
                comboBoxTimeForAppt.DataSource = AvailableHours;
            }
            else
            {
                // null validations
                // valide basic appt data
                if (comboBoxCustomerForAppt.SelectedItem == null)
                {
                    MessageBox.Show("Must select a customer for the appointment", "Missing customer selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBoxConsultant.SelectedItem == null)
                {
                    // user has somehow selected no consultant to do the appointment
                    MessageBox.Show("Please select a consultant for the appointment",
                        "Missing consultant", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBoxApptType.Text))
                {
                    MessageBox.Show("Please write in an appointment type",
                        "Missing appointment type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // if the null validations pass, continue on

                // cast data into the appropriate formats for validation and adding
                var apptStartString = dateTimePickerForAppt.Value.ToString("yyyy-MM-dd");
                apptStartString += $" {comboBoxTimeForAppt.SelectedItem}";
                var apptStart = DateTime.ParseExact(apptStartString, "yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);
                var key = comboBoxCustomerForAppt.SelectedItem.ToString();
                var appts = Handler.GetAppointmentsByCustomerId(CustomerMappings[key].Id);

                if (null != appts.Where(a => a.Start == apptStart).FirstOrDefault())
                {
                    // an appointment for this customer in this timeslot already exists
                    MessageBox.Show("Customer already has an appointment at this time.\nPlease schedule for a different time", 
                        "Missing already has an appointment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (apptStart < DateTime.Now)
                {
                    // appointment is scheduled for the past. This may be useful in some cases, so give an option to the user
                    var choice = MessageBox.Show("This appointment is scheduled for some time in the past. Continue anyway?",
                        "Appointment is in the past", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (choice == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                if (apptStart.DayOfWeek == DayOfWeek.Saturday || apptStart.DayOfWeek == DayOfWeek.Sunday)
                {
                    // not open on weekends
                    MessageBox.Show("Please schedule for a time that is not on the weekend",
                        "Appointment scheduled for the weekend", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // now we can add the appointment
                var customer = Handler.GetCustomerById(CustomerMappings[key].Id);
                Handler.AddAppointment(customer, apptStart, textBoxApptType.Text, comboBoxConsultant.SelectedItem.ToString(), CurrentUser);


                // set our UI back to normal
                AddingAppt = false;
                dataGridViewAppts.Enabled = true;
                buttonAddSaveAppt.Text = "Add";
                buttonRemoveCancelAppt.Text = "Remove";
                ToggleEditableControls(AddingAppt, UISection.APPT);

                // Refresh customer data list
                PopulateData();
                PopulateApptTable(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd);
                buttonEditAppt.Enabled = true;
                comboBoxTimeForAppt.DataSource = OpenHours;
            }
        }

        private void UpdateAppt()
        {
            if (EditingAppt == false)
            {
                // Not editing an appt, we're adding a new appt
                // The button is meant to be clicked again to save
                EditingAppt = true;
                // disable the DGV so that the user cannot select a different appointment
                dataGridViewAppts.Enabled = false;
                

                buttonAddSaveAppt.Text = "Save";
                buttonRemoveCancelAppt.Text = "Cancel";

                // text boxes will stay the same

                ToggleEditableControls(EditingAppt, UISection.APPT);
                buttonEditAppt.Enabled = false;
                // since this appt is being edited, the time slot will be free again. Add the time slot back in it's correct place
                var currentApptTime = comboBoxTimeForAppt.SelectedItem.ToString();
                var format = "hh:mm tt";
                AvailableHours.Add(currentApptTime);
                // using a quick lambda here 
                AvailableHours = new BindingList<string>(
                    AvailableHours
                    .OrderBy(h => DateTime.ParseExact(h, format, CultureInfo.InvariantCulture))
                    .ToList()
                );
                comboBoxTimeForAppt.DataSource = AvailableHours;
                comboBoxTimeForAppt.SelectedItem = currentApptTime;

            }
            else
            {
                // null validations
                // valide basic appt data
                if (comboBoxCustomerForAppt.SelectedItem == null)
                {
                    MessageBox.Show("Must select a customer for the appointment", "Missing customer selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBoxConsultant.SelectedItem == null)
                {
                    // user has somehow selected no consultant to do the appointment
                    MessageBox.Show("Please select a consultant for the appointment",
                        "Missing consultant", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBoxApptType.Text))
                {
                    MessageBox.Show("Please write in an appointment type",
                        "Missing appointment type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // if the null validations pass, continue on

                // cast data into the appropriate formats for validation and adding
                var row = dataGridViewAppts.SelectedRows[0];
                var appt = Appointments.Where(a => a.AppointmentId == int.Parse(row.Cells["AppointmentId"].Value.ToString())).First();
                var apptStartString = dateTimePickerForAppt.Value.ToString("yyyy-MM-dd");
                apptStartString += $" {comboBoxTimeForAppt.SelectedItem}";
                var apptStart = DateTime.ParseExact(apptStartString, "yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);
                var key = comboBoxCustomerForAppt.SelectedItem.ToString();
                var appts = Handler.GetAppointmentsByCustomerId(CustomerMappings[key].Id);

                // the current appointment should be able to be scheduled for the current time
                if (null != appts.Where(a => a.AppointmentId != appt.AppointmentId)
                    .Where(a => a.Start == apptStart).FirstOrDefault())
                {
                    // an appointment for this customer in this timeslot already exists
                    MessageBox.Show("Customer already has an appointment at this time.\nPlease schedule for a different time",
                        "Customer already has an appointment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (apptStart < DateTime.Now)
                {
                    // appointment is scheduled for the past. This may be useful in some cases, so give an option to the user
                    var choice = MessageBox.Show("This appointment is scheduled for some time in the past. Continue anyway?",
                        "Appointment is in the past", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (choice == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                if (apptStart.DayOfWeek == DayOfWeek.Saturday || apptStart.DayOfWeek == DayOfWeek.Sunday)
                {
                    // not open on weekends
                    MessageBox.Show("Please schedule for a time that is not on the weekend",
                        "Appointment scheduled for the weekend", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // now we can add the appointment
                var customer = Handler.GetCustomerById(CustomerMappings[key].Id);
                appt.Start = apptStart.ToUniversalTime();
                appt.End = appt.Start.AddMinutes(30);
                appt.Type = textBoxApptType.Text;
                appt.CustomerId = customer.Id;
                Handler.UpdateAppointment(appt, comboBoxConsultant.SelectedItem.ToString(), CurrentUser);


                // set our UI back to normal
                EditingAppt = false;
                buttonAddSaveAppt.Text = "Add";
                buttonRemoveCancelAppt.Text = "Remove";
                ToggleEditableControls(EditingAppt, UISection.APPT);
                dataGridViewAppts.Enabled = true;

                // Refresh customer data list
                PopulateData();
                PopulateApptTable(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd);
                buttonEditAppt.Enabled = true;
                comboBoxTimeForAppt.DataSource = OpenHours;
            }
        }

        private void RemoveAppt()
        {
            var choice = MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (choice != DialogResult.OK)
            {
                return;
            }

            var row = dataGridViewAppts.SelectedRows[0];
            // cust is where the selected appointmentId in row -> appointments -> customerId -> get customer
            var apptId = int.Parse(row.Cells["AppointmentId"].Value.ToString());

            Handler.DeleteAppointmentById(apptId);
        }

        private void SetAvailableHours()
        {
            // reset available hours
            // if we're editing the appointment, we want to keep the current timeslot open
            // DateTime.Now is only used to avoid a null compliation exception - it should not be used
            var currentApptSelection = EditingAppt ? DateTime.ParseExact(comboBoxTimeForAppt.SelectedItem.ToString(), "hh:mm tt", CultureInfo.InvariantCulture) 
                : DateTime.Now;
            AvailableHours.Clear();
            var allAppts = Handler.GetAppointmentsByConsultantName(comboBoxConsultant.SelectedItem.ToString());
            var todayAppts = allAppts.Where(a => a.Start.Date == dateTimePickerForAppt.Value.Date).ToList();

            // don't remove the current slot if we're editing the appt
            if (EditingAppt)
            {
                var row = dataGridViewAppts.SelectedRows[0];
                var appt = Appointments.Where(a => a.AppointmentId == int.Parse(row.Cells["AppointmentId"].Value.ToString())).First();

                todayAppts.Remove(
                    todayAppts.Where(a => a.Start.TimeOfDay == currentApptSelection.TimeOfDay)
                    .Where(a => a.AppointmentId == appt.AppointmentId)
                    .FirstOrDefault()
                );
            }

            // set the consultant's available hours to all hours
            OpenHours.ForEach(h => AvailableHours.Add(h));
            // remove hours for any appointments the consultant has today
            if (todayAppts.Count != 0)
            {
                todayAppts.ForEach(a => AvailableHours.Remove(a.Start.ToLocalTime().ToString("hh:mm tt")));
            }
        }

        private void ReportTypesByMonth()
        {
            // the appointments variable already has this info, just use that
            var thisMonthAppts = Appointments.Where(a => a.Start.Date.Month == monthCalendarAppts.SelectionStart.Date.Month).ToList();
            var types = thisMonthAppts.Select(a => a.Type).Distinct().ToList();
            var apptString = "This month has the following number of appointment types:\n\n";
            foreach (var type in types)
            {
                var count = thisMonthAppts.Where(a => a.Type == type).ToList().Count;
                apptString += $"{type} - {count}\n";
            }

            MessageBox.Show(apptString, "Appointments by type", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReportConsultantSchedule()
        {
            var reportString = "Consultant schedules for this month:\n\n";
            var customers = Handler.GetAllCustomers();
            var thisMonthAppts = Appointments.Where(a => a.Start.Date.Month == monthCalendarAppts.SelectionStart.Date.Month).ToList();
            // consultant list will already contain unique values
            var consultants = Handler.GetAllConsultants();
            foreach (var consultant in consultants)
            {
                reportString += $"{consultant.Name}:\n";
                var appts = Appointments.Where(a => a.UserId == consultant.Id).ToList();
                if (appts.Count == 0)
                {
                    // using 4 spaces here as tabs (\t) are huge (8 spaces)
                    reportString += "    Free!\n";
                    continue;
                }
                // using 4 spaces here as tabs (\t) are huge (8 spaces)
                appts.ForEach(a => reportString += $"    - {a.Start} {a.Type} with {customers.Where(c => c.Id == a.CustomerId).First().CustomerName}\n");
            }

            MessageBox.Show(reportString, "Consultant schedule", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReportCustomersByMonth()
        {
            // the appointments variable already has this info, just use that
            var thisMonthAppts = Appointments.Where(a => a.Start.Date.Month == monthCalendarAppts.SelectionStart.Date.Month).ToList();
            var customers = Handler.GetAllCustomers();
            var thisMonthCustomers = thisMonthAppts.Select(a => 
                customers.Where(c => c.Id == a.CustomerId).Select(c => c).First()).Distinct().ToList();

            var reportString = "Customers have the following number of appointments:\n\n";
            foreach (var customer in thisMonthCustomers)
            {
                var count = thisMonthAppts.Where(a => a.CustomerId == customer.Id).ToList().Count;
                reportString += $"{customer.CustomerName} - {count}\n";
            }

            MessageBox.Show(reportString, "Appointments by customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ui methods
        private void buttonAddSaveCustomer_Click(object sender, EventArgs e)
        {
            // "save" can mean save the newly added customer, or save the updated customer data
            if (EditingCustomer)
            {
                UpdateCustomer();
            }
            else
            {
                AddCustomer();
            }
        }

        private void dataGridViewAppts_SelectionChanged(object sender, EventArgs e)
        {
            SelectAppointment();
        }

        private void buttonRemoveCancelCustomer_Click(object sender, EventArgs e)
        {
            if (EditingCustomer || AddingCustomer)
            {
                // Cancel an edit
                EditingCustomer = false;
                AddingCustomer = false;

                buttonAddSaveCustomer.Text = "Add";
                buttonRemoveCancelCustomer.Text = "Remove";

                ToggleEditableControls(EditingCustomer, UISection.CUSTOMER);

                // repopulate the customer data into the appropriate text fields
                SelectAppointment();
                buttonEditCustomer.Enabled = true;
            }
            else
            {
                RemoveCustomer();
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

        private void buttonAddSaveAppt_Click(object sender, EventArgs e)
        {
            // "save" can mean save the newly added customer, or save the updated customer data
            if (EditingAppt)
            {
                UpdateAppt();
            }
            else
            {
                AddAppt();
            }
        }

        private void buttonRemoveCancelAppt_Click(object sender, EventArgs e)
        {
            if (EditingAppt || AddingAppt)
            {
                // Cancel an edit
                EditingAppt = false;
                AddingAppt = false;
                dataGridViewAppts.Enabled = true;

                buttonAddSaveAppt.Text = "Add";
                buttonRemoveCancelAppt.Text = "Remove";

                ToggleEditableControls(EditingAppt, UISection.APPT);

                // repopulate the customer data into the appropriate text fields
                comboBoxTimeForAppt.DataSource = OpenHours;
                SelectAppointment();
                buttonEditAppt.Enabled = true;
            }
            else
            {
                RemoveAppt();
                PopulateApptTable(monthCalendarAppts.SelectionStart, monthCalendarAppts.SelectionEnd);
                SelectAppointment();
            }
        }

        private void comboBoxConsultant_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAvailableHours();
        }

        private void dateTimePickerForAppt_ValueChanged(object sender, EventArgs e)
        {
            SetAvailableHours();
        }

        private void buttonEditAppt_Click(object sender, EventArgs e)
        {
            UpdateAppt();
        }

        private void numberOfAppointmentTypesByMonthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportTypesByMonth();
        }

        private void theScheduleForEachConsultantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportConsultantSchedule();
        }

        private void oneOtherTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportCustomersByMonth();
        }
    }
}
