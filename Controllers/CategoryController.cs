using Microsoft.AspNetCore.Mvc;
using StoreAPI.Data;

namespace StoreAPI.Controllers;

public class CategoryController : ControllerBase
{

    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }
}