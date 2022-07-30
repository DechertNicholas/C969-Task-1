using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C969_Task_1.Models;
using System.ComponentModel;
using System.Globalization;
using MySql.Data.Types;

namespace C969_Task_1
{
    public class SQLHandler
    {
        private Translator LocalTranslator = new Translator();

        public MySqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["LocalHost"].ConnectionString;
            return new MySqlConnection(connStr);
        }

        public void TestLogin(string username, string password, Translator.LanguageCode languageCode)
        {
            var conn = GetConnection();
            try
            {
                // If it can't open the connection, it'll throw an error. If we don't get an error, it worked
                conn.Open();
                var query = $"SELECT * FROM user WHERE userName = '{username}' AND password = '{password}'";
                var adapter = new MySqlDataAdapter(query, conn);
                var dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    throw new UnknownUserException(LocalTranslator.Translate(languageCode, "UnknownUser"));
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // countries
        public List<Country> GetAllCountries()
        {
            var results = new List<Country>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM country";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Country(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        (DateTime)rdr[2],
                        rdr[3].ToString(),
                        (DateTime)rdr[4],
                        rdr[5].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public Country GetCountryById(int id)
        {
            var country = new Country();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = $"SELECT * FROM country WHERE countryId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    country.CountryId = int.Parse(rdr[0].ToString()); // id
                    country.CountryName = rdr[1].ToString(); // name
                    country.CreateDate = (DateTime)rdr[2];
                    country.CreatedBy = rdr[3].ToString();
                    country.LastUpdate = (DateTime)rdr[4];
                    country.LastUpdateBy = rdr[5].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return country;
        }

        public Country AddCountry(string countryName, string user)
        {
            var conn = GetConnection();
            var countries = GetAllCountries();
            var countryId = countries.Count == 0 ? 1 : countries.Last().CountryId + 1;
            var nowDate = DateTime.UtcNow;

            var country = new Country(countryId, countryName, nowDate, user, nowDate, user);

            try
            {
                conn.Open();

                var query = $"INSERT INTO country VALUES ({country.CountryId}, '{country.CountryName}', '{country.CreateDate:yyyy-MM-dd HH:mm:ss}', '{country.CreatedBy}', '{country.LastUpdate:yyyy-MM-dd HH:mm:ss}', '{country.LastUpdateBy}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return country;
        }

        // cities
        public List<City> GetAllCities()
        {
            var results = new List<City>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM city";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new City(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        int.Parse(rdr[2].ToString()), // countryId
                        (DateTime)rdr[3],
                        rdr[4].ToString(),
                        (DateTime)rdr[5],
                        rdr[6].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public City GetCityById(int id)
        {
            var city = new City();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = $"SELECT * FROM city WHERE cityId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    city.CityId = int.Parse(rdr[0].ToString()); // id
                    city.CityName = rdr[1].ToString(); // name
                    city.CountryId = int.Parse(rdr[2].ToString()); // countryId
                    city.CreateDate = (DateTime)rdr[3];
                    city.CreatedBy = rdr[4].ToString();
                    city.LastUpdate = (DateTime)rdr[5];
                    city.LastUpdateBy = rdr[6].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return city;
        }

        public City AddCity(string cityName, int countryId, string user)
        {
            var conn = GetConnection();
            var cities = GetAllCities();
            var cityId = cities.Count == 0 ? 1 : cities.Last().CityId + 1;
            var nowDate = DateTime.UtcNow;
            var city = new City(cityId, cityName, countryId, nowDate, user, nowDate, user);

            try
            {
                conn.Open();

                var query = $"INSERT INTO city VALUES ({city.CityId}, '{city.CityName}', {city.CountryId}, '{city.CreateDate:yyyy-MM-dd HH:mm:ss}', " +
                    $"'{city.CreatedBy}', '{city.LastUpdate:yyyy-MM-dd HH:mm:ss}', '{city.LastUpdateBy}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return city;
        }

        // addresses
        public List<Address> GetAllAddresses()
        {
            var results = new List<Address>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM address";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Address(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        int.Parse(rdr[3].ToString()), // cityId - skip to entry 3 here, as address2 is not used
                        rdr[4].ToString(), // postal code
                        rdr[5].ToString(), // phone
                        (DateTime)rdr[6],
                        rdr[7].ToString(),
                        (DateTime)rdr[8],
                        rdr[9].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public Address GetAddressById(int id)
        {
            var address = new Address();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = $"SELECT * FROM address WHERE addressId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    address.AddressId = int.Parse(rdr[0].ToString()); // id
                    address.AddressName = rdr[1].ToString(); // name
                    address.Address2 = rdr[2].ToString();
                    address.CityId = int.Parse(rdr[3].ToString()); // cityId
                    address.PostalCode = rdr[4].ToString(); // postal code
                    address.Phone = rdr[5].ToString(); // phone
                    address.CreateDate = (DateTime)rdr[6];
                    address.CreatedBy = rdr[7].ToString();
                    address.LastUpdate = (DateTime)rdr[8];
                    address.LastUpdateBy = rdr[9].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return address;
        }

        public Address AddAddress(string addressName, int cityId, string postalCode, string phone, string user)
        {
            var conn = GetConnection();
            var addresses = GetAllAddresses();
            var addressId = addresses.Count == 0 ? 1 : addresses.Last().AddressId + 1;
            var nowDate = DateTime.UtcNow;

            var address = new Address(addressId, addressName, cityId, postalCode, phone, nowDate, user, nowDate, user);

            try
            {
                conn.Open();

                var query = $"INSERT INTO address VALUES ({address.AddressId}, '{address.AddressName}', '{address.Address2}', {address.CityId}, " + 
                    $"'{address.PostalCode}', '{address.Phone}', '{address.CreateDate:yyyy-MM-dd HH:mm:ss}', '{address.CreatedBy}', " + 
                    $"'{address.LastUpdate:yyyy-MM-dd HH:mm:ss}', '{address.LastUpdateBy}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return address;
        }

        // customers
        public List<Customer> GetAllCustomers()
        {
            var results = new List<Customer>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM customer";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                var customer = new Customer();
                while (rdr.Read())
                {
                    results.Add(new Customer(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        int.Parse(rdr[2].ToString()), // address id
                        (bool)rdr[3], // active (0, 1)
                        (DateTime)rdr[4],
                        rdr[5].ToString(),
                        (DateTime)rdr[6],
                        rdr[7].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public Customer GetCustomerById(int id)
        {
            var customer = new Customer();
            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT * FROM customer WHERE customerId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    customer.Id = int.Parse(rdr[0].ToString()); // id
                    customer.CustomerName = rdr[1].ToString(); // name
                    customer.AddressId = int.Parse(rdr[2].ToString()); // address id
                    customer.Active = (bool)rdr[3]; // active (0, 1)
                    customer.CreateDate = (DateTime)rdr[4];
                    customer.CreatedBy = rdr[5].ToString();
                    customer.LastUpdate = (DateTime)rdr[6];
                    customer.LastUpdateBy = rdr[7].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return customer;
        }

        public Customer AddCustomer(string customerName, int addressId, string user)
        {
            var conn = GetConnection();
            var nowDate = DateTime.UtcNow;
            var customers = GetAllCustomers();
            var customerId = customers.Count == 0 ? 1 : customers.Last().Id + 1;
            var customer = new Customer(customerId, customerName, addressId, true, nowDate, user, nowDate, user);

            try
            {
                conn.Open();

                // an existing customer will be the same as address, with a composite key of name and addressId
                var dbCustomer = GetAllCustomers()
                    .Where(c => c.CustomerName == customer.CustomerName)
                    .Where(c => c.AddressId == customer.AddressId)
                    .FirstOrDefault();
                if (dbCustomer != null)
                {
                    throw new DuplicateRecordException("Customer is already present in the database");
                }



                var query = $"INSERT INTO customer VALUES ({customer.Id}, '{customer.CustomerName}', {customer.AddressId}, {customer.Active}, " + 
                    $"'{customer.CreateDate:yyyy-MM-dd HH:mm:ss}', '{user}', '{customer.LastUpdate:yyyy-MM-dd HH:mm:ss}', '{user}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return customer;
        }

        public void DeleteCustomerById(int custId)
        {
            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"DELETE FROM customer WHERE customerId = {custId}";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Deleted customer with ID {custId}");
        }

        public Customer UpdateCustomer(Customer customer, string user)
        {
            var conn = GetConnection();
            customer.LastUpdate = DateTime.UtcNow;
            customer.LastUpdateBy = user;

            try
            {
                conn.Open();

                var nowDate = DateTime.UtcNow;

                var query = $"UPDATE customer SET customerName = '{customer.CustomerName}', addressId = {customer.AddressId}, " +
                    $"lastUpdate = '{customer.LastUpdate:yyyy-MM-dd HH:mm:ss}', lastUpdateBy = '{customer.LastUpdateBy}' " +
                    $"WHERE customerId = {customer.Id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return customer;
        }

        // consultants
        public List<Consultant> GetAllConsultants()
        {
            var conn = GetConnection();

            var results = new List<Consultant>();
            try
            {
                conn.Open();
                
                var query = "SELECT userId, userName FROM user";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Consultant(
                        int.Parse(rdr[0].ToString()),
                        rdr[1].ToString())
                        );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return results;
        }

        // appointments
        public BindingList<Appointment> GetAllAppointments()
        {
            var appts = new BindingList<Appointment>();

            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT appointmentId, customerId, userId, type, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    appts.Add(new Appointment
                    {
                        AppointmentId = int.Parse(rdr[0].ToString()),
                        CustomerId = int.Parse(rdr[1].ToString()),
                        UserId = int.Parse(rdr[2].ToString()),
                        Type = rdr[3].ToString(),
                        Start = DateTime.Parse(rdr[4].ToString()).ToLocalTime(),
                        End = DateTime.Parse(rdr[5].ToString()).ToLocalTime(),
                        CreateDate = DateTime.Parse(rdr[6].ToString()).ToLocalTime(),
                        CreatedBy = rdr[7].ToString(),
                        LastUpdate = DateTime.Parse(rdr[8].ToString()).ToLocalTime(),
                        LastUpdateBy = rdr[9].ToString()
                    });
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Got {appts.Count} appointments");
            return appts;
        }

        public void DeleteAppointmentById(int apptId)
        {
            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"DELETE FROM appointment WHERE appointmentId = {apptId}";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Deleted appointment with ID {apptId}");
        }

        public BindingList<Appointment> GetAppointmentsByRange(DateTime startDate, DateTime endDate)
        {
            var appts = new BindingList<Appointment>();

            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT appointmentId, customerId, userId, type, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment\n" +
                             $"WHERE start BETWEEN '{startDate:yyyy-MM-dd}' AND DATE_ADD('{endDate:yyyy-MM-dd}', INTERVAL 1 DAY)";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    appts.Add(new Appointment
                    {
                        AppointmentId = int.Parse(rdr[0].ToString()),
                        CustomerId = int.Parse(rdr[1].ToString()),
                        UserId = int.Parse(rdr[2].ToString()),
                        Type = rdr[3].ToString(),
                        Start = DateTime.Parse(rdr[4].ToString()).ToLocalTime(),
                        End = DateTime.Parse(rdr[5].ToString()).ToLocalTime(),
                        CreateDate = DateTime.Parse(rdr[6].ToString()).ToLocalTime(),
                        CreatedBy = rdr[7].ToString(),
                        LastUpdate = DateTime.Parse(rdr[8].ToString()).ToLocalTime(),
                        LastUpdateBy = rdr[9].ToString()
                    }) ;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Got {appts.Count} appointments");
            appts = new BindingList<Appointment>(appts.OrderBy(a => a.Start).ToList());
            return appts;
        }

        public List<Appointment> GetAppointmentsByCustomerId(int customerId)
        {
            var appts = new List<Appointment>();

            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT appointmentId, customerId, userId, type, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment\n" +
                             $"WHERE customerId = {customerId}";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    appts.Add(new Appointment
                    {
                        AppointmentId = int.Parse(rdr[0].ToString()),
                        CustomerId = int.Parse(rdr[1].ToString()),
                        UserId = int.Parse(rdr[2].ToString()),
                        Type = rdr[3].ToString(),
                        Start = DateTime.Parse(rdr[4].ToString()).ToLocalTime(),
                        End = DateTime.Parse(rdr[5].ToString()).ToLocalTime(),
                        CreateDate = DateTime.Parse(rdr[6].ToString()).ToLocalTime(),
                        CreatedBy = rdr[7].ToString(),
                        LastUpdate = DateTime.Parse(rdr[8].ToString()).ToLocalTime(),
                        LastUpdateBy = rdr[9].ToString()
                    });
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Got {appts.Count} appointments");
            return appts;
        }

        public Appointment AddAppointment(Customer customer, DateTime start, string type, string consultant, string currentUser)
        {
            var conn = GetConnection();
            var nowDate = DateTime.UtcNow;
            start = start.ToUniversalTime();
            var appts = GetAllAppointments();
            var apptId = appts.Count == 0 ? 1 : appts.Last().AppointmentId + 1;
            var userId = GetAllConsultants().Where(c => c.Name == consultant).FirstOrDefault().Id;
            var appt = new Appointment(apptId, customer.Id, userId, type, start, start.AddMinutes(30), nowDate, currentUser, nowDate, currentUser);
            string title = "", desc = "", loc = "", contact = "", url = "";

            try
            {
                conn.Open();

                var query = $"INSERT INTO appointment VALUES ({appt.AppointmentId}, {customer.Id}, {userId}, '{title}', '{desc}', '{loc}', '{contact}', " +
                    $"'{type}', '{url}', '{appt.Start:yyyy-MM-dd HH:mm:ss}', '{appt.End:yyyy-MM-dd HH:mm:ss}', '{appt.CreateDate:yyyy-MM-dd HH:mm:ss}', " + 
                    $"'{appt.CreatedBy}', '{appt.LastUpdate:yyyy-MM-dd HH:mm:ss}', '{appt.LastUpdateBy}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return appt;
        }

        public Appointment UpdateAppointment(Appointment appt, string consultant, string currentUser)
        {
            var conn = GetConnection();
            var nowDate = DateTime.UtcNow;
            appt.UserId = GetAllConsultants().Where(c => c.Name == consultant).First().Id;
            appt.LastUpdate = nowDate;
            appt.LastUpdateBy = currentUser;

            try
            {
                conn.Open();

                var query = $"UPDATE appointment SET customerId = {appt.CustomerId}, userId = {appt.UserId}, " +
                    $"type = '{appt.Type}', start = '{appt.Start:yyyy-MM-dd HH:mm:ss}', end = '{appt.End:yyyy-MM-dd HH:mm:ss}', " +
                    $"lastUpdate = '{appt.LastUpdate:yyyy-MM-dd HH:mm:ss}', lastUpdateBy = '{appt.LastUpdateBy}'" +
                    $"WHERE appointmentId = {appt.AppointmentId}";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return appt;
        }

        public List<Appointment> GetAppointmentsByConsultantName(string consultantName)
        {
            var conn = GetConnection();
            var appts = new List<Appointment>();

            try
            {
                conn.Open();

                var query = $"SELECT appointmentId, customerId, userId, type, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment " +
                    $"WHERE userId = (SELECT userId FROM user WHERE userName = '{consultantName}')";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    appts.Add(new Appointment(
                        int.Parse(rdr[0].ToString()),
                        int.Parse(rdr[1].ToString()),
                        int.Parse(rdr[2].ToString()),
                        rdr[3].ToString(),
                        ((DateTime)rdr[4]).ToLocalTime(),
                        ((DateTime)rdr[5]).ToLocalTime(),
                        (DateTime)rdr[6],
                        rdr[7].ToString(),
                        (DateTime)rdr[8],
                        rdr[9].ToString()
                    ));
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return appts;
        }
    }
}
