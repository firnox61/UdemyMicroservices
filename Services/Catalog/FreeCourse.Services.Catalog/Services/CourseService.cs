using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourses.Shared.Dtos;
using Mass=MassTransit;
using MongoDB.Driver;
using FreeCourses.Shared.Messages;
using Nest;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;
        private readonly Mass.IPublishEndpoint _publishEndpoint;
        private readonly IElasticClient _elasticClient;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, Mass.IPublishEndpoint publishEndpoint, IElasticClient elastic)
        {
            var client=new MongoClient(databaseSettings.ConnectionString);
            var database=client.GetDatabase(databaseSettings.DatabaseName);
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _elasticClient = elastic;
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

            return Response<NoContent>.Success(204);

        }
        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result=await _courseCollection.DeleteOneAsync(x=>x.Id==id);
            if(result.DeletedCount>0)
            {
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
            .Query(q => q.Match(m => m.Field(f => f.Name).Query(query)))
        );

            if (!searchResponse.IsValid || searchResponse.Documents.Count == 0)
            {
                return Response<List<CourseDto>>.Fail("No courses found", 404);
            }

            // Elasticsearch'den dönen veriyi CourseDto'ya dönüştürme işlemi
            var courses = searchResponse.Documents.Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                UserId = c.UserId,
                Picture = c.Picture,
                CreatedTime = c.CreatedTime,
                CategoryId = c.CategoryId,
                // Eğer Feature ve Category gibi ilişkili veriler varsa, bunları da dönüştürmeniz gerekebilir
                Feature = c.Feature != null ? new FeatureDto
                {
                    // FeatureDto içindeki alanları doldur
                    // Örneğin:
                    // Id = c.Feature.Id,
                    // Description = c.Feature.Description,
                } : null,

                Category = c.Category != null ? new CategoryDto
                {
                    // CategoryDto içindeki alanları doldur
                    // Örneğin:
                    // Id = c.Category.Id,
                    // Name = c.Category.Name,
                } : null

            }).ToList();

            return Response<List<CourseDto>>.Success(courses, 200);
        }
    }
}
