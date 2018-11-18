using System;
using System.Collections.Generic;
using AlleyCat.Autowire;
using AlleyCat.IO;
using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace AlleyCat.Setting
{
    [AutowireContext]
    public class SettingsConfiguration : AutowiredNode, IServiceDefinitionProvider
    {
        public IEnumerable<Type> ProvidedTypes => new[] {typeof(IConfiguration)};

        [Service(false)]
        protected IEnumerable<ISettingsProvider> Providers { get; private set; } = Seq<ISettingsProvider>(); 

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            var builder = CreateBuilder();

            Providers.Iter(p => p.AddSettings(builder));

            var configuration = builder.Build();

            collection
                .AddOptions()
                .AddSingleton<IConfiguration>(configuration);

            Providers.Iter(p => p.BindSettings(configuration, collection));
        }

        protected virtual IConfigurationBuilder CreateBuilder()
        {
            return new ConfigurationBuilder()
                .SetFileProvider(new FileProvider())
                .SetFileLoadExceptionHandler(OnError);
        }

        protected virtual void OnError(FileLoadExceptionContext context)
        {
        }
    }
}
