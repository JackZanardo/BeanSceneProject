using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.StaffReservation
{
    public class AddTables
    {
        public int SittingId { get; set; }
        public string SittingInfo { get; set; }
        public int ReservationId { get; set; }
        public string ReservationInfo { get; set; }

        public string PersonInfo { get; set; }
        public string ChosenTables { get; set; }
        public int[] ChosenTablesId { get; set; }
        public int Heads { get; set; }
        public IList<Table> Tables { get; set; }

        public IList<Area> Areas { get; set; }

        [BindProperty]
        public List<int> SelectedTables { get; set; } = new List<int>();
    }
}
