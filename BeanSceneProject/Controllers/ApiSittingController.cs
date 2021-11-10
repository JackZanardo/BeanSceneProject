using BeanSceneProject.Data;
using BeanSceneProject.DTOs;
using BeanSceneProject.Models.Reservation;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace BeanSceneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSittingController : ControllerBase
    {
        private readonly SittingService _sittingService;

        public ApiSittingController(SittingService sittingService)
        {
            _sittingService = sittingService;
        }

        private readonly Expression<Func<Sitting, SittingDto>> AsSittingDto =
            x => new SittingDto
            {
                Id = x.Id,
                IsClosed = x.IsClosed,
                Available = x.Available,
                Capacity = x.Capacity,
                Open = x.Open,
                Close = x.Close,
                RestaurantId = x.RestaurantId,
                Restaurant = new RestaurantDto 
                { 
                    Id = x.Restaurant.Id,
                    Address = x.Restaurant.Address,
                    Name = x.Restaurant.Name
                },
                SittingTypeId = x.SittingTypeId,
                SittingType = new SittingTypeDto 
                { 
                    Id = x.SittingType.Id,
                    Name = x.SittingType.Name
                }
            };

        //GET: /api/ApiSitting/GetSittings
        [HttpGet("GetSittings")]
        public ActionResult GetSittings()
        {
            var sittings = _sittingService.GetSittings().Select(AsSittingDto).ToList();
            var jsonSittings = JsonSerializer.Serialize(sittings);

            return Content(jsonSittings);
        }

        //GET: /api/ApiSitting/GetSittingsByDate?date=yyyy-mm-dd
        [HttpGet("GetSittingsByDate")]
        public ActionResult GetSittings(string date)
        {
            var sittings = _sittingService.GetSittings(DateTime.Parse(date)).Select(AsSittingDto).ToList();
            var jsonSittings = JsonSerializer.Serialize(sittings);

            return Content(jsonSittings);
        }

    }
}
