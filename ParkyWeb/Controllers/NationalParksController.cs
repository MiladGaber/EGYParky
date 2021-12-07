﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        public NationalParksController(INationalParkRepository npRepo)
        {
            _npRepo = npRepo;
        }
        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();

            if (id == null) // Mean Create ...
            {
                return View(obj);
            }

            obj = await _npRepo.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));

            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {

            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream()) 
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    obj.Picture = p1;
                }
                else
                {
                    var objForm = await _npRepo.GetAsync(SD.NationalParkAPIPath, obj.Id, HttpContext.Session.GetString("JWToken"));
                    obj.Picture = objForm.Picture;
                }

                if (obj.Id == 0)
                {
                    await _npRepo.CreateAsync(SD.NationalParkAPIPath, obj, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _npRepo.UpdateAsync(SD.NationalParkAPIPath+obj.Id, obj, HttpContext.Session.GetString("JWToken"));

                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
            
        }




        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _npRepo.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken")) });
        }



        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _npRepo.DeleteAsync(SD.NationalParkAPIPath, id, HttpContext.Session.GetString("JWToken"));
            if(status)
            {
                return Json(new { success = true , message="Delete Succesfully" });

            }
            return Json(new { success = false, message = "Not Deleted" });

        }
    }
}
