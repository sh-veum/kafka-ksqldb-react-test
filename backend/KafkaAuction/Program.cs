using KafkaAuction.Constants;
using KafkaAuction.Data;
using KafkaAuction.Http;
using KafkaAuction.Initializers;
using KafkaAuction.Middleware;
using KafkaAuction.Models;
using KafkaAuction.Services;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Config;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.Query.Context.Options;
using ksqlDB.RestApi.Client.KSql.Query.Options;
using ksqlDB.RestApi.Client.KSql.RestApi;
using ksqlDB.RestApi.Client.KSql.RestApi.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

// Add authorization
builder.Services.AddAuthorizationBuilder();

// Configure DbContext
var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseNpgsql(defaultConnectionString));

builder.Services.AddIdentityCore<UserModel>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<MainDbContext>()
    .AddApiEndpoints();

// Setup ksqlDB context
var ksqlDbUrl = configuration.GetValue<string>("KSqlDb:Url");

var contextOptions = new KSqlDbContextOptionsBuilder()
    .UseKSqlDb(ksqlDbUrl!)
    .SetBasicAuthCredentials("veum", "letmein")
    .SetJsonSerializerOptions(jsonOptions =>
    {
        jsonOptions.IgnoreReadOnlyFields = true;
    })
    //.SetAutoOffsetReset(AutoOffsetReset.Earliest) // global setting
    .SetProcessingGuarantee(ProcessingGuarantee.ExactlyOnce) // global setting
    .SetIdentifierEscaping(IdentifierEscaping.Keywords)
    .SetEndpointType(EndpointType.QueryStream) // uses HTTP/2.0
                                               //.SetEndpointType(EndpointType.Query) // uses HTTP/1.0
    .SetupPushQuery(options =>
    {
        options.Properties[KSqlDbConfigs.KsqlQueryPushV2Enabled] = "true";
    })
    .SetupPullQuery(options =>
    {
        options[KSqlDbConfigs.KsqlQueryPullTableScanEnabled] = "true";
    })
    .Options;

contextOptions.DisposeHttpClient = false;

builder.Services.AddSingleton(new KSqlDBContext(contextOptions));

// Register IHttpClientFactory
builder.Services.AddHttpClient();

var httpClientFactory = new HttpClientFactory(new Uri(ksqlDbUrl!));


builder.Services.AddSingleton<IKSqlDbRestApiClient>(new KSqlDbRestApiClient(httpClientFactory));

// AuctionService
var restApiProvider = new KSqlDbRestApiProvider(httpClientFactory, builder.Configuration)
{
    DisposeHttpClient = false
};

builder.Services.AddScoped<IAuctionService, AuctionService>(
    sp => new AuctionService(
        sp.GetRequiredService<ILogger<AuctionService>>(),
        restApiProvider, configuration)
    );

builder.Services.AddScoped<IKsqlDbService, KsqlDbService>(
    sp => new KsqlDbService(
        sp.GetRequiredService<ILogger<KsqlDbService>>(),
        restApiProvider)
    );

builder.Services.AddScoped(typeof(EntityInserter<>));
builder.Services.AddScoped(typeof(TableCreator<>));
builder.Services.AddScoped(typeof(StreamCreator<>));

// WebSocket services
// builder.Services.AddSingleton<IKSqlDbRestApiProvider, KSqlDbRestApiProvider>();
builder.Services.AddSingleton<IAuctionWebSocketService, AuctionWebSocketService>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();

// CORS policy with the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigin",
                      policy =>
                      {
                          var frontendUrl = configuration["FrontendUrl"];
                          if (!string.IsNullOrEmpty(frontendUrl))
                          {
                              policy.WithOrigins(frontendUrl)
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          }
                      });
    options.AddPolicy(name: "AllowAnyOrigin",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

app.MapIdentityApi<UserModel>();

app.UseCors("AllowSpecificOrigin");
// app.UseCors("AllowAnyOrigin"); 

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Automatic migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<MainDbContext>();

    dbContext.Database.Migrate();

    // Make sure the roles and admin user is created
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<UserModel>>();

        await DbInitializer.SeedRoles(roleManager);

        // Admin user
        var adminEmail = configuration["DefaultUsers:Admin:Email"];
        var adminPassword = configuration["DefaultUsers:Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException("Admin email and password must be set in the configuration.");
        }

        await DbInitializer.EnsureUser(userManager, roleManager, adminEmail, adminPassword, RoleConstants.AdminRole);

        // Test user
        var userEmail = configuration["DefaultUsers:User:Email"];
        var userPassword = configuration["DefaultUsers:User:Password"];

        if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(userPassword))
        {
            throw new InvalidOperationException("User email and password must be set in the configuration.");
        }

        await DbInitializer.EnsureUser(userManager, roleManager, userEmail, userPassword, RoleConstants.UserRole);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
