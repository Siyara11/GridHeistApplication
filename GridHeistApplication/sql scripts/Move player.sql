USE GridHeistDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_MovePlayer
    @GameID INT,
    @PlayerID INT,
    @TargetTileID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentRow INT, @CurrentCol INT;
    DECLARE @TargetRow INT, @TargetCol INT, @TargetWalkable BIT, @TargetType VARCHAR(20);

    -- 1. Get current position
    SELECT @CurrentRow = GT.RowNumber, @CurrentCol = GT.ColumnNumber
    FROM dbo.GamePlayers GP
    INNER JOIN dbo.GameTiles GT ON GP.CurrentTileID = GT.TileID
    WHERE GP.GameID = @GameID AND GP.PlayerID = @PlayerID;

    -- 2. Get target position
    SELECT @TargetRow = RowNumber, @TargetCol = ColumnNumber, 
           @TargetWalkable = IsWalkable, @TargetType = TileType
    FROM dbo.GameTiles
    WHERE TileID = @TargetTileID AND GameID = @GameID;

    -- 3. Validate Move (1 step adjacent)
    IF (ABS(@CurrentRow - @TargetRow) + ABS(@CurrentCol - @TargetCol) != 1)
    BEGIN
        SELECT 0 AS MoveStatus; RETURN;
    END

    -- 4. Validate Walkable
    IF @TargetWalkable = 0
    BEGIN
        SELECT 0 AS MoveStatus; RETURN;
    END

    -- 5. Validate Occupied
    IF @TargetType != 'Home' AND EXISTS (SELECT 1 FROM dbo.GamePlayers WHERE CurrentTileID = @TargetTileID AND GameID = @GameID)
    BEGIN
        SELECT -1 AS MoveStatus; RETURN;
    END

    -- 6. Move the Player
    UPDATE dbo.GamePlayers
    SET CurrentTileID = @TargetTileID
    WHERE GameID = @GameID AND PlayerID = @PlayerID;

    -- 7. NEW: Check if there is an item on this new tile
    DECLARE @FoundItemID INT;
    
    SELECT TOP 1 @FoundItemID = GameItemID
    FROM dbo.GameItems 
    WHERE CurrentTileID = @TargetTileID AND GameID = @GameID AND ItemStatus = 'OnTile';

    IF @FoundItemID IS NOT NULL
    BEGIN
        -- Pick up the item (Remove from the physical board using your exact constraint word)
        UPDATE dbo.GameItems 
        SET ItemStatus = 'InInventory', CurrentTileID = NULL 
        WHERE GameItemID = @FoundItemID;
        
        -- Add to PlayerInventory (Using exact columns from your schema)
        INSERT INTO dbo.PlayerInventory (GamePlayerID, GameItemID, CollectedAt)
        SELECT GamePlayerID, @FoundItemID, SYSUTCDATETIME() 
        FROM dbo.GamePlayers WHERE GameID = @GameID AND PlayerID = @PlayerID;

        -- Update the player's Score (+10 points)
        UPDATE dbo.GamePlayers 
        SET Score = Score + 10 
        WHERE GameID = @GameID AND PlayerID = @PlayerID;

        SELECT 2 AS MoveStatus; -- Return '2' to tell C# an item was picked up!
        RETURN;
    END

    SELECT 1 AS MoveStatus; -- Return '1' for a normal move
END
GO