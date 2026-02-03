using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Models;
using System.Reflection;

namespace MyWebApplication.Controllers
{
    /// <summary>
    /// Employee Controller with AUTHORIZATION
    /// 
    /// AUTHORIZATION RULES (What can each role do?):
    /// 
    /// ADMIN - Full Access
    /// - Can CREATE new employees
    /// - Can READ all employees
    /// - Can UPDATE any employee
    /// - Can DELETE any employee
    /// 
    /// MANAGER - Limited Modifications
    /// - Can READ all employees
    /// - Can UPDATE employees
    /// - CANNOT create or delete
    /// 
    /// EMPLOYEE - Read Only
    /// - Can READ all employees
    /// - CANNOT create, update, or delete
    /// 
    /// UNAUTHENTICATED - No Access
    /// - Cannot access any endpoint
    /// - Gets 401 Unauthorized
    /// 
    /// NOTE: All endpoints require [Authorize] attribute
    /// This means you must be logged in (have valid JWT token)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ALL endpoints in this controller require authentication
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET /api/employee
        /// 
        /// AUTHORIZATION: Any authenticated user (Admin, Manager, Employee)
        /// DESCRIPTION: Get all employees
        /// 
        /// CURL EXAMPLE:
        /// curl -X GET "https://localhost:5001/api/employee" \
        ///   -H "Authorization: Bearer {JWT_TOKEN}"
        /// 
        /// RESPONSES:
        /// - 200 OK: List of employees
        /// - 401 Unauthorized: No valid token provided
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin, Manager, Employee")] // All authenticated users
        public ActionResult<IEnumerable<Employee>> GetAllEmployees()
        {
            var employees = _repository.GetAll();
            return Ok(employees);
        }

        /// <summary>
        /// GET /api/employee/{id}
        /// 
        /// AUTHORIZATION: Any authenticated user
        /// DESCRIPTION: Get a specific employee by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public ActionResult<Employee> GetEmployeeById(int id)
        {
            var employee = _repository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        /// <summary>
        /// POST /api/employee
        /// 
        /// AUTHORIZATION: ADMIN ONLY
        /// DESCRIPTION: Create a new employee
        /// 
        /// WHY ADMIN ONLY?
        /// - Creating employee records is sensitive
        /// - Should only be done by authorized personnel
        /// - Maintains data integrity
        /// 
        /// CURL EXAMPLE:
        /// curl -X POST "https://localhost:5001/api/employee" \
        ///   -H "Authorization: Bearer {JWT_TOKEN}" \
        ///   -H "Content-Type: application/json" \
        ///   -d '{"name":"John Smith","position":"Developer","age":30,"email":"john@example.com"}'
        /// 
        /// RESPONSES:
        /// - 201 Created: Employee created successfully
        /// - 400 Bad Request: Invalid data
        /// - 401 Unauthorized: Not logged in
        /// - 403 Forbidden: User is not an admin
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // ADMIN ONLY
        public ActionResult<Employee> CreateEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Add(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// PUT /api/employee/{id}
        /// 
        /// AUTHORIZATION: ADMIN or MANAGER
        /// DESCRIPTION: Full update of an employee
        /// 
        /// WHY ADMIN/MANAGER?
        /// - Updating employee info should be controlled
        /// - Managers need to update their team
        /// - Only admins can do everything
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager")] // ADMIN or MANAGER
        public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest("Employee ID mismatch.");
            }
            if (!_repository.Exists(id))
            {
                return NotFound();
            }
            _repository.Update(employee);
            return NoContent();
        }

        /// <summary>
        /// PATCH /api/employee/{id}
        /// 
        /// AUTHORIZATION: ADMIN or MANAGER
        /// DESCRIPTION: Partial update of an employee
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult PatchEmployee(int id, [FromBody] Employee employee)
        {
            var existingEmployee = _repository.GetById(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }
            existingEmployee.Name = employee.Name ?? existingEmployee.Name;
            existingEmployee.Position = employee.Position ?? existingEmployee.Position;
            existingEmployee.Age = employee.Age != 0 ? employee.Age : existingEmployee.Age;
            existingEmployee.Email = employee.Email ?? existingEmployee.Email;
            _repository.Update(existingEmployee);
            return NoContent();
        }

        /// <summary>
        /// DELETE /api/employee/{id}
        /// 
        /// AUTHORIZATION: ADMIN ONLY
        /// DESCRIPTION: Delete an employee
        /// 
        /// WHY ADMIN ONLY?
        /// - Deleting is a dangerous operation
        /// - Should only be done by top-level administrators
        /// - Prevents accidental data loss
        /// 
        /// CURL EXAMPLE:
        /// curl -X DELETE "https://localhost:5001/api/employee/1" \
        ///   -H "Authorization: Bearer {JWT_TOKEN}"
        /// 
        /// RESPONSES:
        /// - 204 No Content: Employee deleted successfully
        /// - 404 Not Found: Employee not found
        /// - 401 Unauthorized: Not logged in
        /// - 403 Forbidden: User is not an admin
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // ADMIN ONLY
        public IActionResult DeleteEmployee(int id)
        {
            if (!_repository.Exists(id))
            {
                return NotFound();
            }
            _repository.Delete(id);
            return NoContent();
        }

        /// <summary>
        /// GET /api/employee/search
        /// 
        /// AUTHORIZATION: Any authenticated user
        /// DESCRIPTION: Search employees by position or age
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public ActionResult<IEnumerable<Employee>> SearchEmployees([FromQuery] string? position, [FromQuery] int? age)
        {
            var employees = _repository.GetAll();
            var filteredEmployees = employees.ToList();
            
            if (!string.IsNullOrEmpty(position))
            {
                filteredEmployees = filteredEmployees.Where(e => e.Position.Equals(position, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (age.HasValue)
            {
                filteredEmployees = filteredEmployees.Where(e => e.Age == age).ToList();
            }

            return Ok(filteredEmployees);
        }
    }
}

