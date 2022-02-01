using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext context;

        public PlatformRepository(AppDbContext context)
        {
            this.context = context;
        }
        public void Create(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAll() => context.Platforms;

        public Platform GetById(int id) => context.Platforms.FirstOrDefault(t => t.Id == id);

        public bool SaveChanges() => context.SaveChanges() >= 0;
    }
}
