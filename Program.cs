using HoneyRaesAPI.Models;

List<HoneyRaesAPI.Models.Customer> customers = new List<HoneyRaesAPI.Models.Customer> 
{ 
    new Customer() { Id =  0, Name = "Nicholas Davidson", Address = ""},
    new Customer() { Id = 1, Name = "Jonathan Pouch", Address = ""},
    new Customer() { Id = 2, Name = "T Pain", Address = ""},
    new Customer() { Id = 3, Name = "Francis Pickle", Address = ""}
};
List<HoneyRaesAPI.Models.Employee> employees = new List<HoneyRaesAPI.Models.Employee> 
{
    new Employee() { Id = 0, Name = "Samantha Overall"},
    new Employee() { Id = 1, Name = "Bowser Man"},
    new Employee() { Id = 2, Name = "Mr. Mario"},
    new Employee() { Id = 3, Name = "Mr. Luigi"},
    new Employee() { Id = 4, Name = "Charles Spurges"}
};
List<HoneyRaesAPI.Models.ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket> 
{
    new ServiceTicket() { Id = 0, CustomerId = 0, EmployeeId = 4, Emergency = true, DateCompleted = DateTime.Now },
    new ServiceTicket() { CustomerId = 1, EmployeeId = 1, Emergency = true, DateCompleted = DateTime.Now },
    new ServiceTicket() { Id = 1, CustomerId = 2, EmployeeId = 0, Emergency = false, DateCompleted = DateTime.Now },
    new ServiceTicket() { Id = 2, CustomerId = 2, EmployeeId = 1, Emergency = true, DateCompleted = null },
    new ServiceTicket() { Id = 3, CustomerId = 2, EmployeeId = 3, Emergency = true, DateCompleted = DateTime.Now },
    new ServiceTicket() { Id = 4, CustomerId = 3, Emergency = false, DateCompleted = null },
    new ServiceTicket() { Id = 5, CustomerId = 1, Emergency = false, DateCompleted = null },
    new ServiceTicket() { Id = 6, CustomerId = 0, Emergency = false, DateCompleted = null }
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

app.MapGet("/serviceTickets", () =>
{
    return serviceTickets;
});

app.MapGet("/serviceTickets/{id}", (int id) =>
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


app.MapPost("/serviceTickets", (ServiceTicket serviceTicket) =>
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

app.MapPut("/serviceTickets/{id}", (int id, ServiceTicket serviceTicket) =>
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

app.MapPost("/serviceTickets/{id}/Complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});





// Last Assignment

// 1. Emergencies
app.MapGet("/serviceTickets/incomplete/emergencies", () =>
{
    var incompleteEmergencies = serviceTickets.Where(st => !st.DateCompleted.HasValue && st.Emergency).ToList();

    return Results.Ok(incompleteEmergencies);
});


// 2. Unassigned
app.MapGet("/serviceTickets/Unassigned", () =>
{
    var unassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();

    return Results.Ok(unassignedTickets);
});


// 3. Inactive Customers
app.MapGet("/customers/notClosedForAYear", () =>
{
    DateTime oneYearAgo = DateTime.Now.AddYears(-1);

    var customersWithoutClosedService = customers
        .Where(customer =>
        {
            var lastClosedServiceDate = serviceTickets
                .Where(st => st.CustomerId == customer.Id && st.DateCompleted.HasValue)
                .Select(st => st.DateCompleted.Value)
                .DefaultIfEmpty(DateTime.MinValue)
                .Max();

            return lastClosedServiceDate < oneYearAgo;
        })
        .ToList();

    return Results.Ok(customersWithoutClosedService);
});


// 4. Available Employees
app.MapGet("/employees/Unassigned", () =>
{
    var unassignedEmployees = employees
        .Where(employee =>
        {
            var assignedServiceTickets = serviceTickets
                .Where(st => st.EmployeeId == employee.Id && !st.DateCompleted.HasValue)
                .ToList();

            return assignedServiceTickets.Count == 0;
        })
        .ToList();

    return Results.Ok(unassignedEmployees);
});


// 5. Employee's Customers
app.MapGet("/employees/{employeeId}/Customers", (int employeeId) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == employeeId);

    if (employee == null)
    {
        return Results.NotFound();
    }

    var customerIdsForEMployee = serviceTickets
    .Where(st => st.EmployeeId == employeeId)
    .Select(st => st.CustomerId)
    .Distinct() 
    .ToList();

    var customersForEmployee = customers
    .Where(customer => customerIdsForEMployee.Contains(customer.Id))
    .ToList();

    return Results.Ok(customersForEmployee);
});


// 6. Employee of the Month
/*app.MapGet("", () =>
{

});


// 7. Past Ticket Review
app.MapGet("", () =>
{

});


// 8. Prioritized Tickets (Challenge)
app.MapGet("", () =>
{

});*/



app.Run();


