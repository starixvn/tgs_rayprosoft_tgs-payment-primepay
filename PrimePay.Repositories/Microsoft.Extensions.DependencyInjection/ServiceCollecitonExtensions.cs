using System;
using Microsoft.EntityFrameworkCore;
using Tgs.Payment.PrimePay;
using Tgs.Payment.PrimePay.Repositories;
using Tgs.Payment.PrimePay.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollecitonExtensions
    {
        public static IServiceCollection AddPrimePayRepositories(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> configure,
            int poolSize = 128)
            => services
                .AddDbContextPool<TgsContext>(configure, poolSize)
                .AddTransient<ITgsRepository, TgsRepository>();
    }
}
