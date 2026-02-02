using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Models.DIExamples;

namespace MyWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DependencyInjectionController : ControllerBase
    {
        private readonly ITransientService _transientService;
        private readonly IScopedService _scopedService;
        private readonly ISingletonService _singletonService;

        // Constructor injection - Services are injected here
        public DependencyInjectionController(
            ITransientService transientService,
            IScopedService scopedService,
            ISingletonService singletonService)
        {
            _transientService = transientService;
            _scopedService = scopedService;
            _singletonService = singletonService;
        }

        /// <summary>
        /// GET api/dependencyinjection/lifetimes
        /// Demonstrates DI lifetimes by showing instance IDs
        /// </summary>
        [HttpGet("lifetimes")]
        public IActionResult GetDILifetimes()
        {
            return Ok(new
            {
                transient = new
                {
                    id = _transientService.GetId(),
                    lifetime = _transientService.GetLifetime(),
                    explanation = "New instance created each time. Call this endpoint twice and you'll get different IDs."
                },
                scoped = new
                {
                    id = _scopedService.GetId(),
                    lifetime = _scopedService.GetLifetime(),
                    explanation = "Same instance within a single request. Inject twice in same controller and you'll get same ID."
                },
                singleton = new
                {
                    id = _singletonService.GetId(),
                    lifetime = _singletonService.GetLifetime(),
                    explanation = "Same instance across entire application. Call this endpoint multiple times and you'll always get same ID."
                }
            });
        }

        /// <summary>
        /// GET api/dependencyinjection/info
        /// Returns detailed information about each lifetime
        /// </summary>
        [HttpGet("info")]
        public IActionResult GetDIInfo()
        {
            return Ok(new[]
            {
                new
                {
                    type = "Transient",
                    registration = "builder.Services.AddTransient<IInterface, Implementation>()",
                    lifetime = "New instance created every time it's requested",
                    useCase = "Lightweight, stateless services (logging, utilities)",
                    performance = "Lower memory usage, higher object creation overhead"
                },
                new
                {
                    type = "Scoped",
                    registration = "builder.Services.AddScoped<IInterface, Implementation>()",
                    lifetime = "One instance per HTTP request",
                    useCase = "Database contexts, unit of work patterns",
                    performance = "Balanced - reused within request, created per request"
                },
                new
                {
                    type = "Singleton",
                    registration = "builder.Services.AddSingleton<IInterface, Implementation>()",
                    lifetime = "One instance for entire application lifetime",
                    useCase = "Configuration, caching, shared state",
                    performance = "Highest performance, but thread-safe code required"
                }
            });
        }

        /// <summary>
        /// GET api/dependencyinjection/demo-multiple-instances
        /// Demonstrates creating multiple instances of transient service
        /// </summary>
        [HttpGet("demo-multiple-instances")]
        public IActionResult DemoMultipleInstances(
            [FromServices] ITransientService transient1,
            [FromServices] ITransientService transient2,
            [FromServices] ITransientService transient3)
        {
            return Ok(new
            {
                message = "Each injected transient service gets a new instance",
                instances = new[]
                {
                    transient1.GetId(),
                    transient2.GetId(),
                    transient3.GetId()
                },
                areAllDifferent = !transient1.GetId().Equals(transient2.GetId()) &&
                                 !transient2.GetId().Equals(transient3.GetId())
            });
        }
    }
}
