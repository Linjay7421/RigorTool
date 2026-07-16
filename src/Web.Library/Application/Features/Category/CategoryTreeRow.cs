using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Library.Application.Features.Category
{
    public sealed record CategoryTreeRow() 
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid? ParentId { get; init; }
        public bool IsActive { get; init; } = false;
        public DateTime CreatedAt { get; init; }
        public int ProductCount { get; init; } = 0;
    };
}
