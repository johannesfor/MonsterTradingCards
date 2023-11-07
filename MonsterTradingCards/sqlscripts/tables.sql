use monstertradingcards;

create table "user" (
	id UUID,
	username varchar(255),
	password varchar(255),
	profile_description varchar(255),
	coins number,
	deck_id UUID,
	stack_id UUID,
	elo number,
	played_games number,
	//foreign keys
)

create table deck (
	id UUID,
	card_id number,
)

create table stack (
	id UUID,
	card_id UUID,

	//foreign keys
)

create table card (
	id UUID,
	name varchar(255),
	damage number,
	element_type number,
	card_type number,

)

create table package_cards (
	id UUID,
	package_id UUID,
	card_id UUID,

	//foreign keys
)

create table package (
	id UUID
)

create table marketplace (
	id UUID,
	card_id UUID,

	//foreign keys
)