using System;

namespace OrbitOne.BuildScreen.Models
{
    public class Build
    {
        public DateTime FinishTime { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
        public Request[] Requests { get; set; }
        public string Uri { get; set; }
        public Definition Definition { get; set; }
    }

    public class Definition
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class RequestedFor
    {
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
    }

    public class Request
    {
        public RequestedFor RequestedFor { get; set; }
    }
}