using Microsoft.AspNetCore.Mvc;
using StoreAPI.Data;
using StoreAPI.Models;

namespace StoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{

    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet("test-db")]
    public void TestConnectDB()
    {
        if (_context.Database.CanConnect())
        {
            Response.WriteAsync("Connected Database Success.");
        }
        else
        {
            Response.WriteAsync("Not Connected Failed.");
        }
    }


    [HttpGet]
    public ActionResult<Product> GetProducts()
    {
        // var product = _context.products.ToList();  // get all
        // var products = _context.products.Where(p => p.unit_price > 45000).ToList(); // has condition
        var products = _context.products.Join(   // join multi table
            _context.categories,
            p => p.category_id,
            c => c.category_id,
            (p, c) => new
            {
                p.product_id,
                p.product_name,
                p.unit_price,
                p.unit_in_stock,
                c.category_name
            }
        ).ToList();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _context.products.FirstOrDefault(p => p.product_id == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromForm] Product product, IFormFile file)
    {
        _context.products.Add(product);

        // check image
        if (file != null)
        {
            // new filename
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // save image
            string uploadFolder = Path.Combine(_env.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // copy file
            using (var fileStream = new FileStream(Path.Combine(uploadFolder, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            product.product_picture = fileName; // save filename
        }

        _context.SaveChanges();  // save

        return Ok(product);
    }

    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProduct(int id, Product product)
    {
        var existingProduct = _context.products.FirstOrDefault(p => p.product_id == id);

        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.product_name = product.product_name;
        existingProduct.unit_price = product.unit_price;
        existingProduct.unit_in_stock = product.unit_in_stock;
        existingProduct.category_id = product.category_id;

        _context.SaveChanges();

        return Ok(existingProduct);
    }

    [HttpDelete("{id}")]
    public ActionResult<Product> DeleteProduct(int id)
    {
        var product = _context.products.FirstOrDefault(p => p.product_id == id);

        if (product == null)
        {
            return NotFound();
        }

        _context.products.Remove(product);
        _context.SaveChanges();

        return Ok(product);
    }

}