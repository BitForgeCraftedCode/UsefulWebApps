CREATE TABLE calendar_events (
    Id             BIGINT UNSIGNED AUTO_INCREMENT,
    UserId         VARCHAR(255) NULL,  -- NULL = public event
    Title          VARCHAR(255) NOT NULL,
    Description    TEXT NULL,

    StartDate      DATETIME NOT NULL,
    EndDate        DATETIME NOT NULL,
    IsAllDay       BOOLEAN NOT NULL DEFAULT FALSE,

    RRule          TEXT NULL,  -- recurrence rule
    RDate          TEXT NULL,  -- optional included dates
    ExDate         TEXT NULL,  -- optional excluded dates

    CreatedAt      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    INDEX idx_user (UserId),
    INDEX idx_start (StartDate),
    INDEX idx_user_start (UserId, StartDate),
    PRIMARY KEY (`Id`)
);