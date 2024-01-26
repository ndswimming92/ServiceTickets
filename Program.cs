using HoneyRaesAPI.Models;

List<HoneyRaesAPI.Models.Customer> customers = new List<HoneyRaesAPI.Models.Customer> 
{ 
    new Customer() { Id =  0, Name = "Nicholas Davidson"},
    new Customer() { Id = 1, Name = "Jonathan Pouch"},
    new Customer() { Id = 2, Name = "Francis Pickle"}
};
List<HoneyRaesAPI.Models.Employee> employees = new List<HoneyRaesAPI.Models.Employee> 
{
    new Employee() { Id = 0, Name = "Samantha Overall"},
    new Employee() { Id = 1, Name = "Charles Spurges"}
};
List<HoneyRaesAPI.Models.ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket> 
{
    new ServiceTicket() { Id = 0, CustomerId = 0, EmployeeId = 0, DateCompleted = DateTime.Now },
    new ServiceTicket() { CustomerId = 1, EmployeeId = 1, DateCompleted = DateTime.Now },
    new ServiceTicket() { Id = 2, CustomerId = 2, EmployeeId = 0, DateCompleted = DateTime.Now },
    new ServiceTicket() { Id = 1, CustomerId = 2, EmployeeId = 1 },

};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    return serviceTickets.FirstOrDefault(st => st.Id == id);
});

app.Run();
