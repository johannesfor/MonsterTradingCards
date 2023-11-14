\c monster_trading_cards

DROP TABLE IF EXISTS trading;
DROP TABLE IF EXISTS card;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS package;

create table users (
	id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
	username varchar(255),
	password varchar(255),
	bio varchar(255),
	image varchar(255),
	coins integer,
	elo integer,
	played_games integer,
	name varchar(255)
);

create table package (
	id UUID PRIMARY KEY DEFAULT uuid_generate_v4()
);

create table card (
	id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
	name varchar(255),
	damage decimal,
	element_type integer,
	card_type integer,

	user_id UUID,
	is_in_deck bool,

	package_id UUID,

	CONSTRAINT fk_card_user FOREIGN KEY (user_id) REFERENCES users(id),
	CONSTRAINT fk_card_package FOREIGN KEY (package_id) REFERENCES package(id)
);

create table trading (
	id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
	card_id UUID,
	user_id UUID,
	requirement_card_type integer,
	requirement_min_damage decimal,

	CONSTRAINT fk_trading_card FOREIGN KEY (card_id) REFERENCES card(id),
	CONSTRAINT fk_trading_user FOREIGN KEY (user_id) REFERENCES users(id)
);