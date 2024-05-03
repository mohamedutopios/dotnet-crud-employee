using MySql.EntityFrameworkCore.Extensions;
using WebApplication1.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFrameworkMySQL()
                .AddDbContext<DemoContext>(options =>
                {
                    options.UseMySQL("server=demo-mohamed-cesi.mysql.database.azure.com;user=user;password=MOHmoh1780@;database=demo");
                });


var app = builder.Build();


// to get all employees
app.MapGet("/employees", async (DemoContext dbContext) => {
    var employees = await dbContext.Employees.ToListAsync();
    return employees;

});

// to get employees by ID
app.MapGet("/employees/{id}", async (int id, DemoContext dbContext) =>
await dbContext.Employees.FindAsync(id)
    is Employee employee
        ? Results.Ok(employee)
        : Results.NotFound()
    );


//to update the data of existing employee by ID
app.MapPut("/employees/{id}", async (int id, Employee employee, DemoContext dbContext) =>
{
    var employees = await dbContext.Employees.FindAsync(id);

    if (employees is null) return Results.NotFound();

    employees.Id = employee.Id;
    employees.Name = employee.Name;
    employees.Position = employee.Position;
    employees.Department = employee.Department;

    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});



//to add new employeee
app.MapPost("/employees", async (Employee employee, DemoContext dbContext) => {

    dbContext.Employees.Add(employee);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/employees/{employee.Id}", employee);
});


// to delete existing employee by ID
app.MapDelete("/employees/{id}", async (int id, DemoContext dbContext) =>
{
    if (await dbContext.Employees.FindAsync(id) is Employee employee)
    {
        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync();
        return Results.Ok(employee);
    }
    return Results.NotFound();
});

app.Run();
