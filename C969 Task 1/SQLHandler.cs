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

namespace C969_Task_1
{
    public class SQLHandler
    {
        private Translator LocalTranslator = new Translator();


        public MySqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["RQLDEV01"].ConnectionString;
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

        public List<Customer> GetCustomers()
        {
            var results = new List<Customer>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = @"SELECT customer.customerId, customer.customerName, address.phone, address.address, city.city, address.postalCode, country.country FROM customer
                            INNER JOIN address ON customer.addressId = address.addressId
                            INNER JOIN city on city.cityId = address.cityId
                            INNER JOIN country on city.countryId = country.countryId";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Customer(
                        int.Parse(rdr[0].ToString()),
                        rdr[1].ToString(),
                        rdr[2].ToString(),
                        rdr[3].ToString(),
                        rdr[4].ToString(),
                        int.Parse(rdr[5].ToString()),
                        rdr[6].ToString()));
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

        public void AddCustomer(Customer customer, string user)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();

                // check for customer table duplication
                var query = @$"SELECT customer.customerName, address.phone, address.address, city.city, address.postalCode, country.country FROM customer
                            INNER JOIN address ON customer.addressId = address.addressId
                            INNER JOIN city on city.cityId = address.cityId
                            INNER JOIN country on city.countryId = country.countryId
                            WHERE customerName = '{customer.Name}'
                            AND address.phone = '{customer.PhoneNumber}'
                            AND address.address = '{customer.Address}'
                            AND city.city = '{customer.City}'
                            AND address.postalCode = '{customer.ZipCode}'
                            AND country.country = '{customer.Country}'";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    throw new DuplicateRecordException("Customer is already present in the database:\n" +
                                                      $"Name: {customer.Name}\nPhone: {customer.PhoneNumber}\nAddress: {customer.Address}\nCity: {customer.City}\n" +
                                                      $"Zip: {customer.ZipCode}\nCountry: {customer.Country}");
                }

                // TODO: extact this?
                var countryId = AddOrGetCountryId(customer, user);
                var cityId = AddOrGetCityId(customer, user, countryId);

                // ------------- Address -------------

                // Add the address
                // get the highest ID from the table
                query = "SELECT MAX(addressId) FROM address";
                cmd = new MySqlCommand(query, conn);
                rdr = cmd.ExecuteReader();
                int addressId = 0; // initialize to something
                while (rdr.Read())
                {
                    addressId = int.Parse(rdr[0].ToString());
                }
                addressId = addressId++;

                // DB uses an address 2, but we don't
                var address2 = "";
                

                // Address insert is done Second, needs cityId
                query = $"INSERT INTO address VALUES ({addressId}, '{customer.Address}', '{address2}', {cityId}, '{customer.ZipCode}', '{customer.PhoneNumber}', '{createDate}', '{createdBy}', '{lastUpdate}', '{updatedBy}')";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();


                // ------------- Customer -------------

                // don't use magic numbers or strings
                var active = 1;

                // get the highest ID from the table
                query = "SELECT MAX(customerId) FROM customer";
                cmd = new MySqlCommand(query, conn);
                rdr = cmd.ExecuteReader();
                int customerId = 0; // initialize to something
                while (rdr.Read())
                {
                    customerId = int.Parse(rdr[0].ToString());
                }

                // Increment to the new customer ID
                customer.Id = customerId++;

                // Customer insert is done last, needs addressId
                query = $"INSERT INTO customer VALUES ({customer.Id}, '{customer.Name}', {addressId}, {active}, '{createDate}', '{createdBy}', '{lastUpdate}', '{updatedBy}')";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
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

        public List<string> GetConsultants()
        {
            var conn = GetConnection();

            var results = new List<string>();
            try
            {
                conn.Open();
                
                var query = "SELECT userName FROM user";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(rdr[0].ToString());
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

        public BindingList<Appointment> GetAppointments(DateTime startDate, DateTime endDate)
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
            return appts;
        }

        public BindingList<Appointment> GetAppointments(DateTime startDate, DateTime endDate, int consultantId)
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

        public List<Appointment> GetAppointmentsById(int? customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException("customerId", "customerId cannot be null.");
            }
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

        public void DeleteCustomerById(int? custId)
        {
            if (custId == null)
            {
                throw new ArgumentNullException("custId", "custId cannot be null.");
            }
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

        public void UpdateCustomer(Customer customer)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();

                // check for customer table duplication
                var query = @$"SELECT customer.customerName, address.phone, address.address, city.city, address.postalCode, country.country FROM customer
                            INNER JOIN address ON customer.addressId = address.addressId
                            INNER JOIN city on city.cityId = address.cityId
                            INNER JOIN country on city.countryId = country.countryId
                            WHERE customerName = '{customer.Name}'
                            AND address.phone = '{customer.PhoneNumber}'
                            AND address.address = '{customer.Address}'
                            AND city.city = '{customer.City}'
                            AND address.postalCode = '{customer.ZipCode}'
                            AND country.country = '{customer.Country}'";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    throw new DuplicateRecordException("Customer is already present in the database:\n" +
                                                      $"Name: {customer.Name}\nPhone: {customer.PhoneNumber}\nAddress: {customer.Address}\nCity: {customer.City}\n" +
                                                      $"Zip: {customer.ZipCode}\nCountry: {customer.Country}");
                }

                // get the highest ID from the table
                query = "SELECT MAX(customerId) FROM customer";
                cmd = new MySqlCommand(query, conn);
                rdr = cmd.ExecuteReader();
                int id = 1; // initialize to something
                while (rdr.Read())
                {
                    id = int.Parse(rdr[0].ToString());
                }

                id++;

                query = $"INSERT INTO customer VALUES ('{id}, '{customer.Name}')";
                cmd = new MySqlCommand(query, conn);
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

        public void GetAppointmentsByConsultantId(int consultantId)
        {

        }

        private int AddOrGetCountryId(Customer customer, string user)
        {
            var conn = GetConnection();
            int countryId = 0; // initialize to something

            try
            {
                var createDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var lastUpdate = createDate;
                var createdBy = user;
                var updatedBy = createdBy;

                // ------------- Country -------------
                // Country has the potential to exist already

                var query = $"SELECT countryId FROM country WHERE country = {customer.Country}";
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();

                // query will either return a number to be used later, or will add a new entry to be used
                if (result == null)
                {
                    // country doesn't exist yet, so add it
                    query = "SELECT MAX(countryId) FROM country";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        // no country is present yet in the database
                        result = 1;
                    }
                    else
                    {
                        result = int.Parse(result.ToString()) + 1;
                    }

                    query = $"INSERT INTO country VALUES ({int.Parse(result.ToString())}, '{customer.Country}', '{createDate}', '{createdBy}', '{lastUpdate}', '{updatedBy}')";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
                countryId = int.Parse(result.ToString());
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return countryId;
        }

        private int AddOrGetCityId(Customer customer, string user, int countryId)
        {
            var conn = GetConnection();
            var cityId = 0; // initialize to something
            try
            {
                var createDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var lastUpdate = createDate;
                var createdBy = user;
                var updatedBy = createdBy;

                // ------------- City -------------
                // City has the potential to exist already

                var query = $"SELECT cityId FROM city WHERE city = '{customer.City}' AND countryId = {countryId}";
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();

                // query will either return a number to be used later, or will add a new entry to be used
                if (result == null)
                {
                    // city doesn't exist yet, so add it
                    query = "SELECT MAX(cityId) FROM city";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        // no city is present yet in the datbase
                        result = 1;
                    }
                    else
                    {
                        result = int.Parse(result.ToString()) + 1;
                    }

                    query = $"INSERT INTO city VALUES ({int.Parse(result.ToString())}, '{customer.City}', {countryId}, '{createDate}', '{createdBy}', '{lastUpdate}', '{updatedBy}')";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
                cityId = int.Parse(result.ToString());
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return cityId;
            
        }
    }
}
