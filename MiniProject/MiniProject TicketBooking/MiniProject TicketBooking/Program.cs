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
            Console.WriteLine("Welcome to Railway Booking!");

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
            Console.WriteLine("\nAdmin Login:");
            Console.Write("Username: ");
            string usernameInput = Console.ReadLine();
            Console.Write("Password: ");
            string passwordInput = Console.ReadLine();

            
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
                    Console.WriteLine("7.DeleteUser");
                    Console.WriteLine("8.UpdateTrainDetails");
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
                            Console.WriteLine("Enter the user ID you want to delete:");
                            int userIdToDelete = Convert.ToInt32(Console.ReadLine());
                            DeleteUser(userIdToDelete);
                            break;

                        case "8":
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


        static void UserLogin()
        {
            Console.WriteLine("\nUser Login:");
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
            string password = Console.ReadLine();

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
                foreach (var booking in bookings)
                {
                    Console.WriteLine($"Booking ID: {booking.BookingId}, Train ID: {booking.TrainId}, Class: {booking.Class}, Passenger Name: {booking.PassengerName}, Seats Booked: {booking.SeatsBooked}, Booking Date: {booking.BookingDate}, Date of Travel: {booking.DateOfTravel}, Total Amount: {booking.TotalAmount}");
                }

                var cancellations = context.Cancellations.Include("Booking").ToList(); 
                Console.WriteLine("\nCancelled Tickets:");
                foreach (var cancellation in cancellations)
                {
                    Console.WriteLine($"Cancellation ID: {cancellation.CancellationId}, Booking ID: {cancellation.BookingId}, Seats Cancelled: {cancellation.SeatsCancelled}, Cancellation Date: {cancellation.CancellationDate}, Cancellation Reason: {cancellation.CancellationReason}");

                    
                }
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

                       
                        SqlParameter totalAmountParam = new SqlParameter("@Amount", SqlDbType.Decimal);
                        totalAmountParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(totalAmountParam);

                        SqlParameter bookingIdParam = new SqlParameter("@BookingId", SqlDbType.Int);
                        bookingIdParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(bookingIdParam);

                        SqlParameter passengerNameOutputParam = new SqlParameter("@PassengerNameOutput", SqlDbType.VarChar, 100);
                        passengerNameOutputParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(passengerNameOutputParam);

                        SqlParameter seatsBookedOutputParam = new SqlParameter("@SeatsBookedOutput", SqlDbType.Int);
                        seatsBookedOutputParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(seatsBookedOutputParam);

                        SqlParameter dateOfTravelOutputParam = new SqlParameter("@DateOfTravelOutput", SqlDbType.Date);
                        dateOfTravelOutputParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(dateOfTravelOutputParam);

                        command.ExecuteNonQuery();

                       
                        totalAmount = (decimal)totalAmountParam.Value;
                        bookingId = (int)bookingIdParam.Value;
                        string passengerNameOutput = passengerNameOutputParam.Value.ToString();
                        int seatsBookedOutput = (int)seatsBookedOutputParam.Value;
                        DateTime dateOfTravelOutput = (DateTime)dateOfTravelOutputParam.Value;

                        
                        Console.WriteLine("Your Ticket Booking Details:");
                        Console.WriteLine($"Booking ID: {bookingId}");
                        Console.WriteLine($"Train ID: {trainId}");
                        Console.WriteLine($"Class: {trainClass}");
                        Console.WriteLine($"Passenger Name: {passengerNameOutput}");
                        Console.WriteLine($"Seats Booked: {seatsBookedOutput}");
                        Console.WriteLine($"Booking Date: {bookingDate}");
                        Console.WriteLine($"Date of Travel: {dateOfTravelOutput}");
                        Console.WriteLine($"Amount Paid: {totalAmount:C}");
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

                       
                        Console.WriteLine("Your Ticket Cancellation Details:");
                        Console.WriteLine($"Booking ID: {bookingId}");
                        Console.WriteLine($"Seats Cancelled: {seatsToCancel}");
                        Console.WriteLine($"Refund Amount: {refundAmount:C}");
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
                foreach (var train in trains)
                {
                    Console.WriteLine($"Train ID: {train.TrainId}");
                    Console.WriteLine($"Class: {train.Class}");
                    Console.WriteLine($"Name: {train.TrainName}");
                    Console.WriteLine($"From: {train.From}");
                    Console.WriteLine($"To: {train.To}");
                    Console.WriteLine($"Manager: {train.Name}");
                    Console.WriteLine($"Total Berths: {train.TotalBerths}");
                    Console.WriteLine($"Available Berths: {train.AvailableBerths}");
                    Console.WriteLine($"Fare: {train.Fare}");
                    Console.WriteLine($"IsActive: {train.IsActive}");
                    Console.WriteLine(); 
                }
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

        static void DeleteUser(int userId)
        {
            const string ConnectionString = "YourConnectionString";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("DeleteUser", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);

                        connection.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("User deleted successfully.");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
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

            
        
   