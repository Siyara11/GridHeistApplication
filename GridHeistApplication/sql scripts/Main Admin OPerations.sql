USE GridHeistDB;
GO

/*
    ADMIN: KILL RUNNING GAME
*/
CREATE OR ALTER PROCEDURE dbo.sp_AdminKillGame
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Stop the game and record the end time
    UPDATE dbo.Games
    SET GameStatus = 'Stopped', EndTime = SYSUTCDATETIME()
    WHERE GameID = @GameID AND GameStatus = 'Active';
    
    -- Set all players in that game to 'Finished' so they are kicked to the lobby
    UPDATE dbo.GamePlayers
    SET PlayerStatus = 'Finished'
    WHERE GameID = @GameID;
    
    SELECT 1 AS SuccessStatus;
END
GO

/* 
   ADMIN: ADD NEW PLAYER
 */
CREATE OR ALTER PROCEDURE dbo.sp_AdminAddPlayer
    @PlayerName VARCHAR(50),
    @PasswordHash VARCHAR(255),
    @IsAdmin BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the username already exists
    IF EXISTS (SELECT 1 FROM dbo.Players WHERE PlayerName = @PlayerName)
    BEGIN
        SELECT 0 AS RegistrationStatus; -- Failure: Username taken
        RETURN;
    END

    -- Hash the incoming plain text password
    DECLARE @InputHash VARCHAR(255) = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @PasswordHash), 2);

    -- Insert the new player (Admin created accounts start offline)
    INSERT INTO dbo.Players 
        (PlayerName, PasswordHash, FailedLoginAttempts, IsLockedOut, IsOnline, IsAdmin, HighestScore, CreatedAt)
    VALUES 
        (@PlayerName, @InputHash, 0, 0, 0, @IsAdmin, 0, SYSUTCDATETIME());

    SELECT 1 AS RegistrationStatus; -- Success
END
GO

/* 
   ADMIN: UPDATE EXISTING PLAYER
 */
CREATE OR ALTER PROCEDURE dbo.sp_AdminUpdatePlayer
    @TargetPlayerID INT,
    @NewPlayerName VARCHAR(50),
    @ResetPasswordHash VARCHAR(255), -- Pass empty string if not changing password
    @UnlockAccount BIT,
    @IsAdmin BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the new username is already taken by a DIFFERENT player
    IF EXISTS (SELECT 1 FROM dbo.Players WHERE PlayerName = @NewPlayerName AND PlayerID != @TargetPlayerID)
    BEGIN
        SELECT 0 AS UpdateStatus; -- Failure: Username taken
        RETURN;
    END

    -- Update basic info (Name and Admin privileges)
    UPDATE dbo.Players
    SET PlayerName = @NewPlayerName,
        IsAdmin = @IsAdmin
    WHERE PlayerID = @TargetPlayerID;

    -- Unlock the account and reset failed attempts if requested
    IF @UnlockAccount = 1
    BEGIN
        UPDATE dbo.Players
        SET IsLockedOut = 0, FailedLoginAttempts = 0
        WHERE PlayerID = @TargetPlayerID;
    END

    -- Update the password if a new one was typed into the admin form
    IF @ResetPasswordHash IS NOT NULL AND LEN(LTRIM(RTRIM(@ResetPasswordHash))) > 0
    BEGIN
        DECLARE @InputHash VARCHAR(255) = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @ResetPasswordHash), 2);
        UPDATE dbo.Players
        SET PasswordHash = @InputHash
        WHERE PlayerID = @TargetPlayerID;
    END

    SELECT 1 AS UpdateStatus; -- Success
END
GO

/* 
   ADMIN: DELETE PLAYER
 */
USE GridHeistDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AdminDeletePlayer
    @TargetPlayerID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Remove Chat Messages linked to this player
        DELETE FROM dbo.ChatMessages
        WHERE GamePlayerID IN (SELECT GamePlayerID FROM dbo.GamePlayers WHERE PlayerID = @TargetPlayerID);

        -- 2. Remove Player Inventory linked to this player
        DELETE FROM dbo.PlayerInventory
        WHERE GamePlayerID IN (SELECT GamePlayerID FROM dbo.GamePlayers WHERE PlayerID = @TargetPlayerID);

        -- 3. Remove the player from any games
        DELETE FROM dbo.GamePlayers
        WHERE PlayerID = @TargetPlayerID;

        -- 4. Unlink games created by this player (Set CreatedByPlayerID to NULL)
        UPDATE dbo.Games
        SET CreatedByPlayerID = NULL
        WHERE CreatedByPlayerID = @TargetPlayerID;

        -- 5. Finally, delete the Player
        DELETE FROM dbo.Players
        WHERE PlayerID = @TargetPlayerID;

        COMMIT TRANSACTION;
        SELECT 1 AS DeleteStatus; -- Success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        SELECT 0 AS DeleteStatus; -- Failure
    END CATCH
END
GO