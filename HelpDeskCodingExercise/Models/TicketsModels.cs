using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HelpDeskCodingExercise.Models
{
    public class TicketModels
    {
        //Basis for the ticket entity and its attributes
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Severity { get; set; }
        public string Assignee { get; set; }
        public DateTime CreatedDate { get; set; }

        //Constructor since tickets will always be open and this will input the time submitted as well.
        public TicketModels()
        {
            Status = "OPEN";
            Severity = "MEDIUM";
            CreatedDate = DateTime.Now;
        }


        //Enums to be used in the ticket dropdowns. The severity and ticket status are fine as enum since they're fixed.
        public enum SeverityTypeDropDown
        {
            LOW, MEDIUM, HIGH
        }

        public enum AvailableTechsDropDown
        {
            James, Mike, Joe, Nick, RJ, Noah
        }
        public enum TicketStatusDropDown
        {
            OPEN, CLOSED
        }
    }

    //Create a separate Tickets DB for the tickets. Hindsight, using a separate table within
    // the initial DB would've been fine since it would've allowed easier razor page design. 
    //ViewModel might be needed now to incorporate the available techs to the ticket dropdown.
    public class Tickets : DbContext
    {
        public Tickets()
            : base("Help_Desk_Tickets")
        {

        }

        public DbSet<TicketModels> Ticket { get; set; }
    }
}