### What is REST?

REST, or  **Representational State Transfer** , is an architectural style for designing networked applications. REST relies on a stateless, client-server communication protocol—almost always HTTP. It is designed to be simple, scalable, and easy to use.

### Benefits of RESTful APIs for .NET Developers

* **Scalability** : RESTful APIs are stateless, meaning each request is independent. This makes them highly scalable.
* **Simplicity** : REST uses standard HTTP methods (GET, POST, PUT, DELETE) and simple URIs, making it easy to understand and use.
* **Flexibility** : RESTful APIs can return data in multiple formats, such as JSON or XML, depending on the client’s needs.
* **Separation of Concerns** : REST separates the client (front-end) from the server (back-end), allowing both to evolve independently

Each request from the client to the server **must contain all the information needed** to process it.

### **Client-Server Architecture**

The client-server architecture is a fundamental principle of REST. In this model:

* The **client** (e.g., a web browser or mobile app) is responsible for the user interface and user experience.
* The **server** (e.g., a .NET Web API) handles data storage, business logic, and resource management.

This separation of concerns allows the client and server to evolve independently. For example, you can update the server-side logic without affecting the client, or you can develop a new client (like a mobile app) without changing the server.

#### Example:

Imagine you’re building an e-commerce application. The client (a mobile app) displays product details to the user, while the server (a .NET Web API) manages the product database and processes order

### **Uniform Interface**

1. **Resource-Based** : Everything in a RESTful API is a resource, such as users, products, or orders. Each resource is identified by a unique URI (Uniform Resource Identifier).

* Example: `/api/users` represents a collection of users.


1. **HTTP Methods** : RESTful APIs use standard HTTP methods to perform actions on resources:

* **GET** : Retrieve a resource or list of resources.
* **POST** : Create a new resource.
* **PUT** : Update an existing resource (replace it entirely).
* **PATCH** : Partially update a resource.
* **DELETE** : Delete a resource.


### **Cacheable**

RESTful APIs support caching to improve performance. The server can indicate whether a response is cacheable and for how long.

#### Example:

If a client requests a list of products, the server can include a `Cache-Control` header in the response to specify how long the client can cache the data.

#### Benefits of Caching:

* **Improved Performance** : Caching reduces the number of requests to the server, improving response times.
* **Reduced Server Load** : Caching reduces the load on the server, making it more scalable


## **Use Plural Nouns for Resource URIs**

When designing RESTful APIs, resource names should be plural to indicate collections of entities. This keeps the API consistent and aligns with REST principles.

#### Example:

* **Good:** `/api/users`
* **Bad:** `/api/user`

Even when dealing with a single entity, the plural form remains intuitive:

* **Fetching all users:** `GET /api/users`
* **Fetching a single user:** `GET /api/users/{id}`

This approach improves clarity and maintains consistency across endpoint


## **Use Nouns for Resource URIs**

URIs should represent resources, not actions. Use nouns instead of verbs.

#### Example:

* Good: `/api/users`
* Bad: `/api/getUsers`


## **Use Nesting on Endpoints to Show Relationships**

When resources have a hierarchical relationship, use nested routes to reflect their structure. This improves clarity and makes the API more intuitive.

Nesting endpoints helps represent hierarchical relationships — like `/api/users/1/orders` — making the API structure more logical and readable

#### Example:

* **Good:** `/api/users/{userId}/orders` (Get all orders for a user)
* **Bad:** `/api/orders?userId={userId}`

For specific entities:

* **Good:** `/api/users/{userId}/orders/{orderId}` (Get a specific order for a user)
* **Bad:** `/api/orders/{orderId}`

Use nesting only when the child resource is strongly dependent on the parent.



## **Use Caching to Improve API Performance**

Caching helps reduce server load and response time by storing frequently accessed data. Implementing proper caching strategies can significantly enhance API performance



**Server-Side Caching*** Store frequently accessed responses in memory (e.g., Redis, in-memory caching).

* Ideal for reducing repeated database queries.


#### **1. GET (Retrieve a Resource)**

* Used to fetch data from the server.
* Should be  **idempotent** , meaning multiple requests should return the same result without modifying data.
* Example:
  GET /api/users       → Retrieves a list of users
  GET /api/users/1     → Retrieves details of a specific user (ID = 1)


#### ** POST (Create a New Resource)**

* Used to create a new resource on the server.
* **Not idempotent** – if you send the same request multiple times, multiple resources will be created.
* Returns `201 Created` on success, along with a `Location` header pointing to the new resource.


Example:
POST /api/users
Body: { "name": "John Doe" }

Response:
201 Created
Location: /api/users/3


#### **3. PUT (Update an Existing Resource - Full Replacement)**

* Used to update an **entire resource** by replacing it with new data.
* **Idempotent** – sending the same request multiple times should result in the same state on the server.
* If the resource does not exist, some implementations create a new resource (`Upsert` behavior).


#### **4. PATCH (Partial Update - Modify Specific Fields)**

* Used to update **only specific fields** of a resource, instead of replacing the entire object.
* **Not necessarily idempotent** – depending on the implementation, sending the same request multiple times could have different effects.


#### **5. DELETE (Remove a Resource)**

* Used to delete a resource from the server.
* Should be **idempotent** – if the resource is already deleted, subsequent DELETE requests should return `204 No Content` or `404 Not Found`.


## **Use HTTP Status Codes**

Return the appropriate HTTP status code to indicate the result of the request:

* `200 OK`: Success.
* `201 Created`: Resource created successfully.
* `400 Bad Request`: Invalid input.
* `404 Not Found`: Resource not found.
* `500 Internal Server Error`: Server error.

#### Example:
