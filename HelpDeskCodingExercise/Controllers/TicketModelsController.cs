using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HelpDeskCodingExercise.Models;
using Microsoft.AspNet.Identity;

namespace HelpDeskCodingExercise.Controllers
{
    //Require the user to be logged into an account to access the ticket views.
    [Authorize]
    public class TicketModelsController : Controller
    {
        //Create a private DB context for use later on with LINQ statements.
        private Tickets db = new Tickets();
        

        // GET: TicketModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketModels ticketModels = db.Ticket.Find(id);
            if (ticketModels == null)
            {
                return HttpNotFound();
            }
            return View(ticketModels);
        }

        [AllowAnonymous]
        // GET: TicketModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Status,Severity,Assignee,CreatedDate")] TicketModels ticketModels)
        {
            if (ModelState.IsValid)
            {
                db.Ticket.Add(ticketModels);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(ticketModels);
        }

        [Authorize(Roles ="admin")]
        // GET: TicketModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketModels ticketModels = db.Ticket.Find(id);
            if (ticketModels == null)
            {
                return HttpNotFound();
            }

            return View(ticketModels);
        }

        // POST: TicketModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Status,Severity,Assignee,CreatedDate")] TicketModels ticketModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("OpenTickets");
            }
            return View(ticketModels);
        }

        // GET: TicketModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketModels ticketModels = db.Ticket.Find(id);
            if (ticketModels == null)
            {
                return HttpNotFound();
            }
            return View(ticketModels);
        }


        // POST: TicketModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketModels ticketModels = db.Ticket.Find(id);
            db.Ticket.Remove(ticketModels);
            db.SaveChanges();
            return RedirectToAction("OpenTickets");
        }

        
        public ActionResult AllTickets(string sortOrder, string statusType)
        {
                return SortFunction(sortOrder, "all");
        }

        
        public ActionResult ClosedTickets(string sortOrder)
        {
            return SortFunction(sortOrder, "closed");
        }

        
        public ActionResult MyTickets(string sortOrder, string statusType)
        {
            if (string.IsNullOrEmpty(sortOrder))
            {
                var tickets = from t in db.Ticket
                              select t;

                //NOTE: Hard coded just for fun
                tickets = tickets.Where(t => t.Status.Equals("open") && t.Assignee.Equals("james"));

                return View(tickets.ToList());
            }
            else
            {
                return SortFunction(sortOrder, statusType);
            }
        }

        public ActionResult OpenTickets(string sortOrder)
        {
            return SortFunction(sortOrder, "open");
        }



        /**********************************/
        //Custom function for all the sorting
        //to be handled in one spot versus
        //having all of the same code repeated
        //within each view.
        /**********************************/
        public ViewResult SortFunction(string sortOrder, string statusType)
        {

            //Store the type of sort that needs to be applied to the column
            ViewBag.IdSortParm = (String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("id_asc")) ? "id_desc" : "id_asc";
            ViewBag.AssigneeSortParm = (String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("assignee_asc")) ? "assignee_desc" : "assignee_asc";
            ViewBag.DateSortParm = (String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("date_asc")) ? "date_desc" : "date_asc";
            ViewBag.SeveritySortParm = (String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("severity_asc")) ? "severity_desc" : "severity_asc";

            //Start the DB query for the sorting.
            var tickets = from t in db.Ticket
                          select t;

            //Quick test to help with the different ticket status views.
            if (statusType.Equals("all"))
                tickets = tickets.Where(t => t.Status.Equals("open") || t.Status.Equals("closed"));
            else
                tickets = tickets.Where(t => t.Status.Equals(statusType));


            //To handle the sorting for each section and which direction is needed.
            switch (sortOrder)
            {
                case "id_asc":
                    tickets = tickets.OrderBy(t => t.Id);
                    break;
                case "id_desc":
                    tickets = tickets.OrderByDescending(t => t.Id);
                    break;
                case "assignee_asc":
                    tickets = tickets.OrderBy(t => t.Assignee);
                    break;
                case "assignee_desc":
                    tickets = tickets.OrderByDescending(t => t.Assignee);
                    break;
                case "date_asc":
                    tickets = tickets.OrderBy(t => t.CreatedDate);
                    break;
                case "date_desc":
                    tickets = tickets.OrderByDescending(t => t.CreatedDate);
                    break;
                case "severity_asc":
                    tickets = tickets.OrderBy(t => t.Severity);
                    break;
                case "severity_desc":
                    tickets = tickets.OrderByDescending(t => t.Severity);
                    break;
            }//End of SWTICH for sortOrder

            return View(tickets.ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
