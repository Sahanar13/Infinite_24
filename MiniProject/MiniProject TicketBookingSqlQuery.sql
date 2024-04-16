Create database MiniProjectTicketBooking
use MiniProjectTicketBooking
Create TABLE Trains (
    TrainId INT,
    Class VARCHAR(50),
    TrainName VARCHAR(100),
    [From] VARCHAR(100),
    [To] VARCHAR(100),
    Name VARCHAR(100),
    TotalBerths INT,
    AvailableBerths INT,
    Fare DECIMAL(10, 2),
    IsActive BIT,
    PRIMARY KEY (TrainId, Class) 
);



Create TABLE Booking (
    BookingId INT PRIMARY KEY IDENTITY,
    TrainId INT,
    Class VARCHAR(50), 
    PassengerName VARCHAR(100),
    SeatsBooked INT,
    BookingDate DATETIME,
    DateOfTravel DATE,
    TotalAmount DECIMAL(10, 2),
    FOREIGN KEY (TrainId, Class) REFERENCES Trains(TrainId, Class) 
);



SELECT * FROM Booking;
SELECT * FROM Trains;
SELECT * FROM Cancellation
SELECT * FROM Users





CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    LoginId NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL
);


CREATE OR ALTER PROCEDURE UserLoginAndSignup
    @LoginId NVARCHAR(50),
    @Password NVARCHAR(50),
    @IsExistingUser BIT OUTPUT,
    @IsValid BIT OUTPUT
AS
BEGIN
    SET @IsExistingUser = 0; 

   
    IF EXISTS (SELECT 1 FROM Users WHERE LoginId = @LoginId)
    BEGIN
        SET @IsExistingUser = 1; 
    END
    ELSE
    BEGIN
        
        INSERT INTO Users (LoginId, Password) VALUES (@LoginId, @Password);
    END

    
    IF EXISTS (SELECT 1 FROM Users WHERE LoginId = @LoginId AND Password = @Password)
    BEGIN
        SET @IsValid = 1; 
    END
    ELSE
    BEGIN
        SET @IsValid = 0; 
    END
END;


CREATE TABLE Cancellation (
    CancellationId INT PRIMARY KEY IDENTITY,
    BookingId INT,
    SeatsCancelled INT,
    CancellationDate DATETIME,
    FOREIGN KEY (BookingId) REFERENCES Booking(BookingId)
);
ALTER TABLE Cancellation
ADD CancellationReason NVARCHAR(MAX);
select * from Cancellation

CREATE OR ALTER PROCEDURE AddTrain
    @TrainId INT,
    @Class VARCHAR(50),
    @TrainName VARCHAR(100),
    @FromStation VARCHAR(100),
    @ToStation VARCHAR(100),
    @TrainManagerName VARCHAR(100),
    @TotalBerths INT,
    @AvailableBerths INT,
    @Fare DECIMAL(10, 2),
    @IsActive BIT
AS
BEGIN
    IF @IsActive = 1 
    BEGIN
        INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
        VALUES (@TrainId, @Class, @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare, 1); 
    END
    ELSE
    BEGIN
        INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
        VALUES (@TrainId, @Class, @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare, 0); 
    END

    PRINT 'Train added successfully.'
END


CREATE OR ALTER PROCEDURE BookTicket
    @LoginId NVARCHAR(50),
    @Password NVARCHAR(50),
    @TrainId INT,
    @Class VARCHAR(50),
    @PassengerName VARCHAR(100),
    @SeatsBooked INT,
    @BookingDate DATETIME,
    @DateOfTravel DATE,
    @Amount DECIMAL(10, 2) OUTPUT,
    @BookingId INT OUTPUT,
    @PassengerNameOutput VARCHAR(100) OUTPUT,
    @SeatsBookedOutput INT OUTPUT,
    @DateOfTravelOutput DATE OUTPUT
AS
BEGIN
    DECLARE @UserId INT
    
   
    SELECT @UserId = UserId
    FROM Users
    WHERE LoginId = @LoginId AND Password = @Password
    
    IF @UserId IS NULL
    BEGIN
        PRINT 'Invalid login credentials. Booking failed.'
        RETURN
    END
    
   
    DECLARE @AvailableSeats INT
    SELECT @AvailableSeats = AvailableBerths
    FROM Trains
    WHERE TrainId = @TrainId AND Class = @Class
    
    IF @AvailableSeats < @SeatsBooked
    BEGIN
        PRINT 'Requested seats not available. Booking failed.'
        RETURN
    END
    
    
    DECLARE @IsTrainActive BIT
    SELECT @IsTrainActive = IsActive
    FROM Trains
    WHERE TrainId = @TrainId
    
    IF @IsTrainActive = 0
    BEGIN
        PRINT 'Train is inactive. Booking failed.'
        RETURN
    END
    
   
    SET @Amount = @SeatsBooked * (SELECT Fare FROM Trains WHERE TrainId = @TrainId AND Class = @Class)
    
    
    UPDATE Trains
    SET AvailableBerths = AvailableBerths - @SeatsBooked
    WHERE TrainId = @TrainId AND Class = @Class
    
   
    INSERT INTO Booking (TrainId, Class, PassengerName, SeatsBooked, BookingDate, DateOfTravel, TotalAmount)
    VALUES (@TrainId, @Class, @PassengerName, @SeatsBooked, @BookingDate, @DateOfTravel, @Amount)
    
    
    SET @BookingId = SCOPE_IDENTITY(); 
    
   
    SET @PassengerNameOutput = @PassengerName
    SET @SeatsBookedOutput = @SeatsBooked
    SET @DateOfTravelOutput = @DateOfTravel
    
   
   -- Print booking details
    PRINT 'Ticket(s) booked successfully.'
    PRINT 'Booking ID: ' + CAST(@BookingId AS NVARCHAR(20))
    PRINT 'Total Amount: ' + CAST(@Amount AS NVARCHAR(20))
    PRINT 'Passenger Name: ' + @PassengerName
    PRINT 'Seats Booked: ' + CAST(@SeatsBooked AS NVARCHAR(10))
    PRINT 'Date of Travel: ' + CONVERT(NVARCHAR(20), @DateOfTravel, 23)
    PRINT 'Class: ' + @Class
    PRINT 'Booking Date: ' + CONVERT(NVARCHAR(20), @BookingDate, 23)
    
    -- Display additional ticket details such as from and to, if available
    DECLARE @FromStation NVARCHAR(100), @ToStation NVARCHAR(100)
    SELECT @FromStation = [From], @ToStation = [To]
    FROM Trains
    WHERE TrainId = @TrainId
    
    IF @FromStation IS NOT NULL AND @ToStation IS NOT NULL
    BEGIN
        PRINT 'From: ' + @FromStation
        PRINT 'To: ' + @ToStation
    END
END






CREATE OR ALTER PROCEDURE CancelTicket
    @BookingId INT,
    @SeatsToCancel INT,
    @CancellationReason NVARCHAR(MAX),
    @RefundAmount DECIMAL(10, 2) OUTPUT
AS
BEGIN
    DECLARE @TrainId INT, @Class VARCHAR(50), @Amount DECIMAL(10, 2)
    
    
    SELECT @TrainId = TrainId, @Class = Class, @Amount = TotalAmount
    FROM Booking
    WHERE BookingId = @BookingId
    
   
    IF @TrainId IS NULL
    BEGIN
        PRINT 'Booking not found. Cancellation failed.'
        RETURN
    END
    
   
    SET @RefundAmount = @SeatsToCancel * (@Amount / (SELECT SeatsBooked FROM Booking WHERE BookingId = @BookingId))
    
   
    UPDATE Trains
    SET AvailableBerths = AvailableBerths + @SeatsToCancel
    WHERE TrainId = @TrainId AND Class = @Class
    
   
    UPDATE Booking
    SET SeatsBooked = SeatsBooked - @SeatsToCancel,
        TotalAmount = TotalAmount - @RefundAmount
    WHERE BookingId = @BookingId
    
   
    IF @CancellationReason IS NOT NULL AND @CancellationReason <> ''
    BEGIN
        INSERT INTO Cancellation (BookingId, SeatsCancelled, CancellationDate, CancellationReason)
        VALUES (@BookingId, @SeatsToCancel, GETDATE(), @CancellationReason)
    END
    ELSE
    BEGIN
        INSERT INTO Cancellation (BookingId, SeatsCancelled, CancellationDate)
        VALUES (@BookingId, @SeatsToCancel, GETDATE())
    END
    
    PRINT 'Ticket(s) cancelled successfully.'
    PRINT 'Refund Amount: ' + CAST(@RefundAmount AS NVARCHAR(20))
    
    
    SELECT @RefundAmount AS RefundAmount
END



CREATE OR ALTER PROCEDURE ViewAdminDashboard
AS
BEGIN
    SET NOCOUNT ON;

    PRINT 'Viewing admin dashboard...';

    
    PRINT 'Booked Tickets:';
    SELECT 
        b.BookingId,
        b.TrainId,
        b.Class,
        b.PassengerName,
        b.SeatsBooked,
        b.BookingDate,
        b.DateOfTravel,
        b.TotalAmount
    FROM 
        Bookings b;

   
    PRINT 'Cancelled Tickets:';
    SELECT 
        c.CancellationId,
        c.BookingId,
        c.SeatsCancelled,
        c.CancellationDate,
        c.CancellationReason,
        b.CancellationReason AS BookingCancellationReason
    FROM 
        Cancellations c
    INNER JOIN 
        Bookings b ON c.BookingId = b.BookingId;
END


	CREATE OR ALTER PROCEDURE ActivateTrain
    @TrainId INT
AS
BEGIN
    UPDATE Trains
    SET IsActive = 1
    WHERE TrainId = @TrainId;
END






	CREATE OR ALTER PROCEDURE ActivateTrain
    @TrainId INT
AS
BEGIN
    UPDATE Trains
    SET IsActive = 1
    WHERE TrainId = @TrainId;
END

CREATE OR ALTER PROCEDURE DeleteTrain
    @TrainId INT
AS
BEGIN
    SET NOCOUNT ON;

    
    IF EXISTS (SELECT 1 FROM Trains WHERE TrainId = @TrainId)
    BEGIN
       
        DELETE FROM Trains WHERE TrainId = @TrainId;
        PRINT 'Train deleted successfully.';
    END
    ELSE
    BEGIN
        PRINT 'Train not found.';
    END
END



CREATE OR ALTER PROCEDURE GetBookedTickets
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        BookingId, 
        TrainId, 
        Class, 
        PassengerName, 
        SeatsBooked, 
        BookingDate, 
        DateOfTravel, 
        TotalAmount
    FROM 
        Booking;
END


CREATE OR ALTER PROCEDURE GetCancelledTickets
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CancellationId, 
        BookingId, 
        SeatsCancelled, 
        CancellationDate, 
        CancellationReason
    FROM 
        Cancellation;
END


CREATE OR ALTER PROCEDURE GetCancelledTickets
AS
BEGIN
    SELECT c.CancellationId, c.BookingId, c.SeatsCancelled, c.CancellationDate, c.CancellationReason,
           b.PassengerName  
    FROM Cancellations c
    INNER JOIN Bookings b ON c.BookingId = b.BookingId;
END

CREATE OR ALTER PROCEDURE SoftDeleteTrain
    @TrainId INT
AS
BEGIN
    UPDATE Trains
    SET IsActive = 0
    WHERE TrainId = @TrainId;
END;




CREATE OR ALTER PROCEDURE GenerateTotalRevenue
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalRevenue DECIMAL(10, 2);

    SELECT @TotalRevenue = SUM(TotalAmount)
    FROM Booking;

    PRINT 'Total Revenue Generated: ' + CAST(@TotalRevenue AS NVARCHAR(20));
END;


CREATE OR ALTER PROCEDURE DeleteUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

   
    IF EXISTS (SELECT 1 FROM Users WHERE LoginId = @UserId)
    BEGIN
        
        DELETE FROM Users WHERE LoginId = @UserId;
        PRINT 'User deleted successfully.';
    END
    ELSE
    BEGIN
        PRINT 'User not found.';
    END
END;


CREATE OR ALTER PROCEDURE DeactivateTrain
    @TrainId INT
AS
BEGIN
    UPDATE Trains
    SET IsActive = 0
    WHERE TrainId = @TrainId;
END;


CREATE OR ALTER PROCEDURE ViewBookedTicket
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TrainName VARCHAR(100),
            @FromStation VARCHAR(100),
            @ToStation VARCHAR(100),
            @Class VARCHAR(50),
            @PassengerName VARCHAR(100),
            @SeatsBooked INT,
            @BookingDate DATETIME,
            @DateOfTravel DATE,
            @TotalAmount DECIMAL(10, 2);

   
    SELECT  @TrainName = t.TrainName,
            @FromStation = t.[From],
            @ToStation = t.[To],
            @Class = b.Class,
            @PassengerName = b.PassengerName,
            @SeatsBooked = b.SeatsBooked,
            @BookingDate = b.BookingDate,
            @DateOfTravel = b.DateOfTravel,
            @TotalAmount = b.TotalAmount
    FROM    Booking b
    INNER JOIN Trains t ON b.TrainId = t.TrainId
    WHERE   b.BookingId = @BookingId;

   
    IF @@ROWCOUNT > 0
    BEGIN
        
        PRINT '----------------------------------------------';
        PRINT '              BOOKED TICKET                   ';
        PRINT '----------------------------------------------';
        PRINT 'Train Name: ' + @TrainName;
        PRINT 'From: ' + @FromStation;
        PRINT 'To: ' + @ToStation;
        PRINT 'Class: ' + @Class;
        PRINT 'Passenger Name: ' + @PassengerName;
        PRINT 'Seats Booked: ' + CAST(@SeatsBooked AS VARCHAR(10));
        PRINT 'Booking Date: ' + CONVERT(VARCHAR(20), @BookingDate, 121);
        PRINT 'Date of Travel: ' + CONVERT(VARCHAR(20), @DateOfTravel, 121);
        PRINT 'Total Amount: ' + CAST(@TotalAmount AS VARCHAR(20));
        PRINT '----------------------------------------------';
    END
    ELSE
    BEGIN
        
        PRINT 'Ticket not found for Booking ID: ' + CAST(@BookingId AS VARCHAR(10));
    END
END;


CREATE OR ALTER PROCEDURE UpdateTrainDetails
    @TrainId INT,
    @Class VARCHAR(50),
    @TrainName VARCHAR(100),
    @FromStation VARCHAR(100),
    @ToStation VARCHAR(100),
    @TrainManagerName VARCHAR(100),
    @TotalBerths INT,
    @AvailableBerths INT,
    @Fare DECIMAL(10, 2),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    
    IF NOT EXISTS (SELECT 1 FROM Trains WHERE TrainId = @TrainId AND Class = @Class)
    BEGIN
        PRINT 'Train not found.';
        RETURN;
    END

   
    UPDATE Trains
    SET TrainName = @TrainName,
        [From] = @FromStation,
        [To] = @ToStation,
        Name = @TrainManagerName,
        TotalBerths = @TotalBerths,
        AvailableBerths = @AvailableBerths,
        Fare = @Fare,
        IsActive = @IsActive
    WHERE TrainId = @TrainId AND Class = @Class;

    PRINT 'Train details updated successfully.';
END;


CREATE OR ALTER PROCEDURE SearchTrains
    @FromStation VARCHAR(100),
    @ToStation VARCHAR(100),
    @DateOfTravel DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Retrieve trains based on search criteria
    SELECT 
        TrainId, 
        TrainName, 
        [From] AS DepartureStation, 
        [To] AS ArrivalStation, 
        Class, 
        AvailableBerths AS AvailableSeats, 
        Fare
    FROM 
        Trains
    WHERE 
        [From] = @FromStation AND [To] = @ToStation AND IsActive = 1
        AND TrainId NOT IN (SELECT TrainId FROM Booking WHERE DateOfTravel = @DateOfTravel)
END;


CREATE OR ALTER PROCEDURE ViewBookingHistory
    @LoginId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    
    SELECT 
        BookingId, 
        TrainId, 
        Class, 
        PassengerName, 
        SeatsBooked, 
        BookingDate, 
        DateOfTravel, 
        TotalAmount
    FROM 
        Booking
    WHERE 
        TrainId IN (SELECT TrainId FROM Users WHERE LoginId = @LoginId);
END;

-- Change Password
CREATE OR ALTER PROCEDURE ChangePassword
    @LoginId NVARCHAR(50),
    @OldPassword NVARCHAR(50),
    @NewPassword NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    
    IF EXISTS (SELECT 1 FROM Users WHERE LoginId = @LoginId AND Password = @OldPassword)
    BEGIN
       
        UPDATE Users SET Password = @NewPassword WHERE LoginId = @LoginId;
        PRINT 'Password changed successfully.';
    END
    ELSE
    BEGIN
        PRINT 'Old password does not match. Password change failed.';
    END
END;


CREATE OR ALTER PROCEDURE GetAllTrains
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive
    FROM Trains;
END;

