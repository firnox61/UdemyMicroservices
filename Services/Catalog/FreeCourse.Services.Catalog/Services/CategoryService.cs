﻿using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    internal class CategoryService:ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client=new MongoClient(databaseSettings.ConnectionString);//clienta bağlandık
            var database=client.GetDatabase(databaseSettings.DatabaseName);//database adını aldık

            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var categories=await _categoryCollection.Find(category=>true).ToListAsync();
            //bir sorgunun sonuçlarını asenkron olarak bir listeye dönüştürür. 
            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), 200);
        }

        public async Task<Response<CategoryDto>> CreateAsync(Category category)
        {
            await _categoryCollection.InsertOneAsync(category);
            //Bu metot, bir belgeyi asenkron olarak MongoDB koleksiyonuna eklemek için kullanılır.
            //MongoDB.Driver kütüphanesinde bulunur.
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category),200);
        }
        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
           var category= await _categoryCollection.Find<Category>(x=>x.Id==id).FirstOrDefaultAsync();
            //Bu metot, bir koleksiyondaki ilk elemanı veya eğer koleksiyon boşsa varsayılan değeri
            //(genellikle null) asenkron olarak döndürür. 
            if (category==null)
            {
                return Response<CategoryDto>.Fail("Category not found", 404);
            }
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }
    }
}
