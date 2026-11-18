

USE GridHeistDB2;
GO


-- 1. INSERT ITEM TYPES (7 types)

SET IDENTITY_INSERT dbo.ItemTypes ON;
INSERT INTO dbo.ItemTypes (ItemTypeID, ItemName, ItemEffect) VALUES
    (1, 'Fragment Alpha',  'Key fragment needed to unlock the exit'),
    (2, 'Fragment Beta',   'Key fragment needed to unlock the exit'),
    (3, 'Fragment Gamma',  'Key fragment needed to unlock the exit'),
    (4, 'Data Chip',       'Bonus points +10'),
    (5, 'EMP Tool',        'Temporarily disables a nearby NPC'),
    (6, 'Stealth Cloak',   'Invisible to NPCs for 1 turn'),
    (7, 'Speed Boost',     'Move 2 tiles in one turn');
SET IDENTITY_INSERT dbo.ItemTypes OFF;
GO

PRINT 'ItemTypes inserted (7 types).';


-- 2. INSERT TEST PLAYERS (using procedures)


-- Admin account
DECLARE @AdminID INT;
EXEC dbo.sp_RegisterPlayer @PlayerName = 'GridAdmin', @Password = 'Admin#123', @PlayerID = @AdminID OUTPUT;
UPDATE dbo.Players SET IsAdmin = 1 WHERE PlayerID = @AdminID;
PRINT 'GridAdmin created (ID: ' + CAST(@AdminID AS VARCHAR) + ')';
GO

-- Regular player: Nova
DECLARE @NovaID INT;
EXEC dbo.sp_RegisterPlayer @PlayerName = 'Nova', @Password = 'Nova#123', @PlayerID = @NovaID OUTPUT;
PRINT 'Nova created (ID: ' + CAST(@NovaID AS VARCHAR) + ')';
GO

-- Regular player: Cipher
DECLARE @CipherID INT;
EXEC dbo.sp_RegisterPlayer @PlayerName = 'Cipher', @Password = 'Cipher#123', @PlayerID = @CipherID OUTPUT;
PRINT 'Cipher created (ID: ' + CAST(@CipherID AS VARCHAR) + ')';
GO

-- Regular player: Shadow
DECLARE @ShadowID INT;
EXEC dbo.sp_RegisterPlayer @PlayerName = 'Shadow', @Password = 'Shadow#123', @PlayerID = @ShadowID OUTPUT;
PRINT 'Shadow created (ID: ' + CAST(@ShadowID AS VARCHAR) + ')';
GO

-- Locked account for testing lockout
DECLARE @LockedID INT;
EXEC dbo.sp_RegisterPlayer @PlayerName = 'LockedRunner', @Password = 'Locked#123', @PlayerID = @LockedID OUTPUT;
UPDATE dbo.Players SET IsLockedOut = 1, FailedLoginAttempts = 5 WHERE PlayerID = @LockedID;
PRINT 'LockedRunner created and locked (ID: ' + CAST(@LockedID AS VARCHAR) + ')';
GO


-- 3. CREATE A TEST GAME (Nova hosts, Cipher joins)


-- Nova creates a game
DECLARE @GameID INT;
DECLARE @NovaPlayerID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Nova');

EXEC dbo.sp_GenerateGameBoard @HostPlayerID = @NovaPlayerID, @NewGameID = @GameID OUTPUT;
PRINT 'Test game created (GameID: ' + CAST(@GameID AS VARCHAR) + ')';

-- Place items
EXEC dbo.sp_PlaceInitialItems @GameID = @GameID;
PRINT 'Items placed in game ' + CAST(@GameID AS VARCHAR);

-- Cipher joins the game
DECLARE @CipherPlayerID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Cipher');
EXEC dbo.sp_JoinGame @GameID = @GameID, @PlayerID = @CipherPlayerID;
PRINT 'Cipher joined game ' + CAST(@GameID AS VARCHAR);
GO


-- 4. VERIFY TEST DATA

PRINT '';
PRINT 'VERIFICATION ';

PRINT '-- Players:';
SELECT PlayerID, PlayerName, IsAdmin, IsLockedOut, IsOnline FROM dbo.Players;

PRINT '-- Games:';
SELECT GameID, CreatedByPlayerID, GameStatus, MaxPlayers FROM dbo.Games;

PRINT '-- GamePlayers:';
SELECT GP.GamePlayerID, GP.GameID, GP.PlayerID, P.PlayerName, GP.Score, GP.PlayerStatus
FROM dbo.GamePlayers GP
INNER JOIN dbo.Players P ON GP.PlayerID = P.PlayerID;

PRINT '-- Items on board:';
SELECT GI.GameItemID, IT.ItemName, GT.RowNumber, GT.ColumnNumber
FROM dbo.GameItems GI
INNER JOIN dbo.ItemTypes IT ON GI.ItemTypeID = IT.ItemTypeID
INNER JOIN dbo.GameTiles GT ON GI.CurrentTileID = GT.TileID
WHERE GI.ItemStatus = 'OnTile';

PRINT '-- NPCs:';
SELECT N.NPCID, N.NPCName, GT.RowNumber, GT.ColumnNumber, N.IsMovingForward
FROM dbo.NPCs N
INNER JOIN dbo.GameTiles GT ON N.CurrentTileID = GT.TileID;

GO


