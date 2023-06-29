CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS settings (
    tournament_name VARCHAR(1000) PRIMARY KEY,
    battle_ticket_price INT NOT NULL,
    battle_ticket_count INT NOT NULL,
    tournament_start TIMESTAMP NOT NULL,
    tournament_end TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS usermodel (
    user_id serial PRIMARY KEY,
    username varchar(2000) NOT NULL,
    elo_rating FLOAT NOT NULL,
    on_joined TIMESTAMP NOT NULL,
    mm_address VARCHAR(100) NOT NULL,
    units VARCHAR(1000) NOT NULL,
    battlepower INT NOT NULL,
    wins INT NOT NULL,
    losses INT NOT NULL,
    totalBattles INT NOT NULL,
    tickets INT NOT NULL
);

CREATE TABLE IF NOT EXISTS battles (
    battle_id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    metamask_address VARCHAR(100) NOT NULL,
    metamask_address_opponent VARCHAR(100) NOT NULL,
    battlepower INT NOT NULL,
    battlepower_opponent INT NOT NULL,
    on_created TIMESTAMP NOT NULL,
    concluded BOOLEAN NOT NULL,
    flagged BOOLEAN NOT NULL
);

create table if not exists stats (
    totalTicketsPurchased int,
    totalFiatConsumed int
);

insert into stats (totalTicketsPurchased, totalFiatConsumed) values (0, 0);

SELECT PG_GET_SERIAL_SEQUENCE('"usermodel"', 'user_id');
SELECT CURRVAL(PG_GET_SERIAL_SEQUENCE('"usermodel"', 'user_id')) AS "Current Value", MAX("user_id") AS "Max Value" FROM "usermodel";
SELECT SETVAL((SELECT PG_GET_SERIAL_SEQUENCE('"usermodel"', 'user_id')), (SELECT (MAX("user_id") + 1) FROM "usermodel"), FALSE);