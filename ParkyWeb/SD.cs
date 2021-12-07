using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SD
    {
        public const string APIBaseUrl = "http://localhost:49463/";
        public const string NationalParkAPIPath = APIBaseUrl+"api/v1/nationalParks/";
        public const string TrailAPIPath = APIBaseUrl+"api/v1/trails/";
        public const string AccountAPIPath = APIBaseUrl+"api/v1/users/";
    }
}
