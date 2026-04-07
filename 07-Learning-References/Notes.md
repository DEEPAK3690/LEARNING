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
