# ?? Authentication & Authorization Implementation Guide

## ?? Quick Summary

**Authentication** = WHO are you? (Verifying identity)  
**Authorization** = WHAT can you do? (Checking permissions)

```
????????????????????????????????????????????????????
? User sends username & password                   ?
????????????????????????????????????????????????????
         ?
         ?
????????????????????????????????????????????????????
? AUTHENTICATION (AuthController.Login)            ?
? - Verify credentials                             ?
? - Create JWT token with user info & role         ?
????????????????????????????????????????????????????
         ?
         ?
????????????????????????????????????????????????????
? User receives JWT token                          ?
? Stores it in local storage / cookie              ?
????????????????????????????????????????????????????
         ?
         ?
????????????????????????????????????????????????????
? User sends JWT with each request                 ?
? Header: Authorization: Bearer {token}            ?
????????????????????????????????????????????????????
         ?
         ?
????????????????????????????????????????????????????
? AUTHORIZATION (UseAuthentication + UseAuthorization)
? - Verify JWT signature                           ?
? - Extract user role from token                   ?
? - Check if user has required role                ?
? - Allow or deny access                           ?
????????????????????????????????????????????????????
```

---

## ?? Testing Authentication & Authorization

### Step 1: Register a User (Optional)

```bash
curl -X POST "https://localhost:5001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test@123"
  }'
```

### Step 2: Login and Get JWT Token

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

**Save the token!** You'll use it for the next requests.

### Step 3: Use Token to Access Protected Endpoint

#### ? GET /api/employee (Should Work - Any Authenticated User)

```bash
curl -X GET "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer {PASTE_YOUR_TOKEN_HERE}"
```

**Response:** 200 OK - List of employees

#### ? POST /api/employee (Admin Only)

```bash
curl -X POST "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer {ADMIN_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Employee",
    "position": "Developer",
    "age": 28,
    "email": "new@example.com"
  }'
```

**Response:** 201 Created

#### ? POST /api/employee (Employee Cannot Do This)

```bash
curl -X POST "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer {EMPLOYEE_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Employee",
    "position": "Developer",
    "age": 28,
    "email": "new@example.com"
  }'
```

**Response:** 403 Forbidden - "Access denied"

#### ? Without Token (No Authorization)

```bash
curl -X GET "https://localhost:5001/api/employee"
```

**Response:** 401 Unauthorized - "Authorization header missing"

---

## ?? Predefined Users for Testing

Three users are pre-created in `UserRepository.cs`:

| Username | Password | Role | Email |
|----------|----------|------|-------|
| admin | Admin@123 | Admin | admin@example.com |
| manager | Manager@123 | Manager | manager@example.com |
| employee | Employee@123 | Employee | employee@example.com |

---

## ?? Authorization Rules by Role

### Admin Role ? Full Access
```csharp
[Authorize(Roles = "Admin")]
```
- ? View all employees
- ? Create new employees
- ? Update any employee
- ? Delete any employee

**Example:** Admin can delete an employee
```bash
curl -X DELETE "https://localhost:5001/api/employee/1" \
  -H "Authorization: Bearer {ADMIN_TOKEN}"
```

### Manager Role ?? Limited Access
```csharp
[Authorize(Roles = "Admin, Manager")]
```
- ? View all employees
- ? Update employees
- ? Cannot create employees
- ? Cannot delete employees

**Example:** Manager can update but not delete
```bash
# ? This works
curl -X PUT "https://localhost:5001/api/employee/1" \
  -H "Authorization: Bearer {MANAGER_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Updated Name",...}'

# ? This fails with 403 Forbidden
curl -X DELETE "https://localhost:5001/api/employee/1" \
  -H "Authorization: Bearer {MANAGER_TOKEN}"
```

### Employee Role ?? Read Only
```csharp
[Authorize(Roles = "Admin, Manager, Employee")]
```
- ? View all employees
- ? Search employees
- ? Cannot create employees
- ? Cannot update employees
- ? Cannot delete employees

**Example:** Employee can view but not modify
```bash
# ? This works
curl -X GET "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer {EMPLOYEE_TOKEN}"

# ? This fails with 403 Forbidden
curl -X POST "https://localhost:5001/api/employee" \
  -H "Authorization: Bearer {EMPLOYEE_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{...}'
```

---

## ?? New Files Created

### 1. **Models/User.cs**
- `User` class - Represents a user in the system
- `LoginRequest` - Request model for login
- `LoginResponse` - Response after successful login
- `UserInfo` - Safe user info to send to clients

### 2. **Services/JwtTokenService.cs**
- Generates JWT tokens for authenticated users
- Validates JWT tokens
- Extracts claims from tokens

**Key Methods:**
- `GenerateToken(User)` - Creates JWT
- `GetPrincipalFromToken(string)` - Validates and extracts claims

### 3. **Services/UserRepository.cs**
- Stores and retrieves user data
- Pre-populated with 3 test users (Admin, Manager, Employee)
- In production: Replace with actual database

**Key Methods:**
- `GetByUsername(string)` - Finds user by username
- `GetById(int)` - Finds user by ID
- `AddUser(User)` - Creates new user
- `UserExists(string)` - Checks if user exists

### 4. **Services/AuthenticationService.cs**
- Authenticates users (verifies username/password)
- Handles password hashing with bcrypt
- Validates passwords

**Key Methods:**
- `AuthenticateUser(username, password)` - Verifies credentials
- `ValidatePassword(plainPassword, hash)` - Checks password
- `HashPassword(password)` - Hashes password for storage

### 5. **Controllers/AuthController.cs**
- Handles user login and registration
- Generates JWT tokens
- Public endpoints (no authentication required)

**Endpoints:**
- `POST /api/auth/login` - Login with username/password
- `POST /api/auth/register` - Create new user
- `GET /api/auth/validate` - Validate JWT token

### 6. **Controllers/EmployeeController.cs** (Updated)
- Added `[Authorize]` attributes to all endpoints
- Role-based authorization for different operations

**Authorization Rules:**
- `GET /api/employee` ? All authenticated users
- `POST /api/employee` ? Admin only
- `PUT /api/employee/{id}` ? Admin or Manager
- `DELETE /api/employee/{id}` ? Admin only

---

## ?? How JWT Works in Detail

### JWT Structure

A JWT token looks like this:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxIiwibmFtZSI6ImpvaG4iLCJpYXQiOjE1MTYyMzkyMjB9.
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

It has 3 parts separated by dots:

**1. Header** (Algorithm)
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**2. Payload** (User Claims/Data)
```json
{
  "sub": "1",
  "name": "admin",
  "email": "admin@example.com",
  "role": "Admin",
  "iat": 1516239022,
  "exp": 1516325422
}
```

**3. Signature** (Verification)
```
HMACSHA256(
  base64UrlEncode(header) + "." +
  base64UrlEncode(payload),
  secret_key
)
```

### How Validation Works

1. **Server receives token** from client header: `Authorization: Bearer {token}`
2. **Server extracts signature** from token
3. **Server recreates signature** using same secret key
4. **If signatures match** ? Token is authentic (not tampered)
5. **Server reads payload** to get user info and role
6. **Server checks if user has required role** for the endpoint

---

## ?? Program.cs Configuration

### JWT Authentication Setup

```csharp
// 1. Configure JWT Bearer authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(...),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
    };
});

// 2. Register DI services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
```

### Middleware Order

```csharp
app.UseHttpsRedirection();
app.UseAuthentication();  // ? Must be before UseAuthorization!
app.UseAuthorization();   // ? Must be after UseAuthentication!
app.MapControllers();
```

**?? Important:** Order matters! If you reverse the order, authorization won't work.

---

## ?? Real-World Scenarios Explained

### Scenario 1: Employee Viewing Employee List
```
1. Employee logs in with username/password
   POST /api/auth/login
   
2. Receives JWT token with role="Employee"
   
3. Sends GET request with token
   GET /api/employee
   Header: Authorization: Bearer {token}
   
4. Server validates token and reads role
   
5. Checks if "Employee" is in allowed roles ["Admin","Manager","Employee"]
   
6. ? Access granted ? Returns employee list
```

### Scenario 2: Employee Trying to Delete Employee
```
1. Employee sends DELETE request with token
   DELETE /api/employee/1
   Header: Authorization: Bearer {token}
   
2. Server validates token and reads role="Employee"
   
3. Checks if "Employee" is in allowed roles for DELETE ["Admin"]
   
4. ? Access denied ? Returns 403 Forbidden
   Response: "Access Denied. You must be in role: Admin"
```

### Scenario 3: No Token Provided
```
1. User sends request WITHOUT Authorization header
   GET /api/employee
   
2. Server sees [Authorize] attribute
   
3. No token found in header
   
4. ? Throws authentication challenge
   Response: 401 Unauthorized
   Header: WWW-Authenticate: Bearer
```

---

## ?? Security Best Practices Implemented

| Practice | What We Did | Why |
|----------|-----------|-----|
| **Password Hashing** | Use bcrypt | Never store plain passwords |
| **JWT Signature** | HMAC-SHA256 | Verify token wasn't modified |
| **Token Expiration** | 60 minutes | Reduces damage if token leaked |
| **Clock Skew = 0** | No time tolerance | Strict expiration checking |
| **HTTPS Only** | Required in production | Encrypt token in transit |
| **Secret Key** | Read from config | Don't hardcode secrets |

### To Use in Production

1. **Never hardcode secrets:**
```csharp
// ? WRONG
var secret = "my-secret-key";

// ? RIGHT
var secret = configuration["Jwt:SecretKey"];
// Store in: appsettings.json, environment variables, or Azure Key Vault
```

2. **Use HTTPS everywhere**
3. **Use strong secret keys (min 32 characters)**
4. **Rotate secrets periodically**
5. **Monitor failed login attempts**
6. **Use refresh tokens for long sessions**

---

## ?? HTTP Status Codes

| Status | Meaning | Example |
|--------|---------|---------|
| **200** | OK | Successfully retrieved data |
| **201** | Created | Employee successfully created |
| **204** | No Content | Successfully deleted |
| **400** | Bad Request | Invalid JSON in request |
| **401** | Unauthorized | Missing or invalid token |
| **403** | Forbidden | User doesn't have required role |
| **404** | Not Found | Employee ID doesn't exist |
| **500** | Server Error | Unexpected server error |

---

## ?? Testing Checklist

- [ ] Login as admin ? Should get token
- [ ] Login as manager ? Should get token
- [ ] Login as employee ? Should get token
- [ ] Login with wrong password ? Should fail
- [ ] GET /api/employee with valid token ? Should work
- [ ] GET /api/employee without token ? Should return 401
- [ ] POST /api/employee as admin ? Should work (201)
- [ ] POST /api/employee as manager ? Should fail (403)
- [ ] POST /api/employee as employee ? Should fail (403)
- [ ] DELETE /api/employee as admin ? Should work (204)
- [ ] DELETE /api/employee as manager ? Should fail (403)
- [ ] PUT /api/employee as manager ? Should work (204)
- [ ] Expired token ? Should return 401

---

## ?? Key Concepts

### Authentication
- **What it is**: Proving you are who you claim to be
- **Method**: Username/Password verification
- **Result**: JWT token if successful
- **When**: Happens at login

### Authorization
- **What it is**: Checking if you have permission for an action
- **Method**: Role-based access control (RBAC)
- **Result**: 200 OK or 403 Forbidden
- **When**: Happens with every request

### JWT Token
- **What it is**: Digitally signed credential containing user info
- **Why**: Stateless (no server session needed), scalable, mobile-friendly
- **Format**: header.payload.signature
- **Lifetime**: Expires after set time (60 minutes here)

### Roles
- **What they are**: Group of permissions assigned to users
- **Examples**: Admin, Manager, Employee
- **How they work**: Specified in [Authorize(Roles = "...")] attribute

---

## ?? Troubleshooting

### Issue: Getting 401 Unauthorized on every request

**Possible causes:**
1. Token not included in request header
2. Token has expired
3. Token signature is invalid
4. Secret key doesn't match between token generation and validation

**Fix:**
- Get new token from /api/auth/login
- Check that Authorization header is correctly formatted
- Ensure secret key in configuration matches

### Issue: Getting 403 Forbidden instead of 401

**Possible cause:** Token is valid, but user's role doesn't match required role

**Fix:**
- Use a user with higher privilege (e.g., admin instead of employee)
- Check [Authorize(Roles = "...")] attribute matches user's role

### Issue: /api/auth/login returns 400 Bad Request

**Possible causes:**
1. Username or password is null/empty
2. JSON format is incorrect

**Fix:**
```bash
# Correct format:
{
  "username": "admin",
  "password": "Admin@123"
}
```

---

## ?? Summary Table

| Component | Purpose | When Used |
|-----------|---------|-----------|
| **AuthController** | Login/Register | User authentication |
| **JwtTokenService** | Generate/Validate tokens | Token management |
| **AuthenticationService** | Verify credentials | Login process |
| **UserRepository** | Store/Retrieve users | User data access |
| **[Authorize]** | Check authentication | Every protected endpoint |
| **[Authorize(Roles)]** | Check authorization | Role-based access control |
| **UseAuthentication()** | Middleware to validate token | Request pipeline |
| **UseAuthorization()** | Middleware to check roles | Request pipeline |

---

Generated: Authentication & Authorization Guide  
Last Updated: 2024
