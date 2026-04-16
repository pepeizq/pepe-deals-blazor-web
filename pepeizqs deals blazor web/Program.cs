using ApexCharts;
using Herramientas;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using pepeizqs_deals_blazor_web.Componentes;
using pepeizqs_deals_blazor_web.Componentes.Account;
using pepeizqs_deals_web.Data;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

ClasesDapper.Registrar();

var builder = WebApplication.CreateBuilder(args);

#region Compresion (Primero)

builder.Services.AddResponseCompression(opciones =>
{
	opciones.Providers.Add<BrotliCompressionProvider>();
	opciones.Providers.Add<GzipCompressionProvider>();
	opciones.EnableForHttps = true;
	opciones.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
				new[] { "application/octet-stream", "application/rss+xml", "text/html", "text/css", "image/png", "image/x-icon", "text/javascript" });
});
builder.Services.AddRequestDecompression();

builder.Services.Configure<GzipCompressionProviderOptions>(opciones =>
{
	opciones.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(opciones =>
{
	opciones.Level = CompressionLevel.Optimal;
});

#endregion

#region Optimizador

builder.Services.AddWebOptimizer(
	acciones => 
	{
		acciones.AddCssBundle("/css/bundle.css", new NUglify.Css.CssSettings
		{
			CommentMode = NUglify.Css.CssComment.None,
			OutputMode =  NUglify.OutputMode.SingleLine,
			IgnoreAllErrors = true,
			TermSemicolons = true,
			ColorNames = NUglify.Css.CssColor.Hex
		},
			"lib/bootstrap/dist/css/bootstrap-reboot.css",
			"lib/bootstrap/dist/css/bootstrap-grid.css",
			"lib/bootstrap/dist/css/bootstrap-utilities.css",
			"css/maestro.css",
			"css/cabecera_cuerpo_pie.css",
			"css/resto.css",
			"css/site.css"
		);

		acciones.AddJavaScriptBundle("/superjs.js",
			"lib/jquery/dist/jquery.min.js",
			"lib/bootstrap/dist/js/bootstrap.bundle.min.js"
		);

		acciones.MinifyJsFiles("inicio.js", "video.js", "push-notifications.js");
	});

#endregion

builder.Services.AddRazorComponents(opciones =>
{
	opciones.DetailedErrors = true;
}).AddInteractiveServerComponents(opciones =>
{
	opciones.DetailedErrors = true;
	opciones.DisconnectedCircuitMaxRetained = 120;
	opciones.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(2);
	opciones.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(60);
}).AddHubOptions(opciones =>
{
	opciones.EnableDetailedErrors = true;
	opciones.MaximumReceiveMessageSize = null;
	opciones.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
	opciones.HandshakeTimeout = TimeSpan.FromSeconds(30);
	opciones.KeepAliveInterval = TimeSpan.FromSeconds(20);
});

builder.Services.AddSignalR(options =>
{
	options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<UsuarioAcceso>();
builder.Services.AddScoped<IdentityRedirectManager>();
//builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(opciones =>
{
	opciones.DefaultScheme = IdentityConstants.ApplicationScheme;
	opciones.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();

var conexionTexto = builder.Configuration.GetConnectionString("pepeizqs_deals_webContextConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
Herramientas.BaseDatos.cadenaConexion = conexionTexto;

builder.Services.AddDbContextPool<pepeizqs_deals_webContext>(opciones => {
	opciones.UseSqlServer(conexionTexto, opciones2 =>
	{
		opciones2.CommandTimeout(30);
		opciones2.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), new List<int>() { 18, 19 });
	});
	opciones.EnableSensitiveDataLogging();
	opciones.EnableDetailedErrors();
});

builder.Services.ConfigureApplicationCookie(opciones =>
{
	opciones.AccessDeniedPath = "/";
	opciones.Cookie.Name = "cookiePepeizq";
	opciones.ExpireTimeSpan = TimeSpan.FromDays(30);
	opciones.LoginPath = "/account/login";
	opciones.LogoutPath = "/account/logout";
	opciones.SlidingExpiration = true;
	opciones.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddDataProtection().PersistKeysToDbContext<pepeizqs_deals_webContext>().SetDefaultKeyLifetime(TimeSpan.FromDays(30));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<Usuario>(opciones =>
{
	opciones.SignIn.RequireConfirmedAccount = false;
	opciones.Lockout.MaxFailedAccessAttempts = 15;
	opciones.Lockout.AllowedForNewUsers = true;
	opciones.User.RequireUniqueEmail = true;
})
	.AddEntityFrameworkStores<pepeizqs_deals_webContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<Usuario>, IdentityNoOpEmailSender>();

#region Tareas

builder.Services.AddScoped<Servicios.Moneda>();

builder.Services.Configure<HostOptions>(opciones =>
{
	opciones.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddSingleton<Tareas.VigiladorRAM>();
builder.Services.AddSingleton<Tareas.Comprobador>();
builder.Services.AddSingleton<Tareas.Minimos.Europa>();
builder.Services.AddSingleton<Tareas.LimpiezaLog>();
builder.Services.AddSingleton<Tareas.LimpiezaCircuits>();
builder.Services.AddSingleton<Tareas.Mantenimiento>();
builder.Services.AddSingleton<Tareas.Pings>();
builder.Services.AddSingleton<Tareas.CorreosEnviar>();
builder.Services.AddSingleton<Tareas.Patreon>();
builder.Services.AddSingleton<Tareas.JuegosActualizar>();
builder.Services.AddSingleton<Tareas.Duplicados>();
builder.Services.AddSingleton<Tareas.UsuariosActualizar>();
builder.Services.AddSingleton<Tareas.RedesSociales>();
builder.Services.AddSingleton<Tareas.IndexNow>();
builder.Services.AddSingleton<Tareas.SteamBundles>();
builder.Services.AddSingleton<Tareas.SteamDLCs>();
builder.Services.AddSingleton<Tareas.Minimos.EstadosUnidos>();

builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.VigiladorRAM>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Comprobador>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Minimos.Europa>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.LimpiezaLog>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.LimpiezaCircuits>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Mantenimiento>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Pings>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.CorreosEnviar>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Patreon>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.JuegosActualizar>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Duplicados>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.UsuariosActualizar>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.RedesSociales>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.IndexNow>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.SteamBundles>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.SteamDLCs>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Minimos.EstadosUnidos>());

#endregion

#region Decompilador

builder.Services.AddHttpClient<IDecompiladores, Decompiladores2>()
	.ConfigurePrimaryHttpMessageHandler(() =>
		new HttpClientHandler
		{
			AutomaticDecompression = System.Net.DecompressionMethods.GZip,
			MaxConnectionsPerServer = 50
		});

builder.Services.AddSingleton<IDecompiladores, Decompiladores2>();

#endregion

#region CORS necesario para extension navegador

builder.Services.AddCors(policy => {
	policy.AddPolicy("Extension", builder =>
		builder.WithOrigins("https://*:5001/").SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyOrigin()
	);
});

#endregion

#region Redireccionador

builder.Services.AddControllersWithViews().AddMvcOptions(opciones =>
	opciones.Filters.Add(
		new ResponseCacheAttribute
		{
			NoStore = true,
			Location = ResponseCacheLocation.None
		}));

#endregion

#region Linea Grafico

builder.Services.AddApexCharts(e =>
{
	e.GlobalOptions = new ApexChartBaseOptions
	{
		Debug = false,
		Theme = new ApexCharts.Theme
		{
			Palette = PaletteType.Palette2,
			Mode = Mode.Dark
		}
	};
});

#endregion

#region Mejora velocidad carga

builder.Services.AddHsts(opciones =>
{
	opciones.Preload = true;
	opciones.IncludeSubDomains = true;
	opciones.MaxAge = TimeSpan.FromDays(730);
});

#endregion

#region Tiempo Token Enlaces Correos 

builder.Services.Configure<DataProtectionTokenProviderOptions>(opciones => opciones.TokenLifespan = TimeSpan.FromHours(3));

#endregion

#region Acceder Usuario en Codigo y RSS

builder.Services.AddControllers(opciones =>
{
	opciones.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
}).AddJsonOptions(opciones2 =>
{
	opciones2.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
	opciones2.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

});
builder.Services.AddHttpContextAccessor();

#endregion

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

#region Notificaciones Push

builder.Services.AddScoped<NotificacionesPush>();

#endregion

var app = builder.Build();

#region Guardo Service Provider para notificaciones push

ServiciosGlobales.ServiceProvider = app.Services;

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
	app.UseDeveloperExceptionPage();
	//app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

#region Carpeta Imagenes

string imagenesRuta = Path.Combine(builder.Environment.ContentRootPath, "imagenes");

if (Directory.Exists(imagenesRuta) == true)
{
	app.UseStaticFiles(new StaticFileOptions
	{
		FileProvider = new PhysicalFileProvider(imagenesRuta),
		RequestPath = "/imagenes"
	});
}

#endregion

app.MapRazorComponents<App>().AddInteractiveServerRenderMode(opciones =>
{
	opciones.DisableWebSocketCompression = false;
});

#region Optimizador

app.UseWebOptimizer();

#endregion

#region Compresion

app.UseRequestDecompression();
app.UseResponseCompression();

#endregion

#region CORS necesario para extension navegador web

app.UseCors("Extension");

#endregion

#region Redireccionador

app.MapControllers();

#endregion

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

#region Extension

app.MapGet("extension/steam3/{id}/{region}/{clave}/", async (int id, string region, string clave) =>
{
	#nullable disable

	string claveExtension = builder.Configuration.GetValue<string>("Extension:Clave");

	if (clave == claveExtension)
	{
		BaseDatos.Extension.Extension juego = await BaseDatos.Extension.Buscar.Steam3(region, id.ToString());

		if (juego != null)
		{
			if (juego.Id > 0)
			{
				return Results.Json(juego);
			}
		}
	}

	return Results.NotFound();
});

app.MapGet("extension/gog3/{slug}/{region}/{clave}/", async (string slug, string region, string clave) =>
{
	#nullable disable

	string claveExtension = builder.Configuration.GetValue<string>("Extension:Clave");

	if (clave == claveExtension)
	{
		BaseDatos.Extension.Extension juego = await BaseDatos.Extension.Buscar.Gog3(region, slug);

		if (juego != null)
		{
			if (juego.Id > 0)
			{
				return Results.Json(juego);
			}
		}
	}

	return Results.NotFound();
});

app.MapGet("extension/epic3/{slug}/{region}/{clave}/", async (string slug, string region, string clave) =>
{
	#nullable disable

	string claveExtension = builder.Configuration.GetValue<string>("Extension:Clave");

	if (clave == claveExtension)
	{
		BaseDatos.Extension.Extension juego = await BaseDatos.Extension.Buscar.EpicGames3(region, slug);

		if (juego != null)
		{
			if (juego.Id > 0)
			{
				return Results.Json(juego);
			}
		}
	}

	return Results.NotFound();
});

#endregion

#region Sitemaps

app.MapGet("/sitemap.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.Maestro(http);
});

app.MapGet("/sitemap-main.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.Principal(http);
});

app.MapGet("/sitemap-games-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Juegos(http, i);
});

app.MapGet("/sitemap-bundles-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Bundles(http, i);
});

app.MapGet("/sitemap-free-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Gratis(http, i);
});

app.MapGet("/sitemap-subscriptions-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Suscripciones(http, i);
});

app.MapGet("/sitemap-lastnews-en.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.NoticiasUltimasIngles(http);
});

app.MapGet("/sitemap-lastnews-es.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.NoticiasUltimasEspañol(http);
});

app.MapGet("/sitemap-news-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.NoticiasIngles(http, i);
});

app.MapGet("/sitemap-curators-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Curators(http, i);
});

#endregion

#region Robots.txt

app.MapGet("/robots.txt", async (HttpContext http) =>
{
	string dominio = http.Request.Host.Value;

	string texto = @$"Sitemap: https://{dominio}/sitemap.xml

User-agent: *
Disallow: /account/
Disallow: /link/
Disallow: /publisher/
Disallow: /_framework/
Disallow: /_blazor/
";

	return Results.Text(texto, "text/plain; charset=utf-8");
});

#endregion

#region Links muertos para Google Search Console (13/01/2026)

app.Use(async (context, next) =>
{
	var path = context.Request.Path.Value;

	if (path != null && path.StartsWith("/link/", StringComparison.OrdinalIgnoreCase) && path.Contains("/news/") == false)
	{
		context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
		context.Response.Headers.Location = "/";
		return;
	}

	await next();
});

#endregion

#region Manifest

app.MapGet("/manifest.json", async (IConfiguration configuracion) =>
{
	string idioma = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

	var manifest = new
	{
		name = "pepe's deals",
		short_name = "pepe's deals",
		description = Idiomas.BuscarTexto(idioma, "String1", "Manifest"),
		lang = idioma,
		dir = "ltr",
		start_url = "/",
		scope = "/",
		theme_color = "#002033",
		background_color = "#002033",
		display = "standalone",
		display_override = new[] { "window-controls-overlay", "standalone" },
		categories = new[] { "utilities" },
		id = "pepesdeals",
		orientation = "any",
		icons = new[]
		{
			new { src = "/favicons/favicon-192x192.png", sizes = "192x192", type = "image/png" },
			new { src = "/favicons/favicon-512x512.png", sizes = "512x512", type = "image/png" },
			new { src = "/favicons/android-icon-192x192.png", sizes = "192x192", type = "image/png" },
			new { src = "/favicons/favicon-192x192.webp", sizes = "192x192", type = "image/webp" },
			new { src = "/favicons/favicon-512x512.webp", sizes = "512x512", type = "image/webp" },
			new { src = "/favicons/android-icon-192x192.webp", sizes = "192x192", type = "image/webp" }
		},
		screenshots = new[]
		{
			new {
				src = "/imagenes/screenshots/pepe.deals_.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String2", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_phone.webp",
				sizes = "750x1334",
				type = "image/webp",
				form_factor = "narrow",
				label = Idiomas.BuscarTexto(idioma, "String2", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_bundle_humblebundle.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String3", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_bundles.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String4", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_game.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String5", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_historicallows.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String6", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_search.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String7", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_steamdeck.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String8", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_subscriptions.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String9", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_subscriptions_humblechoice.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String10", "Manifest")
			}
		},
		prefer_related_applications = false,
		related_applications = new object[]
		{
			new Dictionary<string, string>
			{
				["platform"] = "chrome_web_store",
				["url"] = configuracion.GetValue<string>("Extension:Chrome"),
				["id"] = configuracion.GetValue<string>("Extension:ChromeId")
			},
			new Dictionary<string, string>
			{
				["platform"] = "firefox",
				["url"] = configuracion.GetValue<string>("Extension:Firefox"),
				["id"] = configuracion.GetValue<string>("Extension:FirefoxId")
			},
			new Dictionary<string, string>
			{
				["platform"] = "edge",
				["url"] = configuracion.GetValue<string>("Extension:Edge")
			}
		}
	};

	var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	});

	return Results.Content(json, "application/manifest+json");
});

#endregion

app.MapHealthChecks("/vida");

app.UseStatusCodePagesWithReExecute("/");

app.Run();
