Date : 21_oct_26
1. I am going with separate tables for buy and selling ledger to see performance and compare it with single table ledger.






**Challenges** 
- How to load to check large volumes of data into DB for our testing purpose. 
- Comparing Dapper, PostgreSQL copy and EF for performance difference. 
- Decision related to what will be our primary key 
	- bigint alone
	- trade_id(bigint) + public_ic(uuid)
- Exploring Query Optimization and Indexing 



**Future question :** 
- in order to maintain steady performance , do we do a migration at end of the day into a larger insert only ledger history? 
- Do we keep one table to track per day transactions for performance sake?
- One the Database size grows extremely large what are we doing?