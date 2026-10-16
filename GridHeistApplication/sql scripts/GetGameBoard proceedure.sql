USE GridHeistDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetGameBoard
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Fetch all 64 tiles and any items sitting on them
    SELECT 
        GT.TileID, 
        GT.RowNumber, 
        GT.ColumnNumber, 
        GT.TileType, 
        GT.IsWalkable,
        IT.ItemName
    FROM dbo.GameTiles GT
    LEFT JOIN dbo.GameItems GI 
        ON GT.TileID = GI.CurrentTileID AND GI.ItemStatus = 'OnTile'
    LEFT JOIN dbo.ItemTypes IT 
        ON GI.ItemTypeID = IT.ItemTypeID
    WHERE GT.GameID = @GameID
    ORDER BY GT.RowNumber, GT.ColumnNumber;
END
GO