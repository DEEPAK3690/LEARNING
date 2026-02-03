# ?? Quick Start: Authentication & Authorization

## 5-Minute Setup to Understand Everything

---

## ?? What You Just Learned

Your project now has:
- ? **User Registration** - Create new accounts
- ? **User Login** - Authenticate with credentials  
- ? **JWT Tokens** - Secure stateless authentication
- ? **Role-Based Authorization** - Control who can do what
- ? **3 User Roles** - Admin, Manager, Employee

---

## ?? The Big Picture

```
???????????????????????????????????????????????????????????????
?                    YOUR APPLICATION                          ?
???????????????????????????????????????????????????????????????
?                                                               ?
?  PUBLIC ENDPOINTS (No login needed)                          ?
?  ?? POST /api/auth/login ? Get JWT token                    ?
?  ?? POST /api/auth/register ? Create account                ?
?                                                               ?
?  PROTECTED ENDPOINTS (Login required)                        ?
?  ?? GET /api/employee ? Any authenticated user              ?
?  ?? POST /api/employee ? Admin only                         ?
?  ?? PUT /api/employee/{id} ? Admin or Manager               ?
?  ?? DELETE /api/employee/{id} ? Admin only                  ?
?                                                               ?
???????????????????????????????????????????????????????????????
```

---

## ?? Test It Right Now

### 1?? Login (Get JWT Token)

```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'
```

**Save the token from response**

### 2?? Use Token to Get Employees

```bash
curl -X GET "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 3?? Try to Create Employee (as Admin - Should Work)

```bash
curl -X POST "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"John Doe",
    "position":"Developer",
    "age":30,
    "email":"john@example.com"
  }'
```

### 4?? Login as Employee and Try Same (Should Fail)

```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"employee","password":"Employee@123"}'
```

Then try POST with employee token:

```bash
curl -X POST "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer YOUR_EMPLOYEE_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{...}'
```

**Result:** 403 Forbidden ?

---

## ?? Test Users

| Username | Password | Role | Permissions |
|----------|----------|------|------------|
| `admin` | `Admin@123` | Admin | Everything ? |
| `manager` | `Manager@123` | Manager | View & Update ? |
| `employee` | `Employee@123` | Employee | View only ? |

---

## ?? Three Concepts Explained Simply

### 1. **Authentication** = "Who are you?"

```csharp
// POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Server checks:** 
- Does user exist? ?
- Is password correct? ?
- Send token! ?

### 2. **JWT Token** = "Proof you are who you say you are"

```
Token contains:
- User ID: 1
- Username: admin
- Email: admin@example.com
- Role: Admin  ? KEY PART!
- Created: 2:00 PM
- Expires: 3:00 PM
- Signature: VALID_ONLY_IF_NOT_TAMPERED
```

### 3. **Authorization** = "What can you do?"

```csharp
[Authorize(Roles = "Admin")]  // Only Admin can do this
public ActionResult DeleteEmployee(int id) { ... }
```

**Server checks:**
- Is token valid? ?
- Is user authenticated? ?
- Does user's role match required role? ? or ?

---

## ?? New Files Added

| File | Purpose |
|------|---------|
| `Models/User.cs` | User data model |
| `Services/JwtTokenService.cs` | Create & validate tokens |
| `Services/UserRepository.cs` | Store users (in-memory for demo) |
| `Services/AuthenticationService.cs` | Verify credentials |
| `Controllers/AuthController.cs` | Login & register endpoints |
| `Controllers/EmployeeController.cs` | Updated with authorization |
| `Program.cs` | JWT setup and middleware config |

---

## ??? How to Add New Role

### Step 1: Add Role to Authorization on Endpoint

```csharp
[HttpPost("report")]
[Authorize(Roles = "Admin,Manager,Accountant")]  // Add new role
public ActionResult CreateReport(...) { ... }
```

### Step 2: Create User with That Role

In `UserRepository.cs`:

```csharp
new User {
    Id = 4,
    Username = "accountant",
    Email = "accountant@example.com",
    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Accountant@123"),
    Role = "Accountant",  // New role
    IsActive = true
}
```

### Step 3: Test It!

```bash
# Login as accountant
curl -X POST "https://localhost:5001/api/auth/login" \
  -d '{"username":"accountant","password":"Accountant@123"}'

# Use token to access endpoint requiring that role
curl -X POST "https://localhost:5001/api/report" \
  -H "Authorization: Bearer {TOKEN}"
```

---

## ?? Understanding the Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| **200** | OK | Successfully retrieved data |
| **201** | Created | Successfully created resource |
| **204** | No Content | Successfully deleted/updated |
| **400** | Bad Request | Invalid JSON or data |
| **401** | Unauthorized | No token or invalid token |
| **403** | Forbidden | Authenticated but no permission |
| **404** | Not Found | Resource doesn't exist |
| **500** | Server Error | Server crash/error |

---

## ?? Debugging Tips

### Problem: Getting 401 (Unauthorized)

**Solutions:**
1. Make sure you're sending token: `Authorization: Bearer {token}`
2. Token might be expired - get a new one
3. Token might be invalid - copy-paste carefully

### Problem: Getting 403 (Forbidden)

**Solutions:**
1. User is authenticated ?
2. But user's role doesn't match endpoint requirement
3. Try logging in as different user (e.g., admin instead of employee)

### Problem: Getting 400 (Bad Request)

**Solutions:**
1. Check JSON format is correct
2. All required fields are present
3. No extra commas or typos

---

## ?? Common Workflows

### Workflow 1: Admin Managing Employees

```
1. Admin logs in ? POST /api/auth/login
2. System returns token with role="Admin"
3. Admin creates employee ? POST /api/employee + token
4. Admin updates employee ? PUT /api/employee/1 + token
5. Admin deletes employee ? DELETE /api/employee/1 + token
6. All requests allowed ?
```

### Workflow 2: Manager Working with Team

```
1. Manager logs in ? POST /api/auth/login
2. System returns token with role="Manager"
3. Manager views all employees ? GET /api/employee + token ?
4. Manager updates employee ? PUT /api/employee/1 + token ?
5. Manager tries to delete employee ? DELETE /api/employee/1 + token ?
6. Gets 403 Forbidden - not allowed
```

### Workflow 3: Employee Viewing Data

```
1. Employee logs in ? POST /api/auth/login
2. System returns token with role="Employee"
3. Employee views employees ? GET /api/employee + token ?
4. Employee tries to create ? POST /api/employee + token ?
5. Gets 403 Forbidden - read-only access
```

---

## ?? Security Notes

? **We're doing right:**
- Passwords hashed with bcrypt
- JWT signature prevents tampering
- Tokens expire after 60 minutes
- Role-based access control

?? **In production, also:**
- Use HTTPS everywhere
- Store secrets in environment variables
- Use Azure Key Vault for secrets
- Implement refresh tokens
- Log failed login attempts
- Rate limit login endpoints
- Use strong secret keys (32+ characters)

---

## ?? Complete Request/Response Examples

### Example 1: Successful Login

**Request:**
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIn0...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@example.com",
    "role": "Admin"
  }
}
```

### Example 2: Unauthorized Access

**Request:**
```
GET /api/employee
(No Authorization header)
```

**Response (401 Unauthorized):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header missing"
}
```

### Example 3: Forbidden Access

**Request:**
```
DELETE /api/employee/1
Authorization: Bearer {EMPLOYEE_TOKEN}
```

**Response (403 Forbidden):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "Access denied. User must be in role: Admin"
}
```

---

## ?? Learning Checklist

- [ ] I understand the difference between Authentication and Authorization
- [ ] I can login and get a JWT token
- [ ] I understand JWT structure (header.payload.signature)
- [ ] I can use token to access protected endpoints
- [ ] I understand role-based authorization
- [ ] I know what happens with different roles
- [ ] I understand 401 vs 403 status codes
- [ ] I can add new roles to the system
- [ ] I understand why JWT is secure (signature verification)
- [ ] I can test all three user roles

---

## ?? Key Takeaway

**Authentication + Authorization = Security**

- **Authentication** proves you are who you claim to be (login)
- **Authorization** checks if you're allowed to do something (roles)
- **JWT** is the "proof card" you carry around after logging in
- **Roles** determine what you can do with that "proof card"

---

Generated: Quick Start Guide  
Last Updated: 2024
