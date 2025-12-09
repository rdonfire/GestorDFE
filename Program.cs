using GestorDFe.Components;
using GestorDFe.Data;
using GestorDFe.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// ✅ REGISTRAR O DbContext ANTES de builder.Build()
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=DEV-AMORIM\\SERVRJ;Database=GestorDFe;User Id=sa;Password=inter#system;Encrypt=False;"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IDocumentoFiscalService, DocumentoFiscalService>();

builder.Services.AddScoped<ISefazService, SefazService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDanfeService, DanfeService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/download/{chave}", async (string chave, AppDbContext db) =>
{
    var doc = await db.DocumentosFiscais.FirstOrDefaultAsync(d => d.ChaveAcesso == chave);
    if (doc == null || string.IsNullOrEmpty(doc.XmlConteudo))
        return Results.NotFound();

    var bytes = System.Text.Encoding.UTF8.GetBytes(doc.XmlConteudo);
    var fileName = $"{doc.ChaveAcesso}.xml";

    return Results.File(bytes, "application/xml", fileName);
});

app.Run();



