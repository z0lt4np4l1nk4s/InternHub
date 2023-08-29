using InternHub.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternHub.WebApi.Models
{
    public class StateView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public StateView() { }

        public StateView(State state)
        {
            Id = state.Id;
            Name = state.Name;
        }
    }
}