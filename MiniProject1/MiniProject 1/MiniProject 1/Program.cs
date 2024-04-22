using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MiniProject_1;
using MiniProject_TicketBooking;

namespace MiniProject_TicketBooking
{
    class Program
    {
        private static string connectionString = "Server=ICS-LT-9R368G3\\SQLEXPRESS;Database=MiniProjectTicketBooking1;Integrated Security=True;";
        private static object totalAmount;

        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔══════════════════════════════╗");
            Console.WriteLine("║    Welcome to Railway Booking!   ║");
            Console.WriteLine("╚══════════════════════════════╝");
            Console.ResetColor();
            // Rest of your code follows...



            while (true)
            {

                Console.WriteLine("Select Option:");
                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. User Login");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AdminLogin();
                        break;
                    case "2":
                        UserLogin();
                        break;
                    case "3":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }


            }
        }
        static void DisplayMainMenu()
        {
            Console.WriteLine("Select Option:");
            Console.WriteLine("1. Admin Login");
            Console.WriteLine("2. User Login");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");
        }

        static void WaitForEnter()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void AdminLogin()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("══════════════════════════════");
            Console.WriteLine("          Admin Login:          ");
            Console.WriteLine("══════════════════════════════");
            Console.ResetColor();

            Console.Write("Username: ");
            string usernameInput = Console.ReadLine();
            Console.Write("Password: ");
            string passwordInput = MaskPassword();


            string adminUsername = "admin";
            string adminPassword = "admin123";

            if (usernameInput == adminUsername && passwordInput == adminPassword)
            {
                Console.WriteLine("Admin logged in successfully.");

                while (true)
                {
                    Console.WriteLine("\nSelect Option:");
                    Console.WriteLine("1. Add Train");
                    Console.WriteLine("2. Delete Train");
                    Console.WriteLine("3. Activate Train");
                    Console.WriteLine("4. View Booked and Cancelled Tickets");
                    Console.WriteLine("5.DeactivatedTrain");
                    Console.WriteLine("6.GenerateTotalRevenue");
                    Console.WriteLine("7.UpdateTrainDetails");
                    Console.WriteLine("8.DisplayAllTrains");
                    Console.WriteLine("9. Logout");
                    Console.Write("Enter your choice: ");
                    string adminChoice = Console.ReadLine();

                    switch (adminChoice)
                    {
                        case "1":
                            AddTrain();
                            break;
                        case "2":
                            DeleteTrain();
                            break;
                        case "3":
                            ActivateTrain();
                            break;
                        case "4":
                            ViewAdminDashboard();
                            break;
                        case "5":
                            DeactivatedTrain();
                            break;
                        case "6":
                            GenerateTotalRevenue();
                            break;

                        case "7":
                            Console.WriteLine("Enter the train ID you want to update:");
                            int trainIdToUpdate = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter the train name:");
                            string trainName = Console.ReadLine();
                            Console.WriteLine("Enter the from station:");
                            string fromStation = Console.ReadLine();
                            Console.WriteLine("Enter the to station:");
                            string toStation = Console.ReadLine();
                            Console.WriteLine("Enter the train manager name:");
                            string trainManagerName = Console.ReadLine();
                            Console.WriteLine("Enter the total berths:");
                            int totalBerths = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter the available berths:");
                            int availableBerths = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter the fare for Class 1:");
                            decimal fare1 = Convert.ToDecimal(Console.ReadLine());
                            Console.WriteLine("Enter the fare for Class 2:");
                            decimal fare2 = Convert.ToDecimal(Console.ReadLine());
                            Console.WriteLine("Enter the fare for Class 3:");
                            decimal fare3 = Convert.ToDecimal(Console.ReadLine());
                            Console.WriteLine("Is the train active? (true/false):");
                            bool isActive = Convert.ToBoolean(Console.ReadLine());

                            UpdateTrainDetails(trainIdToUpdate, trainName, fromStation, toStation, trainManagerName, totalBerths, availableBerths, fare1, fare2, fare3, isActive);
                            break;

                        case "8":
                            DisplayAllTrains();
                            break;

                        case "9":
                            Console.WriteLine("Logging out...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid admin login credentials.");
            }
        }
        static string MaskPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Ignore any key other than Backspace or Enter
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine(); // For newline after password input
            return password;
            WaitForEnter();
        }


        static void UserLogin()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("══════════════════════════════");
            Console.WriteLine("          User Login:          ");
            Console.WriteLine("══════════════════════════════");
            Console.ResetColor();

            Console.WriteLine("Are you an existing user? (yes/no)");
            string existingUserChoice = Console.ReadLine().ToLower();

            switch (existingUserChoice)
            {
                case "yes":
                    UserExistingLogin();
                    break;
                case "no":
                    UserSignUp();
                    UserExistingLogin();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
                    WaitForEnter();
            }
        }


        static void UserExistingLogin()
        {
            Console.Write("Login ID: ");
            string loginId = Console.ReadLine();

            Console.Write("Password: ");
            string password = ReadPassword();

            bool isValidLogin = false;
            bool isExistingUser = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("UserLoginAndSignup", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@LoginId", loginId);
                    command.Parameters.AddWithValue("@Password", password);

                    command.Parameters.Add("@IsExistingUser", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@IsValid", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();

                    isExistingUser = (bool)command.Parameters["@IsExistingUser"].Value;
                    isValidLogin = (bool)command.Parameters["@IsValid"].Value;
                }
            }

            if (isValidLogin)
            {
                Console.WriteLine("User logged in successfully.");

                while (true)
                {
                    Console.WriteLine("Select Option:");
                    Console.WriteLine("1. Book Ticket");
                    Console.WriteLine("2. Cancel Ticket");
                    Console.WriteLine("3. View Booking History");
                    Console.WriteLine("4. Print Ticket");

                    Console.WriteLine("5. Exit");
                    Console.Write("Enter your choice: ");
                    string userChoice = Console.ReadLine();

                    switch (userChoice)
                    {
                        case "1":
                            BookTicket(loginId, password);
                            break;
                        case "2":
                            CancelTicket();
                            break;
                        case "3":
                            ViewBookingHistory(loginId);
                            break;
                        case "4":
                            Console.WriteLine("Enter the booking ID to print ticket:");
                            int bookingId = int.Parse(Console.ReadLine());
                            PrintTicket(bookingId);
                            break;
                        case "5":
                            Console.WriteLine("Logging out...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
            }
            else if (!isExistingUser)
            {
                Console.WriteLine("User does not exist. Please sign up.");
                UserSignUp();
            }
            else
            {
                Console.WriteLine("Invalid user login credentials.");
            }
        }

        // Method to read password with masking
        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }


        static void UserSignUp()
        {
            Console.WriteLine("\nUser Sign Up:");
            Console.Write("Enter new Login ID: ");
            string newLoginId = Console.ReadLine();
            Console.Write("Enter new Password: ");
            string newPassword = Console.ReadLine();



            Console.WriteLine("User signed up successfully.");

        }


        static void AddTrain()
        {
            using (var context = new MiniProjectTicketBooking1Entities())
            {
                Console.Write("Enter Train ID: ");
                int trainId = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Train Name: ");
                string trainName = Console.ReadLine();
                Console.Write("Enter From Station: ");
                string fromStation = Console.ReadLine();
                Console.Write("Enter To Station: ");
                string toStation = Console.ReadLine();
                Console.Write("Enter Train Manager Name: ");
                string managerName = Console.ReadLine();
                Console.Write("Enter Total Berths: ");
                int totalBerths = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Available Berths: ");
                int availableBerths = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Fare for First Class: ");
                decimal fare1 = Convert.ToDecimal(Console.ReadLine());
                Console.Write("Enter IsActive (true/false): ");
                bool isActive = Convert.ToBoolean(Console.ReadLine());

                try
                {
                    // Execute the stored procedure
                    context.Database.ExecuteSqlCommand("EXEC AddTrain @TrainId, @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare1, @IsActive",
                        new SqlParameter("@TrainId", trainId),
                        new SqlParameter("@TrainName", trainName),
                        new SqlParameter("@FromStation", fromStation),
                        new SqlParameter("@ToStation", toStation),
                        new SqlParameter("@TrainManagerName", managerName),
                        new SqlParameter("@TotalBerths", totalBerths),
                        new SqlParameter("@AvailableBerths", availableBerths),
                        new SqlParameter("@Fare1", fare1),
                        new SqlParameter("@IsActive", isActive));

                    Console.WriteLine("Train added successfully.");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred while adding the train:");
                    Console.WriteLine(ex.Message);
                }
            }
        }




        static void DeleteTrain()
        {
            using (var context = new MiniProjectTicketBooking1Entities())
            {

                Console.Write("Enter Train ID to delete: ");
                int trainIdToDelete = Convert.ToInt32(Console.ReadLine());


                context.Database.ExecuteSqlCommand("EXEC SoftDeleteTrain @TrainId",
                    new SqlParameter("@TrainId", trainIdToDelete));

                Console.WriteLine("Train deleted successfully (soft delete).");
            }
        }

        static void ActivateTrain()
        {
            Console.WriteLine("Activating a train...");

            Console.Write("Enter Train ID to activate: ");
            int trainId = Convert.ToInt32(Console.ReadLine());

            using (var context = new MiniProjectTicketBooking1Entities())
            {
                var trainsToActivate = context.Trains.Where(t => t.TrainId == trainId);

                foreach (var trainToActivate in trainsToActivate)
                {
                    trainToActivate.IsActive = true;
                }

                context.SaveChanges();
                Console.WriteLine("Train activated successfully.");
            }
        }


        static void ViewAdminDashboard()
        {
            Console.WriteLine("Viewing admin dashboard...");

            using (var context = new MiniProjectTicketBooking1Entities())
            {
                var bookings = context.Bookings.ToList();
                Console.WriteLine("Booked Tickets:");
                Console.WriteLine("======================================================================================================================");
                Console.WriteLine("| Booking ID | Train ID | Class   | Passenger Name | Seats Booked | Booking Date         | Date of Travel       | Total Amount |");
                Console.WriteLine("======================================================================================================================");
                foreach (var booking in bookings)
                {
                    Console.WriteLine($"| {booking.BookingId,-11} | {booking.TrainId,-8} | {booking.Class,-7} | {booking.PassengerName,-15} | {booking.SeatsBooked,-12} | {booking.BookingDate,-20} | {booking.DateOfTravel,-20} | {booking.TotalAmount,-13} |");
                }
                Console.WriteLine("======================================================================================================================");

                var cancellations = context.Cancellations.Include("Booking").ToList();
                Console.WriteLine("\nCancelled Tickets:");
                Console.WriteLine("======================================================================================================================");
                Console.WriteLine("| Cancellation ID | Booking ID | Seats Cancelled | Cancellation Date      | Cancellation Reason                          |");
                Console.WriteLine("======================================================================================================================");
                foreach (var cancellation in cancellations)
                {
                    Console.WriteLine($"| {cancellation.CancellationId,-16} | {cancellation.BookingId,-10} | {cancellation.SeatsCancelled,-15} | {cancellation.CancellationDate,-23} | {cancellation.CancellationReason,-43} |");
                }
                Console.WriteLine("======================================================================================================================");
            }
        }

        static void BookTicket(string loginId, string password)
        {
            Console.WriteLine("Booking a ticket...");

            DisplayAllTrains();

            Console.Write("Enter Train ID: ");
            int trainId = Convert.ToInt32(Console.ReadLine());

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand checkTrainCommand = new SqlCommand("SELECT IsActive FROM Trains WHERE TrainId = @TrainId", connection);
                    checkTrainCommand.Parameters.AddWithValue("@TrainId", trainId);
                    bool isTrainActive = (bool)checkTrainCommand.ExecuteScalar();

                    if (!isTrainActive)
                    {
                        Console.WriteLine("Booking is not allowed for this train as it is inactive.");
                        return;
                    }

                    Console.Write("Enter Class: ");
                    string trainClass = Console.ReadLine();

                    Console.Write("Enter Seats to Book: ");
                    int seatsBooked = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Enter Passenger Name(s) (comma-separated if multiple): ");
                    string passengerNames = Console.ReadLine();

                    DateTime bookingDate = DateTime.Now;
                    Console.Write("Enter Date of Travel (YYYY-MM-DD): ");
                    DateTime dateOfTravel = Convert.ToDateTime(Console.ReadLine());

                    using (SqlCommand command = new SqlCommand("BookTicket", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@LoginId", loginId);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@TrainId", trainId);
                        command.Parameters.AddWithValue("@Class", trainClass);
                        command.Parameters.AddWithValue("@SeatsBooked", seatsBooked);
                        command.Parameters.AddWithValue("@PassengerNames", passengerNames);
                        command.Parameters.AddWithValue("@BookingDate", bookingDate);
                        command.Parameters.AddWithValue("@DateOfTravel", dateOfTravel);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int bookingId = reader.GetInt32(0);
                                decimal amount = reader.GetDecimal(1);

                                Console.WriteLine("------------------------------------");
                                Console.WriteLine("Your Ticket Booking Details:");
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine($"Booking ID: {bookingId}");
                                Console.WriteLine($"Total Amount: {amount:C}");
                                Console.WriteLine($"Train ID: {trainId}");
                                Console.WriteLine($"Class: {trainClass}");
                                Console.WriteLine($"Passenger Name(s): {passengerNames}");
                                Console.WriteLine($"Seats Booked: {seatsBooked}");
                                Console.WriteLine($"Booking Date: {bookingDate}");
                                Console.WriteLine($"Date of Travel: {dateOfTravel}");
                                Console.WriteLine("------------------------------------");
                            }
                        }
                    }
                }

                Console.WriteLine("Ticket booked successfully!");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 12345)
                {
                    Console.WriteLine("Booking is not allowed for this train as it is inactive.");
                }
                else
                {
                    Console.WriteLine("An error occurred while booking the ticket.");
                    Console.WriteLine(ex.Message);
                }
            }
        }





        static void CancelTicket()
        {
            Console.WriteLine("Canceling a ticket...");

            Console.Write("Enter Booking ID: ");
            int bookingId = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Seats to Cancel: ");
            int seatsToCancel = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Passenger Names to Delete (separated by comma if multiple): ");
            string passengerNamesToDelete = Console.ReadLine();
            Console.Write("Enter Cancellation Reason: ");
            string cancellationReason = Console.ReadLine();

            decimal refundAmount = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("CancelTicket", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@BookingId", bookingId);
                        command.Parameters.AddWithValue("@SeatsToCancel", seatsToCancel);
                        command.Parameters.AddWithValue("@PassengerNamesToDelete", passengerNamesToDelete);
                        command.Parameters.AddWithValue("@CancellationReason", !string.IsNullOrWhiteSpace(cancellationReason) ? cancellationReason : null);

                        SqlParameter refundAmountParam = new SqlParameter("@RefundAmount", SqlDbType.Decimal);
                        refundAmountParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(refundAmountParam);

                        command.ExecuteNonQuery();

                        refundAmount = (decimal)refundAmountParam.Value;

                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("Your Ticket Cancellation Details:");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"Booking ID: {bookingId}");
                        Console.WriteLine($"Seats Cancelled: {seatsToCancel}");
                        Console.WriteLine($"Refund Amount: {refundAmount:C}");
                        Console.WriteLine("------------------------------------");
                    }
                }

                Console.WriteLine("Ticket is Cancelled.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred while canceling the ticket.");
                Console.WriteLine(ex.Message);
            }
        }


        static void DisplayAllTrains()
        {
            using (var context = new MiniProjectTicketBooking1Entities())
            {
                var trains = context.Trains.ToList();

                Console.WriteLine("---------------------------------------------------------------------------------------------------");
                Console.WriteLine("| Train ID |  Class  |       Name       | From |   To  | Manager | Total Berths | Available Berths | Fare   | Status   |");
                Console.WriteLine("|----------|---------|------------------|------|-------|---------|---------------|------------------|--------|----------|");

                foreach (var train in trains)
                {
                    Console.WriteLine($"| {train.TrainId,-9} | {train.Class,-7} | {train.TrainName,-16} | {train.From,-4} | {train.To,-5} | {train.Name,-8} | {train.TotalBerths,-13} | {train.AvailableBerths,-16} | {train.Fare,-6:C} | {((bool)train.IsActive ? "Active" : "Inactive"),-9} |");
                }

                Console.WriteLine("---------------------------------------------------------------------------------------------------");
            }
        }



        static void GenerateTotalRevenue()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GenerateTotalRevenue", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            decimal totalRevenue = reader.GetDecimal(0);
                            Console.WriteLine($"Total Revenue Generated: {totalRevenue:C}");
                        }
                        else
                        {
                            Console.WriteLine("No revenue generated yet.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating total revenue: {ex.Message}");
            }
        }


        static void DeactivatedTrain()
        {
            const string ConnectionString = "Server=ICS-LT-9R368G3\\SQLEXPRESS;Database=MiniProjectTicketBooking1;Integrated Security=True;";

            Console.Write("Enter TrainId: ");
            int trainId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("DeactivatedTrain", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TrainId", trainId);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("Train and all associated classes deactivated successfully.");
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }







        static void UpdateTrainDetails(int trainId, string trainName, string fromStation, string toStation, string trainManagerName, int totalBerths, int availableBerths, decimal fare1, decimal fare2, decimal fare3, bool isActive)
        {
            const string ConnectionString = "Server=ICS-LT-9R368G3\\SQLEXPRESS; Database=MiniProjectTicketBooking; Integrated Security=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("UpdateTrainDetails", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TrainId", trainId);
                        command.Parameters.AddWithValue("@TrainName", trainName);
                        command.Parameters.AddWithValue("@FromStation", fromStation);
                        command.Parameters.AddWithValue("@ToStation", toStation);
                        command.Parameters.AddWithValue("@TrainManagerName", trainManagerName);
                        command.Parameters.AddWithValue("@TotalBerths", totalBerths);
                        command.Parameters.AddWithValue("@AvailableBerths", availableBerths);
                        command.Parameters.AddWithValue("@Fare1", fare1);
                        command.Parameters.AddWithValue("@Fare2", fare2);
                        command.Parameters.AddWithValue("@Fare3", fare3);
                        command.Parameters.AddWithValue("@IsActive", isActive);

                        connection.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("Train details updated successfully.");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        static void ViewBookingHistory(string loginId)
        {
            try
            {
                Console.WriteLine("Enter the Booking ID to view details:");
                int bookingId = int.Parse(Console.ReadLine());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("ViewBookingHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LoginId", loginId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            bool found = false;

                            while (reader.Read())
                            {
                                if ((int)reader["BookingId"] == bookingId)
                                {
                                    Console.WriteLine("Booking Details:");
                                    Console.WriteLine($"Booking ID: {reader["BookingId"]}");
                                    Console.WriteLine($"Train ID: {reader["TrainId"]}");
                                    Console.WriteLine($"Class: {reader["Class"]}");
                                    Console.WriteLine($"Passenger Name: {reader["PassengerName"]}");
                                    Console.WriteLine($"Seats Booked: {reader["SeatsBooked"]}");
                                    Console.WriteLine($"Booking Date: {reader["BookingDate"]}");
                                    Console.WriteLine($"Date of Travel: {reader["DateOfTravel"]}");
                                    Console.WriteLine($"Total Amount: {reader["TotalAmount"]}");
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                Console.WriteLine("Booking ID not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }


        static void PrintTicket(int bookingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PrintTicket", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookingId", bookingId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("Printing Ticket:");
                                Console.WriteLine("===============================================");
                                Console.WriteLine($"|{"Train Name",-15}|{reader["Train Name"],-30}|");
                                Console.WriteLine($"|{"From Station",-15}|{reader["From Station"],-30}|");
                                Console.WriteLine($"|{"To Station",-15}|{reader["To Station"],-30}|");
                                Console.WriteLine($"|{"Class",-15}|{reader["Class"],-30}|");
                                Console.WriteLine($"|{"Passenger Name",-15}|{reader["PassengerName"],-30}|");
                                Console.WriteLine($"|{"Seats Booked",-15}|{reader["Seats Booked"],-30}|");
                                Console.WriteLine($"|{"Booking Date",-15}|{reader["Booking Date"],-30}|");
                                Console.WriteLine($"|{"Date of Travel",-15}|{reader["Date of Travel"],-30}|");
                                Console.WriteLine($"|{"Total Amount",-15}|{reader["Total Amount"],-30}|");
                                Console.WriteLine("===============================================");
                            }
                            else
                            {
                                Console.WriteLine("Ticket not found for the specified Booking ID.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }





    }


}










