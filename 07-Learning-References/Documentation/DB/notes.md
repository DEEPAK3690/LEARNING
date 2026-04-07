1. Why business rules are in Application/Services/OrderService.cs, not Controllers/OrdersController.cs

* Controllers should handle transport concerns:
  * HTTP request/response
  * status code shape
  * route/model binding
* Services should handle business decisions:
  * “Is this order valid for our domain?”
  * “Can this product be ordered?”
  * “Are duplicate product lines allowed?”

If business rules are put in controllers:

* Rule duplication appears across endpoints.
* Testing becomes harder because rules are tied to HTTP context.
* Reuse breaks when another entry point is added later (background job, message consumer, gRPC, etc.).
