using System;
using System.Collections.Generic;

namespace Project11.Models;

public partial class Book
{
    public int Id { get; set; }

    public int? NumberDouble { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? BooleanCheck { get; set; }

    public string? StringCheck { get; set; }
}
