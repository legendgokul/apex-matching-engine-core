### Understanding the basic of stock market to design the database.

1. There will always be slight delay and difference in amount, by the time we click sell and buy 
2. limit order is used to preset when to sell or when to buy so the system will keep track of it and does immediate buy sell once the price reaches that point. 
3. I think there are 2 ledgers one where it has users and how much stock they are willing to sell and what price. 
4. second ledger is where we have buyers how much stock they want and price they are willing to pay. 
5. I assume the backend logic must match multiple seller to one buyer if order is big or multiple buyer to one seller. - should i do it one step at a time or find all buyer/seller to fullfil and make it atomic .

**Simple scenario** 
Seller A : 50 stock, price 50 rupee each stock 
Seller B : 50 stock price 51 rupee each stock
Seller C : 50 stock price 50 rupee each stock
Seller D : 40 stock price 49 rupee each stock 

now if there is buyer who wants to purchase lets say 60 stock and he has set price at 50 rupee , as a back-end how do i process this? 

### DB
1. starting with Db, i am aware i must track the buyer entry and seller entry
2. stock needs to be tracked separate
3. transactions on one table

4. **Table Partitioning (Partitioning by Time or Symbol):** 
5. Append-Only Ledger Table
6. **UNLOGGED Tables / Batch Inserts:** For ultra-high performance testing, look into `UNLOGGED` tables in Postgres (which bypass the Write-Ahead Log (WAL) for non-critical audit data, though trade off crash recovery) or use **Batching (`NpgsqlBatch`)** to bulk-insert matched trades rather than round-tripping every single trade individually.
7. Explore indexing and if they can help.


Things to ask
- Assume for Partial map, we try to find n number of partial map which add up to a buyers request then do we save each partial trade as atomic or combined operation should be atomic?






### Dotnet 
- we are starting with pulling data from DB and then processing it. 
- In-memory Order Book
	- Does it scale well for Distributed systems 
	- can redis help here ?
- Zero-Allocation Logging & Parsing:
- **`ValueTask` over `Task`:** Use `ValueTask` for hot-path asynchronous methods where results are frequently synchronous to avoid allocating Task objects on the heap.