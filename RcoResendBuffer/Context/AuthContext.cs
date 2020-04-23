using Microsoft.Extensions.Logging;
using RcoResendBuffer.Models;
using System.Data.Entity;
using System.Threading;


namespace RcoResendBuffer.Context
{
    public class AuthContext : DbContext
    {
        private readonly ILogger<AuthContext> _logger;

        public AuthContext(ILogger<AuthContext> logger)
        {
            Database.Connection.ConnectionString = "data source=(local)\\SQL2016;initial catalog=auth;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            Database.SetInitializer(new CreateDatabaseIfNotExists<AuthContext>());
            _logger = logger;
        }
        public virtual DbSet<AuthSyncMessage> AuthSyncMessages { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void Dispose(bool disposing)
        {
            _logger.LogInformation($"Disposing {Thread.CurrentThread.ManagedThreadId}");
            base.Dispose(disposing);
        }


    }
    public class DevConfig : CreateDatabaseIfNotExists<AuthContext>
    {
        protected override void Seed(AuthContext context)
        {
            context.Users.Add(new User
            {
                FirstName = "Lasse",
                LastName = "Medina",
                Age = 41,
                Email = "lars.medina@rco.se"
            });
            base.Seed(context);
        }
    }
}
