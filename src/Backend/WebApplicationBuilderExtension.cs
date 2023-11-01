using Backend.Services;

namespace Backend;

public static class WebApplicationBuilderExtension {
    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder) {
        builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => {
                if (builder.Configuration["FRONTEND_HOST"] is { } frontendHost) {
                    policyBuilder.WithOrigins($"https://{frontendHost}");
                }

                if (builder.Environment.IsDevelopment()) {
                    policyBuilder.WithOrigins("http://localhost:5188", "https://localhost:7076");
                }
            })
        );
        return builder;
    }

    public static WebApplicationBuilder AddShutdowner(this WebApplicationBuilder builder) {
        builder.Services.AddHostedService<Shutdowner>();
        return builder;
    }
    
    public static WebApplicationBuilder AddHttpClient(this WebApplicationBuilder builder) {
        builder.Services.AddHttpClient();
        return builder;
    }

}