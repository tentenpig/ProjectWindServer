CREATE DATABASE IF NOT EXISTS project_wind
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_unicode_ci;

USE project_wind;

CREATE TABLE IF NOT EXISTS accounts (
    id          INT             NOT NULL AUTO_INCREMENT,
    account_name  VARCHAR(50)    NOT NULL,
    password_hash VARCHAR(60)    NOT NULL,
    player_name   VARCHAR(50)    NOT NULL,
    level       INT             NOT NULL DEFAULT 1,
    map_id      VARCHAR(50)    NOT NULL DEFAULT 'town_01',
    pos_x       INT             NOT NULL DEFAULT 10,
    pos_y       INT             NOT NULL DEFAULT 7,
    created_at  DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_login  DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_account_name (account_name)
) ENGINE=InnoDB;
