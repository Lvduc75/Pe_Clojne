using System;
using System.Collections.Generic;

namespace Project11.Models;

public class Book
{
    public int Id { get; set; }

    public int? NumberDouble { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? BooleanCheck { get; set; }

    public string? StringCheck { get; set; }

    public override string ToString()
    {
        return $"{Id} \t {NumberDouble} \t {UpdateDate?.ToShortDateString()} \t {BooleanCheck} \t {StringCheck}";
    }
}
