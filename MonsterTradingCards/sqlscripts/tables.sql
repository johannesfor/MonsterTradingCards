use monstertradingcards;

DROP TABLE IF EXISTS marketplace;
DROP TABLE IF EXISTS card;
DROP TABLE IF EXISTS "user";
DROP TABLE IF EXISTS package;

create table "user" (
	id UUID primary key,
	username varchar(255),
	password varchar(255),
	bio varchar(255),
	image varchar(255),
	coins number,
	elo number,
	played_games number,
)

create table card (
	id UUID primary key,
	name varchar(255),
	damage number,
	element_type number,
	card_type number,

	user_id UUID,
	is_in_deck bool,

	package_id UUID,

	CONSTRAINT fk_card_user FOREIGN KEY (user_id) REFERENCES "user"(id),
	CONSTRAINT fk_card_package FOREIGN KEY (package_id) REFERENCES package(id)
)

create table package (
	id UUID primary key
)

create table marketplace (
	id UUID primary key,
	card_id UUID,
	requirement_card_type number,
	requirement_min_damage number

	CONSTRAINT fk_marketplace_card FOREIGN KEY (card_id) REFERENCES card(id)
)