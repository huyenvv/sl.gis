using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SLGIS.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SLGIS.Implementation
{
    public class HydropowerService
    {
        private ConcurrentDictionary<ObjectId, HydropowerPlant> _currentHydropowerId = new ConcurrentDictionary<ObjectId, HydropowerPlant>();
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly IUserRepository _userRepository;
        public HydropowerService(IHydropowerPlantRepository hydropowerPlantRepository, IUserRepository userRepository)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _userRepository = userRepository;
        }

        public async Task<HydropowerPlant> GetCurrent(ObjectId currentUserId)
        {
            if (!_currentHydropowerId.ContainsKey(currentUserId))
            {
                var listHydropowers = _hydropowerPlantRepository.Find(m => true);
                var user = await _userRepository.GetById(currentUserId.ToString());
                var currentHydropower = user.Roles?.Count > 0 ? listHydropowers.FirstOrDefault() 
                    : listHydropowers.Where(m => m.Owners.Contains(currentUserId)).FirstOrDefault();
                _currentHydropowerId.GetOrAdd(currentUserId, currentHydropower);
            }

            _currentHydropowerId.TryGetValue(currentUserId, out var hydropower);
           return hydropower;
        }
    }
}
