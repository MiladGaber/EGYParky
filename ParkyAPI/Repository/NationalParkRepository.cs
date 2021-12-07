﻿using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext _db;

        public NationalParkRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateNationalPark(NationalPark nationalPark)
        {
            _db.NationalParks.Add(nationalPark);
            return Save();
        }

        public bool DeleteNationalPark(int id)
        {
            NationalPark nationalPark = _db.NationalParks.Where(i => i.Id == id).FirstOrDefault();
            _db.NationalParks.Remove(nationalPark);
            return Save();
        }

        public NationalPark GetNationalPark(int id)
        {
            return _db.NationalParks.FirstOrDefault(a => a.Id == id);
        }

        public bool NationalParkExist(string name)
        {
            bool val = _db.NationalParks.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return val;
        }

        public bool NationalParkExist(int id)
        {
            bool val = _db.NationalParks.Any(a =>a.Id == id);
            return val;
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return _db.NationalParks.OrderBy(a=>a.Name).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateNationalPark(NationalPark nationalPark)
        {
            _db.NationalParks.Update(nationalPark);
            return Save();
        }
    }
}
