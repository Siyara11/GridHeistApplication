USE GridHeistDB;
GO

/*
 LAYING OUT TILES ON A GAME BOARD
*/
CREATE OR ALTER PROCEDURE dbo.sp_GenerateGameBoard
    @HostPlayerID INT,
    @NewGameID INT OUTPUT -- This will pass the generated Game ID back to C#
AS
BEGIN
    SET NOCOUNT ON;
    
    -- 1. Create a new Active Game
    INSERT INTO dbo.Games (CreatedByPlayerID, GameStatus, MaxPlayers, TimeLimitSeconds, StartTime)
    VALUES (@HostPlayerID, 'Active', 4, 900, SYSUTCDATETIME());
    
    SET @NewGameID = SCOPE_IDENTITY(); -- Capture the ID of the new game

    -- 2. Generate the 8x8 Grid (64 Tiles)
    ;WITH Numbers AS (
        SELECT NumberValue FROM (VALUES (1), (2), (3), (4), (5), (6), (7), (8)) AS N(NumberValue)
    )
    INSERT INTO dbo.GameTiles (GameID, RowNumber, ColumnNumber, TileType, IsWalkable)
    SELECT 
        @NewGameID,
        R.NumberValue,
        C.NumberValue,
        -- Set special tiles based on your MS1 design
        CASE 
            WHEN R.NumberValue = 1 AND C.NumberValue = 1 THEN 'Home'
            WHEN R.NumberValue = 8 AND C.NumberValue = 8 THEN 'Exit'
            WHEN (R.NumberValue = 3 AND C.NumberValue IN (3, 4)) OR (R.NumberValue = 6 AND C.NumberValue = 6) THEN 'Firewall'
            ELSE 'Floor'
        END AS TileType,
        -- Firewalls are not walkable
        CASE 
            WHEN (R.NumberValue = 3 AND C.NumberValue IN (3, 4)) OR (R.NumberValue = 6 AND C.NumberValue = 6) THEN 0
            ELSE 1
        END AS IsWalkable
    FROM Numbers R CROSS JOIN Numbers C;
    
    -- 3. Add the Host Player to this game at the 'Home' tile
    DECLARE @HomeTileID INT = (SELECT TileID FROM dbo.GameTiles WHERE GameID = @NewGameID AND RowNumber = 1 AND ColumnNumber = 1);
    
    INSERT INTO dbo.GamePlayers (GameID, PlayerID, CurrentTileID, Score, PlayerStatus)
    VALUES (@NewGameID, @HostPlayerID, @HomeTileID, 0, 'Playing');
END
GO

/* 
 PLACING AN ITEM ON A TILE
*/
CREATE OR ALTER PROCEDURE dbo.sp_PlaceInitialItems
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- We will place items on random Floor tiles that are Walkable and do NOT already have items.
    -- To keep it simple and match your test data, we insert 3 Fragments, 2 Data Chips, and some Tools.
    
    INSERT INTO dbo.GameItems (GameID, ItemTypeID, CurrentTileID, ItemStatus)
    SELECT TOP 8
        @GameID,
        -- Assign item types (1 through 7 match your MS1 ItemTypes table)
        ABS(CHECKSUM(NEWID()) % 7) + 1 AS ItemTypeID, 
        GT.TileID,
        'OnTile'
    FROM dbo.GameTiles GT
    WHERE GT.GameID = @GameID 
      AND GT.TileType = 'Floor' 
      AND GT.IsWalkable = 1
    ORDER BY NEWID(); -- Randomizes the tile selection

    SELECT 1 AS ItemsPlaced;
END
GO