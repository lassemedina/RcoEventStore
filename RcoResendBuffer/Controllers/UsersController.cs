using Microsoft.AspNetCore.Mvc;
using RcoResendBuffer.Context;
using RcoResendBuffer.Models;
using RcoResendBuffer.Service;

namespace RcoResendBuffer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthContext _authContext;
        private readonly QueueWorkerService _queueWorkerService;

        public UsersController(AuthContext authContext, QueueWorkerService queueWorkerService)
        {
            _authContext = authContext;
            _queueWorkerService = queueWorkerService;
        }

        public IActionResult Get()
        {
            var user = new User { Age = 31, Email = "la@rco.se", FirstName = "Leo", LastName = "King" };
            _queueWorkerService.AddMessage(user, "users");
            _authContext.Users.Add(user);
            var i = _authContext.SaveChanges();
            if (i > 0)
                _queueWorkerService.Submit();
            return Ok( _authContext.Users); 
        }
    }
}