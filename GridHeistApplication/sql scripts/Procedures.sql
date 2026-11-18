

USE GridHeistDB2;
GO


-- SECTION A: AUTHENTICATION PROCEDURES


/*
    sp_ValidatePlayer
    Validates a player login with password hashing and lockout.
    Returns: 1 = success, 0 = invalid, -1 = locked
    OUTPUT: @PlayerID = the logged-in player's ID (NULL on failure)
*/
CREATE OR ALTER PROCEDURE dbo.sp_ValidatePlayer
    @PlayerName  VARCHAR(50),
    @Password    VARCHAR(100),
    @PlayerID    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @PlayerID = NULL;

    BEGIN TRY
        DECLARE @PasswordHash VARCHAR(255) = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @Password), 2);
        DECLARE @ID INT, @StoredHash VARCHAR(255), @IsLocked BIT, @Attempts INT;

        SELECT @ID = PlayerID,
               @StoredHash = PasswordHash,
               @IsLocked = IsLockedOut,
               @Attempts = FailedLoginAttempts
        FROM dbo.Players
        WHERE PlayerName = @PlayerName;

        -- Player not found
        IF @ID IS NULL
        BEGIN
            SELECT 0 AS LoginStatus;
            RETURN;
        END

        -- Account locked
        IF @IsLocked = 1
        BEGIN
            SELECT -1 AS LoginStatus;
            RETURN;
        END

        -- Password matches
        IF @StoredHash = @PasswordHash
        BEGIN
            BEGIN TRANSACTION;
                UPDATE dbo.Players
                SET FailedLoginAttempts = 0, IsOnline = 1
                WHERE PlayerID = @ID;
            COMMIT TRANSACTION;

            SET @PlayerID = @ID;
            SELECT 1 AS LoginStatus;
            RETURN;
        END

        -- Wrong password - increment failed attempts
        BEGIN TRANSACTION;
            SET @Attempts = @Attempts + 1;
            UPDATE dbo.Players
            SET FailedLoginAttempts = @Attempts,
                IsLockedOut = CASE WHEN @Attempts >= 5 THEN 1 ELSE 0 END
            WHERE PlayerID = @ID;
        COMMIT TRANSACTION;

        SELECT 0 AS LoginStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT -99 AS LoginStatus;  -- Error indicator
    END CATCH
END
GO


/*
    sp_RegisterPlayer
    Registers a new player with hashed password.
    Returns: 1 = success, 0 = username taken
    OUTPUT: @PlayerID = new player's ID
*/
CREATE OR ALTER PROCEDURE dbo.sp_RegisterPlayer
    @PlayerName  VARCHAR(50),
    @Password    VARCHAR(100),
    @PlayerID    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @PlayerID = NULL;

    BEGIN TRY
        -- Check duplicate username
        IF EXISTS (SELECT 1 FROM dbo.Players WHERE PlayerName = @PlayerName)
        BEGIN
            SELECT 0 AS RegisterStatus;
            RETURN;
        END

        BEGIN TRANSACTION;
            INSERT INTO dbo.Players (PlayerName, PasswordHash, IsAdmin)
            VALUES (@PlayerName,
                    CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @Password), 2),
                    0);

            SET @PlayerID = SCOPE_IDENTITY();
        COMMIT TRANSACTION;

        SELECT 1 AS RegisterStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS RegisterStatus;
    END CATCH
END
GO



-- SECTION B: GAME SETUP PROCEDURES


/*
    sp_GenerateGameBoard
    Creates a new game with an 8x8 grid and places the host player at Home.
    OUTPUT: @NewGameID = the created game's ID
*/
CREATE OR ALTER PROCEDURE dbo.sp_GenerateGameBoard
    @HostPlayerID  INT,
    @NewGameID     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewGameID = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Create the game
        INSERT INTO dbo.Games (CreatedByPlayerID, GameStatus, MaxPlayers, TimeLimitSeconds, StartTime)
        VALUES (@HostPlayerID, 'Active', 4, 900, SYSUTCDATETIME());

        SET @NewGameID = SCOPE_IDENTITY();

        -- Guard: if insert failed, stop
        IF @NewGameID IS NULL OR @NewGameID = 0
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1 AS GameStatus;
            RETURN;
        END

        -- 2. Generate 8x8 grid (64 tiles)
        ;WITH Numbers AS (
            SELECT N FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8)) AS T(N)
        )
        INSERT INTO dbo.GameTiles (GameID, RowNumber, ColumnNumber, TileType, IsWalkable)
        SELECT
            @NewGameID, R.N, C.N,
            CASE
                WHEN R.N = 1 AND C.N = 1 THEN 'Home'
                WHEN R.N = 8 AND C.N = 8 THEN 'Exit'
                WHEN (R.N = 3 AND C.N IN (3, 4)) OR (R.N = 6 AND C.N = 6) THEN 'Firewall'
                ELSE 'Floor'
            END,
            CASE
                WHEN (R.N = 3 AND C.N IN (3, 4)) OR (R.N = 6 AND C.N = 6) THEN 0
                ELSE 1
            END
        FROM Numbers R CROSS JOIN Numbers C;

        -- 3. Place host player at Home tile (1,1)
        DECLARE @HomeTileID INT;
        SELECT @HomeTileID = TileID FROM dbo.GameTiles
        WHERE GameID = @NewGameID AND RowNumber = 1 AND ColumnNumber = 1;

        INSERT INTO dbo.GamePlayers (GameID, PlayerID, CurrentTileID, Score, PlayerStatus)
        VALUES (@NewGameID, @HostPlayerID, @HomeTileID, 0, 'Playing');

        -- 4. Create NPCs for this game
        -- Sentinel patrols row 4
        DECLARE @SentinelTile INT;
        SELECT @SentinelTile = TileID FROM dbo.GameTiles
        WHERE GameID = @NewGameID AND RowNumber = 4 AND ColumnNumber = 1;

        INSERT INTO dbo.NPCs (GameID, NPCName, CurrentTileID, IsMovingForward)
        VALUES (@NewGameID, 'Sentinel', @SentinelTile, 1);

        DECLARE @SentinelID INT = SCOPE_IDENTITY();

        -- Sentinel path: row 4, columns 1 through 6
        INSERT INTO dbo.NPCPath (NPCID, TileID, PathOrder)
        SELECT @SentinelID, TileID, ColumnNumber
        FROM dbo.GameTiles
        WHERE GameID = @NewGameID AND RowNumber = 4 AND ColumnNumber BETWEEN 1 AND 6
        ORDER BY ColumnNumber;

        -- Glitch patrols row 7
        DECLARE @GlitchTile INT;
        SELECT @GlitchTile = TileID FROM dbo.GameTiles
        WHERE GameID = @NewGameID AND RowNumber = 7 AND ColumnNumber = 2;

        INSERT INTO dbo.NPCs (GameID, NPCName, CurrentTileID, IsMovingForward)
        VALUES (@NewGameID, 'Glitch', @GlitchTile, 1);

        DECLARE @GlitchID INT = SCOPE_IDENTITY();

        INSERT INTO dbo.NPCPath (NPCID, TileID, PathOrder)
        SELECT @GlitchID, TileID, ColumnNumber
        FROM dbo.GameTiles
        WHERE GameID = @NewGameID AND RowNumber = 7 AND ColumnNumber BETWEEN 2 AND 7
        ORDER BY ColumnNumber;

        COMMIT TRANSACTION;
        SELECT 1 AS GameStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @NewGameID = 0;
        SELECT -1 AS GameStatus;
    END CATCH
END
GO


/*
    sp_PlaceInitialItems
    Places items randomly on walkable Floor tiles for a game.
    Ensures at least 3 Fragments (ItemTypeID 1-3) for the win condition.
*/
CREATE OR ALTER PROCEDURE dbo.sp_PlaceInitialItems
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Place 3 guaranteed Fragments (types 1, 2, 3) on random floor tiles
        INSERT INTO dbo.GameItems (GameID, ItemTypeID, CurrentTileID, ItemStatus)
        SELECT TOP 1 @GameID, 1, TileID, 'OnTile'
        FROM dbo.GameTiles
        WHERE GameID = @GameID AND TileType = 'Floor' AND IsWalkable = 1
        ORDER BY NEWID();

        INSERT INTO dbo.GameItems (GameID, ItemTypeID, CurrentTileID, ItemStatus)
        SELECT TOP 1 @GameID, 2, TileID, 'OnTile'
        FROM dbo.GameTiles
        WHERE GameID = @GameID AND TileType = 'Floor' AND IsWalkable = 1
              AND TileID NOT IN (SELECT CurrentTileID FROM dbo.GameItems WHERE GameID = @GameID AND CurrentTileID IS NOT NULL)
        ORDER BY NEWID();

        INSERT INTO dbo.GameItems (GameID, ItemTypeID, CurrentTileID, ItemStatus)
        SELECT TOP 1 @GameID, 3, TileID, 'OnTile'
        FROM dbo.GameTiles
        WHERE GameID = @GameID AND TileType = 'Floor' AND IsWalkable = 1
              AND TileID NOT IN (SELECT CurrentTileID FROM dbo.GameItems WHERE GameID = @GameID AND CurrentTileID IS NOT NULL)
        ORDER BY NEWID();

        -- Place 5 more random items (types 4-7: chips, tools)
        INSERT INTO dbo.GameItems (GameID, ItemTypeID, CurrentTileID, ItemStatus)
        SELECT TOP 5
            @GameID,
            ABS(CHECKSUM(NEWID()) % 4) + 4,  -- ItemTypeID 4-7
            TileID,
            'OnTile'
        FROM dbo.GameTiles
        WHERE GameID = @GameID AND TileType = 'Floor' AND IsWalkable = 1
              AND TileID NOT IN (SELECT CurrentTileID FROM dbo.GameItems WHERE GameID = @GameID AND CurrentTileID IS NOT NULL)
        ORDER BY NEWID();

        COMMIT TRANSACTION;
        SELECT 1 AS ItemsPlaced;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS ItemsPlaced;
    END CATCH
END
GO



-- SECTION C: GAMEPLAY PROCEDURES


/*
    sp_GetGameBoard
    Returns all 64 tiles with any items on them for rendering the grid.
*/
CREATE OR ALTER PROCEDURE dbo.sp_GetGameBoard
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
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
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


/*
    sp_GetGameStatus
    Returns the current status of a game.
*/
CREATE OR ALTER PROCEDURE dbo.sp_GetGameStatus
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT GameStatus FROM dbo.Games WHERE GameID = @GameID;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


/*
    sp_MovePlayer
    Moves a player to an adjacent tile. Picks up items automatically.
    Returns: 1 = moved, 2 = moved + picked up item, 3 = WIN,
             0 = invalid move, -1 = tile occupied, -2 = not enough fragments to exit
*/
CREATE OR ALTER PROCEDURE dbo.sp_MovePlayer
    @GameID      INT,
    @PlayerID    INT,
    @TargetTileID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @CurrentRow INT, @CurrentCol INT;
        DECLARE @TargetRow INT, @TargetCol INT, @TargetWalkable BIT, @TargetType VARCHAR(20);

        -- 1. Get current position
        SELECT @CurrentRow = GT.RowNumber, @CurrentCol = GT.ColumnNumber
        FROM dbo.GamePlayers GP
        INNER JOIN dbo.GameTiles GT ON GP.CurrentTileID = GT.TileID
        WHERE GP.GameID = @GameID AND GP.PlayerID = @PlayerID;

        -- 2. Get target tile info
        SELECT @TargetRow = RowNumber, @TargetCol = ColumnNumber,
               @TargetWalkable = IsWalkable, @TargetType = TileType
        FROM dbo.GameTiles
        WHERE TileID = @TargetTileID AND GameID = @GameID;

        -- 3. Must be exactly 1 step adjacent
        IF (ABS(@CurrentRow - @TargetRow) + ABS(@CurrentCol - @TargetCol) != 1)
        BEGIN
            SELECT 0 AS MoveStatus;
            RETURN;
        END

        -- 4. Must be walkable
        IF @TargetWalkable = 0
        BEGIN
            SELECT 0 AS MoveStatus;
            RETURN;
        END

        -- 5. Can't move to occupied tile (except Home tile can hold multiple)
        IF @TargetType != 'Home' AND EXISTS (
            SELECT 1 FROM dbo.GamePlayers
            WHERE CurrentTileID = @TargetTileID AND GameID = @GameID AND PlayerID != @PlayerID
        )
        BEGIN
            SELECT -1 AS MoveStatus;
            RETURN;
        END

        -- 6. Exit tile: check win condition (need 3 fragments)
        IF @TargetType = 'Exit'
        BEGIN
            DECLARE @FragmentCount INT;
            SELECT @FragmentCount = COUNT(*)
            FROM dbo.PlayerInventory PI
            INNER JOIN dbo.GamePlayers GP ON PI.GamePlayerID = GP.GamePlayerID
            INNER JOIN dbo.GameItems GI ON PI.GameItemID = GI.GameItemID
            WHERE GP.GameID = @GameID AND GP.PlayerID = @PlayerID
                  AND GI.ItemTypeID IN (1, 2, 3);  -- Fragment types

            IF @FragmentCount < 3
            BEGIN
                SELECT -2 AS MoveStatus;  -- Not enough fragments
                RETURN;
            END

            -- WIN! Move to exit, mark as winner
            BEGIN TRANSACTION;
                UPDATE dbo.GamePlayers
                SET CurrentTileID = @TargetTileID, IsWinner = 1, PlayerStatus = 'Finished'
                WHERE GameID = @GameID AND PlayerID = @PlayerID;

                -- End the game
                UPDATE dbo.Games
                SET GameStatus = 'Completed', EndTime = SYSUTCDATETIME()
                WHERE GameID = @GameID;
            COMMIT TRANSACTION;

            SELECT 3 AS MoveStatus;  -- WIN
            RETURN;
        END

        -- 7. Normal move
        BEGIN TRANSACTION;
            UPDATE dbo.GamePlayers
            SET CurrentTileID = @TargetTileID
            WHERE GameID = @GameID AND PlayerID = @PlayerID;

            -- 8. Check for item pickup
            DECLARE @FoundItemID INT;
            SELECT TOP 1 @FoundItemID = GameItemID
            FROM dbo.GameItems
            WHERE CurrentTileID = @TargetTileID AND GameID = @GameID AND ItemStatus = 'OnTile';

            IF @FoundItemID IS NOT NULL
            BEGIN
                -- Pick up the item
                UPDATE dbo.GameItems
                SET ItemStatus = 'InInventory', CurrentTileID = NULL
                WHERE GameItemID = @FoundItemID;

                -- Add to inventory
                INSERT INTO dbo.PlayerInventory (GamePlayerID, GameItemID, CollectedAt)
                SELECT GamePlayerID, @FoundItemID, SYSUTCDATETIME()
                FROM dbo.GamePlayers
                WHERE GameID = @GameID AND PlayerID = @PlayerID;

                -- Score +10
                UPDATE dbo.GamePlayers
                SET Score = Score + 10
                WHERE GameID = @GameID AND PlayerID = @PlayerID;

                COMMIT TRANSACTION;
                SELECT 2 AS MoveStatus;  -- Moved + item picked up
                RETURN;
            END
        COMMIT TRANSACTION;

        SELECT 1 AS MoveStatus;  -- Normal move
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS MoveStatus;
    END CATCH
END
GO


/*
    sp_MoveNPC
    Moves all NPCs in a game one step along their patrol path.
    Reverses direction when reaching the end.
*/
CREATE OR ALTER PROCEDURE dbo.sp_MoveNPC
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Process each NPC in this game
        DECLARE @NPCID INT, @CurrentTile INT, @IsForward BIT;
        DECLARE @CurrentOrder INT, @MinOrder INT, @MaxOrder INT, @NextOrder INT, @NextTile INT;

        DECLARE npc_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT NPCID, CurrentTileID, IsMovingForward
            FROM dbo.NPCs
            WHERE GameID = @GameID;

        OPEN npc_cursor;
        FETCH NEXT FROM npc_cursor INTO @NPCID, @CurrentTile, @IsForward;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Find current position in path
            SELECT @CurrentOrder = PathOrder
            FROM dbo.NPCPath
            WHERE NPCID = @NPCID AND TileID = @CurrentTile;

            -- Get path boundaries
            SELECT @MinOrder = MIN(PathOrder), @MaxOrder = MAX(PathOrder)
            FROM dbo.NPCPath
            WHERE NPCID = @NPCID;

            -- Determine next position
            IF @CurrentOrder IS NOT NULL
            BEGIN
                IF @IsForward = 1
                BEGIN
                    SET @NextOrder = @CurrentOrder + 1;
                    IF @NextOrder > @MaxOrder
                    BEGIN
                        SET @NextOrder = @CurrentOrder - 1;
                        SET @IsForward = 0;  -- Reverse
                    END
                END
                ELSE
                BEGIN
                    SET @NextOrder = @CurrentOrder - 1;
                    IF @NextOrder < @MinOrder
                    BEGIN
                        SET @NextOrder = @CurrentOrder + 1;
                        SET @IsForward = 1;  -- Reverse
                    END
                END

                -- Get the tile at next position
                SELECT @NextTile = TileID
                FROM dbo.NPCPath
                WHERE NPCID = @NPCID AND PathOrder = @NextOrder;

                IF @NextTile IS NOT NULL
                BEGIN
                    UPDATE dbo.NPCs
                    SET CurrentTileID = @NextTile, IsMovingForward = @IsForward
                    WHERE NPCID = @NPCID;
                END
            END

            FETCH NEXT FROM npc_cursor INTO @NPCID, @CurrentTile, @IsForward;
        END

        CLOSE npc_cursor;
        DEALLOCATE npc_cursor;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO



-- SECTION D: MULTIUSER PROCEDURES


/*
    sp_GetAvailableGames
    Lists active games that have room for more players.
*/
CREATE OR ALTER PROCEDURE dbo.sp_GetAvailableGames
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            G.GameID,
            G.CreatedByPlayerID,
            P.PlayerName AS HostName,
            G.MaxPlayers,
            (SELECT COUNT(*) FROM dbo.GamePlayers WHERE GameID = G.GameID AND PlayerStatus = 'Playing') AS CurrentPlayers,
            G.StartTime
        FROM dbo.Games G
        INNER JOIN dbo.Players P ON G.CreatedByPlayerID = P.PlayerID
        WHERE G.GameStatus = 'Active'
              AND (SELECT COUNT(*) FROM dbo.GamePlayers WHERE GameID = G.GameID AND PlayerStatus = 'Playing') < G.MaxPlayers
        ORDER BY G.StartTime DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


/*
    sp_JoinGame
    Adds a player to an existing active game at the Home tile.
    Returns: 1 = success, 0 = game full or not active, -1 = already in game
*/
CREATE OR ALTER PROCEDURE dbo.sp_JoinGame
    @GameID    INT,
    @PlayerID  INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if already in this game
        IF EXISTS (SELECT 1 FROM dbo.GamePlayers WHERE GameID = @GameID AND PlayerID = @PlayerID)
        BEGIN
            SELECT -1 AS JoinStatus;
            RETURN;
        END

        -- Check game is active and has room
        DECLARE @MaxP INT, @CurrentP INT, @Status VARCHAR(20);
        SELECT @MaxP = MaxPlayers, @Status = GameStatus FROM dbo.Games WHERE GameID = @GameID;

        IF @Status != 'Active'
        BEGIN
            SELECT 0 AS JoinStatus;
            RETURN;
        END

        SELECT @CurrentP = COUNT(*) FROM dbo.GamePlayers WHERE GameID = @GameID AND PlayerStatus = 'Playing';

        IF @CurrentP >= @MaxP
        BEGIN
            SELECT 0 AS JoinStatus;
            RETURN;
        END

        BEGIN TRANSACTION;
            -- Place at Home tile
            DECLARE @HomeTile INT;
            SELECT @HomeTile = TileID FROM dbo.GameTiles
            WHERE GameID = @GameID AND RowNumber = 1 AND ColumnNumber = 1;

            INSERT INTO dbo.GamePlayers (GameID, PlayerID, CurrentTileID, Score, PlayerStatus)
            VALUES (@GameID, @PlayerID, @HomeTile, 0, 'Playing');
        COMMIT TRANSACTION;

        SELECT 1 AS JoinStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS JoinStatus;
    END CATCH
END
GO


/*
    sp_GetOtherPlayers
    Returns all players in a game (for drawing them on the board).
*/
CREATE OR ALTER PROCEDURE dbo.sp_GetOtherPlayers
    @GameID    INT,
    @PlayerID  INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            GP.PlayerID,
            P.PlayerName,
            GP.CurrentTileID,
            GT.RowNumber,
            GT.ColumnNumber,
            GP.Score,
            GP.PlayerStatus
        FROM dbo.GamePlayers GP
        INNER JOIN dbo.Players P ON GP.PlayerID = P.PlayerID
        INNER JOIN dbo.GameTiles GT ON GP.CurrentTileID = GT.TileID
        WHERE GP.GameID = @GameID AND GP.PlayerID != @PlayerID AND GP.PlayerStatus = 'Playing';
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO



-- SECTION E: CHAT PROCEDURES


/*
    sp_SendChatMessage
    Sends a chat message from a player in a game.
    Returns: 1 = success, 0 = failed
*/
CREATE OR ALTER PROCEDURE dbo.sp_SendChatMessage
    @GameID      INT,
    @PlayerID    INT,
    @MessageText VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @GamePlayerID INT;
        SELECT @GamePlayerID = GamePlayerID FROM dbo.GamePlayers
        WHERE GameID = @GameID AND PlayerID = @PlayerID;

        IF @GamePlayerID IS NULL
        BEGIN
            SELECT 0 AS SendStatus;
            RETURN;
        END

        BEGIN TRANSACTION;
            INSERT INTO dbo.ChatMessages (GamePlayerID, MessageText)
            VALUES (@GamePlayerID, @MessageText);
        COMMIT TRANSACTION;

        SELECT 1 AS SendStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS SendStatus;
    END CATCH
END
GO


/*
    sp_GetChatMessages
    Returns the last 50 chat messages for a game.
*/
CREATE OR ALTER PROCEDURE dbo.sp_GetChatMessages
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT TOP 50
            P.PlayerName,
            CM.MessageText,
            CM.SentAt
        FROM dbo.ChatMessages CM
        INNER JOIN dbo.GamePlayers GP ON CM.GamePlayerID = GP.GamePlayerID
        INNER JOIN dbo.Players P ON GP.PlayerID = P.PlayerID
        WHERE GP.GameID = @GameID
        ORDER BY CM.SentAt DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO



-- SECTION F: ADMIN PROCEDURES


/*
    sp_AdminGetAllPlayers
    Returns all players for the admin panel.
*/
CREATE OR ALTER PROCEDURE dbo.sp_AdminGetAllPlayers
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT PlayerID, PlayerName, IsLockedOut, IsAdmin, IsOnline, HighestScore
        FROM dbo.Players
        ORDER BY PlayerName;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


/*
    sp_AdminAddPlayer
    Admin creates a new player account.
    Returns: 1 = success, 0 = username taken
*/
CREATE OR ALTER PROCEDURE dbo.sp_AdminAddPlayer
    @PlayerName    VARCHAR(50),
    @PasswordHash  VARCHAR(255),
    @IsAdmin       BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.Players WHERE PlayerName = @PlayerName)
        BEGIN
            SELECT 0 AS RegistrationStatus;
            RETURN;
        END

        BEGIN TRANSACTION;
            INSERT INTO dbo.Players (PlayerName, PasswordHash, IsOnline, IsAdmin)
            VALUES (@PlayerName,
                    CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @PasswordHash), 2),
                    0, @IsAdmin);
        COMMIT TRANSACTION;

        SELECT 1 AS RegistrationStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS RegistrationStatus;
    END CATCH
END
GO


/*
    sp_AdminUpdatePlayer
    Admin updates a player's name, password, lock status, or admin role.
    Returns: 1 = success, 0 = username taken
*/
CREATE OR ALTER PROCEDURE dbo.sp_AdminUpdatePlayer
    @TargetPlayerID    INT,
    @NewPlayerName     VARCHAR(50),
    @ResetPasswordHash VARCHAR(255),
    @UnlockAccount     BIT,
    @IsAdmin           BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if new username is taken by someone else
        IF EXISTS (SELECT 1 FROM dbo.Players WHERE PlayerName = @NewPlayerName AND PlayerID != @TargetPlayerID)
        BEGIN
            SELECT 0 AS UpdateStatus;
            RETURN;
        END

        BEGIN TRANSACTION;
            -- Update name and admin status
            UPDATE dbo.Players
            SET PlayerName = @NewPlayerName,
                IsAdmin = @IsAdmin
            WHERE PlayerID = @TargetPlayerID;

            -- Unlock if requested
            IF @UnlockAccount = 1
            BEGIN
                UPDATE dbo.Players
                SET IsLockedOut = 0, FailedLoginAttempts = 0
                WHERE PlayerID = @TargetPlayerID;
            END

            -- Reset password if provided
            IF @ResetPasswordHash IS NOT NULL AND LEN(LTRIM(RTRIM(@ResetPasswordHash))) > 0
            BEGIN
                UPDATE dbo.Players
                SET PasswordHash = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @ResetPasswordHash), 2)
                WHERE PlayerID = @TargetPlayerID;
            END
        COMMIT TRANSACTION;

        SELECT 1 AS UpdateStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS UpdateStatus;
    END CATCH
END
GO


/*
    sp_AdminDeletePlayer
    Admin deletes a player and all their related data.
    Uses transaction to ensure all-or-nothing deletion.
    Returns: 1 = success, 0 = failed
*/
CREATE OR ALTER PROCEDURE dbo.sp_AdminDeletePlayer
    @TargetPlayerID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Remove chat messages
        DELETE FROM dbo.ChatMessages
        WHERE GamePlayerID IN (SELECT GamePlayerID FROM dbo.GamePlayers WHERE PlayerID = @TargetPlayerID);

        -- 2. Remove inventory
        DELETE FROM dbo.PlayerInventory
        WHERE GamePlayerID IN (SELECT GamePlayerID FROM dbo.GamePlayers WHERE PlayerID = @TargetPlayerID);

        -- 3. Remove from games
        DELETE FROM dbo.GamePlayers
        WHERE PlayerID = @TargetPlayerID;

        -- 4. Unlink games they created
        UPDATE dbo.Games
        SET CreatedByPlayerID = NULL
        WHERE CreatedByPlayerID = @TargetPlayerID;

        -- 5. Delete the player
        DELETE FROM dbo.Players
        WHERE PlayerID = @TargetPlayerID;

        COMMIT TRANSACTION;
        SELECT 1 AS DeleteStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS DeleteStatus;
    END CATCH
END
GO


/*
    sp_AdminKillGame
    Admin terminates a running game.
    Returns: 1 = success, 0 = no active game found
*/
CREATE OR ALTER PROCEDURE dbo.sp_AdminKillGame
    @GameID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check game exists and is active
        IF NOT EXISTS (SELECT 1 FROM dbo.Games WHERE GameID = @GameID AND GameStatus = 'Active')
        BEGIN
            SELECT 0 AS SuccessStatus;
            RETURN;
        END

        BEGIN TRANSACTION;
            UPDATE dbo.Games
            SET GameStatus = 'Stopped', EndTime = SYSUTCDATETIME()
            WHERE GameID = @GameID;

            UPDATE dbo.GamePlayers
            SET PlayerStatus = 'Finished'
            WHERE GameID = @GameID;
        COMMIT TRANSACTION;

        SELECT 1 AS SuccessStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS SuccessStatus;
    END CATCH
END
GO



GO


