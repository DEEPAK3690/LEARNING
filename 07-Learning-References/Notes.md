**IEnumerable:**

* Fetches all `Products` from DB into memory first, then filters prices > 100.

**IQueryable:**

* Converts query into SQL:
* Only fetches filtered data.
* ORM → maps C# objects to DB tables
* DbContext → that hadles the db connection,map C# objs to sql tables ,
* DbSet → represents table

Migrations

“Migration is used to update database schema when model changes. EF generates scripts to sync code and DB.”

“I have worked mostly on API side, but I understand SQL basics like joins, indexing, and query optimization.”


by default primary key is clusted index , 

clustured index is have the address for the pafes in intermediate node in B tree structure 

non clusted index have the address for the pafes in intermediate node in B tree structure and additional node that have pointer to the exact data in data pages 

Without Index:
→ Scan everything

With Non-Clustered Index:
→ Search index → Jump to row

With Clustered Index:
→ Data already sorted → Direct access

INSERT row →
Find correct position in index →
Shift/rebalance tree →
Write to disk

Increased Storage Usage

in memory collection ?

as is no tracking. Now, by default, if we search or whenever we are querying data, the EF Core will have the snapshot of the object hidden. The object will be stored in some memory now. When we are updating, that is fine. If we want to display the read-only, then we need to use as-no-tracking. 
When we are using AS OF node tracking in our query, it will give only the required object. It won't store the historic snapshots. The query will execute faster and uses half of the RAM. 

"By default, EF Core tracks entities in memory using snapshots so it can detect changes during SaveChanges(). However, for read-only queries—like displaying a list of items on a UI—this wastes CPU and RAM. I always apply .AsNoTracking() to my read-only queries to bypass the change tracker, which significantly improves application performance and scalability."