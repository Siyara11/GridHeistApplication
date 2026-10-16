USE GridHeistDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ValidatePlayer
    @PlayerName VARCHAR(50),
    @PasswordHash VARCHAR(255) 
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ActualPassword VARCHAR(255);
    DECLARE @IsLockedOut BIT;
    DECLARE @FailedAttempts INT;

    -- Hash the plain text password coming from C# to match the database format
    DECLARE @InputHash VARCHAR(255) = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @PasswordHash), 2);

    -- 1. Retrieve current player data
    SELECT 
        @ActualPassword = PasswordHash, 
        @IsLockedOut = IsLockedOut, 
        @FailedAttempts = FailedLoginAttempts
    FROM dbo.Players
    WHERE PlayerName = @PlayerName;

    -- 2. Check if player exists
    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS LoginStatus; -- Invalid Username
        RETURN;
    END

    -- 3. Check if already locked out
    IF @IsLockedOut = 1
    BEGIN
        SELECT -1 AS LoginStatus; -- Locked Out
        RETURN;
    END

    -- 4. Validate Password using the hashed input
    IF @ActualPassword = @InputHash
    BEGIN
        -- Success: Reset failed attempts
        UPDATE dbo.Players 
        SET FailedLoginAttempts = 0 
        WHERE PlayerName = @PlayerName;
        
        SELECT 1 AS LoginStatus; -- Success
    END
    ELSE
    BEGIN
        -- Failure: Increment attempts
        SET @FailedAttempts = @FailedAttempts + 1;

        -- Lock account if 5 or more failed attempts
        IF @FailedAttempts >= 5
        BEGIN
            UPDATE dbo.Players 
            SET FailedLoginAttempts = @FailedAttempts, IsLockedOut = 1 
            WHERE PlayerName = @PlayerName;
        END
        ELSE
        BEGIN
            UPDATE dbo.Players 
            SET FailedLoginAttempts = @FailedAttempts 
            WHERE PlayerName = @PlayerName;
        END

        SELECT 0 AS LoginStatus; -- Invalid Password
    END
END
GO