USE project_wind;

-- =============================================
-- 계정 조회 또는 생성
-- 없으면 새로 INSERT, 있으면 last_login 갱신 후 SELECT
-- =============================================
DROP PROCEDURE IF EXISTS sp_get_or_create_account;

DELIMITER //
CREATE PROCEDURE sp_get_or_create_account(
    IN p_account_name  VARCHAR(50),
    IN p_password_hash VARCHAR(60)
)
BEGIN
    DECLARE v_id INT;

    SELECT id INTO v_id
    FROM accounts
    WHERE account_name = p_account_name;

    IF v_id IS NULL THEN
        INSERT INTO accounts (account_name, password_hash, player_name)
        VALUES (p_account_name, p_password_hash, p_account_name);

        SET v_id = LAST_INSERT_ID();
    ELSE
        UPDATE accounts
        SET last_login = CURRENT_TIMESTAMP
        WHERE id = v_id;
    END IF;

    SELECT * FROM accounts WHERE id = v_id;
END //
DELIMITER ;

-- =============================================
-- 위치 저장
-- =============================================
DROP PROCEDURE IF EXISTS sp_save_position;

DELIMITER //
CREATE PROCEDURE sp_save_position(
    IN p_account_id INT,
    IN p_map_id     VARCHAR(50),
    IN p_pos_x      INT,
    IN p_pos_y      INT
)
BEGIN
    UPDATE accounts
    SET map_id = p_map_id,
        pos_x  = p_pos_x,
        pos_y  = p_pos_y
    WHERE id = p_account_id;
END //
DELIMITER ;
