using System;
using System.Collections.Generic;

namespace BaoLoi.Models;

public partial class Listjig
{
    public int Id { get; set; }

    public string? Model { get; set; }

    public string? Cell { get; set; }

    public string Station { get; set; } = null!;

    public string? Jigname { get; set; }

    public string? Jigno { get; set; }

    public string? Quantrong { get; set; }
}
