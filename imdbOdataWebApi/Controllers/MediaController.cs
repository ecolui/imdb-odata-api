using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.OData;
using Microsoft.AspNet.OData.Extensions;

using DataAccess;
using Microsoft.AspNet.OData;

namespace imdbOdataWebApi.Controllers
{
    public class MediaController : /*Controller*/ ODataController
    {

        private readonly imdbContext _appDbContext;
        public MediaController(imdbContext sampleODataDbContext)
        {
            _appDbContext = sampleODataDbContext;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_appDbContext.Media.AsQueryable());
        }

        //Note: You would have to do a little more work for PUT, POST, and DELETE
        //I didn't impelement this, because my goal was just to create an 
        //endpoint that will allow users to create complex, custom GET queries

        //Sample Queries #1: Get 2005 movies that are at least 150 minutes long
        //http://localhost:5000/odata/Media?$filter=startYear eq 2005 and runtimeMinutes ge 150&$expand=Genres&$orderby=runtimeMinutes desc

        //Sample Query #2: Find the 100 top rated movies in 2010. Make sure the movies have at least 1000 votes
        //http://localhost:5000/odata/Media?$filter=startYear eq 2010 and runtimeMinutes ge 150&$expand=Rating

        //Sample Query #3: Find movies with a start year of 2005 that are at least 150 minutes long whose Ratings have at least 10 votes. Show the ratings and order by movie duration
        //http://localhost:5000/odata/Media?$filter=startYear eq 2005 and runtimeMinutes ge 150 and Rating/NumVotes ge 10&$expand=Rating&$orderby=runtimeMinutes desc

        //Note: you can use tools (eg. JavaScript fluent API, F# OData Provider) to build http OData Queries more easily

    }
}
