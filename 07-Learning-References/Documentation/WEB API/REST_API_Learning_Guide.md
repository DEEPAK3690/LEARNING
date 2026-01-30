# 🌐 RESTful API Learning Guide for .NET Developers

## 🔹 What is REST?

**REST (Representational State Transfer)** is an architectural style for designing networked applications that communicate using **stateless** HTTP requests.

### 💡 Key Idea:
Each client request to the server **must include all necessary information** for processing, as the server does not store session state between requests.

### ✅ Why REST?
- Simplicity (uses HTTP methods)
- Scalability (stateless)
- Flexibility (supports JSON, XML, etc.)
- Separation of concerns (client ↔ server independence)

### 🧩 Real-Time Example:
In an **e-commerce app**, your frontend (React, Angular, etc.) requests product data via REST APIs hosted in your **ASP.NET Web API** backend.

**Example Flow:**
```http
GET /api/products
```
Response (JSON):
```json
[
  { "id": 1, "name": "Laptop", "price": 65000 },
  { "id": 2, "name": "Mouse", "price": 1200 }
]
```
... (content truncated for brevity) ...
