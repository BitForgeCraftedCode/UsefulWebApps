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

/*Federal Holidays RRule's inserted via web server command line*/

/*New Year's Day*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'New Year''s Day',
  '2025-01-01 00:00:00',
  '2025-01-01 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=1;BYMONTHDAY=1'
);

/*Martin Luther King Jr. Day*/
/*3rd Monday in January*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Martin Luther King Jr. Day',
  '2025-01-20 00:00:00',
  '2025-01-20 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=1;BYDAY=MO;BYSETPOS=3'
);

/*Washington's Birthday*/
/*3rd Monday in February*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Washington''s Birthday',
  '2025-02-17 00:00:00',
  '2025-02-17 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=2;BYDAY=MO;BYSETPOS=3'
);

/*Memorial Day*/
/*Last Monday in May*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Memorial Day',
  '2025-05-26 00:00:00',
  '2025-05-26 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=5;BYDAY=MO;BYSETPOS=-1'
);

/*Juneteenth*/
/*June 19*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Juneteenth National Independence Day',
  '2025-06-19 00:00:00',
  '2025-06-19 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=6;BYMONTHDAY=19'
);

/*Independence Day*/
/*July 4*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Independence Day',
  '2025-07-04 00:00:00',
  '2025-07-04 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=7;BYMONTHDAY=4'
);

/*Labor Day*/
/*1st Monday in September*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Labor Day',
  '2025-09-01 00:00:00',
  '2025-09-01 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=9;BYDAY=MO;BYSETPOS=1'
);

/*Columbus Day*/
/*2nd Monday in October*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Columbus Day',
  '2025-10-13 00:00:00',
  '2025-10-13 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=10;BYDAY=MO;BYSETPOS=2'
);

/*Veterans Day*/
/*November 11*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Veterans Day',
  '2025-11-11 00:00:00',
  '2025-11-11 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=11;BYMONTHDAY=11'
);

/*Thanksgiving Day*/
/*4th Thursday in November*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Thanksgiving Day',
  '2025-11-27 00:00:00',
  '2025-11-27 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=11;BYDAY=TH;BYSETPOS=4'
);

/*Christmas Eve*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Christmas Eve',
  '2025-12-24 00:00:00',
  '2025-12-24 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=12;BYMONTHDAY=24'
);

/*Christmas Day*/
/*December 25*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Christmas Day',
  '2025-12-25 00:00:00',
  '2025-12-25 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=12;BYMONTHDAY=25'
);

/*New Year's Eve*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'New Year''s Eve',
  '2025-12-31 00:00:00',
  '2025-12-31 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=12;BYMONTHDAY=31'
);

/*Pearl Harbor Remembrance Day*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Pearl Harbor Remembrance Day',
  '2025-12-07 00:00:00',
  '2025-12-07 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=12;BYMONTHDAY=7'
);

/*Patriot Day (September 11)*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Patriot Day',
  '2025-09-11 00:00:00',
  '2025-09-11 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=9;BYMONTHDAY=11'
);

/*Valentine's Day*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Valentine''s Day',
  '2025-02-14 00:00:00',
  '2025-02-14 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=2;BYMONTHDAY=14'
);

/*St. Patrick's Day*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'St. Patrick''s Day',
  '2025-03-17 00:00:00',
  '2025-03-17 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=3;BYMONTHDAY=17'
);

/*Mother's Day*/
/*2nd Sunday in May*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Mother''s Day',
  '2025-05-11 00:00:00',
  '2025-05-11 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=5;BYDAY=SU;BYSETPOS=2'
);

/*Father's Day*/
/*3rd Sunday in June*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Father''s Day',
  '2025-06-15 00:00:00',
  '2025-06-15 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=6;BYDAY=SU;BYSETPOS=3'
);

/*Halloween*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Halloween',
  '2025-10-31 00:00:00',
  '2025-10-31 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=10;BYMONTHDAY=31'
);

/*Cinco de Mayo*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Cinco de Mayo',
  '2025-05-05 00:00:00',
  '2025-05-05 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=5;BYMONTHDAY=5'
);

/*April Fool''s Day*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'April Fool''s Day',
  '2025-04-01 00:00:00',
  '2025-04-01 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=4;BYMONTHDAY=1'
);

/*Earth Day*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Earth Day',
  '2025-04-22 00:00:00',
  '2025-04-22 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=4;BYMONTHDAY=22'
);

/*Flag Day*/
INSERT INTO calendar_events
(UserId, Title, StartDate, EndDate, IsAllDay, RRule)
VALUES
(
  NULL,
  'Flag Day',
  '2025-06-14 00:00:00',
  '2025-06-14 23:59:59',
  1,
  'FREQ=YEARLY;BYMONTH=6;BYMONTHDAY=14'
);