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
    new ServiceTicket() { Id = 1, CustomerId = 2, EmployeeId = 1, DateCompleted = null }
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
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});


app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/deleteServiceTicket/{id}", (int id) =>
{
    var ticketToDelete = serviceTickets.FirstOrDefault(ticket => ticket.Id == id);
    if (ticketToDelete != null)
    {
        serviceTickets.Remove(ticketToDelete);
        // Return the updated list of service tickets
        return Results.Ok(new
        {
            Message = $"Deleted: {id}.",
            ServiceTickets = serviceTickets
        });
    }
    else
    {
        return Results.NotFound();
    }
});

// Updating a service ticket.

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    //the id in the request route doesn't match the id from the ticket in the request body. That's a bad request!
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

//Custom endpoint

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});





// Last Assignment

// 1. Emergencies
app.MapGet("/serviceTickets/incomplete/emergencies", () =>
{

    return Results.Ok(serviceTickets);
});


// 2. Unassigned
app.MapGet("", () =>
{

});


// 3. Inactive Customers
app.MapGet("", () =>
{

});


// 4. Available Employees
app.MapGet("", () =>
{

});


// 5. Employee's Customers
app.MapGet("", () =>
{

});


// 6. Employee of the Month
app.MapGet("", () =>
{

});


// 7. Past Ticket Review
app.MapGet("", () =>
{

});


// 8. Prioritized Tickets (Challenge)
app.MapGet("", () =>
{

});



app.Run();


