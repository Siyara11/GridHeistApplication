DECLARE @GeneratedGameID INT;

-- 1. Create a game hosted by Player 2 (Nova)
EXEC dbo.sp_GenerateGameBoard @HostPlayerID = 2, @NewGameID = @GeneratedGameID OUTPUT;

-- 2. Populate the items for that game
EXEC dbo.sp_PlaceInitialItems @GameID = @GeneratedGameID;

-- Verify the 64 tiles were created
SELECT COUNT(*) AS TotalTiles, TileType FROM dbo.GameTiles WHERE GameID = @GeneratedGameID GROUP BY TileType;

-- Verify the items were placed
SELECT * FROM dbo.GameItems WHERE GameID = @GeneratedGameID;