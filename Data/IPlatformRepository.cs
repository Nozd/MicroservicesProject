﻿using PlatformService.Models;
using System.Collections.Generic;

namespace PlatformService.Data
{
    public interface IPlatformRepository
    {
        bool SaveChanges();
        IEnumerable<Platform> GetAll();
        Platform GetPlatformById(int id);
        void CreatePlatform(Platform platform);
    }
}
