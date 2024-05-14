CREATE DATABASE MiniProjectTicketBooking;
USE MiniProjectTicketBooking;

CREATE TABLE Trains (
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
    CONSTRAINT PK_Trains PRIMARY KEY (TrainId, Class)  -- Composite primary key on TrainId and Class
);


CREATE TABLE Booking (
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

CREATE TABLE Cancellation (
    CancellationId INT PRIMARY KEY IDENTITY,
    BookingId INT,
    SeatsCancelled INT,
    CancellationDate DATETIME,
    CancellationReason NVARCHAR(MAX),
    FOREIGN KEY (BookingId) REFERENCES Booking(BookingId)
);

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

CREATE OR ALTER PROCEDURE AddTrain
    @TrainId INT,
    @TrainName VARCHAR(100),
    @FromStation VARCHAR(100),
    @ToStation VARCHAR(100),
    @TrainManagerName VARCHAR(100),
    @TotalBerths INT,
    @AvailableBerths INT,
    @Fare1 DECIMAL(10, 2), -- Fare for Business Class
    @Fare2 DECIMAL(10, 2), -- Fare for Economy Class
    @Fare3 DECIMAL(10, 2), -- Fare for First Class
    @IsActive BIT
AS
BEGIN
    -- Check if the TrainId already exists
    IF EXISTS (SELECT 1 FROM Trains WHERE TrainId = @TrainId)
    BEGIN
        PRINT 'Train with the specified TrainId already exists. Cannot insert duplicate TrainId.'
        RETURN
    END

    -- Insert Business Class
    INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
    VALUES (@TrainId, 'Business', @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare1, @IsActive);

    -- Insert Economy Class
    INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
    VALUES (@TrainId, 'Economy', @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare2, @IsActive);

    -- Insert First Class
    INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
    VALUES (@TrainId, 'First Class', @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare3, @IsActive);

    PRINT 'Train added successfully.'
END;





CREATE OR ALTER PROCEDURE BookTicket
    @LoginId NVARCHAR(50),
    @Password NVARCHAR(50),
    @TrainId INT,
    @Class VARCHAR(20),
    @SeatsBooked INT,
    @PassengerNames NVARCHAR(MAX),
    @BookingDate DATETIME,
    @DateOfTravel DATE
AS
BEGIN
    DECLARE @UserId INT

    -- Check login credentials
    SELECT @UserId = UserId
    FROM Users
    WHERE LoginId = @LoginId AND Password = @Password

    IF @UserId IS NULL
    BEGIN
        PRINT 'Invalid login credentials. Booking failed.'
        RETURN
    END

    -- Check if the train is inactive
    IF NOT EXISTS (SELECT 1 FROM Trains WHERE TrainId = @TrainId AND IsActive = 1)
    BEGIN
        PRINT 'Train is inactive. Booking failed.'
        RETURN
    END

    -- Check available seats
    DECLARE @AvailableSeats INT
    SELECT @AvailableSeats = AvailableBerths FROM Trains WHERE TrainId = @TrainId AND Class = @Class

    IF @AvailableSeats < @SeatsBooked
    BEGIN
        PRINT 'Requested seats not available. Booking failed.'
        RETURN
    END

    -- Calculate amount
    DECLARE @Fare DECIMAL(10, 2)
    SELECT @Fare = Fare FROM Trains WHERE TrainId = @TrainId AND Class = @Class
    DECLARE @Amount DECIMAL(10, 2)
    SET @Amount = @SeatsBooked * @Fare

    -- Update available berths
    UPDATE Trains SET AvailableBerths = AvailableBerths - @SeatsBooked WHERE TrainId = @TrainId AND Class = @Class

    -- Insert booking details
    INSERT INTO Booking (TrainId, Class, PassengerName, SeatsBooked, BookingDate, DateOfTravel, TotalAmount)
    VALUES (@TrainId, @Class, @PassengerNames, @SeatsBooked, @BookingDate, @DateOfTravel, @Amount)

    -- Get the generated BookingId
    DECLARE @BookingId INT
    SET @BookingId = SCOPE_IDENTITY()

    -- Get 'From' station and 'To' station
    DECLARE @FromStation VARCHAR(100)
    DECLARE @ToStation VARCHAR(100)

    SELECT @FromStation = [From], @ToStation = [To] FROM Trains WHERE TrainId = @TrainId

    -- Print booking details
    PRINT 'Ticket booked successfully.'
    PRINT 'Booking ID: ' + CAST(@BookingId AS NVARCHAR(20))
    PRINT 'Total Amount: ' + CAST(@Amount AS NVARCHAR(20))
    PRINT 'Seats Booked: ' + CAST(@SeatsBooked AS NVARCHAR(10))
    PRINT 'Date of Travel: ' + CONVERT(NVARCHAR(20), @DateOfTravel, 23)
    PRINT 'Class: ' + @Class
    PRINT 'Booking Date: ' + CONVERT(NVARCHAR(20), @BookingDate, 23)
    PRINT 'Passenger Names: ' + @PassengerNames
    PRINT 'From: ' + @FromStation
    PRINT 'To: ' + @ToStation
END;




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



CREATE OR ALTER PROCEDURE UpdateTrainDetails
    @TrainId INT,
    @TrainName VARCHAR(100),
    @FromStation VARCHAR(100),
    @ToStation VARCHAR(100),
    @TrainManagerName VARCHAR(100),
    @TotalBerths INT,
    @AvailableBerths INT,
    @Fare1 DECIMAL(10, 2), -- Fare for Class 1
    @Fare2 DECIMAL(10, 2), -- Fare for Class 2
    @Fare3 DECIMAL(10, 2), -- Fare for Class 3
    @IsActive BIT
AS
BEGIN
    -- Update Class 1
    UPDATE Trains
    SET TrainName = @TrainName,
        [From] = @FromStation,
        [To] = @ToStation,
        Name = @TrainManagerName,
        TotalBerths = @TotalBerths,
        AvailableBerths = @AvailableBerths,
        Fare = @Fare1,
        IsActive = @IsActive
    WHERE TrainId = @TrainId AND Class = 'Business';

    -- Update Class 2
    UPDATE Trains
    SET TrainName = @TrainName,
        [From] = @FromStation,
        [To] = @ToStation,
        Name = @TrainManagerName,
        TotalBerths = @TotalBerths,
        AvailableBerths = @AvailableBerths,
        Fare = @Fare2,
        IsActive = @IsActive
    WHERE TrainId = @TrainId AND Class = 'Economy';

    -- Update Class 3
    UPDATE Trains
    SET TrainName = @TrainName,
        [From] = @FromStation,
        [To] = @ToStation,
        Name = @TrainManagerName,
        TotalBerths = @TotalBerths,
        AvailableBerths = @AvailableBerths,
        Fare = @Fare3,
        IsActive = @IsActive
    WHERE TrainId = @TrainId AND Class = 'First Class';

    PRINT 'Train details updated successfully.'
END

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

CREATE OR ALTER PROCEDURE PrintTicket
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.TrainName AS [Train Name], 
        t.[From] AS [From Station], 
        t.[To] AS [To Station], 
        b.Class, 
        b.PassengerName, 
        b.SeatsBooked AS [Seats Booked], 
        b.BookingDate AS [Booking Date], 
        b.DateOfTravel AS [Date of Travel], 
        b.TotalAmount AS [Total Amount]
    FROM 
        Booking b
    INNER JOIN 
        Trains t ON b.TrainId = t.TrainId
    WHERE 
        b.BookingId = @BookingId;
END;



CREATE OR ALTER PROCEDURE UpdateUserPassword
    @LoginId NVARCHAR(50),
    @OldPassword NVARCHAR(50),
    @NewPassword NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the old password matches for the given LoginId
    IF EXISTS (SELECT 1 FROM Users WHERE LoginId = @LoginId AND Password = @OldPassword)
    BEGIN
        -- Update the password
        UPDATE Users SET Password = @NewPassword WHERE LoginId = @LoginId;
        PRINT 'Password updated successfully.';
    END
    ELSE
    BEGIN
        -- If old password doesn't match, print an error message
        PRINT 'Old password does not match. Password update failed.';
    END
END;

CREATE OR ALTER PROCEDURE SoftDeleteTrain
    @TrainId INT
AS
BEGIN
    UPDATE Trains
    SET IsActive = 0
    WHERE TrainId = @TrainId;
END;

select * from Trains

DELETE FROM booking WHERE bookingId = 4


CREATE OR ALTER PROCEDURE AddTrain
    @TrainId INT,
    @TrainName VARCHAR(100),
    @FromStation VARCHAR(100),
    @ToStation VARCHAR(100),
    @TrainManagerName VARCHAR(100),
    @TotalBerths INT,
    @AvailableBerths INT,
    @Fare1 DECIMAL(10, 2), -- Fare for Class 1 (First Class)
    @IsActive BIT
AS
BEGIN
    -- Calculate fares for other classes based on a ratio or proportion

    -- Calculate the ratio of fares for Economy and Business classes based on typical proportions
    DECLARE @BusinessToEconomyRatio DECIMAL(10, 2) = 1.5; -- Example: Business Class fare is 1.5 times Economy Class fare
    DECLARE @EconomyToFirstClassRatio DECIMAL(10, 2) = 0.75; -- Example: Economy Class fare is 75% of First Class fare

    -- Calculate fares for other classes based on the ratio
    DECLARE @Fare2 DECIMAL(10, 2) = @Fare1 / @BusinessToEconomyRatio; -- Economy Class fare
    DECLARE @Fare3 DECIMAL(10, 2) = @Fare1 * @EconomyToFirstClassRatio; -- First Class fare

    -- Insert Class 1 (First Class)
    INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
    VALUES (@TrainId, 'First Class', @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare1, @IsActive);

    -- Insert Class 2 (Economy)
    INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
    VALUES (@TrainId, 'Economy', @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare2, @IsActive);

    -- Insert Class 3 (Business)
    INSERT INTO Trains (TrainId, Class, TrainName, [From], [To], Name, TotalBerths, AvailableBerths, Fare, IsActive)
    VALUES (@TrainId, 'Business', @TrainName, @FromStation, @ToStation, @TrainManagerName, @TotalBerths, @AvailableBerths, @Fare3, @IsActive);

    PRINT 'Train added successfully.'
END;




select * from booking
select * from trains


CREATE OR ALTER PROCEDURE BookTicket
    @LoginId NVARCHAR(50),
    @Password NVARCHAR(50),
    @TrainId INT,
    @Class VARCHAR(20),
    @SeatsBooked INT,
    @PassengerNames NVARCHAR(MAX),
    @BookingDate DATETIME,
    @DateOfTravel DATE
AS
BEGIN
    DECLARE @UserId INT

    -- Check login credentials
    SELECT @UserId = UserId
    FROM Users
    WHERE LoginId = @LoginId AND Password = @Password

    IF @UserId IS NULL
    BEGIN
        PRINT 'Invalid login credentials. Booking failed.'
        RETURN
    END

    -- Check if the train is inactive
    IF NOT EXISTS (SELECT 1 FROM Trains WHERE TrainId = @TrainId AND IsActive = 1)
    BEGIN
        PRINT 'Train is inactive. Booking failed.'
        RETURN
    END

    -- Check available seats
    DECLARE @AvailableSeats INT
    SELECT @AvailableSeats = AvailableBerths FROM Trains WHERE TrainId = @TrainId AND Class = @Class

    IF @AvailableSeats < @SeatsBooked
    BEGIN
        PRINT 'Requested seats not available. Booking failed.'
        RETURN
    END

    -- Calculate amount
    DECLARE @Fare DECIMAL(10, 2)
    SELECT @Fare = Fare FROM Trains WHERE TrainId = @TrainId AND Class = @Class
    DECLARE @Amount DECIMAL(10, 2)
    SET @Amount = @SeatsBooked * @Fare

    -- Update available berths
    UPDATE Trains SET AvailableBerths = AvailableBerths - @SeatsBooked WHERE TrainId = @TrainId AND Class = @Class

    -- Insert booking details
    INSERT INTO Booking (TrainId, Class, PassengerName, SeatsBooked, BookingDate, DateOfTravel, TotalAmount)
    VALUES (@TrainId, @Class, @PassengerNames, @SeatsBooked, @BookingDate, @DateOfTravel, @Amount)

    -- Get the generated BookingId
    DECLARE @BookingId INT
    SET @BookingId = SCOPE_IDENTITY()

    -- Get 'From' station and 'To' station
    DECLARE @FromStation VARCHAR(100)
    DECLARE @ToStation VARCHAR(100)

    SELECT @FromStation = [From], @ToStation = [To] FROM Trains WHERE TrainId = @TrainId

    -- Print booking details
    PRINT 'Ticket booked successfully.'
    PRINT 'Booking ID: ' + CAST(@BookingId AS NVARCHAR(20))
    PRINT 'Total Amount: ' + CAST(@Amount AS NVARCHAR(20))
    PRINT 'Seats Booked: ' + CAST(@SeatsBooked AS NVARCHAR(10))
    PRINT 'Date of Travel: ' + CONVERT(NVARCHAR(20), @DateOfTravel, 23)
    PRINT 'Class: ' + @Class
    PRINT 'Booking Date: ' + CONVERT(NVARCHAR(20), @BookingDate, 23)
    PRINT 'Passenger Names: ' + @PassengerNames
    PRINT 'From: ' + @FromStation
    PRINT 'To: ' + @ToStation

    -- Return booking details
    SELECT @BookingId AS BookingId, @Amount AS TotalAmount
END;



CREATE OR ALTER PROCEDURE CancelTicket
    @BookingId INT,
    @SeatsToCancel INT,
    @PassengerNamesToDelete NVARCHAR(MAX),  -- New parameter to accept passenger names to be deleted
    @CancellationReason NVARCHAR(MAX),
    @RefundAmount DECIMAL(10, 2) OUTPUT
AS
BEGIN
    DECLARE @TrainId INT, @Class VARCHAR(50), @Amount DECIMAL(10, 2)

    -- Retrieve TrainId, Class, and TotalAmount based on BookingId
    SELECT @TrainId = TrainId, @Class = Class, @Amount = TotalAmount
    FROM Booking
    WHERE BookingId = @BookingId

    IF @TrainId IS NULL
    BEGIN
        PRINT 'Booking not found. Cancellation failed.'
        RETURN
    END

    -- Calculate refund amount
    SET @RefundAmount = @SeatsToCancel * (@Amount / (SELECT SeatsBooked FROM Booking WHERE BookingId = @BookingId))

    -- Update available berths
    UPDATE Trains
    SET AvailableBerths = AvailableBerths + @SeatsToCancel
    WHERE TrainId = @TrainId AND Class = @Class

    -- Update booking details (subtract seats and refund amount)
    UPDATE Booking
    SET SeatsBooked = SeatsBooked - @SeatsToCancel,
        TotalAmount = TotalAmount - @RefundAmount,
        PassengerName = REPLACE(
            REPLACE(
                REPLACE(
                    REPLACE(PassengerName, ',' + LTRIM(RTRIM(@PassengerNamesToDelete)) + ',', ','),
                    LTRIM(RTRIM(@PassengerNamesToDelete)) + ',', ''
                ),
                ',' + LTRIM(RTRIM(@PassengerNamesToDelete)), ''
            ),
            LTRIM(RTRIM(@PassengerNamesToDelete)), ''
        )
    WHERE BookingId = @BookingId

    -- Log cancellation details
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


select * from booking
select * from trains

CREATE OR ALTER PROCEDURE DeactivatedTrain
    @TrainId INT
AS
BEGIN
    BEGIN TRY
        -- Check if the train exists and is active before deactivating
        IF NOT EXISTS (SELECT 1 FROM Trains WHERE TrainId = @TrainId AND IsActive = 1)
        BEGIN
            PRINT 'No active train found with the specified TrainId.';
            RETURN; -- Exit the procedure
        END

        -- Deactivate the train and all associated classes
        UPDATE Trains
        SET IsActive = 0
        WHERE TrainId = @TrainId;

        -- Check if any rows were affected by the update
        IF @@ROWCOUNT = 0
        BEGIN
            PRINT 'No train found with the specified TrainId.';
            RETURN; -- Exit the procedure
        END

        -- Print message indicating the deactivation
        PRINT 'Train and all associated classes deactivated successfully.';
    END TRY
    BEGIN CATCH
        -- Print error message
        PRINT 'An error occurred: ' + ERROR_MESSAGE();
    END CATCH
END;



CREATE OR ALTER PROCEDURE GenerateTotalRevenue
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalRevenue DECIMAL(10, 2);

    BEGIN TRY
        SELECT @TotalRevenue = SUM(TotalAmount)
        FROM Booking;
        
        IF @TotalRevenue IS NOT NULL
            SELECT @TotalRevenue AS TotalRevenue;
        ELSE
            SELECT 0 AS TotalRevenue; -- Assuming total revenue is zero when no bookings exist
    END TRY
    BEGIN CATCH
        -- Handle any errors that occur during execution
        SELECT ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;




SELECT * FROM Booking
SELECT * FROM  Trains
SELECT * FROM Users
SELECT * FROM Cancellation
