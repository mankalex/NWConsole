using NLog;
using System.Linq;
using NWConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWContext();
    string choice;
    do
    {
        Console.WriteLine("1) Display Categories");
        Console.WriteLine("2) Add Category");
        Console.WriteLine("3) Display Category and related products");
        Console.WriteLine("4) Display all Categories and their related products");
        Console.WriteLine("5) Add Product");
        Console.WriteLine("6) Display Products");
        Console.WriteLine("7) Display specific Product");
        Console.WriteLine("8) Remove Category");
        Console.WriteLine("9) Remove Product");
        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice == "1")
        {
            var query = db.Categories.OrderBy(p => p.CategoryName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName} - {item.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (choice == "2")
        {
            Category category = new Category();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    db.Categories.Add(category);
                    db.SaveChanges();
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }
        else if (choice == "3")
        {
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                Console.WriteLine($"\t{p.ProductName}");
            }
        }
        else if (choice == "4")
        {
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    Console.WriteLine($"\t{p.ProductName}");
                }
            }
        }
        else if (choice == "5")
        {
            Product product = new Product();
            Console.WriteLine("Enter Product Name:");
            product.ProductName = Console.ReadLine();
            Console.WriteLine("Enter the Quantity per Unit: ");
            product.QuantityPerUnit = Console.ReadLine();
            Console.WriteLine("Enter product Discountinued (true/false): ");
            string dis = Console.ReadLine();
            if(dis == "true") { product.Discontinued = true; }
            else if (dis == "false") { product.Discontinued = false; }
            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(c => c.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    db.Products.Add(product);
                    db.SaveChanges();
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }
        if (choice == "6")
        {
            Console.WriteLine("What do you want to see (1: All Products, 2: Discontinued, 3: Not Discontinued)");
            string choiceProduct = Console.ReadLine();
            if(choiceProduct == "1"){
                var query = db.Products.OrderBy(p => p.ProductName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductName} - {item.QuantityPerUnit} - {item.Discontinued}");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if(choiceProduct == "2"){
                var query = db.Products.Where(p => p.Discontinued == true).OrderBy(a => a.ProductName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductName} - {item.QuantityPerUnit} - {item.Discontinued}");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            if(choiceProduct == "3"){
                var query = db.Products.Where(p => p.Discontinued == false).OrderBy(a => a.ProductName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductName} - {item.QuantityPerUnit} - {item.Discontinued}");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            
        }
        if(choice == "7")
        {
            var query = db.Products.OrderBy(p => p.ProductId);
            Console.WriteLine("Select what Product you would like to view");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            foreach(var item in query)
            {
                Console.WriteLine($"{item.ProductId}) {item.ProductName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"ProductId {id} selected");
            var newQuery = db.Products.Where(i => i.ProductId == id);
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach(var item in newQuery)
            {
                Console.WriteLine($"Product Id: {item.ProductId}");
                Console.WriteLine($"Product Name: {item.ProductName}");
                Console.WriteLine($"Supplier Id: {item.SupplierId}");
                Console.WriteLine($"Category Id: {item.CategoryId}");
                Console.WriteLine($"QuantityPerUnit: {item.QuantityPerUnit}");
                Console.WriteLine($"Unit Price: {item.UnitPrice}");
                Console.WriteLine($"Units on Order: {item.UnitsOnOrder}");
                Console.WriteLine($"ReOrder Level: {item.ReorderLevel}");
                Console.WriteLine($"Discontinued: {item.Discontinued}");
            }
            Console.ForegroundColor = ConsoleColor.White;

        }
        if(choice == "8")
        {
            Console.WriteLine("Choose the category to delete:");
            var category = GetCategory(db, logger);
            if(category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                logger.Info($"Category (id: {category.CategoryId}) deleted");
            }

        }
        if(choice == "9")
        {
            Console.WriteLine("Choose the product to delete:");
            var product = GetProduct(db, logger);
            if(product != null)
            {
                RemoveOrphan(db, logger);
                db.Products.Remove(product);
                db.SaveChanges();
                logger.Info($"Product (id: {product.ProductId}) deleted");
            }

        }
        Console.WriteLine();

    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}
logger.Info("Program ended");


static Category GetCategory(NWContext db, Logger logger)
{
    // display all categories
    var categories = db.Categories.OrderBy(b => b.CategoryId);
    foreach (Category c in categories)
    {
        Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
    }
    if (int.TryParse(Console.ReadLine(), out int CategoryId))
    {
        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId);
        if (category != null)
        {
            return category;
        }
    }
    logger.Error("Invalid Category Id");
    return null;
}


static Product GetProduct(NWContext db, Logger logger)
{
    // display all products
    var products = db.Products.OrderBy(p => p.ProductId);
    foreach (Product p in products)
    {
        Console.WriteLine($"{p.ProductId}: {p.ProductName}");
    }
    if (int.TryParse(Console.ReadLine(), out int ProductId))
    {
        Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
        if (product != null)
        {
            return product;
        }
    }
    logger.Error("Invalid Product Id");
    return null;
}

static Product RemoveOrphan(NWContext db, Logger logger)
{
    var products = db.Products.Local.Where(p => p.ProductName == null);
    foreach(Product p in products)
    {
        db.Products.Remove(p);
    }
    return null;
}
