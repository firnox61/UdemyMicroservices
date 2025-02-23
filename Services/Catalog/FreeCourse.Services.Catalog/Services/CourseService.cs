using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourses.Shared.Dtos;
using Mass=MassTransit;
using MongoDB.Driver;
using FreeCourses.Shared.Messages;
using Nest;
using Elasticsearch.Net;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;
        private readonly Mass.IPublishEndpoint _publishEndpoint;
        private readonly IElasticClient _elasticClient;
        private readonly ICategoryService _categoryService;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, Mass.IPublishEndpoint publishEndpoint,
            IElasticClient elastic, ICategoryService categoryService)
        {
            var client=new MongoClient(databaseSettings.ConnectionString);
            var database=client.GetDatabase(databaseSettings.DatabaseName);
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _elasticClient = elastic;
            _categoryService = categoryService;
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);
            newCourse.CreatedTime = DateTime.UtcNow;
            await _courseCollection.InsertOneAsync(newCourse);
            _elasticClient.IndexDocument(courseCreateDto);
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse),200);
        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse=_mapper.Map<Course>(courseUpdateDto);
            var result= await _courseCollection.FindOneAndReplaceAsync(x=>x.Id==courseUpdateDto.Id, updateCourse);
            if (result == null)
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }

           /* var updatedCourse = _mapper.Map<CourseDto>(courseUpdateDto);

            var elasticResponse = await _elasticClient.UpdateAsync<CourseDto>(courseUpdateDto.Id, u => u
                .Doc(updatedCourse) // Güncellenmiş nesneyi Elasticsearch'e gönderiyoruz
                .Refresh(Refresh.True)
            );


            if (!elasticResponse.IsValid)
            {
                return Response<NoContent>.Fail($"Elasticsearch update failed: {elasticResponse.DebugInformation}", 500);
            }*/

            await _publishEndpoint.Publish<BasketCourseChangeNameEvent>(new BasketCourseChangeNameEvent
            {
                UserId = courseUpdateDto.UserId,
                CourseId = updateCourse.Id,
                UpdateName = updateCourse.Name,
            });

            await _publishEndpoint.Publish<CourseNameChangeEvent>(new CourseNameChangeEvent
            {
                CourseId = updateCourse.Id,
                UpdateName = courseUpdateDto.Name,
            });
            _elasticClient.IndexDocument(courseUpdateDto);
            return Response<NoContent>.Success(204);

        }
        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result=await _courseCollection.DeleteOneAsync(x=>x.Id==id);
            if(result.DeletedCount>0)
            {
                var elasticResponse = await _elasticClient.DeleteAsync<CourseDto>(id);
                if (!elasticResponse.IsValid)
                {
                    return Response<NoContent>.Fail("Failed to delete from Elasticsearch", 500);
                }
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }
        }
        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses=await _courseCollection.Find(course=>true).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                    //belirli bir koşulu sağlayan ilk elemanı asenkron olarak döndürür. 
                }
            }
            else 
            {
                courses=new List<Course>();
            }


            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses),200);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course=await _courseCollection.Find<Course>(c=>c.Id==id).FirstOrDefaultAsync();
            if(course==null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);
            }
            course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course),200);
        }
        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses=await _courseCollection.Find<Course>(c=>c.UserId==userId).ToListAsync();
            if (courses.Any())//Any() metodu, bir koleksiyonda herhangi bir eleman olup olmadığını
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }


            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<List<CourseDto>>> SearchCoursesAsync(string query)
        {
            var searchResponse = await _elasticClient.SearchAsync<CourseDto>(s => s
    .Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.Name).Query(query)))
);


            if (!searchResponse.IsValid || searchResponse.Documents.Count == 0)
            {
                return Response<List<CourseDto>>.Fail("No courses found", 404);
            }

            // Elasticsearch'den dönen veriyi CourseDto'ya dönüştürme işlemi
            var categoryList = await _categoryService.GetAllAsync();

            var courses = searchResponse.Documents.Select(c =>
            {
                var matchedCategory = categoryList.Data.FirstOrDefault(cat => cat.Id == c.CategoryId);

                return new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Price = c.Price,
                    UserId = c.UserId,
                    Picture = c.Picture,
                    CreatedTime = c.CreatedTime,
                    CategoryId = c.CategoryId,

                    Feature = c.Feature != null ? new FeatureDto
                    {
                        Duration = c.Feature.Duration
                    } : null,

                    Category = matchedCategory != null ? new CategoryDto
                    {
                        Id = matchedCategory.Id,
                        Name = matchedCategory.Name
                    } : null
                };
            }).ToList();

            return Response<List<CourseDto>>.Success(courses, 200);

        }
        public async Task<Response<NoContent>> SyncCoursesToElasticsearchAsync()
        {
            var courses = await _courseCollection.Find(_ => true).ToListAsync();

            if (!courses.Any())
            {
                return Response<NoContent>.Fail("No courses found in the database.", 404);               
            }

            var courseDtos = _mapper.Map<List<CourseDto>>(courses);
            var bulkResponse = await _elasticClient.IndexManyAsync(courseDtos);

            if (!bulkResponse.IsValid)
            {
                return Response<NoContent>.Fail($"Failed to sync courses: {bulkResponse.DebugInformation}", 500);
               
            }
            else
            {
                return Response<NoContent>.Success(204);
            }
        }
    }
}
