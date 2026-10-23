Create table Company (
	company_id int PRIMARY KEY,
	company_name text,
	company_url text,
	createdon timestamptz
);

Create table stock (
	stock_id int PRIMARY KEY,
	stock_name text,
	stock_symbol varchar(5),
	company_id int references Company(company_id),
	createdon timestamptz
);

Create table Users(
	user_id int PRIMARY KEY,
	userName text,
	email text UNIQUE,
	createdOn timestamptz
);

Create table UserHolding(
	user_id int references Users(user_id),
	stock_id int references stock(stock_id),
	Quantity int,
	boughtPrice int,
	isActive bool 
);

ALTER TABLE UserHolding
ADD CONSTRAINT userholding_pk PRIMARY KEY (user_id, stock_id);

Create table BuyLedger (
	ledger_id BIGINT PRIMARY KEY,  -- since this table is going to have lot more records
	stock_id int references stock(stock_id),
	user_id int references Users (user_id),
	stockCount int CHECK(stockCount > 0),
	stockPrice int, 
	createdOn timestamptz
);

Create table SellLedger (
	ledger_id BIGINT PRIMARY KEY,  -- since this table is going to have lot more records
	stock_id int references stock(stock_id),
	user_id int references Users (user_id),
	stockCount int CHECK(stockCount > 0),
	stockPrice int, 
	createdOn timestamptz
);


Create table TransactionRecord (
	trade_id BIGINT PRIMARY KEY,
	buyer_id int references users(user_id),
	seller_id int references users(user_id),
	stock_id int references stock(stock_id),
	salePrice int,
	tnxTime timestamptz
);

/*
Drop table TransactionRecord;
Drop table SellLedger;
Drop table BuyLedger;
Drop table UserHolding;
Drop table Users;
Drop table stock;
Drop table Company
*/


