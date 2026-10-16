USE GridHeistDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AdminGetAllPlayers
AS
BEGIN
    SET NOCOUNT ON;
    -- Fetch the ID, Name, and Lock status to populate the Admin form
    SELECT PlayerID, PlayerName, IsLockedOut 
    FROM dbo.Players 
    ORDER BY PlayerName;
END
GO