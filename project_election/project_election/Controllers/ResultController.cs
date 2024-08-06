using project_election.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project_election.Controllers
{
    public class ResultController : Controller
    {

        private electionEntities5 db = new electionEntities5();
        // GET: Result
        public ActionResult Areas()
        {
            return View(db.Areas.ToList());
        }



        public ActionResult Lists(int? id)
        {
            var area = db.Areas
    .Where(l => l.ElectionAreaID == id)
    .Select(l => l.NameOfElectionArea)
    .FirstOrDefault();


            string nameOfArea = area ?? "Default Name"; // Use a default name if no area is found

            int totalVotesForArea = db.Users
                .Where(l => l.ElectionArea == area)
                .Sum(c => (c.LocalElections.HasValue && c.LocalElections.Value ? 1 : 0) + (c.WhitePaperLocalElections.HasValue && c.WhitePaperLocalElections.Value ? 1 : 0));
            double percentage = 0.07; // percentage value
            double result = percentage * totalVotesForArea;

            Session["totalVotesForArea"] = totalVotesForArea;
            Session["result"] = result;

            var localListsWithMoreVotes = db.LocalLists
.Include(l => l.Area) // Include related Area entity
.Where(l => l.ElectionAreaID == id) // Filter by ElectionAreaID
.Where(l => l.NumberOfVotes > result) // Filter lists where NumberOfVotes > result
.OrderByDescending(l => l.NumberOfVotes) // Sort by NumberOfVotes in descending order
.ToList(); // Convert to list for view
            Session["LocalListsWithMoreVotes"] = localListsWithMoreVotes;


            foreach (var l in localListsWithMoreVotes)
            {
                l.Sucess = true;
            }




            if (id == null)
            {
                // Return a view showing a list of all local lists if ID is not provided
                var localList = db.LocalLists
                    .Include(l => l.Area)
                    .OrderByDescending(c => c.NumberOfVotes) // Sort by NumberOfVotes in descending order
                    .ToList();
                return View(localList); // Ensure the view is set up to accept IEnumerable<LocalList>
            }
            else
            {
                // Return a view showing the details of the specific local list if ID is provided
                var localList = db.LocalLists
                    .Include(l => l.Area)
                    .Where(l => l.ElectionAreaID == id) // Filter by ElectionAreaID
                    .ToList();

                if (!localList.Any())
                {
                    return HttpNotFound(); // Handle the case when no list is found
                }



                return View(localListsWithMoreVotes); // Ensure the view is set up to accept IEnumerable<LocalList>
            }
        }

        public ActionResult Candidates(int? id)
        {

            Session["id"] = id;
            // Retrieve the list of LocalLists with more votes from session
            var localListsWithMoreVotes = Session["LocalListsWithMoreVotes"] as List<LocalList>;
            int totalVotesInFilteredLists = localListsWithMoreVotes?.Sum(l => l.NumberOfVotes ?? 0) ?? 0;

            // Retrieve the number of electoral seats for the given Area ID
            int electoralSeats = db.Areas
                .Where(a => a.ElectionAreaID == id)
                .Select(a => a.NumberOfElectoralSeats)
                .FirstOrDefault() ?? 0;

            // Calculate total votes for the specific list
            var totalVotesOfSpecificList = db.LocalListCandidates
                .Where(c => c.LocalListingID == id)
                .Sum(c => c.NumberOfVotesCandidate ?? 0);

            // Calculate the ratio for top candidates
            double ratio = (totalVotesOfSpecificList / (double)totalVotesInFilteredLists) * electoralSeats;
            int numberOfTopCandidates = (int)Math.Round(ratio);

            // Update candidate success status based on specific criteria
            var topTwoCandidates = db.LocalListCandidates
                .Where(c => c.LocalListingID == id && c.typeofCandidates == "تنافس")
                .OrderByDescending(c => c.NumberOfVotesCandidate)
                .Take(1)
                .ToList();
            foreach (var item in topTwoCandidates)
            {
                item.Sucess = true;
            }

            var topMasehe = db.LocalListCandidates
                .Where(c => c.typeofCandidates == "مسيحي")
                .OrderByDescending(c => c.NumberOfVotesCandidate)
                .Take(1)
                .ToList();
            foreach (var item in topMasehe)
            {
                item.Sucess = true;
            }

            var topQota = db.LocalListCandidates
                .Where(c => c.typeofCandidates == "كوتا")
                .OrderByDescending(c => c.NumberOfVotesCandidate)
                .Take(1)
                .ToList();
            foreach (var item in topQota)
            {
                item.Sucess = true;
            }

            // Save changes to the database
            db.SaveChanges();

            // Handle the case when ID is not provided
            if (id == null)
            {
                var localListCandidates = db.LocalListCandidates.Include(l => l.LocalList);
                return View(localListCandidates.ToList());
            }

            // Retrieve list name from the database
            var listName = db.LocalLists
                .Where(l => l.ID == id)
                .Select(l => l.ListName)
                .FirstOrDefault();
            Session["listName"] = listName;

            // Filter candidates by LocalListingID and include related LocalList
            var filteredCandidates = db.LocalListCandidates
                .Where(c => c.LocalListingID == id)
                .Include(c => c.LocalList)
                .ToList();

            // Return the view with the filtered candidates
            return View(filteredCandidates);
        }


        public ActionResult GeneralList()
        {
            int totalVotes = db.Users
            .Sum(c => (c.PartyElections.HasValue && c.PartyElections.Value ? 1 : 0) + (c.WhitePaperPartyElections.HasValue && c.WhitePaperPartyElections.Value ? 1 : 0));
            double percentage = 2.5 / 100; // Convert percentage to decimal
            double basic = percentage * totalVotes;



            var listingsAboveBasic = db.GeneralListings
               .Where(gl => gl.NumberOfVotes.HasValue && gl.NumberOfVotes.Value > basic)
               .ToList();
            Session["listingsAboveBasic"] = listingsAboveBasic;
            foreach (var l in db.GeneralListings)
            {

                l.Sucess = false;

            }
            foreach (var listing in listingsAboveBasic)
            {
                listing.Sucess = true; // Set to true or false based on your criteria
            }

            db.SaveChanges();





            return View(db.GeneralListings.ToList());
        }




        public ActionResult GeneralListCandidates(int? id)
        {

            // Retrieve the list from the session and cast it to List<GeneralListing>
            var listingsAboveBasic = Session["listingsAboveBasic"] as List<GeneralListing>;



            // Check if the cast was successful

            // Calculate totalVotes by summing votes from the listings
            double totalVotes = listingsAboveBasic
                .Where(gl => gl.NumberOfVotes.HasValue) // Ensure we only consider listings with votes
                .Sum(gl => gl.NumberOfVotes.Value); // Sum the votes

            // Retrieve the specific GeneralListing based on number of votes
            // Assuming 'id' is the number of votes you are looking for
            var totalVotesForGeneral = db.GeneralListings
                .Where(g => g.GeneralListingID == id) // Ensure NumberOfVotes is not null
                .FirstOrDefault().NumberOfVotes;



            double numberOfSeats = 0;

            // Calculate numberOfSeats only if totalVotes is greater than 0
            if (totalVotes > 0)
            {
                numberOfSeats = totalVotesForGeneral.HasValue ? totalVotesForGeneral.Value / totalVotes : 0;

                // Adjust numberOfSeats based on its value
                if (numberOfSeats > 0.5 && numberOfSeats < 1)
                {
                    numberOfSeats = 1;
                }
                else if (numberOfSeats > 1.5 && numberOfSeats < 2)
                {
                    numberOfSeats = 2;
                }
                else
                {
                    numberOfSeats = Math.Round(numberOfSeats);
                }
            }

            // Ensure numberOfSeats is a non-negative integer
            int seatsToTake = Math.Max(0, (int)numberOfSeats);

            // Retrieve the specified number of GeneralListCandidates related to a GeneralListing with the given ID
            var successGeneralCandidates = db.GeneralListCandidates
                .Where(c => c.GeneralListingID == id) // Filter by GeneralListingID
                .Take(seatsToTake) // Limit the number of candidates to the specified number
                .ToList(); // Convert the result to a list
            foreach (var candidate in successGeneralCandidates)
            {

                candidate.Sucess = true;

            }







            // If id is null, return all candidates
            if (id == null)
            {
                var generalListCandidates = db.GeneralListCandidates.Include(l => l.GeneralListing);
                return View(generalListCandidates.ToList());
            }

            // If id is provided, filter candidates by LocalListingID
            var filteredCandidates = db.GeneralListCandidates
                                       .Where(c => c.GeneralListingID == id)
                                       .Include(c => c.GeneralListing);

            return View(filteredCandidates.ToList());
        }


        public ActionResult SelectResult()
        {

            return View();
        }





        //admin



        public ActionResult AreasAdmin()
        {
            return View(db.Areas.ToList());
        }



        public ActionResult ListsAdmin(int? id)
        {
            var area = db.Areas
    .Where(l => l.ElectionAreaID == id)
    .Select(l => l.NameOfElectionArea)
    .FirstOrDefault();


            string nameOfArea = area ?? "Default Name"; // Use a default name if no area is found

            int totalVotesForArea = db.Users
                .Where(l => l.ElectionArea == area)
                .Sum(c => (c.LocalElections.HasValue && c.LocalElections.Value ? 1 : 0) + (c.WhitePaperLocalElections.HasValue && c.WhitePaperLocalElections.Value ? 1 : 0));
            double percentage = 0.07; // percentage value
            double result = percentage * totalVotesForArea;


            Session["result"] = result;

            var localListsWithMoreVotes = db.LocalLists
.Include(l => l.Area) // Include related Area entity
.Where(l => l.ElectionAreaID == id) // Filter by ElectionAreaID
.Where(l => l.NumberOfVotes > result) // Filter lists where NumberOfVotes > result
.OrderByDescending(l => l.NumberOfVotes) // Sort by NumberOfVotes in descending order
.ToList(); // Convert to list for view
            Session["LocalListsWithMoreVotes"] = localListsWithMoreVotes;


            foreach (var l in localListsWithMoreVotes)
            {
                l.Sucess = true;
            }




            if (id == null)
            {
                // Return a view showing a list of all local lists if ID is not provided
                var localList = db.LocalLists
                    .Include(l => l.Area)
                    .OrderByDescending(c => c.NumberOfVotes) // Sort by NumberOfVotes in descending order
                    .ToList();
                return View(localList); // Ensure the view is set up to accept IEnumerable<LocalList>
            }
            else
            {
                // Return a view showing the details of the specific local list if ID is provided
                var localList = db.LocalLists
                    .Include(l => l.Area)
                    .Where(l => l.ElectionAreaID == id) // Filter by ElectionAreaID
                    .ToList();

                if (!localList.Any())
                {
                    return HttpNotFound(); // Handle the case when no list is found
                }



                return View(localListsWithMoreVotes); // Ensure the view is set up to accept IEnumerable<LocalList>
            }
        }


        public ActionResult CandidatesAdmin(int? id)
        {
            Session["id"] = id;
            var localListsWithMoreVotes = Session["LocalListsWithMoreVotes"] as List<LocalList>;

            int totalVotesInFilteredLists = localListsWithMoreVotes
.Sum(l => l.NumberOfVotes ?? 0); // Handle null values



            int electoralSeats = db.Areas
.Where(a => a.ElectionAreaID == id) // Filter by ElectionAreaID
.Select(a => a.NumberOfElectoralSeats)
.FirstOrDefault() ?? 0; // Assuming there's only one Area per ElectionAreaID

            var totalVotesOfSpecificList = db.LocalListCandidates
                         .Where(c => c.LocalListingID == id)
                         .Sum(c => c.NumberOfVotesCandidate ?? 0);

            double Ration = (totalVotesOfSpecificList / (double)totalVotesInFilteredLists) * electoralSeats;

            int numberOfTopCandidates = (int)Math.Round(Ration);

            var topTwoCandidates = db.LocalListCandidates
         .Where(c => c.LocalListingID == id && c.typeofCandidates == "تنافس")
         .OrderByDescending(c => c.NumberOfVotesCandidate)
         .Take(1)
         .ToList();
            foreach (var item in topTwoCandidates)
            {
                item.Sucess = true;
            }


            var topmasehe = db.LocalListCandidates
.Where(c => c.typeofCandidates == "مسيحي")
.OrderByDescending(c => c.NumberOfVotesCandidate)
.Take(1)
.ToList();
            foreach (var item in topmasehe)
            {
                item.Sucess = true;
            }


            var topqota = db.LocalListCandidates
.Where(c => c.Gender == "كوتا")
.OrderByDescending(c => c.NumberOfVotesCandidate)
.Take(1)
.ToList();
            foreach (var item in topqota)
            {
                item.Sucess = true;
            }

            db.SaveChanges();

            // If id is null, return all candidates
            if (id == null)
            {
                var localListCandidates = db.LocalListCandidates.Include(l => l.LocalList);
                return View(localListCandidates.ToList());
            }


            var listName = db.LocalLists
                 .Where(l => l.ID == id)
                 .Select(l => l.ListName)
                 .FirstOrDefault();
            Session["listName"] = listName;

            // If id is provided, filter candidates by LocalListingID
            var filteredCandidates = db.LocalListCandidates
                                       .Where(c => c.LocalListingID == id)
                                       .Include(c => c.LocalList);

            return View(filteredCandidates.ToList());
        }


        public ActionResult GeneralListAdmin()
        {
            int totalVotes = db.Users
            .Where(l => l.ElectionArea == "irbid")
            .Sum(c => (c.PartyElections.HasValue && c.PartyElections.Value ? 1 : 0) + (c.WhitePaperPartyElections.HasValue && c.WhitePaperPartyElections.Value ? 1 : 0));
            double percentage = 2.5 / 100; // Convert percentage to decimal
            double basic = percentage * totalVotes;



            var listingsAboveBasic = db.GeneralListings
               .Where(gl => gl.NumberOfVotes.HasValue && gl.NumberOfVotes.Value > basic)
               .ToList();
            Session["listingsAboveBasic"] = listingsAboveBasic;
            foreach (var l in db.GeneralListings)
            {

                l.Sucess = false;

            }
            foreach (var listing in listingsAboveBasic)
            {
                listing.Sucess = true; // Set to true or false based on your criteria
            }

            db.SaveChanges();





            return View(db.GeneralListings.ToList());
        }



        public ActionResult GeneralListCandidatesAdmin(int? id)
        {

            // Retrieve the list from the session and cast it to List<GeneralListing>
            var listingsAboveBasic = Session["listingsAboveBasic"] as List<GeneralListing>;



            // Check if the cast was successful

            // Calculate totalVotes by summing votes from the listings
            double totalVotes = listingsAboveBasic
                .Where(gl => gl.NumberOfVotes.HasValue) // Ensure we only consider listings with votes
                .Sum(gl => gl.NumberOfVotes.Value); // Sum the votes

            // Retrieve the specific GeneralListing based on number of votes
            // Assuming 'id' is the number of votes you are looking for
            var totalVotesForGeneral = db.GeneralListings
                .Where(g => g.GeneralListingID == id) // Ensure NumberOfVotes is not null
                .FirstOrDefault().NumberOfVotes;



            double numberOfSeats = 0;

            // Calculate numberOfSeats only if totalVotes is greater than 0
            if (totalVotes > 0)
            {
                numberOfSeats = totalVotesForGeneral.HasValue ? totalVotesForGeneral.Value / totalVotes : 0;

                // Adjust numberOfSeats based on its value
                if (numberOfSeats > 0.5 && numberOfSeats < 1)
                {
                    numberOfSeats = 1;
                }
                else if (numberOfSeats >= 1 && numberOfSeats < 2)
                {
                    numberOfSeats = 2;
                }
                else
                {
                    numberOfSeats = Math.Round(numberOfSeats);
                }
            }

            // Ensure numberOfSeats is a non-negative integer
            int seatsToTake = Math.Max(0, (int)numberOfSeats);

            // Retrieve the specified number of GeneralListCandidates related to a GeneralListing with the given ID
            var successGeneralCandidates = db.GeneralListCandidates
                .Where(c => c.GeneralListingID == id) // Filter by GeneralListingID
                .Take(seatsToTake) // Limit the number of candidates to the specified number
                .ToList(); // Convert the result to a list
            foreach (var candidate in successGeneralCandidates)
            {

                candidate.Sucess = true;

            }







            // If id is null, return all candidates
            if (id == null)
            {
                var generalListCandidates = db.GeneralListCandidates.Include(l => l.GeneralListing);
                return View(generalListCandidates.ToList());
            }

            // If id is provided, filter candidates by LocalListingID
            var filteredCandidates = db.GeneralListCandidates
                                       .Where(c => c.GeneralListingID == id)
                                       .Include(c => c.GeneralListing);

            return View(filteredCandidates.ToList());
        }
























    }
}