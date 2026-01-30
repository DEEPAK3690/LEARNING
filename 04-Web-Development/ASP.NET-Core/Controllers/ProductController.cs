using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Models;
using System;

namespace MyWebApplication.Controllers
{
    //As per RESTful Service, each resource should have a Unique Identifier or URI.

    //Attribute-Based Routing:
    [Route("api/[controller]/[action]")] //Sets the base route; [controller] is replaced with the controller name(product), making the URL /api/product.
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1000.00m, Category = "Electronics" },
            new Product { Id = 2, Name = "Desktop", Price = 2000.00m, Category = "Electronics" },
            new Product { Id = 3, Name = "Mobile", Price = 300.00m, Category = "Electronics" },
        };

        [HttpGet] // ActionResult: returns both data and HTTP status
        public ActionResult<IEnumerable<Product>> GetProducts()//action method
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]//with action result we can return http status codes and messages
        public ActionResult<Product> GetProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." });
            }
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]

        //Returns a meaningful object
        public ActionResult<Product> PostProduct([FromBody] Product product)
        {
            // Basic ID assignment logic (for demonstration)
            product.Id = _products.Max(p => p.Id) + 1;
            _products.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);

            //CreatedAtAction:
            //Sets the status code to 201.
            //Adds a Location header pointing to where the newly created resource can be retrieved.
            //Returns the created object in the response body.
        }

        [HttpPut("{id}")]

        //Only returns status codes
        public IActionResult PutProduct(int id, [FromBody] Product updatedProduct)
        {
            if (id != updatedProduct.Id)
            {
                return BadRequest(new { Message = "ID mismatch between route and body." });
            }
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." });
            }
            // Update the product properties
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Category = updatedProduct.Category;
            // In a real application, persist changes to the database here
            return NoContent();
            //return Ok(_products);

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." });
            }
            _products.Remove(product);
            // In a real application, remove the product from the database here
            return NoContent();
        }


        //[HttpGet("{id}")]

        //public Product GetProducts(int id)
        //{
        //    var product = _products.FirstOrDefault(p => p.Id == id);
        //    if (product == null)
        //    {
        //        //return NotFound(new { Message = $"Product with ID {id} not found." });
        //    }
        //    return product;
        //    //return Ok(product);
        //}
    }
}
