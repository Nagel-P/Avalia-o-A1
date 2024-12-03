using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar o banco de dados
builder.Services.AddDbContext<AppDbContext>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minimal API com MySQL",
        Version = "v1",
        Description = "Documentação das APIs para Individuos e Sequencias",
        Contact = new OpenApiContact
        {
            Name = "Seu Nome",
            Email = "seu.email@dominio.com",
            Url = new Uri("https://github.com/seu-github")
        }
    });
});

var app = builder.Build();

// Ativar CORS
app.UseCors("PermitirTudo");

// Usar Swagger no app
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API v1");
        c.RoutePrefix = string.Empty; // Faz com que a UI do Swagger fique na raiz
    });
}

// Endpoints para Individuo
app.MapGet("/individuos", async (AppDbContext db) => await db.Individuos.ToListAsync());

app.MapGet("/individuos/{id:int}", async (int id, AppDbContext db) =>
{
    var individuo = await db.Individuos.FindAsync(id);
    return individuo is not null ? Results.Ok(individuo) : Results.NotFound();
});

app.MapPost("/individuos", async (Individuo individuo, AppDbContext db) =>
{
    await db.Individuos.AddAsync(individuo);
    await db.SaveChangesAsync();
    return Results.Created($"/individuos/{individuo.Id}", individuo);
});

app.MapPut("/individuos/{id:int}", async (int id, Individuo updatedIndividuo, AppDbContext db) =>
{
    var individuo = await db.Individuos.FindAsync(id);
    if (individuo is null) return Results.NotFound();

    individuo.Codigo = updatedIndividuo.Codigo;
    individuo.Nome = updatedIndividuo.Nome;

    await db.SaveChangesAsync();
    return Results.Ok(individuo);
});

app.MapDelete("/individuos/{id:int}", async (int id, AppDbContext db) =>
{
    var individuo = await db.Individuos.FindAsync(id);
    if (individuo is null) return Results.NotFound();

    db.Individuos.Remove(individuo);
    await db.SaveChangesAsync();
    return Results.Ok("Excluído(a) com sucesso!");
});

// Endpoints para Sequencia
app.MapGet("/sequencias", async (AppDbContext db) =>
    await db.Sequencias.Include(s => s.Individuo).ToListAsync());

app.MapGet("/sequencias/{id:int}", async (int id, AppDbContext db) =>
{
    var sequencia = await db.Sequencias.Include(s => s.Individuo).FirstOrDefaultAsync(s => s.Id == id);
    return sequencia is not null ? Results.Ok(sequencia) : Results.NotFound();
});

app.MapPost("/sequencias", async (Sequencia sequencia, AppDbContext db) =>
{
    if (sequencia.Individuo != null && sequencia.Individuo.Id > 0)
    {
        // Busca o Indivíduo no banco para associar à sequência
        var individuoExistente = await db.Individuos.FindAsync(sequencia.Individuo.Id);
        if (individuoExistente == null)
        {
            return Results.BadRequest("Indivíduo não encontrado.");
        }
        sequencia.Individuo = individuoExistente;
    }

    await db.Sequencias.AddAsync(sequencia);
    await db.SaveChangesAsync();

    return Results.Created($"/sequencias/{sequencia.Id}", sequencia);
});

app.MapPut("/sequencias/{id}", async (int id, Sequencia sequencia, AppDbContext db) =>
{
    var sequenciaExistente = await db.Sequencias.Include(s => s.Individuo).FirstOrDefaultAsync(s => s.Id == id);
    if (sequenciaExistente == null)
    {
        return Results.NotFound("Sequência não encontrada.");
    }

    if (sequencia.Individuo != null && sequencia.Individuo.Id > 0)
    {
        // Busca o Indivíduo no banco
        var individuoExistente = await db.Individuos.FindAsync(sequencia.Individuo.Id);
        if (individuoExistente == null)
        {
            return Results.BadRequest("Indivíduo não encontrado.");
        }
        sequenciaExistente.Individuo = individuoExistente;
    }

    // Atualiza outros campos
    sequenciaExistente.SeqGenetica = sequencia.SeqGenetica;

    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.MapDelete("/sequencias/{id:int}", async (int id, AppDbContext db) =>
{
    var sequencia = await db.Sequencias.FindAsync(id);
    if (sequencia is null) return Results.NotFound();

    db.Sequencias.Remove(sequencia);
    await db.SaveChangesAsync();
    return Results.Ok("Excluído(a) com sucesso!");
});


// Chamar o método de seed na inicialização
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao popular o banco de dados: {ex.Message}");
    }
}

// Inicializar a API
app.Run();