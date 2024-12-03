using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
        {
            // Verificar se já existem dados no banco
            if (context.Individuos.Any() || context.Sequencias.Any())
            {
                return; // Dados já foram adicionados
            }

            // Inserir dados iniciais
            context.Individuos.AddRange(
                new Individuo { Codigo = "IND001", Nome = "João Silva" },
                new Individuo { Codigo = "IND002", Nome = "Maria Oliveira" }
            );

            context.Sequencias.AddRange(
                new Sequencia { SeqGenetica = "Seq001", IndividuoId = 1 },
                new Sequencia { SeqGenetica = "Seq002", IndividuoId = 2 }
            );

            context.SaveChanges();
        }
    }
}
