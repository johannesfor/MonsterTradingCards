\c monster_trading_cards

DROP TABLE IF EXISTS marketplace;
DROP TABLE IF EXISTS card;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS package;

create table users (
	id UUID primary key,
	username varchar(255),
	password varchar(255),
	bio varchar(255),
	image varchar(255),
	coins integer,
	elo integer,
	played_games integer
);

create table package (
	id UUID primary key
);

create table card (
	id UUID primary key,
	name varchar(255),
	damage integer,
	element_type integer,
	card_type integer,

	user_id UUID,
	is_in_deck bool,

	package_id UUID,

	CONSTRAINT fk_card_user FOREIGN KEY (user_id) REFERENCES users(id),
	CONSTRAINT fk_card_package FOREIGN KEY (package_id) REFERENCES package(id)
);

create table marketplace (
	id UUID primary key,
	card_id UUID,
	requirement_card_type integer,
	requirement_min_damage integer,

	CONSTRAINT fk_marketplace_card FOREIGN KEY (card_id) REFERENCES card(id)
);