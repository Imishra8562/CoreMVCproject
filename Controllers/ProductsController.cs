using Microsoft.AspNetCore.Mvc;
using CoreMVCproject.DataAccessLayer;
using CoreMVCproject.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace CoreMVCproject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductRepository _productRepository;

        // Inject the ProductRepository via the constructor
        public ProductsController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Action to display a list of products
        public IActionResult Index()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        //// Action to display the form for adding a new product
        //[Authorize(Policy = "AdminOnly")] // Only users in the "Admin" role can access this action
        public IActionResult Create()
        {
            return View();
        }

        // Action to handle the form submission for adding a new product
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Action to display the form for editing a product
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Action to handle the form submission for editing a product
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Action to handle the deletion of a product
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _productRepository.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}