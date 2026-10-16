USE GridHeistDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegisterPlayer
    @PlayerName VARCHAR(50),
    @PasswordHash VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Check if the username already exists
    IF EXISTS (SELECT 1 FROM dbo.Players WHERE PlayerName = @PlayerName)
    BEGIN
        SELECT 0 AS RegistrationStatus; -- Failure: Username taken
        RETURN;
    END

    -- 2. Hash the incoming plain text password
    DECLARE @InputHash VARCHAR(255) = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @PasswordHash), 2);

    -- 3. Insert the new player (IsOnline is set to 1 automatically per requirements)
    INSERT INTO dbo.Players 
        (PlayerName, PasswordHash, FailedLoginAttempts, IsLockedOut, IsOnline, IsAdmin, HighestScore, CreatedAt)
    VALUES 
        (@PlayerName, @InputHash, 0, 0, 1, 0, 0, SYSUTCDATETIME());

    SELECT 1 AS RegistrationStatus; -- Success
END
GO