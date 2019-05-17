using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NLog;
using NorthwindConsole.Models;
using System.Globalization;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string ProductIDD;// i had this for choice 7 to work

        public static void Main(string[] args)
        {
            logger.Info("Program started");
            TextInfo Ti = new CultureInfo("en-US", false).TextInfo;
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Add Products");
                    Console.WriteLine("6) Edit Specific Products");
                    Console.WriteLine("7) Display all Products(see all products, discontinued products, or active products)");
                    Console.WriteLine("8) Edit Category");
                    Console.WriteLine("9) Display Categories(see a specific category & its prodcuts, display a specific category");
                    Console.WriteLine("10) Delete Category");                    
                    Console.WriteLine("11) Delete Specific Products");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(c => c.CategoryName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                    }
                    else if (choice == "2")
                    {
                        Category category = new Category();
                        string categoryChoice = "";
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();
                        var db = new NorthwindContext();
                        var isValid = Validator.TryValidateObject(category, context, results, true);

                        do
                        {
                            Console.WriteLine("Enter Category Name:");
                            category.CategoryName = Console.ReadLine();
                            category.CategoryName = Ti.ToTitleCase(category.CategoryName);


                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                                if (!isValid)
                                {
                                    foreach (var result in results)
                                    {
                                        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                    }
                                }
                            }
                            else if (category.CategoryName == "")
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Opps!!!...No Blank category", new string[] { "CategoryName" }));
                                if (!isValid)
                                {
                                    foreach (var result in results)
                                    {
                                        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Enter the Category Description:");
                                category.Description = Console.ReadLine();
                                //ToDO add new category to database
                                db.Categories.Add(category);
                                db.SaveChanges();
                                logger.Info("Category added");
                            }
                            Console.WriteLine("Do you want to enter another category name or enter q to quit");
                            categoryChoice = Console.ReadLine();
                        } while (categoryChoice.ToLower() != "q");



                    }
                    else if (choice == "3")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(c => c.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(c => c.CategoryId);
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
                        Boolean isValid;
                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();
                        var db = new NorthwindContext();
                        string productChoice = "";

                        do
                        {
                            Console.WriteLine("Enter Product Name:");
                            product.ProductName = Console.ReadLine();
                            product.ProductName = Ti.ToTitleCase(product.ProductName);
                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Data already exists", new string[] { "ProductName" }));
                            }

                            else
                            {
                                Console.WriteLine("Enter Product Quantity:");
                                product.QuantityPerUnit = Console.ReadLine();
                                Console.WriteLine("Enter Product UnitPrice:");
                                string price = Console.ReadLine();
                                decimal unitPrice;
                                if (decimal.TryParse(price, out unitPrice))
                                {
                                    product.UnitPrice = unitPrice;
                                    Console.WriteLine("Enter the Products Units In Stock:");
                                    string stock = Console.ReadLine();
                                    Int16 UnitsInStock;
                                    if (Int16.TryParse(stock, out UnitsInStock))
                                    {
                                        product.UnitsInStock = UnitsInStock;
                                        //logger.Info("Validation passed");
                                        Console.WriteLine("");
                                        Console.WriteLine("Enter the Products Units On Order:");
                                        string Order = Console.ReadLine();
                                        Int16 UnitsOnOrder;
                                        if (Int16.TryParse(Order, out UnitsOnOrder))
                                        {
                                            product.UnitsOnOrder = UnitsOnOrder;
                                            //logger.Info("Validation passed");
                                            Console.WriteLine("Enter the Products ReorderLevel:");
                                            string Level = Console.ReadLine();
                                            Int16 ReorderLevel;

                                            if (Int16.TryParse(Level, out ReorderLevel))
                                            {
                                                product.ReorderLevel = ReorderLevel;
                                                // logger.Info("Validation passed");
                                            }

                                            Console.WriteLine("Discontinued? " + "Enter the active status of the product(false if active and true if discontinued)");
                                            string Disc = Console.ReadLine();
                                            bool Discontinued;
                                            if (bool.TryParse(Disc, out Discontinued))
                                            {
                                                product.Discontinued = Discontinued;
                                                logger.Info("Validation passed");
                                            }
                                            //save to file                                             
                                            db.Products.Add(product);
                                            db.SaveChanges();
                                            logger.Info("Product (id: {productId}) added", product.ProductID);
                                        }

                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid Stock entered");
                                    }

                                }
                                else
                                {
                                    Console.WriteLine("Invalid price entered");
                                }

                            }

                            Console.WriteLine("Do you want to enter another Product or enter q to quit");
                            productChoice = Console.ReadLine();

                        } while (productChoice.ToLower() != "q");


                    }
                    else if (choice == "6")
                    {
                        // edit product

                        Console.WriteLine("Choose the product to edit:");
                        var product = Console.ReadLine();

                        var db = new NorthwindContext();

                        if (product != null)
                        {
                            // input product
                            Product UpdatedProduct = db.Products.FirstOrDefault(p => p.ProductName == product);
                            if (UpdatedProduct != null)
                            {
                                
                                string editChoice = "";
                                do
                                {
                                    //show the product details entered here
                                    Console.WriteLine("1) Edit Product Name");
                                    Console.WriteLine("2) Edit Quantity Per Unit");
                                    Console.WriteLine("3) Edit Unit Price of product");
                                    Console.WriteLine("4) Edit Units In Stock");
                                    Console.WriteLine("5) Edit Units On Order");
                                    Console.WriteLine("6) Edit Reorder Level");
                                    Console.WriteLine("7) Edit Discontinued status(false/true entre only)");
                                    editChoice = Console.ReadLine();
                                    Console.Clear();
                                    logger.Info($"Option {editChoice} selected");
                                    //1) edit product name

                                    
                                    if (editChoice == "1")
                                    {
                                        Console.WriteLine("Enter new Product Name:");
                                        UpdatedProduct.ProductName = Console.ReadLine();
                                        UpdatedProduct.ProductName = Ti.ToTitleCase(UpdatedProduct.ProductName);
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productId}) updated", UpdatedProduct.ProductID);
                                    }
                                    else if (editChoice == "2")
                                    {
                                        Console.WriteLine("Enter new Product Quantity:");
                                        UpdatedProduct.QuantityPerUnit = Console.ReadLine();
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productId}) updated", UpdatedProduct.ProductID);
                                    }
                                    else if (editChoice == "3")
                                    {
                                        Console.WriteLine("Enter new Product UnitPrice:");
                                        string price = Console.ReadLine();
                                        decimal unitPrice;
                                        if (decimal.TryParse(price, out unitPrice))
                                        {
                                            UpdatedProduct.UnitPrice = unitPrice;
                                            logger.Info("Validation passed");
                                        }
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productId}) updated", UpdatedProduct.ProductID);
                                    }
                                    else if (editChoice == "4")
                                    {
                                        Console.WriteLine("Enter the Products Units In Stock:");
                                        string stock = Console.ReadLine();
                                        Int16 UnitsInStock;
                                        if (Int16.TryParse(stock, out UnitsInStock))
                                        {
                                            UpdatedProduct.UnitsInStock = UnitsInStock;

                                            logger.Info("Validation passed");

                                        }
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productId}) updated", UpdatedProduct.ProductID);
                                    }
                                    else if (editChoice == "5")
                                    {
                                        Console.WriteLine("Enter the Products Units In Order:");
                                        string Order = Console.ReadLine();
                                        Int16 UnitsOnOrder;
                                        if (Int16.TryParse(Order, out UnitsOnOrder))
                                        {
                                            UpdatedProduct.UnitsOnOrder = UnitsOnOrder;

                                            logger.Info("Validation passed");

                                        }
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productId}) updated", UpdatedProduct.ProductID);
                                    }
                                    else if (editChoice == "6")
                                    {
                                        Console.WriteLine("Enter the Products reorder level:");
                                        string level = Console.ReadLine();
                                        Int16 ReorderLevel;
                                        if (Int16.TryParse(level, out ReorderLevel))
                                        {
                                            UpdatedProduct.ReorderLevel = ReorderLevel;

                                            logger.Info("Validation passed");

                                        }
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productId}) updated", UpdatedProduct.ProductID);
                                    }
                                    else if (editChoice == "7")
                                    {
                                        Console.WriteLine("Enter the Products Active(Discontuned) status(True/False):");
                                        string disc = Console.ReadLine();
                                        bool Discontinued;
                                        if (bool.TryParse(disc, out Discontinued))
                                        {
                                            UpdatedProduct.Discontinued = Discontinued;

                                            logger.Info("Validation passed");

                                        }
                                        db.EditProduct(UpdatedProduct);
                                        logger.Info("Product (id: {productId}) updated", UpdatedProduct.ProductID);
                                    }


                                    Console.WriteLine("Do you want to enter to edit another Product or enter q to quit");
                                    editChoice = Console.ReadLine();

                                } while (editChoice.ToLower() != "q");
                            }
                        }
                    }
                    else if (choice == "7")// Display Products(all or sub)
                    {

                        do
                        {
                            var db = new NorthwindContext();
                            var query = db.Products.OrderBy(b => b.ProductID);
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductID}) Products from {item.ProductName}");
                            }
                            Console.WriteLine("");
                            Console.WriteLine("Select the product to display:");
                            Console.WriteLine("0) All Products ");
                            Console.WriteLine("1) See a specific Products ");
                            Console.WriteLine("2) Discontinued Products ");
                            Console.WriteLine("3) Active Products ");
                           

                            if (int.TryParse(Console.ReadLine(), out int ProductID))
                            {
                                IEnumerable<Product> Products;
                                if (ProductID != 0 && db.Products.Count(p => p.ProductID == ProductID) == 0)
                                {
                                    logger.Error("No Product was saved with that Id");
                                }
                                else
                                {
                                    // display products from all category
                                    Products = db.Products.OrderBy(p => p.ProductName);

                                    if (ProductID == 0)
                                    {
                                        // display all products from all category
                                        Products = db.Products.OrderBy(p => p.ProductName);

                                    }
                                    else if (ProductID == 2)
                                    {
                                        //display discontinued
                                        Products = db.Products.OrderBy(p => p.ProductName).Where(p => p.Discontinued == true);
                                    }
                                    else if (ProductID == 3)
                                    {
                                        //display discontinued
                                        Products = db.Products.OrderBy(p => p.ProductName).Where(p => p.Discontinued == false);
                                    }
                                    else if (ProductID == 1)
                                    {
                                        // display product from selected category
                                        Console.WriteLine("Choose the product to display:");
                                        var product = Console.ReadLine();
                                        Products = db.Products.Where(p => p.ProductName == product);

                                    }
                                    Console.WriteLine($"{Products.Count()} product(s) returned");
                                    foreach (var item in Products)
                                    {
                                        Console.WriteLine($"Product: {item.ProductName}\nQuantity: {item.QuantityPerUnit}\nPrice: {item.UnitPrice}\nUnits on Stock: {item.UnitsInStock}" +
                                            $"\nUnit on Order: {item.UnitsOnOrder}\nReorder Level: {item.ReorderLevel}\nDiscontinued: {item.Discontinued}\n");
                                    }

                                }

                            }
                           
                            Console.WriteLine("Do you want to enter another Product or enter q to quit");
                            ProductIDD = Console.ReadLine();//converts int to string to work well

                        } while (ProductIDD != "q");//this is the only problem check it later


                    }

                    else if (choice == "8")
                    {
                        // edit category

                        Console.WriteLine("Choose the category to edit:");
                        var category = Console.ReadLine();

                        var db = new NorthwindContext();

                        if (category != null)
                        {
                            // input category
                            Category UpdatedCategory = db.Categories.FirstOrDefault(c => c.CategoryName == category);
                            if (UpdatedCategory != null)
                            {
                                
                                string editChoiceCaty = "";
                                do
                                {
                                    //show the category details entered here
                                    var query = db.Categories.OrderBy(c => c.CategoryId);
                                    Console.WriteLine("1) Edit category Name");
                                    Console.WriteLine("2) Edit description");
                                    editChoiceCaty = Console.ReadLine();
                                    Console.Clear();
                                    logger.Info($"Option {editChoiceCaty} selected");

                                    if (editChoiceCaty == "1")
                                    {
                                        Console.WriteLine("Enter new category Name:");
                                        UpdatedCategory.CategoryName = Console.ReadLine();
                                        UpdatedCategory.CategoryName = Ti.ToTitleCase(UpdatedCategory.CategoryName);
                                        db.EditCategory(UpdatedCategory);
                                        logger.Info("Category (id: {CategoryId}) updated", UpdatedCategory.CategoryId);
                                    }
                                    else if (editChoiceCaty == "2")
                                    {
                                        Console.WriteLine("Enter new category description:");
                                        UpdatedCategory.Description = Console.ReadLine();
                                        db.EditCategory(UpdatedCategory);
                                        logger.Info("Category (id: {CategoryId}) updated", UpdatedCategory.CategoryId);
                                    }



                                    Console.WriteLine("Do you want to enter to edit another Category or enter q to quit");
                                    editChoiceCaty = Console.ReadLine();

                                } while (editChoiceCaty.ToLower() != "q");
                            }
                        }
                    }
                    else if (choice == "9")// Display Categories
                    {
                        string CategoryIdd;
                        do
                        {
                            var db = new NorthwindContext();
                            var query = db.Categories.OrderBy(c => c.CategoryId);

                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                            }
                            Console.WriteLine("");
                            Console.WriteLine("Select the product to display:");
                            Console.WriteLine("0) All Category ");
                            Console.WriteLine("1) Display a specific Category ");
                            Console.WriteLine("2) Display a specific Category and its active products ");



                            if (int.TryParse(Console.ReadLine(), out int CategoryId))
                            {
                                IEnumerable<Category> Categories;
                                if (CategoryId != 0 && db.Categories.Count(p => p.CategoryId == CategoryId) == 0)
                                {
                                    logger.Error("No category was saved with that Id");
                                }
                                

                                    if (CategoryId == 0)
                                    {
                                        // display all  category
                                        db = new NorthwindContext();
                                        query = db.Categories.OrderBy(c => c.CategoryName);

                                        Console.WriteLine($"{query.Count()} records returned");
                                        foreach (var item in query)
                                        {
                                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                                        }

                                    }

                                    else if (CategoryId == 1)
                                    {
                                        //display a specific category
                                        {
                                            Console.WriteLine("Choose the category to display:");
                                            var category = Console.ReadLine();
                                            Categories  = db.Categories.Where(c => c.CategoryName == category);
                                       
                                    }
                                        Console.WriteLine($"{Categories.Count()} category returned");

                                    }

                                    else if (CategoryId == 2)
                                    {
                                        // display a specific category and its active products
                                       
                                        Console.WriteLine("Choose the category to display:");
                                        var category = Console.ReadLine();
                                        Categories = db.Categories.Where(c => c.CategoryName == category);
                                    
                                    Console.WriteLine($"{Categories.Count()} category(ies) returned");
                                    Console.WriteLine(" ");
                                    foreach (var item in Categories)
                                        Console.WriteLine($"{item.CategoryName}");
                                    foreach (Product p in db.Products)
                                    {
                                        Console.WriteLine($"Product: {p.ProductName}\nQuantity: {p.QuantityPerUnit}\nPrice: {p.UnitPrice}\nUnits on Stock: {p.UnitsInStock}" +
                                            $"\nUnit on Order: {p.UnitsOnOrder}\nReorder Level: {p.ReorderLevel}\nDiscontinued: {p.Discontinued}\n");
                                    }

                                }

                            }
                            
                            Console.WriteLine("Do you want to enter another Product or enter q to quit");
                            CategoryIdd = Console.ReadLine();

                        } while (CategoryIdd != "q");
                    
                    
                    }
                    else if (choice == "10")// delete category.....
                    {

                        Console.WriteLine("Choose the category to delete:");
                        var category = Console.ReadLine();

                        var db = new NorthwindContext();

                        if (category != null)
                        {
                            // input category
                            Category RemoveCategory = db.Categories.FirstOrDefault(c => c.CategoryName == category);
                            if (RemoveCategory != null)
                            {

                                db.DeleteCategory(RemoveCategory);
                                logger.Info("Category (id: {categoryId}) deleted", RemoveCategory);
                            }
                            else
                            {
                                logger.Info("Not found");
                            }


                        }
                    }
                    else if (choice == "11")
                    {
                        
                        Console.WriteLine("Choose the product to delete:");
                        var product = Console.ReadLine();
                
                        var db = new NorthwindContext();
                        
                            if (product != null)
                            {
                                // input product
                                Product RemoveProduct = db.Products.FirstOrDefault(p => p.ProductName == product);
                                if (RemoveProduct != null)
                                {

                                    db.DeleteProduct(RemoveProduct);
                                    logger.Info("Product (id: {productid}) deleted", RemoveProduct);
                                }
                                else
                            {
                                logger.Info("Not found");
                            }
                        

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
        }
    }
}