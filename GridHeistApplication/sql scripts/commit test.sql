

USE GridHeistDB2;
GO


PRINT '  GRID HEIST - TRANSACTION COMMIT /  SUITE';

PRINT '';



-- TEST 1: sp_ValidatePlayer

PRINT '--- TEST 1: sp_ValidatePlayer ---';
PRINT '';

-- : Login with correct credentials
PRINT ': Login with valid credentials (Nova / Nova#123)';
DECLARE @LoginPlayerID INT;
EXEC dbo.sp_ValidatePlayer @PlayerName = 'Nova', @Password = 'Nova#123', @PlayerID = @LoginPlayerID OUTPUT;
PRINT 'Result PlayerID: ' + ISNULL(CAST(@LoginPlayerID AS VARCHAR), 'NULL');
PRINT 'Expected: LoginStatus = 1, PlayerID = Nova ID';
PRINT '';
GO

-- : Login with wrong password
PRINT ': Login with wrong password';
DECLARE @BadLoginID INT;
EXEC dbo.sp_ValidatePlayer @PlayerName = 'Nova', @Password = 'WrongPassword', @PlayerID = @BadLoginID OUTPUT;
PRINT 'Result PlayerID: ' + ISNULL(CAST(@BadLoginID AS VARCHAR), 'NULL');
PRINT 'Expected: LoginStatus = 0, PlayerID = NULL';
-- Reset failed attempts so it doesnt affect other tests
UPDATE dbo.Players SET FailedLoginAttempts = 0 WHERE PlayerName = 'Nova';
PRINT '';
GO

-- : Login to locked account
PRINT ': Login to locked account (LockedRunner)';
DECLARE @LockedLoginID INT;
EXEC dbo.sp_ValidatePlayer @PlayerName = 'LockedRunner', @Password = 'Locked#123', @PlayerID = @LockedLoginID OUTPUT;
PRINT 'Result PlayerID: ' + ISNULL(CAST(@LockedLoginID AS VARCHAR), 'NULL');
PRINT 'Expected: LoginStatus = -1, PlayerID = NULL';
PRINT '';
GO



-- TEST 2: sp_RegisterPlayer

PRINT '--- TEST 2: sp_RegisterPlayer ---';
PRINT '';

-- : Register a new unique player
PRINT ': Register new player TestUser001';
DECLARE @NewRegID INT;
EXEC dbo.sp_RegisterPlayer @PlayerName = 'TestUser001', @Password = 'Test#123', @PlayerID = @NewRegID OUTPUT;
PRINT 'Result PlayerID: ' + ISNULL(CAST(@NewRegID AS VARCHAR), 'NULL');
PRINT 'Expected: RegisterStatus = 1, PlayerID = new ID';
PRINT '';
GO

-- : Register with duplicate username
PRINT ': Register duplicate username (Nova already exists)';
DECLARE @DupRegID INT;
EXEC dbo.sp_RegisterPlayer @PlayerName = 'Nova', @Password = 'Test#123', @PlayerID = @DupRegID OUTPUT;
PRINT 'Result PlayerID: ' + ISNULL(CAST(@DupRegID AS VARCHAR), 'NULL');
PRINT 'Expected: RegisterStatus = 0, PlayerID = NULL';
PRINT '';
GO



-- TEST 3: sp_GenerateGameBoard

PRINT '--- TEST 3: sp_GenerateGameBoard ---';
PRINT '';

-- : Create a game for an existing player
PRINT ': Generate game board for Shadow';
DECLARE @TestGameID INT;
DECLARE @ShadowID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Shadow');
EXEC dbo.sp_GenerateGameBoard @HostPlayerID = @ShadowID, @NewGameID = @TestGameID OUTPUT;
PRINT 'Result GameID: ' + ISNULL(CAST(@TestGameID AS VARCHAR), 'NULL');
PRINT 'Expected: GameStatus = 1, GameID = new game ID';
-- Verify tiles were created
SELECT COUNT(*) AS TileCount FROM dbo.GameTiles WHERE GameID = @TestGameID;
PRINT 'Expected TileCount: 64';
PRINT '';
GO

-- : Create a game for a non-existent player (FK violation)
PRINT ': Generate game board for non-existent PlayerID 9999';
DECLARE @BadGameID INT;
EXEC dbo.sp_GenerateGameBoard @HostPlayerID = 9999, @NewGameID = @BadGameID OUTPUT;
PRINT 'Result GameID: ' + ISNULL(CAST(@BadGameID AS VARCHAR), '0');
PRINT 'Expected: GameStatus = -1, GameID = 0 (rolled back)';
PRINT '';
GO



-- TEST 4: sp_PlaceInitialItems

PRINT '--- TEST 4: sp_PlaceInitialItems ---';
PRINT '';

-- : Place items in an existing game
PRINT ': Place items in Shadows game';
DECLARE @ShadowGameID INT = (SELECT TOP 1 GameID FROM dbo.Games
    WHERE CreatedByPlayerID = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Shadow')
    ORDER BY GameID DESC);
EXEC dbo.sp_PlaceInitialItems @GameID = @ShadowGameID;
SELECT COUNT(*) AS ItemsOnBoard FROM dbo.GameItems WHERE GameID = @ShadowGameID AND ItemStatus = 'OnTile';
PRINT 'Expected: ItemsPlaced = 1, 8 items on the board';
PRINT '';
GO

-- : Place items in a non-existent game
PRINT ': Place items in non-existent GameID 9999';
EXEC dbo.sp_PlaceInitialItems @GameID = 9999;
PRINT 'Expected: ItemsPlaced = 0 or 1 with 0 items (no matching tiles)';
PRINT '';
GO



-- TEST 5: sp_GetGameBoard

PRINT '--- TEST 5: sp_GetGameBoard ---';
PRINT '';

-- : Fetch board for existing game
PRINT ': Get game board for Game 1';
EXEC dbo.sp_GetGameBoard @GameID = 1;
PRINT 'Expected: 64 rows of tile data';
PRINT '';
GO

-- : Fetch board for non-existent game
PRINT ': Get game board for non-existent GameID 9999';
EXEC dbo.sp_GetGameBoard @GameID = 9999;
PRINT 'Expected: Empty result set (0 rows)';
PRINT '';
GO



-- TEST 6: sp_MovePlayer

PRINT '--- TEST 6: sp_MovePlayer ---';
PRINT '';

-- : Move Nova one step from Home (1,1) to (1,2)
PRINT ': Move Nova from Home to (1,2)';
DECLARE @NovaPID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Nova');
DECLARE @NovaGameID INT = (SELECT TOP 1 GameID FROM dbo.GamePlayers WHERE PlayerID = @NovaPID AND PlayerStatus = 'Playing');
DECLARE @TargetTile INT = (SELECT TileID FROM dbo.GameTiles WHERE GameID = @NovaGameID AND RowNumber = 1 AND ColumnNumber = 2);
EXEC dbo.sp_MovePlayer @GameID = @NovaGameID, @PlayerID = @NovaPID, @TargetTileID = @TargetTile;
PRINT 'Expected: MoveStatus = 1 (normal move) or 2 (item pickup)';
PRINT '';
GO

-- : Move to a non-adjacent tile (invalid)
PRINT ': Move Nova to non-adjacent tile (5,5)';
DECLARE @NovaPID2 INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Nova');
DECLARE @NovaGameID2 INT = (SELECT TOP 1 GameID FROM dbo.GamePlayers WHERE PlayerID = @NovaPID2 AND PlayerStatus = 'Playing');
DECLARE @FarTile INT = (SELECT TileID FROM dbo.GameTiles WHERE GameID = @NovaGameID2 AND RowNumber = 5 AND ColumnNumber = 5);
EXEC dbo.sp_MovePlayer @GameID = @NovaGameID2, @PlayerID = @NovaPID2, @TargetTileID = @FarTile;
PRINT 'Expected: MoveStatus = 0 (invalid move, not adjacent)';
PRINT '';
GO

-- : Move to a Firewall tile (not walkable)
PRINT ': Move to Firewall tile (3,3)';
-- First move Nova close to the firewall to make it adjacent
-- For now, test with the tile directly
DECLARE @NovaPID3 INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Nova');
DECLARE @NovaGameID3 INT = (SELECT TOP 1 GameID FROM dbo.GamePlayers WHERE PlayerID = @NovaPID3 AND PlayerStatus = 'Playing');
DECLARE @WallTile INT = (SELECT TileID FROM dbo.GameTiles WHERE GameID = @NovaGameID3 AND RowNumber = 3 AND ColumnNumber = 3);
EXEC dbo.sp_MovePlayer @GameID = @NovaGameID3, @PlayerID = @NovaPID3, @TargetTileID = @WallTile;
PRINT 'Expected: MoveStatus = 0 (not adjacent or not walkable)';
PRINT '';
GO



-- TEST 7: sp_MoveNPC

PRINT '--- TEST 7: sp_MoveNPC ---';
PRINT '';

-- : Move NPCs in existing game
PRINT ': Move NPCs in Game 1';
PRINT 'Before:';
SELECT N.NPCName, GT.RowNumber, GT.ColumnNumber FROM dbo.NPCs N
INNER JOIN dbo.GameTiles GT ON N.CurrentTileID = GT.TileID WHERE N.GameID = 1;

EXEC dbo.sp_MoveNPC @GameID = 1;

PRINT 'After:';
SELECT N.NPCName, GT.RowNumber, GT.ColumnNumber FROM dbo.NPCs N
INNER JOIN dbo.GameTiles GT ON N.CurrentTileID = GT.TileID WHERE N.GameID = 1;
PRINT 'Expected: NPCs moved one step along their patrol path';
PRINT '';
GO

-- : Move NPCs in non-existent game
PRINT ': Move NPCs in non-existent GameID 9999';
EXEC dbo.sp_MoveNPC @GameID = 9999;
PRINT 'Expected: No error, nothing happens (no NPCs to move)';
PRINT '';
GO



-- TEST 8: sp_JoinGame

PRINT '--- TEST 8: sp_JoinGame ---';
PRINT '';

-- : Shadow joins Game 1
PRINT ': Shadow joins Game 1';
DECLARE @ShadowJoinID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Shadow');
EXEC dbo.sp_JoinGame @GameID = 1, @PlayerID = @ShadowJoinID;
PRINT 'Expected: JoinStatus = 1';
SELECT PlayerID, PlayerStatus FROM dbo.GamePlayers WHERE GameID = 1;
PRINT '';
GO

-- : Shadow tries to join again (already in game)
PRINT ': Shadow tries to join Game 1 again (duplicate)';
DECLARE @ShadowJoinID2 INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Shadow');
EXEC dbo.sp_JoinGame @GameID = 1, @PlayerID = @ShadowJoinID2;
PRINT 'Expected: JoinStatus = -1 (already in game)';
PRINT '';
GO

-- : Join a non-existent game
PRINT ': Join non-existent GameID 9999';
EXEC dbo.sp_JoinGame @GameID = 9999, @PlayerID = 1;
PRINT 'Expected: JoinStatus = 0 (game not active)';
PRINT '';
GO



-- TEST 9: sp_GetAvailableGames

PRINT '--- TEST 9: sp_GetAvailableGames ---';
PRINT '';

-- : List available games
PRINT ': List available games';
EXEC dbo.sp_GetAvailableGames;
PRINT 'Expected: Shows active games with room for more players';
PRINT '';
GO



-- TEST 10: sp_GetOtherPlayers

PRINT '--- TEST 10: sp_GetOtherPlayers ---';
PRINT '';

-- : Get other players from Novas perspective
PRINT ': Get other players in Game 1 (from Nova perspective)';
DECLARE @NovaSeeID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Nova');
EXEC dbo.sp_GetOtherPlayers @GameID = 1, @PlayerID = @NovaSeeID;
PRINT 'Expected: Shows Cipher and Shadow (not Nova)';
PRINT '';
GO

-- : Get other players for a player not in any game
PRINT ': Get other players for non-existent scenario';
EXEC dbo.sp_GetOtherPlayers @GameID = 9999, @PlayerID = 9999;
PRINT 'Expected: Empty result set';
PRINT '';
GO



-- TEST 11: sp_SendChatMessage / sp_GetChatMessages

PRINT '--- TEST 11: sp_SendChatMessage + sp_GetChatMessages ---';
PRINT '';

-- : Nova sends a message
PRINT ': Nova sends a chat message in Game 1';
DECLARE @NovaChatID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Nova');
EXEC dbo.sp_SendChatMessage @GameID = 1, @PlayerID = @NovaChatID, @MessageText = 'I found a fragment! Heading to exit.';
PRINT 'Expected: SendStatus = 1';
PRINT '';

-- : Retrieve messages
PRINT ': Get chat messages for Game 1';
EXEC dbo.sp_GetChatMessages @GameID = 1;
PRINT 'Expected: Shows Novas message';
PRINT '';
GO

-- : Send message from player not in this game
PRINT ': Send message from player not in game';
DECLARE @AdminChatID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'GridAdmin');
EXEC dbo.sp_SendChatMessage @GameID = 1, @PlayerID = @AdminChatID, @MessageText = 'Should not work';
PRINT 'Expected: SendStatus = 0 (admin is not a GamePlayer in this game)';
PRINT '';
GO



-- TEST 12: sp_GetGameStatus

PRINT '--- TEST 12: sp_GetGameStatus ---';
PRINT '';

-- : Check status of active game
PRINT ': Get status of Game 1';
EXEC dbo.sp_GetGameStatus @GameID = 1;
PRINT 'Expected: Active';
PRINT '';

-- : Check status of non-existent game
PRINT ': Get status of non-existent GameID 9999';
EXEC dbo.sp_GetGameStatus @GameID = 9999;
PRINT 'Expected: Empty result (no game found)';
PRINT '';
GO



-- TEST 13: sp_AdminGetAllPlayers

PRINT '--- TEST 13: sp_AdminGetAllPlayers ---';
PRINT '';

PRINT ': List all players';
EXEC dbo.sp_AdminGetAllPlayers;
PRINT 'Expected: All registered players with their details';
PRINT '';
GO



-- TEST 14: sp_AdminAddPlayer

PRINT '--- TEST 14: sp_AdminAddPlayer ---';
PRINT '';

-- : Admin adds a new player
PRINT ': Admin adds player AdminTestUser';
EXEC dbo.sp_AdminAddPlayer @PlayerName = 'AdminTestUser', @PasswordHash = 'Temp#123', @IsAdmin = 0;
PRINT 'Expected: RegistrationStatus = 1';
PRINT '';
GO

-- : Admin adds duplicate player
PRINT ': Admin adds duplicate player (Nova already exists)';
EXEC dbo.sp_AdminAddPlayer @PlayerName = 'Nova', @PasswordHash = 'Temp#123', @IsAdmin = 0;
PRINT 'Expected: RegistrationStatus = 0';
PRINT '';
GO



-- TEST 15: sp_AdminUpdatePlayer

PRINT '--- TEST 15: sp_AdminUpdatePlayer ---';
PRINT '';

-- : Update AdminTestUser's name
PRINT ': Rename AdminTestUser to RenamedUser';
DECLARE @UpdateTarget INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'AdminTestUser');
EXEC dbo.sp_AdminUpdatePlayer
    @TargetPlayerID = @UpdateTarget,
    @NewPlayerName = 'RenamedUser',
    @ResetPasswordHash = '',
    @UnlockAccount = 0,
    @IsAdmin = 0;
PRINT 'Expected: UpdateStatus = 1';
SELECT PlayerID, PlayerName FROM dbo.Players WHERE PlayerID = @UpdateTarget;
PRINT '';
GO

-- : Rename to an existing username
PRINT ': Rename RenamedUser to Nova (already exists)';
DECLARE @UpdateTarget2 INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'RenamedUser');
EXEC dbo.sp_AdminUpdatePlayer
    @TargetPlayerID = @UpdateTarget2,
    @NewPlayerName = 'Nova',
    @ResetPasswordHash = '',
    @UnlockAccount = 0,
    @IsAdmin = 0;
PRINT 'Expected: UpdateStatus = 0 (username taken)';
PRINT '';
GO



-- TEST 16: sp_AdminDeletePlayer

PRINT '--- TEST 16: sp_AdminDeletePlayer ---';
PRINT '';

-- : Delete the renamed test user
PRINT ': Delete RenamedUser';
DECLARE @DeleteTarget INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'RenamedUser');
EXEC dbo.sp_AdminDeletePlayer @TargetPlayerID = @DeleteTarget;
PRINT 'Expected: DeleteStatus = 1';
PRINT 'Verify deleted:';
SELECT COUNT(*) AS RemainingCount FROM dbo.Players WHERE PlayerName = 'RenamedUser';
PRINT 'Expected RemainingCount: 0';
PRINT '';
GO

-- : Delete a non-existent player
PRINT ': Delete non-existent PlayerID 9999';
EXEC dbo.sp_AdminDeletePlayer @TargetPlayerID = 9999;
PRINT 'Expected: DeleteStatus = 1 (no error, just nothing deleted)';
PRINT '';
GO



-- TEST 17: sp_AdminKillGame

PRINT '--- TEST 17: sp_AdminKillGame ---';
PRINT '';

-- First create a game to kill
DECLARE @KillTestPlayerID INT = (SELECT PlayerID FROM dbo.Players WHERE PlayerName = 'Shadow');
DECLARE @KillGameID INT;
EXEC dbo.sp_GenerateGameBoard @HostPlayerID = @KillTestPlayerID, @NewGameID = @KillGameID OUTPUT;
PRINT 'Created test game to kill: GameID = ' + CAST(@KillGameID AS VARCHAR);
GO

-- : Kill the active game
PRINT ': Kill the test game';
DECLARE @KillGameID2 INT = (SELECT TOP 1 GameID FROM dbo.Games WHERE GameStatus = 'Active' ORDER BY GameID DESC);
EXEC dbo.sp_AdminKillGame @GameID = @KillGameID2;
PRINT 'Expected: SuccessStatus = 1';
SELECT GameID, GameStatus FROM dbo.Games WHERE GameID = @KillGameID2;
PRINT 'Expected: GameStatus = Stopped';
PRINT '';
GO

-- : Try to kill an already stopped game
PRINT ': Kill an already stopped game';
DECLARE @StoppedGameID INT = (SELECT TOP 1 GameID FROM dbo.Games WHERE GameStatus = 'Stopped');
EXEC dbo.sp_AdminKillGame @GameID = @StoppedGameID;
PRINT 'Expected: SuccessStatus = 0 (not active)';
PRINT '';
GO




PRINT '  ALL TESTS COMPLETE ';

GO


