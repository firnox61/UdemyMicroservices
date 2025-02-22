﻿using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Services;
using FreeCourses.Shared.ControllerBases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace FreeCourse.Services.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return CreateActionResultInstance(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var categories = await _categoryService.GetByIdAsync(id);
            return CreateActionResultInstance(categories);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            var categories = await _categoryService.CreateAsync(categoryDto);
            return CreateActionResultInstance(categories);
        }
        [HttpPost("sync-elasticsearch")]
        public async Task<IActionResult> SyncElasticsearch()
        {
            var response = await _categoryService.SyncCategoriesToElasticsearchAsync();

            if (!response.IsSuccessful)
            {
                return StatusCode(response.StatusCode, response.Errors);
            }

            return NoContent();
        }

    }
}
