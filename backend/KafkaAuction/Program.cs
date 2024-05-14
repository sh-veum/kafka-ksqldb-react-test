using System.Text.Json.Serialization;
using Confluent.Kafka;
using KafkaAuction.Http;
using KafkaAuction.Services;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Config;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.Query.Context.Options;
using ksqlDB.RestApi.Client.KSql.Query.Options;
using ksqlDB.RestApi.Client.KSql.RestApi;
using ksqlDB.RestApi.Client.KSql.RestApi.Enums;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Setup ksqlDB context
var ksqlDbUrl = builder.Configuration.GetValue<string>("KSqlDb:Url");

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
        options[KSqlDbConfigs.KsqlQueryPullTableScanEnabled] = "false";
    })
    .Options;

contextOptions.DisposeHttpClient = false;

builder.Services.AddSingleton(new KSqlDBContext(contextOptions));

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
        restApiProvider)
    );

builder.Services.AddScoped<IKsqlDbService, KsqlDbService>(
    sp => new KsqlDbService(
        sp.GetRequiredService<ILogger<KsqlDbService>>(),
        restApiProvider)
    );

builder.Services.AddScoped(typeof(EntityInserter<>));
builder.Services.AddScoped(typeof(TableCreator<>));
builder.Services.AddScoped(typeof(StreamCreator<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
