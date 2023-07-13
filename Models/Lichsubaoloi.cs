using System;
using System.Collections.Generic;

namespace BaoLoi.Models;

public partial class Lichsubaoloi
{
    public int Id { get; set; }

    public string Model { get; set; } = null!;

    public string Cell { get; set; } = null!;

    public string Station { get; set; } = null!;

    public string Thoigian { get; set; } = null!;
}
