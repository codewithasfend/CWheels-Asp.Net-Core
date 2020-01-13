using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CWheelsApi.Data;
using CWheelsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CWheelsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private CWheelsDbContext _cWheelsDbContext;
        public VehiclesController(CWheelsDbContext cWheelsDbContext)
        {
            _cWheelsDbContext = cWheelsDbContext;
        }

        public IActionResult Post(Vehicle vehicle)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound();
            }

            var vehicleObj = new Vehicle()
            {
                Title = vehicle.Title,
                Description = vehicle.Description,
                Color = vehicle.Color,
                Company = vehicle.Company,
                Condition = vehicle.Condition,
                DatePosted = vehicle.DatePosted,
                Engine = vehicle.Engine,
                Price = vehicle.Price,
                Model = vehicle.Model,
                Location = vehicle.Location,
                CategoryId = vehicle.CategoryId,
                IsFeatured = false,
                IsHotAndNew = false,
                UserId = user.Id,
            };
            _cWheelsDbContext.Vehicles.Add(vehicleObj);
            _cWheelsDbContext.SaveChanges();

            return Ok(new { vehicleId = vehicleObj.Id, message = "Vehicle Added Successfully" });
        }

        [HttpGet("[action]")]
        public IActionResult HotAndNewAds()
        {
            var vehicles = from v in _cWheelsDbContext.Vehicles
                           where v.IsHotAndNew == true
                           select new
                           {
                               Id = v.Id,
                               Title = v.Title,
                               ImageUrl = v.Images.FirstOrDefault().ImageUrl
                           };

            return Ok(vehicles);
        }

        [HttpGet("[action]")]
        public IActionResult SearchVehicles(string search)
        {
            var vehicles = from v in _cWheelsDbContext.Vehicles
                           where v.Title.StartsWith(search)
                           select new
                           {
                               Id = v.Id,
                               Title = v.Title,
                           };

            return Ok(vehicles);
        }


        [HttpGet]
        public IActionResult GetVehicles(int categoryId)
        {
            var vehicles = from v in _cWheelsDbContext.Vehicles
                           where v.CategoryId == categoryId
                           select new
                           {
                               Id = v.Id,
                               Title = v.Title,
                               Price = v.Price,
                               Location = v.Location,
                               DatePosted = v.DatePosted,
                               IsFeatured = v.IsFeatured,
                               ImageUrl = v.Images.FirstOrDefault().ImageUrl
                           };

            return Ok(vehicles);
        }

        [HttpGet("[action]")]
        public IActionResult VehicleDetails(int id)
        {
            var foundVehicle = _cWheelsDbContext.Vehicles.Find(id);
            if (foundVehicle == null)
            {
                return NoContent();
            }

            var vehicle = from v in _cWheelsDbContext.Vehicles
                          join u in _cWheelsDbContext.Users on v.UserId equals u.Id
                          where v.Id == id
                          select new
                          {
                              Id = v.Id,
                              Title = v.Title,
                              Description = v.Description,
                              Price = v.Price,
                              Model = v.Model,
                              Engine = v.Engine,
                              Color = v.Color,
                              Company = v.Company,
                              DatePosted = v.DatePosted,
                              Condition = v.Condition,
                              Location = v.Location,
                              Images = v.Images,
                              Email = u.Email,
                              Phone = u.Phone,
                              ImageUrl = u.ImageUrl
                          };

            return Ok(vehicle);
        }


        [HttpGet("[action]")]
        public IActionResult MyAds()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound();
            }

            var vehicles = from v in _cWheelsDbContext.Vehicles
                           where v.UserId == user.Id
                           select new
                           {
                               Id = v.Id,
                               Title = v.Title,
                               Price = v.Price,
                               Location = v.Location,
                               DatePosted = v.DatePosted,
                               IsFeatured = v.IsFeatured,
                               ImageUrl = v.Images.FirstOrDefault().ImageUrl
                           };

            return Ok(vehicles);
        }



    }
}