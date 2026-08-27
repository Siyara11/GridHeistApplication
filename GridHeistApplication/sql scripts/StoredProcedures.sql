CREATE PROCEDURE sp_ValidatePlayer
    @PlayerName VARCHAR(50),
    @PasswordHash VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Players WHERE PlayerName = @PlayerName AND PasswordHash = @PasswordHash AND IsLockedOut = 0)
    BEGIN
        SELECT 1 AS IsValid;
    END
    ELSE
    BEGIN
        SELECT 0 AS IsValid;
    END
END
GO


-- Procedure for the Game Form to test its connection
CREATE PROCEDURE sp_GetGameStatus
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;
    -- For the prototype, I just return a simple status string.
    -- In the full game, this would return all the game state data.
    SELECT 'Game Session ' + CAST(@GameID AS VARCHAR) + ' is Active. Player ' + CAST(CurrentTurn_PlayerID AS VARCHAR) + '''s turn.' AS GameStatus
    FROM ActiveGames
    WHERE GameID = @GameID;
END
GO

-- Procedure for the Admin Form to test its connection
CREATE PROCEDURE sp_GetAllPlayerNames
AS
BEGIN
    SET NOCOUNT ON;
    -- This procedure simply returns a list of all player names.
    SELECT PlayerName FROM Players ORDER BY PlayerName;
END
GO