using System;
using System.Collections.Generic;

namespace avalonia_application.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public int? Category { get; set; }

    public virtual ProductCategory? CategoryNavigation { get; set; }
}
