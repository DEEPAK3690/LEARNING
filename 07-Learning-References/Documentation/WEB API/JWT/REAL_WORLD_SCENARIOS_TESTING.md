# ?? Real-World Scenarios Testing Guide

## Complete End-to-End Examples

This guide shows real-world scenarios for understanding Authentication and Authorization.

---

## ?? Scenario 1: Admin Creating and Deleting Employees

### Context
An admin needs to:
1. Log in to the system
2. Create a new employee
3. Delete that employee
4. Only they should be able to do this

### Step-by-Step Implementation

#### Step 1: Admin Logs In (AUTHENTICATION)

**What happens?**
- Admin enters their credentials
- System verifies username = "admin" and password = "Admin@123"
- System creates JWT token containing admin's role

**Request:**
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123"
  }'
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsInJvbGUiOiJBZG1pbiIsImlhdCI6MTUxNjIzOTAyMn0...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@example.com",
    "role": "Admin"
  }
}
```

**What was verified?**
- ? Username exists in database
- ? Password matches using bcrypt
- ? User account is active

**Token contains:**
- User ID: 1
- Username: admin
- Email: admin@example.com
- **Role: Admin** ? This is crucial for authorization!

---

#### Step 2: Admin Creates a New Employee (AUTHORIZATION - Admin Only)

**What happens?**
1. Admin sends token with request
2. System validates JWT signature (not tampered)
3. System reads role from token: "Admin"
4. System checks if endpoint requires role: `[Authorize(Roles = "Admin")]`
5. Role matches ? Request allowed ?

**Request:**
```bash
curl -X POST "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sarah Johnson",
    "position": "Senior Developer",
    "age": 30,
    "email": "sarah@example.com"
  }'
```

**Response (Success):**
```
Status: 201 Created
Location: /api/employee/3
```

**What was authorized?**
- ? User is authenticated (has valid token)
- ? User's role is "Admin"
- ? POST /api/employee allows "Admin" role
- ? Request allowed

---

#### Step 3: Admin Deletes the Employee (AUTHORIZATION - Admin Only)

**What happens?**
1. Admin sends token with request to delete employee
2. System validates JWT
3. Reads role: "Admin"
4. Checks: `[Authorize(Roles = "Admin")]` on DELETE endpoint
5. Role matches ? Request allowed ?

**Request:**
```bash
curl -X DELETE "https://localhost:5001/api/employee/3" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Response (Success):**
```
Status: 204 No Content
```

**Employee deleted successfully!**

---

## ?? Scenario 2: Employee Trying to Delete (Authorization Failure)

### Context
An employee (low privilege) tries to delete an employee record.
This should fail because only Admins can delete.

#### Step 1: Employee Logs In

**Request:**
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "employee",
    "password": "Employee@123"
  }'
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzIiwibmFtZSI6ImVtcGxveWVlIiwiZW1haWwiOiJlbXBsb3llZUBleGFtcGxlLmNvbSIsInJvbGUiOiJFbXBsb3llZSIsImlhdCI6MTUxNjIzOTAyMn0...",
  "user": {
    "id": 3,
    "username": "employee",
    "email": "employee@example.com",
    "role": "Employee"  ? Note: Employee role, not Admin!
  }
}
```

#### Step 2: Employee Tries to Delete (Will Fail)

**Request:**
```bash
curl -X DELETE "https://localhost:5001/api/employee/1" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Response (Failure):**
```
Status: 403 Forbidden

Response Body:
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "Access Denied. You must be in role: Admin"
}
```

**Why was it rejected?**
1. ? Token is valid (JWT signature verified)
2. ? User is authenticated
3. ? User's role is "Employee"
4. ? Endpoint requires role "Admin"
5. ? Role mismatch ? 403 Forbidden

---

## ?? Scenario 3: Request Without Authentication Token

### Context
Someone (hacker/attacker) tries to access the Employee API without logging in.
No token = No authentication = Should be rejected.

**Request (No Authorization header):**
```bash
curl -X GET "https://localhost:5001/api/employee"
```

**Response (Failure):**
```
Status: 401 Unauthorized

Response Header:
WWW-Authenticate: Bearer realm="MyWebApplication"
Bearer error="invalid_token", error_description="The token is missing"

Response Body:
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header is missing or invalid"
}
```

**What happened?**
1. ? No Authorization header provided
2. ? No JWT token to validate
3. ? User is not authenticated
4. ? Request rejected immediately

---

## ? Scenario 4: Manager Can Update But Not Delete

### Context
A manager logs in and tries two operations:
1. Update employee (should work)
2. Delete employee (should fail)

#### Step 1: Manager Logs In

**Request:**
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "manager",
    "password": "Manager@123"
  }'
```

**Response:**
```json
{
  "user": {
    "role": "Manager"  ? Manager role
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Step 2: Manager Updates Employee (Allowed)

**Endpoint:** `PUT /api/employee/{id}`  
**Authorization:** `[Authorize(Roles = "Admin, Manager")]` ? Manager is included!

**Request:**
```bash
curl -X PUT "https://localhost:5001/api/employee/1" \
  -H "Authorization: Bearer {MANAGER_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "Updated Name",
    "position": "New Position",
    "age": 35,
    "email": "updated@example.com"
  }'
```

**Response (Success):**
```
Status: 204 No Content
```

**Why was it allowed?**
- ? Token valid
- ? Role is "Manager"
- ? Endpoint requires "Admin, Manager"
- ? Manager is in the list ? Allowed

#### Step 3: Manager Tries to Delete (Denied)

**Endpoint:** `DELETE /api/employee/{id}`  
**Authorization:** `[Authorize(Roles = "Admin")]` ? Manager NOT included!

**Request:**
```bash
curl -X DELETE "https://localhost:5001/api/employee/1" \
  -H "Authorization: Bearer {MANAGER_TOKEN}"
```

**Response (Failure):**
```
Status: 403 Forbidden
```

**Why was it denied?**
- ? Token valid
- ? Role is "Manager"
- ? Endpoint only allows "Admin"
- ? Manager not in the list ? Denied

---

## ?? Scenario 5: Token Expiration

### Context
A token becomes invalid after 60 minutes.

#### Step 1: User Gets Token at 2:00 PM
Token expires at 3:00 PM

#### Step 2: User Makes Request at 2:30 PM
```bash
curl -X GET "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer {TOKEN}"
```

**Response:**
```
Status: 200 OK
```
? Token still valid

#### Step 3: User Makes Request at 3:05 PM
```bash
curl -X GET "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer {TOKEN}"
```

**Response:**
```
Status: 401 Unauthorized

{
  "detail": "The token has expired"
}
```

? Token expired ? Need to login again

**Solution:** User must call `/api/auth/login` again to get a new token

---

## ?? Scenario 6: Invalid/Tampered Token

### Context
An attacker tries to modify the token to claim they're an admin.

#### Original Token Payload:
```json
{
  "sub": "3",
  "name": "employee",
  "role": "Employee"  ? Actually an employee
}
```

#### Attacker Modifies It:
```json
{
  "sub": "3",
  "name": "employee",
  "role": "Admin"  ? Changed to admin!
}
```

#### Attacker's Request:
```bash
curl -X DELETE "https://localhost:5001/api/employee/1" \
  -H "Authorization: Bearer {MODIFIED_TOKEN}"
```

**Response (Failure):**
```
Status: 401 Unauthorized

{
  "detail": "Invalid signature. Token has been tampered with."
}
```

**Why did it fail?**
1. Attacker modified the payload
2. JWT signature was based on original payload
3. Signature no longer matches
4. Token is invalid
5. Request rejected

**This is why JWT is secure!** Tampering is detectable.

---

## ?? Authorization Matrix

| Operation | URL | Method | Admin | Manager | Employee | Unauthenticated |
|-----------|-----|--------|-------|---------|----------|-----------------|
| View Employees | /api/employee | GET | ? | ? | ? | ? |
| Search Employees | /api/employee/search | GET | ? | ? | ? | ? |
| Create Employee | /api/employee | POST | ? | ? | ? | ? |
| Update Employee | /api/employee/{id} | PUT | ? | ? | ? | ? |
| Patch Employee | /api/employee/{id} | PATCH | ? | ? | ? | ? |
| Delete Employee | /api/employee/{id} | DELETE | ? | ? | ? | ? |

---

## ????? Real Business Logic Examples

### Example 1: HR System
**Admin:** Can hire/fire employees (create/delete)  
**Manager:** Can update employee info (salary, position)  
**Employee:** Can view employee directory (read-only)

**Code:**
```csharp
[HttpPost("employee")]
[Authorize(Roles = "Admin")] // Only HR admins can hire
public ActionResult CreateEmployee(...) { ... }

[HttpPut("employee/{id}")]
[Authorize(Roles = "Admin,Manager")] // Managers can modify
public ActionResult UpdateEmployee(...) { ... }

[HttpGet("employee")]
[Authorize(Roles = "Admin,Manager,Employee")] // Everyone can view
public ActionResult GetEmployees() { ... }
```

### Example 2: Financial System
**Admin:** Can create/delete/update transactions  
**Manager:** Can approve large transactions  
**Employee:** Can only view their own transactions

**Code:**
```csharp
[HttpPost("transaction")]
[Authorize(Roles = "Admin")] // Only admins can create
public ActionResult CreateTransaction(...) { ... }

[HttpPost("transaction/{id}/approve")]
[Authorize(Roles = "Admin,Manager")] // Managers approve
public ActionResult ApproveTransaction(...) { ... }

[HttpGet("transaction")]
[Authorize] // Any authenticated user sees their own
public ActionResult GetTransactions() { ... }
```

---

## ?? Key Takeaways

### Authentication (WHO) ?
- Happens at `/api/auth/login`
- Validates username + password
- Returns JWT token if successful
- **Error: 401 Unauthorized** (no token or invalid token)

### Authorization (WHAT) ?
- Happens on every protected endpoint
- Checks JWT token is valid
- Extracts role from token
- Compares role with endpoint requirement
- **Error: 403 Forbidden** (authenticated but no permission)

### Flow ??
```
1. Login ? Authenticate (username/password)
   ?
2. Receive JWT token with role
   ?
3. Send token with every request
   ?
4. System validates token signature
   ?
5. System reads role from token
   ?
6. System checks if role is allowed
   ?
7. Allow or deny request
```

---

## ?? Quick Test Checklist

- [ ] Login as admin ? Get token
- [ ] Use admin token to CREATE employee ? 201
- [ ] Login as employee ? Get token
- [ ] Use employee token to CREATE employee ? 403
- [ ] Use employee token to GET employees ? 200
- [ ] Use employee token to DELETE employee ? 403
- [ ] No token to GET employees ? 401
- [ ] Tampered token ? 401
- [ ] Expired token ? 401
- [ ] Manager UPDATE employee ? 200
- [ ] Manager DELETE employee ? 403

---

Generated: Real-World Scenarios Testing Guide  
Last Updated: 2024
