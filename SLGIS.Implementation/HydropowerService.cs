using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SLGIS.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public HydropowerPlant GetCurrent(ObjectId currentUserId)
        {
            if (!_currentHydropowerId.ContainsKey(currentUserId))
            {
                var listHydropowers = _hydropowerPlantRepository.Find(m => true);
                var user = _userRepository.Find(m => m.Id == currentUserId).FirstOrDefault();
                var currentHydropower = user.Roles?.Count > 0 ? listHydropowers.FirstOrDefault()
                    : listHydropowers.Where(m => m.Owners.Contains(currentUserId)).FirstOrDefault();
                if (currentHydropower == null)
                {
                    return null;
                }

                _currentHydropowerId.GetOrAdd(currentUserId, currentHydropower);
            }

            _currentHydropowerId.TryGetValue(currentUserId, out var hydropower);
            return hydropower;
        }

        public bool ChangeCurrent(ObjectId currentUserId, Guid hydropowerId)
        {
            var listHydropowers = _hydropowerPlantRepository.Find(m => m.Id == hydropowerId);
            var user = _userRepository.Find(m => m.Id == currentUserId).FirstOrDefault();
            if (user.Roles == null || user.Roles.Count == 0)
            {
                listHydropowers = listHydropowers.Where(m => m.Owners.Contains(currentUserId));
            }
            var currentHydropower = listHydropowers.FirstOrDefault();
            if (currentHydropower == null)
            {
                return false;
            }

            _currentHydropowerId[currentUserId] = currentHydropower;
            return true;
        }

        public IEnumerable<(Guid Id, string Name)> CurrentList(ObjectId currentUserId)
        {
            var listHydropowers = _hydropowerPlantRepository.Find(m => true);
            var user = _userRepository.Find(m => m.Id == currentUserId).FirstOrDefault();
            if (user.Roles == null || user.Roles.Count == 0)
            {
                listHydropowers = listHydropowers.Where(m => m.Owners.Contains(currentUserId));
            }
            return listHydropowers.ToEnumerable().Select(m => (m.Id, m.Name));
        }
    }
}
