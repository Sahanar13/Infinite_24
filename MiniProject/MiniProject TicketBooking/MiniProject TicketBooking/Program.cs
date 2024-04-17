using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MiniProject_TicketBooking;

namespace MiniProject_TicketBooking
{
    class Program
    {
        private static string connectionString = "Server=ICS-LT-9R368G3\\SQLEXPRESS;Database=MiniProjectTicketBooking;Integrated Security=True;";
       

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

        static void AdminLogin()
        {
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
                            Console.WriteLine("Enter the train class:");
                            string trainClass = Console.ReadLine();
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
                            Console.WriteLine("Enter the fare:");
                            decimal fare = Convert.ToDecimal(Console.ReadLine());
                            Console.WriteLine("Is the train active? (true/false):");
                            bool isActive = Convert.ToBoolean(Console.ReadLine());

                            UpdateTrainDetails(trainIdToUpdate, trainClass, trainName, fromStation, toStation, trainManagerName, totalBerths, availableBerths, fare, isActive);
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
        }


        static void UserLogin()
        {
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
                    Console.WriteLine("3. Exit");
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
            using (var context = new MiniProjectTicketBookingEntities())
            {
                
                Console.Write("Enter Train ID: ");
                int trainId = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Class: ");
                string trainClass = Console.ReadLine();
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
                Console.Write("Enter Fare: ");
                decimal fare = Convert.ToDecimal(Console.ReadLine());
                Console.Write("Enter IsActive (true/false): ");
                bool isActive = Convert.ToBoolean(Console.ReadLine());

               
                context.Database.ExecuteSqlCommand("EXEC AddTrain @TrainId, @Class, @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare, @IsActive",
                    new SqlParameter("@TrainId", trainId),
                    new SqlParameter("@Class", trainClass),
                    new SqlParameter("@TrainName", trainName),
                    new SqlParameter("@FromStation", fromStation),
                    new SqlParameter("@ToStation", toStation),
                    new SqlParameter("@TrainManagerName", managerName),
                    new SqlParameter("@TotalBerths", totalBerths),
                    new SqlParameter("@AvailableBerths", availableBerths),
                    new SqlParameter("@Fare", fare),
                    new SqlParameter("@IsActive", isActive));

                Console.WriteLine("Train added successfully.");
            }
        }

        static void DeleteTrain()
        {
            using (var context = new MiniProjectTicketBookingEntities())
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

            using (var context = new MiniProjectTicketBookingEntities())
            {
                var trainToActivate = context.Trains.FirstOrDefault(t => t.TrainId == trainId);
                if (trainToActivate != null)
                {
                    trainToActivate.IsActive = true;
                    context.SaveChanges();
                    Console.WriteLine("Train activated successfully.");
                }
                else
                {
                    Console.WriteLine("Train not found.");
                }
            }
        }

        static void ViewAdminDashboard()
        {
            Console.WriteLine("Viewing admin dashboard...");

            using (var context = new MiniProjectTicketBookingEntities())
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
            Console.Write("Enter Class: ");
            string trainClass = Console.ReadLine();
            Console.Write("Enter Passenger Name: ");
            string passengerName = Console.ReadLine();
            Console.Write("Enter Seats to Book: ");
            int seatsBooked = Convert.ToInt32(Console.ReadLine());

            DateTime bookingDate = DateTime.Now;
            Console.Write("Enter Date of Travel (YYYY-MM-DD): ");
            DateTime dateOfTravel = Convert.ToDateTime(Console.ReadLine());

            decimal totalAmount = 0;
            int bookingId = 0;
            string passengerNameOutput = string.Empty; // Variable to store the output value of @PassengerNameOutput

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand checkTrainCommand = new SqlCommand("SELECT IsActive, [From], [To] FROM Trains WHERE TrainId = @TrainId", connection);
                    checkTrainCommand.Parameters.AddWithValue("@TrainId", trainId);
                    SqlDataReader reader = checkTrainCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        bool isTrainActive = (bool)reader["IsActive"];
                        string fromStation = reader["From"].ToString();
                        string toStation = reader["To"].ToString();

                        if (!isTrainActive)
                        {
                            Console.WriteLine("Booking is not allowed for this train as it is inactive.");
                            reader.Close();
                            return;
                        }

                        reader.Close();

                        using (SqlCommand command = new SqlCommand("BookTicket", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@LoginId", loginId);
                            command.Parameters.AddWithValue("@Password", password);
                            command.Parameters.AddWithValue("@TrainId", trainId);
                            command.Parameters.AddWithValue("@Class", trainClass);
                            command.Parameters.AddWithValue("@PassengerName", passengerName);
                            command.Parameters.AddWithValue("@SeatsBooked", seatsBooked);
                            command.Parameters.AddWithValue("@BookingDate", bookingDate);
                            command.Parameters.AddWithValue("@DateOfTravel", dateOfTravel);

                            // Add @PassengerNameOutput parameter for output
                            SqlParameter passengerNameOutputParam = new SqlParameter("@PassengerNameOutput", SqlDbType.VarChar, 100);
                            passengerNameOutputParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(passengerNameOutputParam);

                            SqlParameter totalAmountParam = new SqlParameter("@Amount", SqlDbType.Decimal);
                            totalAmountParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(totalAmountParam);

                            SqlParameter bookingIdParam = new SqlParameter("@BookingId", SqlDbType.Int);
                            bookingIdParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(bookingIdParam);

                            SqlParameter seatsBookedOutputParam = new SqlParameter("@SeatsBookedOutput", SqlDbType.Int);
                            seatsBookedOutputParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(seatsBookedOutputParam);


                            SqlParameter dateOfTravelOutputParam = new SqlParameter("@DateOfTravelOutput", SqlDbType.Date);
                            dateOfTravelOutputParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(dateOfTravelOutputParam);

                            command.ExecuteNonQuery();

                            totalAmount = (decimal)totalAmountParam.Value;
                            bookingId = (int)bookingIdParam.Value;
                            passengerNameOutput = passengerNameOutputParam.Value.ToString(); // Retrieve output value
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("Your Ticket Booking Details:");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"Booking ID: {bookingId}");
                            Console.WriteLine($"Train ID: {trainId}");
                            Console.WriteLine($"Class: {trainClass}");
                            Console.WriteLine($"Passenger Name: {passengerNameOutput}"); // Use the output value
                            Console.WriteLine($"Seats Booked: {seatsBooked}");
                            Console.WriteLine($"Booking Date: {bookingDate}");
                            Console.WriteLine($"Date of Travel: {dateOfTravel}");
                            Console.WriteLine($"Amount Paid: {totalAmount:C}");
                            Console.WriteLine($"From: {fromStation}");
                            Console.WriteLine($"To: {toStation}");
                            Console.WriteLine("------------------------------------");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Train details not found.");
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
            using (var context = new MiniProjectTicketBookingEntities())
            {
                var trains = context.Trains.ToList();

                Console.WriteLine("------------------------------------------------------------------------------------------");
                Console.WriteLine("| Train ID |  Class  |       Name       | From |   To  | Manager | Berths |  Status   |");
                Console.WriteLine("|----------|---------|------------------|------|-------|---------|--------|-----------|");

                foreach (var train in trains)
                {
                    Console.WriteLine($"| {train.TrainId,-9} | {train.Class,-7} | {train.TrainName,-16} | {train.From,-4} | {train.To,-5} | {train.Name,-8} | {train.TotalBerths,-6} | {((bool)train.IsActive ? "Active" : "Inactive"),-9} |");
                }

                Console.WriteLine("------------------------------------------------------------------------------------------");
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
           
            const string ConnectionString = ("Server=ICS-LT-9R368G3\\SQLEXPRESS;Database=MiniProjectTicketBooking;Integrated Security=True;");

          
            Console.Write("Enter TrainId: ");
            int trainId = int.Parse(Console.ReadLine());

       
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("DeactivateTrain", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TrainId", trainId);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("Train deactivated successfully.");
                    }
                    catch (SqlException ex)
                    {
                       
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

      
       

        static void UpdateTrainDetails(int trainId, string trainClass, string trainName, string fromStation, string toStation, string trainManagerName, int totalBerths, int availableBerths, decimal fare, bool isActive)
        {
            
            const string ConnectionString = "Server = ICS - LT - 9R368G3\\SQLEXPRESS; Database = MiniProjectTicketBooking; Integrated Security = True; ";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("UpdateTrainDetails", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TrainId", trainId);
                        command.Parameters.AddWithValue("@Class", trainClass);
                        command.Parameters.AddWithValue("@TrainName", trainName);
                        command.Parameters.AddWithValue("@FromStation", fromStation);
                        command.Parameters.AddWithValue("@ToStation", toStation);
                        command.Parameters.AddWithValue("@TrainManagerName", trainManagerName);
                        command.Parameters.AddWithValue("@TotalBerths", totalBerths);
                        command.Parameters.AddWithValue("@AvailableBerths", availableBerths);
                        command.Parameters.AddWithValue("@Fare", fare);
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

        

      
        
    }
}

            
        
   