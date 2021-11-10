using BeanSceneProject.Data;
using BeanSceneProject.DTOs;
using BeanSceneProject.Services;
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
    public class ApiReservationController : ControllerBase
    {
        private readonly ReservationService _reservationService;

        public ApiReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        private static List<TableDto> GetTableDtos(List<Table> tables)
        {
            List<TableDto> tableDtos = new List<TableDto>();
            foreach(var t in tables)
            {
                tableDtos.Add(new TableDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Area = t.Area.Name
                });
            }
            return tableDtos;
        }


        private readonly Expression<Func<Reservation, ReservationDto>> AsReservationDto =
            x => new ReservationDto
            {
                Id = x.Id,
                Start = x.Start,
                Duration = x.Duration,
                CustomerNum = x.CustomerNum,
                Notes = x.Notes,
                ReservationStatus = x.ReservationStatus.ToString(),
                ReservationOrigin = x.ReservationOrigin.Name,
                SittingId = x.SittingId,
                Person = x.Person,
                PersonId = x.PersonId,
                Tables = GetTableDtos(x.Tables)
            };

        //GET: /api/ApiReservation/GetReservations
        [HttpGet("GetReservations")]
        public ActionResult GetReservations()
        {
            var reservations = _reservationService.GetReservations().Select(AsReservationDto).ToList();
            var jsonReservations = JsonSerializer.Serialize(reservations);

            return Content(jsonReservations);
        }

        //GET: /api/ApiReservation/GetReservationsBySitting?sittingId=
        [HttpGet("GetReservationsBySitting")]
        public ActionResult GetReservations(int sittingId)
        {
            var reservations = _reservationService.GetReservations(sittingId).Select(AsReservationDto).ToList();
            var jsonReservations = JsonSerializer.Serialize(reservations);

            return Content(jsonReservations);
        }
    }
}
