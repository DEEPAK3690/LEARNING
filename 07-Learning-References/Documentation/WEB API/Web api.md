**Web API**

Web API is a framework for building **HTTP-based services** that can be consumed by  **browsers, mobile apps, or other servers** .

A **Web API (Web Application Programming Interface)** is a **bridge** that enables two different software applications to communicate with each other over the internet. Instead of directly connecting databases or systems, APIs define **Rules, Protocols, and Data Formats** that applications can use to send requests and receive responses.

* To expose data and functionality over the web using HTTP.
* Ideal for RESTful applications.

HTTP (Hypertext Transfer Protocol) is the backbone of data communication on the web. It defines the rules for how clients (like browsers or apps) send requests to servers and how servers respond with data.

Client → sends HTTP request

Server → returns HTTP response

**HTTPS (HyperText Transfer Protocol Secure)**

is the  **secure version of HTTP** , which uses **SSL/TLS encryption** to protect the communication between the client and the server.

So all data — login info, tokens, or JSON payloads — is  **encrypted before transmission** .

HTTP request

An **HTTP request** is a message sent from a **client** to a  **server** , asking it to perform a specific operation (GET data, POST new data, etc.).

| Component             | Description                                                                                          |
| --------------------- | ---------------------------------------------------------------------------------------------------- |
| 1️⃣ Request Line    | Defines the**HTTP method** , the **URL (resource path)** , and the**HTTP version** |
| 2️⃣ Headers         | Additional info like content type, authorization, language, etc.                                     |
| 3️⃣ Body (Optional) | Data sent to the server (mainly in POST/PUT requests)                                                |

If you send the above POST request, Web API will:

* Read JSON from the **request body**
* Deserialize it into a `Product` object
* Return a response (200 OK)

HTTP response

An **HTTP response** is a message sent by the **server** back to the **client** after processing the request.

| Part                  | Example                             | Meaning                              |
| --------------------- | ----------------------------------- | ------------------------------------ |
| **Status Line** | `HTTP/1.1 201 Created`            | HTTP version + status code + message |
| **Headers**     | `Content-Type: application/json`  | Response format                      |
| **Body**        | `{ "id": 101, "name": "Laptop" }` | JSON data returned to the client     |

| Code            | Meaning                       | When Used                      |
| --------------- | ----------------------------- | ------------------------------ |
| 200 OK          | Success                       | Successful GET/POST            |
| 201 Created     | Resource Created              | After successful POST          |
| 204 No Content  | Success, but no data returned | Successful DELETE              |
| 400 Bad Request | Invalid input                 | Missing fields or invalid data |

Swagger is also a client API Tool, and using Swagger, we can also test the Web APIs.

* **ControllerBase** → Provides features needed for **APIs** (No views, just JSON/XML responses).
* **Controller** → Used in **MVC applications** where you need both API and Razor Views (it inherits from ControllerBase and adds view-related features like View(), PartialView(), etc.).

**Routing in Web API**

Routing decides  **which URL maps to which controller and action** .

Without routing, your API wouldn’t know which method to execute for a given HTTP request.

Every time you define endpoints in your API — so clients can reach specific resources

| Type             | Description                          | Example                          |
| ---------------- | ------------------------------------ | -------------------------------- |
| Convention-based | Defined in `WebApiConfig.cs`       | `/api/{controller}/{id}`       |
| Attribute-based  | Defined using `[Route()]`attribute | `[Route("api/products/{id}")]` |

[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public string GetById(int id) => $"Product ID: {id}";
}

🔹 URL: `/api/products/10`

🔹 Output: `Product ID: 10`

Models and DTOs

A **Model** defines the structure of your data (like a database table).

A **DTO (Data Transfer Object)** is used to control what data is sent to the client.

* **Without DTO:** Send entire model → might leak internal data.
* **With DTO:** Send only safe, required fields → secure & optimized.

### **What:**

**API Versioning** in ASP.NET Core is a method to manage **multiple versions** of your Web API so that existing clients can continue using older endpoints while newer clients use updated ones.

It allows you to evolve your API  **without breaking compatibility** .

---

### **Why:**

API versioning is important because:

* Applications evolve — endpoints may change, parameters may be added, or responses may differ.
* Older clients should still function while newer clients use enhanced versions.
* It allows  **safe iteration** ,  **backward compatibility** , and  **smooth migration** .

---

### **When:**

Use API versioning when:

* Your API is used by external systems or multiple frontend versions.
* You plan to modify contracts (models, endpoints, or behaviors) in the future.
* You need a structured approach to maintain  **v1** ,  **v2** , etc., simultaneously.
